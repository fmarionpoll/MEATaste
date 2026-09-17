using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MEATaste.DataMEA.Models;
using MEATaste.DataMEA.Utilities;
using MEATaste.Infrastructure;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.WPF;


namespace MEATaste.Views.PlotSignal
{
    public class PlotSignalPanelController
    {
        public PlotSignalPanelModel Model { get; }
        private readonly ApplicationState state;
        private List<int> listSelectedChannels;
        private int selectedFilter;
        private string acquisitionSettings;
        private bool suppressAxesChanged;
        private readonly List<(string Legend, double[] Data)> traces = new();
        private VerticalLine playheadLine;
        private const float PlayheadHandleSize = 16;
        private const float PlayheadHandleInsetPx = 14;
        private const double PlayheadHitRadiusPx = 24;
        private const double PlayheadEndBandPx = 32;

        private Scatter playheadHandles;
        private readonly double[] playheadHandleXs = { 0, 0 };
        private readonly double[] playheadHandleYs = { 0, 1 };
        private bool draggingPlayhead;

        public PlotSignalPanelController(ApplicationState state, IEventSubscriber eventSubscriber)
        {
            this.state = state;

            Model = new PlotSignalPanelModel();
            Model.PlotControl.SizeChanged += (_, _) => RenderStoredTraces();
            Model.PlotControl.PreviewMouseDown += OnPlotMouseDown;
            Model.PlotControl.PreviewMouseMove += OnPlotMouseMove;
            Model.PlotControl.PreviewMouseUp += OnPlotMouseUp;
            Model.PlotControl.LostMouseCapture += OnPlotLostMouseCapture;
            eventSubscriber.Subscribe(EventType.MeaExperimentChanged, LoadAcquisitionParameters);
            eventSubscriber.Subscribe(EventType.AxesMaxMinChanged, AxesChanged);
            eventSubscriber.Subscribe(EventType.FilterChanged, ChangeFilter);
            eventSubscriber.Subscribe(EventType.PlayheadTimeChanged, PlayheadChanged);
        }

        private void LoadAcquisitionParameters()
        {
            var meaExperiment = state.MeaExperiment.Get();
            acquisitionSettings = " High-pass=" + meaExperiment.DataAcquisitionSettings.Hpf + " Hz "
                                           + " Sampling rate=" + meaExperiment.DataAcquisitionSettings.SamplingRate /1000 + " kHz "
                                           + " resolution=" + (meaExperiment.DataAcquisitionSettings.Lsb * 1000).ToString("0.###")  + " mV";
        }

        private void ChangeFilter()
        {
            selectedFilter = state.FilterProperty.Get();
            if (listSelectedChannels is not {Count: > 0}) return;
            UpdateSelectedElectrodeData(listSelectedChannels);
        }

        public void UpdateChannelList(List<int> channelList)
        {
            listSelectedChannels = new List<int>( channelList);
            UpdateSelectedElectrodeData(listSelectedChannels);
        }

        private void UpdateSelectedElectrodeData(List<int> selectedChannels)
        {
            PreparePlot();
            traces.Clear();
            if (selectedChannels.Count <= 0) return;
            LoadDataToPlot(selectedChannels);
            DisplayPlot();
        }

        private void LoadDataToPlot(List<int> selectedChannels)
        {
            var meaExp = state.MeaExperiment.Get();
            if (meaExp == null) return;
            foreach (var i in selectedChannels)
            {
                var electrodeData = meaExp.Electrodes.Single(x => x.Electrode.Channel == i);
                var legend = "channel: " + electrodeData.Electrode.Channel;

                Trace.WriteLine("LoadDataToPlot(): " + i
                                + " channel=" + electrodeData.Electrode.Channel);

                var channel = state.DataSelected.Get().Channels[i];
                traces.Add((legend, ComputeFilteredData(channel)));
            }
        }

        private double[] ComputeFilteredData(ushort[] array)
        {
            var mVFactor = state.MeaExperiment.Get().DataAcquisitionSettings.Lsb * 1000;
            var result = Filter.ConvertDataToMV(array, mVFactor, 512);

            switch (selectedFilter)
            {
                case 1:
                    result = Filter.BDerivFast2f3(result, result.Length);
                    break;
            }

            return result;
        }

        private void PreparePlot()
        {
            var plot = Model.PlotControl.Plot;
            plot.Clear();
            playheadLine = null;
            playheadHandles = null;
            plot.XLabel("Time (s)");
            plot.YLabel("Voltage (mV)");
        }

        private void DisplayPlot()
        {
            var plot = Model.PlotControl.Plot;
            plot.ShowLegend();
            plot.Legend.FontSize = 10;

            if (state.AxesMaxMin.Get() == null)
            {
                var acqSettings = state.MeaExperiment.Get().DataAcquisitionSettings;
                var duration = acqSettings.nDataAcquisitionPoints / acqSettings.SamplingRate;
                var yMin = 0.0;
                var yMax = 1.0;
                if (traces.Count > 0 && traces[0].Data is { Length: > 0 })
                {
                    yMin = traces.Min(t => t.Data.Min());
                    yMax = traces.Max(t => t.Data.Max());
                    if (yMin == yMax)
                    {
                        yMin -= 1;
                        yMax += 1;
                    }
                }
                state.AxesMaxMin.Set(new AxesExtrema(0, duration, yMin, yMax));
                return;
            }

            RenderStoredTraces();
        }

        private void RenderStoredTraces()
        {
            if (suppressAxesChanged) return;
            if (Model.PlotControl == null) return;
            var meaExp = state.MeaExperiment.Get();
            if (meaExp == null || traces.Count == 0) return;

            var samplingRate = meaExp.DataAcquisitionSettings.SamplingRate;
            var axes = state.AxesMaxMin.Get();
            var xMin = axes?.XMin ?? 0;
            var xMax = axes?.XMax ?? (meaExp.DataAcquisitionSettings.nDataAcquisitionPoints / samplingRate);
            var pixelWidth = GetPlotPixelWidth();

            var plot = Model.PlotControl.Plot;
            suppressAxesChanged = true;
            plot.Clear();
            playheadLine = null;
            playheadHandles = null;
            plot.XLabel("Time (s)");
            plot.YLabel("Voltage (mV)");
            plot.ShowLegend();
            plot.Legend.FontSize = 10;

            foreach (var (legend, data) in traces)
            {
                MinMaxEnvelope.Build(data, samplingRate, xMin, xMax, pixelWidth, out var times, out var values);
                if (times.Length == 0) continue;
                var scatter = plot.Add.ScatterLine(times, values);
                scatter.LegendText = legend;
                scatter.MarkerSize = 0;
            }

            if (axes != null)
                plot.Axes.SetLimits(axes.XMin, axes.XMax, axes.YMin, axes.YMax);
            AddOrUpdatePlayheadLine(plot);

            Application.Current.Dispatcher.Invoke(() => { Model.PlotControl.Refresh(); });
            suppressAxesChanged = false;
        }

        private int GetPlotPixelWidth()
        {
            var width = Model.PlotControl.ActualWidth;
            if (width < 80)
                width = 500;
            return Math.Max(2, (int)width - 80);
        }

        private void AddOrUpdatePlayheadLine(Plot plot)
        {
            var t = state.PlayheadTime.Get();
            playheadLine = plot.Add.VerticalLine(t);
            playheadLine.LineColor = Colors.Black;
            playheadLine.LineWidth = 1;
            playheadLine.LinePattern = LinePattern.Dashed;

            PlacePlayheadHandles(plot, t);
            playheadHandles = plot.Add.Scatter(playheadHandleXs, playheadHandleYs);
            playheadHandles.MarkerShape = MarkerShape.FilledCircle;
            playheadHandles.MarkerSize = PlayheadHandleSize;
            playheadHandles.MarkerColor = Colors.Black;
            playheadHandles.MarkerLineWidth = 1;
            playheadHandles.MarkerLineColor = Colors.White;
            playheadHandles.LineWidth = 0;
            playheadHandles.LegendText = string.Empty;
        }

        private void PlayheadChanged()
        {
            if (draggingPlayhead)
            {
                UpdatePlayheadGraphics();
                return;
            }

            if (playheadLine == null)
            {
                if (traces.Count > 0)
                    RenderStoredTraces();
                return;
            }

            UpdatePlayheadGraphics();
        }

        private void UpdatePlayheadGraphics()
        {
            var t = state.PlayheadTime.Get();
            if (playheadLine != null)
                playheadLine.X = t;
            if (playheadHandles != null)
                PlacePlayheadHandles(Model.PlotControl.Plot, t);
            Application.Current.Dispatcher.Invoke(() => { Model.PlotControl.Refresh(); });
        }

        private void OnPlotMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left || Model.PlotControl == null) return;
            if (!HitPlayheadHandle(e.GetPosition(Model.PlotControl))) return;

            draggingPlayhead = true;
            Model.PlotControl.CaptureMouse();
            SetUserInputEnabled(false);
            MovePlayheadTo(e.GetPosition(Model.PlotControl));
            e.Handled = true;
        }

        private void OnPlotMouseMove(object sender, MouseEventArgs e)
        {
            if (!draggingPlayhead) return;
            MovePlayheadTo(e.GetPosition(Model.PlotControl));
            e.Handled = true;
        }

        private void OnPlotMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!draggingPlayhead || e.ChangedButton != MouseButton.Left) return;
            MovePlayheadTo(e.GetPosition(Model.PlotControl));
            EndPlayheadDrag();
            e.Handled = true;
        }

        private void OnPlotLostMouseCapture(object sender, MouseEventArgs e) => EndPlayheadDrag();

        private void EndPlayheadDrag()
        {
            if (!draggingPlayhead) return;
            draggingPlayhead = false;
            if (Model.PlotControl.IsMouseCaptured)
                Model.PlotControl.ReleaseMouseCapture();
            SetUserInputEnabled(true);
        }

        private void MovePlayheadTo(Point pos)
        {
            Coordinates coord;
            try
            {
                coord = Model.PlotControl.Plot.GetCoordinates((float)pos.X, (float)pos.Y);
            }
            catch (Exception)
            {
                return;
            }

            SetPlayheadSeconds(coord.X);
        }

        private void PlacePlayheadHandles(Plot plot, double time)
        {
            playheadHandleXs[0] = time;
            playheadHandleXs[1] = time;
            var limits = plot.Axes.GetLimits();
            try
            {
                var topPx = plot.GetPixel(new Coordinates(time, limits.Top));
                var bottomPx = plot.GetPixel(new Coordinates(time, limits.Bottom));
                playheadHandleYs[1] = plot.GetCoordinates(new Pixel(topPx.X, topPx.Y + PlayheadHandleInsetPx)).Y;
                playheadHandleYs[0] = plot.GetCoordinates(new Pixel(bottomPx.X, bottomPx.Y - PlayheadHandleInsetPx)).Y;
            }
            catch (Exception)
            {
                playheadHandleYs[0] = limits.Bottom;
                playheadHandleYs[1] = limits.Top;
            }
        }

        private bool HitPlayheadHandle(Point pos)
        {
            if (playheadHandles == null) return false;
            try
            {
                var plot = Model.PlotControl.Plot;
                var mousePixel = plot.GetPixel(plot.GetCoordinates((float)pos.X, (float)pos.Y));
                var lineX = plot.GetPixel(new Coordinates(playheadHandleXs[0], playheadHandleYs[0])).X;
                if (Math.Abs(mousePixel.X - lineX) > PlayheadHitRadiusPx)
                    return false;

                for (var i = 0; i < 2; i++)
                {
                    var handlePixel = plot.GetPixel(new Coordinates(playheadHandleXs[i], playheadHandleYs[i]));
                    var dx = mousePixel.X - handlePixel.X;
                    var dy = mousePixel.Y - handlePixel.Y;
                    if (dx * dx + dy * dy <= PlayheadHitRadiusPx * PlayheadHitRadiusPx)
                        return true;
                    if (Math.Abs(mousePixel.Y - handlePixel.Y) <= PlayheadEndBandPx)
                        return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        private void SetPlayheadSeconds(double seconds)
        {
            var experiment = state.MeaExperiment.Get();
            if (experiment == null)
            {
                state.PlayheadTime.Set(0);
                return;
            }

            var nPoints = experiment.DataAcquisitionSettings.nDataAcquisitionPoints;
            var samplingRate = experiment.DataAcquisitionSettings.SamplingRate;
            var duration = nPoints > 0 && samplingRate > 0 ? (nPoints - 1) / samplingRate : 0;
            if (duration < 0) duration = 0;
            state.PlayheadTime.Set(Math.Clamp(seconds, 0, duration));
        }

        private void SetUserInputEnabled(bool enabled)
        {
            try
            {
                Model.PlotControl.UserInputProcessor.IsEnabled = enabled;
            }
            catch (Exception)
            {
            }
        }

        public void OnAxesChanged(object sender, EventArgs e)
        {
            if (suppressAxesChanged) return;
            var changedPlot = (WpfPlot)sender;
            var newAxisLimits = changedPlot.Plot.Axes.GetLimits();
            ChangeXYAxes(changedPlot, newAxisLimits);
            state.AxesMaxMin.Set(new AxesExtrema(newAxisLimits.Left, newAxisLimits.Right, newAxisLimits.Bottom, newAxisLimits.Top));
        }

        private void ChangeXYAxes(WpfPlot plot, AxisLimits newAxisLimits)
        {
            suppressAxesChanged = true;
            plot.Plot.Axes.SetLimitsX(newAxisLimits.Left, newAxisLimits.Right);
            plot.Plot.Axes.SetLimitsY(newAxisLimits.Bottom, newAxisLimits.Top);
            plot.Refresh();
            suppressAxesChanged = false;
        }

        private void AxesChanged()
        {
            var axesMaxMin = state.AxesMaxMin.Get();
            if (axesMaxMin == null) return;
            if (traces.Count > 0)
            {
                RenderStoredTraces();
                return;
            }

            ChangeXYAxes(
                Model.PlotControl, new AxisLimits(axesMaxMin.XMin, axesMaxMin.XMax, axesMaxMin.YMin, axesMaxMin.YMax));
        }
    }
}

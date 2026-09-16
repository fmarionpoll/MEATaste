using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
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

        public PlotSignalPanelController(ApplicationState state, IEventSubscriber eventSubscriber)
        {
            this.state = state;

            Model = new PlotSignalPanelModel();
            Model.PlotControl.SizeChanged += (_, _) => RenderStoredTraces();
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

            AddOrUpdatePlayheadLine(plot);
            if (axes != null)
                plot.Axes.SetLimits(axes.XMin, axes.XMax, axes.YMin, axes.YMax);

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
        }

        private void PlayheadChanged()
        {
            if (playheadLine == null)
            {
                if (traces.Count > 0)
                    RenderStoredTraces();
                return;
            }

            playheadLine.X = state.PlayheadTime.Get();
            Application.Current.Dispatcher.Invoke(() => { Model.PlotControl.Refresh(); });
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

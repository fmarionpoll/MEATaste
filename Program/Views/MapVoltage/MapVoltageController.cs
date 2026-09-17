using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using MEATaste.DataMEA.MaxWell;
using MEATaste.DataMEA.Models;
using MEATaste.Infrastructure;
using MEATaste.Views;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.WPF;

namespace MEATaste.Views.MapVoltage
{
    public class MapVoltageController
    {
        private const int PlayFps = 20;
        private const ushort VoltageZero = 512;

        public MapVoltageModel Model { get; }

        private readonly ApplicationState state;
        private readonly VoltageChunkBuffer chunkBuffer = new();
        private readonly SignedVoltageColormap colormap = new();
        private DispatcherTimer playTimer;
        private bool panelActive;
        private bool captureScaleFromFirstFrame = true;
        private double scaleAmplitude = 1;
        private List<int> selectedChannels = new();
        private Heatmap colorScale;
        private Marker[] electrodeMarkers;
        private Scatter selectionOverlay;
        private Point mouseDownPos;
        private bool mouseDown;
        private double playSpeed = 1;

        public MapVoltageController(ApplicationState state, IEventSubscriber eventSubscriber)
        {
            this.state = state;
            Model = new MapVoltageModel();

            eventSubscriber.Subscribe(EventType.MeaExperimentChanged, OnExperimentChanged);
            eventSubscriber.Subscribe(EventType.PlayheadTimeChanged, OnPlayheadTimeChanged);
            eventSubscriber.Subscribe(EventType.SelectedChannelsChanged, OnSelectedChannelsChanged);
        }

        public void AttachControlToModel(WpfPlot wpfControl)
        {
            Model.PlotControl = wpfControl;
            colorScale = null;
            electrodeMarkers = null;
            selectionOverlay = null;
            wpfControl.MouseDown += OnPlotMouseDown;
            wpfControl.MouseUp += OnPlotMouseUp;

            if (panelActive && state.MeaExperiment.Get() != null)
                RedrawHeatmap();
        }

        public void SetActive(bool active)
        {
            panelActive = active;
            if (!active)
                StopPlayback();
            else if (state.MeaExperiment.Get() != null)
                RedrawHeatmap();
        }

        public void ZoomIn() => Zoom(MapViewGeometry.ZoomFactor);

        public void ZoomOut() => Zoom(1 / MapViewGeometry.ZoomFactor);

        public void Fit()
        {
            var experiment = state.MeaExperiment.Get();
            if (experiment?.Electrodes == null || Model.PlotControl == null) return;
            var full = MapViewGeometry.FullLimits(experiment);
            MapViewGeometry.SetLimits(Model.PlotControl.Plot, full.XMin, full.XMax, full.YMin, full.YMax);
            Model.PlotControl.Refresh();
        }

        public void SetPlaySpeed(double speed)
        {
            playSpeed = speed <= 0 ? 1 : speed;
        }

        public void ToggleRun()
        {
            if (Model.IsRunning)
                StopPlayback();
            else
                StartPlayback();
        }

        public void CommitTimeText()
        {
            if (!double.TryParse(Model.TimeText, out var seconds))
            {
                Model.TimeText = state.PlayheadTime.Get().ToString("0.000");
                return;
            }

            SetPlayheadSeconds(seconds);
        }

        public void CommitScaleAmplitude()
        {
            if (!double.TryParse(Model.ScaleAmplitudeText, out var amplitude) || amplitude <= 0)
            {
                Model.ScaleAmplitudeText = scaleAmplitude.ToString("0.000");
                return;
            }

            scaleAmplitude = amplitude;
            Model.AutoScale = false;
            captureScaleFromFirstFrame = false;
            RedrawHeatmap();
        }

        public void AutoScaleChanged()
        {
            RedrawHeatmap();
        }

        private void OnExperimentChanged()
        {
            StopPlayback();
            captureScaleFromFirstFrame = true;
            Model.AutoScale = true;
            colorScale = null;
            electrodeMarkers = null;
            chunkBuffer.TryLoad(state.MeaExperiment.Get(), 0);
            SetPlayheadSeconds(0);
        }

        private void OnPlayheadTimeChanged()
        {
            var seconds = state.PlayheadTime.Get();
            Model.TimeText = seconds.ToString("0.000");
            if (panelActive)
                RedrawHeatmap();
        }

        private void OnSelectedChannelsChanged()
        {
            selectedChannels = state.DataSelected.Get().Channels.Keys.ToList();
            ApplySelection(zoomToSelection: true);
            Model.PlotControl?.Refresh();
        }

        private void StartPlayback()
        {
            if (state.MeaExperiment.Get() == null) return;
            playTimer ??= new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000.0 / PlayFps) };
            playTimer.Tick -= OnPlayTick;
            playTimer.Tick += OnPlayTick;
            Model.IsRunning = true;
            playTimer.Start();
        }

        private void StopPlayback()
        {
            playTimer?.Stop();
            Model.IsRunning = false;
        }

        private void OnPlayTick(object sender, EventArgs e)
        {
            var experiment = state.MeaExperiment.Get();
            if (experiment == null)
            {
                StopPlayback();
                return;
            }

            var samplingRate = experiment.DataAcquisitionSettings.SamplingRate;
            var step = Math.Max(1, (long)Math.Round(samplingRate / PlayFps * playSpeed));
            if (!NudgeSamples(step) && !Model.Loop)
                StopPlayback();
        }

        private bool NudgeSamples(long delta)
        {
            var experiment = state.MeaExperiment.Get();
            if (experiment == null) return false;

            var nPoints = (long)experiment.DataAcquisitionSettings.nDataAcquisitionPoints;
            if (nPoints <= 0) return false;

            var samplingRate = experiment.DataAcquisitionSettings.SamplingRate;
            var index = (long)Math.Round(state.PlayheadTime.Get() * samplingRate) + delta;
            var wrapped = false;
            if (index < 0)
            {
                if (!Model.Loop) index = 0;
                else
                {
                    index = (index % nPoints + nPoints) % nPoints;
                    wrapped = true;
                }
            }
            else if (index >= nPoints)
            {
                if (!Model.Loop)
                {
                    SetPlayheadSeconds((nPoints - 1) / samplingRate);
                    return false;
                }

                index %= nPoints;
                wrapped = true;
            }

            SetPlayheadSeconds(index / samplingRate);
            return delta > 0 || wrapped || index > 0;
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
            var duration = nPoints > 0 ? (nPoints - 1) / samplingRate : 0;
            if (duration < 0) duration = 0;
            seconds = Math.Clamp(seconds, 0, duration);
            state.PlayheadTime.Set(seconds);
        }

        private void Zoom(double factor)
        {
            if (Model.PlotControl == null) return;
            MapViewGeometry.ZoomAboutCenter(Model.PlotControl.Plot, factor);
            Model.PlotControl.Refresh();
        }

        private void ApplySelection(bool zoomToSelection)
        {
            if (Model.PlotControl == null) return;
            var experiment = state.MeaExperiment.Get();
            if (experiment == null) return;
            var plot = Model.PlotControl.Plot;
            selectionOverlay = MapViewGeometry.ReplaceSelectionOverlay(plot, selectionOverlay, experiment, selectedChannels);
            if (zoomToSelection && selectedChannels.Count > 0)
            {
                var sel = MapViewGeometry.SelectionLimits(experiment, selectedChannels);
                if (sel != null)
                    MapViewGeometry.SetLimits(plot, sel.Value.XMin, sel.Value.XMax, sel.Value.YMin, sel.Value.YMax);
            }
        }

        private void RedrawHeatmap()
        {
            if (Model.PlotControl == null) return;
            var experiment = state.MeaExperiment.Get();
            if (experiment?.Electrodes == null || experiment.Electrodes.Length == 0)
                return;

            var samplingRate = experiment.DataAcquisitionSettings.SamplingRate;
            var sampleIndex = (ulong)Math.Max(0, Math.Round(state.PlayheadTime.Get() * samplingRate));
            if (!chunkBuffer.TryLoad(experiment, sampleIndex))
                return;

            var lsbMv = experiment.DataAcquisitionSettings.Lsb * 1000;
            var means = chunkBuffer.MeansMv(lsbMv, VoltageZero);

            if (Model.AutoScale || captureScaleFromFirstFrame)
            {
                var a = MaxAbsOccupied(means);
                if (a <= 0) a = 1e-6;
                scaleAmplitude = a;
                Model.ScaleAmplitudeText = scaleAmplitude.ToString("0.000");
                if (captureScaleFromFirstFrame)
                {
                    captureScaleFromFirstFrame = false;
                    Model.AutoScale = false;
                }
            }

            var colors = MapViewGeometry.MapValues(colormap, means, -scaleAmplitude, scaleAmplitude);
            var plot = Model.PlotControl.Plot;
            var firstDraw = electrodeMarkers == null
                            || colorScale == null
                            || !plot.GetPlottables().Contains(colorScale);

            if (firstDraw)
            {
                plot.Clear();
                MapViewGeometry.RemoveColorBars(plot);
                selectionOverlay = null;
                colorScale = MapViewGeometry.AddHiddenColorScale(plot, colormap, -scaleAmplitude, scaleAmplitude);
                MapViewGeometry.GetElectrodePositions(experiment, out var xs, out var ys);
                electrodeMarkers = MapViewGeometry.AddColoredMarkers(plot, xs, ys, colors);
                plot.Axes.Bottom.Label.Text = "x (µm)";
                plot.Axes.Left.Label.Text = "y (µm)";
            }
            else
            {
                MapViewGeometry.SetColorScaleRange(colorScale, colormap, -scaleAmplitude, scaleAmplitude);
                MapViewGeometry.UpdateMarkerColors(electrodeMarkers, colors);
            }

            plot.Title($"t = {state.PlayheadTime.Get():0.000} s");

            if (firstDraw)
            {
                ApplySelection(zoomToSelection: selectedChannels.Count > 0);
                if (selectedChannels.Count == 0)
                {
                    var (xMin, xMax, yMin, yMax) = MapViewGeometry.FullLimits(experiment);
                    MapViewGeometry.SetLimits(plot, xMin, xMax, yMin, yMax);
                }
            }

            Application.Current?.Dispatcher.Invoke(() => Model.PlotControl.Refresh());
        }

        private static double MaxAbsOccupied(double[] values)
        {
            var max = 0.0;
            foreach (var v in values)
            {
                var abs = Math.Abs(v);
                if (abs > max) max = abs;
            }

            return max;
        }

        private void OnPlotMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;
            mouseDown = true;
            mouseDownPos = e.GetPosition(Model.PlotControl);
        }

        private void OnPlotMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!mouseDown || e.ChangedButton != MouseButton.Left) return;
            mouseDown = false;
            var pos = e.GetPosition(Model.PlotControl);
            if (Math.Abs(pos.X - mouseDownPos.X) > 5 || Math.Abs(pos.Y - mouseDownPos.Y) > 5)
                return;
            SelectAt(pos);
        }

        private void SelectAt(Point pos)
        {
            var experiment = state.MeaExperiment.Get();
            if (experiment?.Electrodes == null || Model.PlotControl == null) return;

            var nearest = MapViewGeometry.FindNearestElectrode(
                Model.PlotControl.Plot, (float)pos.X, (float)pos.Y, experiment);
            if (nearest == null) return;

            var channels = new List<int> { nearest.Electrode.Channel };
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control) || Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                channels.AddRange(selectedChannels);

            selectedChannels = channels.Distinct().ToList();
            var dictionary = state.DataSelected.Get();
            if (dictionary.IsListEqualToStateSelectedItems(selectedChannels)) return;
            dictionary.TrimDictionaryToList(selectedChannels);
            state.DataSelected.Set(dictionary);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using MEATaste.DataMEA.MaxWell;
using MEATaste.DataMEA.Models;
using MEATaste.Infrastructure;
using ScottPlot;
using ScottPlot.WPF;

namespace MEATaste.Views.MapVoltage
{
    public class MapVoltageController
    {
        private const double ClickRadiusUm = 30;
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

        private ScottPlot.Plottables.Heatmap heatmapPlottable;

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
            heatmapPlottable = null;
            wpfControl.MouseDown += OnPlotMouseDown;
            try
            {
                wpfControl.UserInputProcessor.IsEnabled = false;
            }
            catch (Exception)
            {
                // ScottPlot versions differ; click-to-select still works.
            }

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

        public void ToggleRun()
        {
            if (Model.IsRunning)
                StopPlayback();
            else
                StartPlayback();
        }

        public void StepSmall(int direction) => NudgeSamples(direction * ChunkSampleCount());

        public void StepLarge(int direction) => NudgeSamples(direction * ChunkSampleCount() * 10);

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
            heatmapPlottable = null;
            chunkBuffer.TryLoad(state.MeaExperiment.Get(), 0);
            SetPlayheadSeconds(0);
        }

        private void OnPlayheadTimeChanged()
        {
            var seconds = state.PlayheadTime.Get();
            Model.TimeText = seconds.ToString("0.000");
            FollowPlayhead(seconds);
            if (panelActive)
                RedrawHeatmap();
        }

        private void OnSelectedChannelsChanged()
        {
            selectedChannels = state.DataSelected.Get().Channels.Keys.ToList();
            if (panelActive)
                RedrawHeatmap();
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
            var step = Math.Max(1, (long)Math.Round(samplingRate / PlayFps));
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

        private int ChunkSampleCount()
        {
            var experiment = state.MeaExperiment.Get();
            if (experiment == null) return 200;
            return Math.Max(1, experiment.DataAcquisitionSettings.chunkSize);
        }

        private void FollowPlayhead(double timeSeconds)
        {
            var axes = state.AxesMaxMin.Get();
            if (axes == null) return;
            if (timeSeconds >= axes.XMin && timeSeconds <= axes.XMax) return;

            var width = axes.XMax - axes.XMin;
            if (width <= 0) return;

            if (timeSeconds > axes.XMax)
                state.AxesMaxMin.Set(new AxesExtrema(timeSeconds - width, timeSeconds, axes.YMin, axes.YMax));
            else
                state.AxesMaxMin.Set(new AxesExtrema(timeSeconds, timeSeconds + width, axes.YMin, axes.YMax));
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
            var (xMin, xMax, yMin, yMax) = GetElectrodeLimits(experiment);
            var intensities = BuildVoltageGrid(experiment, means, xMin, xMax, yMin, yMax, 10);

            if (Model.AutoScale || captureScaleFromFirstFrame)
            {
                var a = MaxAbsOccupied(intensities);
                if (a <= 0) a = 1e-6;
                scaleAmplitude = a;
                Model.ScaleAmplitudeText = scaleAmplitude.ToString("0.000");
                if (captureScaleFromFirstFrame)
                {
                    captureScaleFromFirstFrame = false;
                    Model.AutoScale = false;
                }
            }

            var plot = Model.PlotControl.Plot;
            if (heatmapPlottable == null || !plot.GetPlottables().Contains(heatmapPlottable))
            {
                plot.Clear();
                RemoveColorBars(plot);
                heatmapPlottable = plot.Add.Heatmap(intensities);
                heatmapPlottable.Colormap = colormap;
                plot.Add.ColorBar(heatmapPlottable);
                plot.Axes.Bottom.Label.Text = "x (µm)";
                plot.Axes.Left.Label.Text = "y (µm)";
            }
            else
            {
                heatmapPlottable.Intensities = intensities;
            }

            heatmapPlottable.Colormap = colormap;
            heatmapPlottable.Rectangle = new CoordinateRect(xMin, xMax, yMin, yMax);
            heatmapPlottable.ManualRange = new ScottPlot.Range(-scaleAmplitude, scaleAmplitude);
            plot.Axes.SetLimits(xMin, xMax, yMin, yMax);
            plot.Title($"t = {state.PlayheadTime.Get():0.000} s");

            Application.Current?.Dispatcher.Invoke(() => Model.PlotControl.Refresh());
        }

        private static void RemoveColorBars(Plot plot)
        {
            foreach (var panel in plot.Axes.GetPanels().ToArray())
            {
                if (panel.GetType().Name.Contains("ColorBar", StringComparison.OrdinalIgnoreCase))
                    plot.Axes.Remove(panel);
            }
        }

        private static double[,] BuildVoltageGrid(
            MeaExperiment experiment,
            double[] means,
            double xMin,
            double xMax,
            double yMin,
            double yMax,
            double step)
        {
            var xItems = (int)((xMax - xMin) / step) + 1;
            var yItems = (int)((yMax - yMin) / step) + 1;
            var intensities = new double[xItems, yItems];

            for (var e = 0; e < experiment.Electrodes.Length; e++)
            {
                var electrode = experiment.Electrodes[e].Electrode;
                var i = (int)((electrode.XuM - xMin) / step);
                var j = (int)((electrode.YuM - yMin) / step);
                if (i < 0 || j < 0 || i >= xItems || j >= yItems) continue;
                intensities[i, j] = means[e];
            }

            return intensities;
        }

        private static double MaxAbsOccupied(double[,] values)
        {
            var max = 0.0;
            foreach (var v in values)
            {
                var abs = Math.Abs(v);
                if (abs > max) max = abs;
            }

            return max;
        }

        private static (double, double, double, double) GetElectrodeLimits(MeaExperiment experiment)
        {
            var first = experiment.Electrodes[0].Electrode;
            var xMin = first.XuM;
            var xMax = first.XuM;
            var yMin = first.YuM;
            var yMax = first.YuM;
            foreach (var electrode in experiment.Electrodes)
            {
                if (xMin > electrode.Electrode.XuM) xMin = electrode.Electrode.XuM;
                if (yMin > electrode.Electrode.YuM) yMin = electrode.Electrode.YuM;
                if (xMax < electrode.Electrode.XuM) xMax = electrode.Electrode.XuM;
                if (yMax < electrode.Electrode.YuM) yMax = electrode.Electrode.YuM;
            }

            return (xMin, xMax, yMin, yMax);
        }

        private void OnPlotMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;
            var experiment = state.MeaExperiment.Get();
            if (experiment?.Electrodes == null || Model.PlotControl == null) return;

            var pos = e.GetPosition(Model.PlotControl);
            Coordinates coord;
            try
            {
                coord = Model.PlotControl.Plot.GetCoordinates((float)pos.X, (float)pos.Y);
            }
            catch (Exception)
            {
                return;
            }

            var nearest = FindNearestElectrode(experiment, coord.X, coord.Y);
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

        private static ElectrodeData FindNearestElectrode(MeaExperiment experiment, double xUm, double yUm)
        {
            ElectrodeData nearest = null;
            var best = ClickRadiusUm * ClickRadiusUm;
            foreach (var electrodeData in experiment.Electrodes)
            {
                var dx = electrodeData.Electrode.XuM - xUm;
                var dy = electrodeData.Electrode.YuM - yUm;
                var d2 = dx * dx + dy * dy;
                if (d2 >= best) continue;
                best = d2;
                nearest = electrodeData;
            }

            return nearest;
        }
    }
}

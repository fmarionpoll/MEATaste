using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MEATaste.DataMEA.Models;
using MEATaste.Infrastructure;
using MEATaste.Views;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.WPF;

namespace MEATaste.Views.MapHeatscale
{
    class MapHeatscalelController
    {
        private const double WindowSeconds = 0.1;

        public MapHeatscalelModel Model { get; }
        private readonly ApplicationState state;
        private readonly IColormap colormap = new ScottPlot.Colormaps.Turbo();
        public List<int> SelectedChannels { get; private set; } = new();
        private Scatter selectionOverlay;
        private Heatmap colorScale;
        private Marker[] electrodeMarkers;
        private double scaleMax = 1;
        private bool panelActive;
        private Point mouseDownPos;
        private bool mouseDown;

        public MapHeatscalelController(
            ApplicationState state,
            IEventSubscriber eventSubscriber)
        {
            this.state = state;

            Model = new MapHeatscalelModel();
            eventSubscriber.Subscribe(EventType.MeaExperimentChanged, OnExperimentChanged);
            eventSubscriber.Subscribe(EventType.SelectedChannelsChanged, ChangeSelectedElectrode);
            eventSubscriber.Subscribe(EventType.PlayheadTimeChanged, OnPlayheadTimeChanged);
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
                Redraw();
        }

        public void SetActive(bool active)
        {
            panelActive = active;
            if (active && state.MeaExperiment.Get() != null)
                Redraw();
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

        private void OnExperimentChanged()
        {
            colorScale = null;
            electrodeMarkers = null;
            selectionOverlay = null;
            scaleMax = MaxTumblingBinCount(state.MeaExperiment.Get(), WindowSeconds);
            if (panelActive)
                Redraw();
        }

        private void OnPlayheadTimeChanged()
        {
            if (panelActive)
                Redraw();
        }

        private void ChangeSelectedElectrode()
        {
            SelectedChannels = state.DataSelected.Get().Channels.Keys.ToList();
            ApplySelection(zoomToSelection: true);
            Model.PlotControl?.Refresh();
        }

        private void Redraw()
        {
            if (Model.PlotControl == null) return;
            var experiment = state.MeaExperiment.Get();
            if (experiment?.Electrodes == null || experiment.Electrodes.Length == 0)
                return;

            var values = CountSpikesInWindow(experiment, state.PlayheadTime.Get(), WindowSeconds);
            var colors = MapViewGeometry.MapValues(colormap, values, 0, scaleMax);
            var plot = Model.PlotControl.Plot;
            var firstDraw = electrodeMarkers == null
                            || colorScale == null
                            || !plot.GetPlottables().Contains(colorScale);

            if (firstDraw)
            {
                plot.Clear();
                MapViewGeometry.RemoveColorBars(plot);
                selectionOverlay = null;
                colorScale = MapViewGeometry.AddHiddenColorScale(plot, colormap, 0, scaleMax);
                MapViewGeometry.GetElectrodePositions(experiment, out var xs, out var ys);
                electrodeMarkers = MapViewGeometry.AddColoredMarkers(plot, xs, ys, colors);
                plot.Axes.Bottom.Label.Text = "x (µm)";
                plot.Axes.Left.Label.Text = "y (µm)";
            }
            else
            {
                MapViewGeometry.UpdateMarkerColors(electrodeMarkers, colors);
            }

            plot.Title($"t = {state.PlayheadTime.Get():0.000} s  (±{WindowSeconds * 500:0} ms)");

            if (firstDraw)
            {
                ApplySelection(zoomToSelection: SelectedChannels.Count > 0);
                if (SelectedChannels.Count == 0)
                {
                    var (xMin, xMax, yMin, yMax) = MapViewGeometry.FullLimits(experiment);
                    MapViewGeometry.SetLimits(plot, xMin, xMax, yMin, yMax);
                }
            }

            Application.Current.Dispatcher.Invoke(() => { Model.PlotControl.Refresh(); });
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
            selectionOverlay = MapViewGeometry.ReplaceSelectionOverlay(plot, selectionOverlay, experiment, SelectedChannels);
            if (zoomToSelection && SelectedChannels.Count > 0)
            {
                var sel = MapViewGeometry.SelectionLimits(experiment, SelectedChannels);
                if (sel != null)
                    MapViewGeometry.SetLimits(plot, sel.Value.XMin, sel.Value.XMax, sel.Value.YMin, sel.Value.YMax);
            }
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
                channels.AddRange(SelectedChannels);
            var selected = channels.Distinct().ToList();
            var dictionary = state.DataSelected.Get();
            if (dictionary.IsListEqualToStateSelectedItems(selected)) return;
            dictionary.TrimDictionaryToList(selected);
            state.DataSelected.Set(dictionary);
        }

        private static double[] CountSpikesInWindow(MeaExperiment experiment, double timeSeconds, double windowSeconds)
        {
            var values = new double[experiment.Electrodes.Length];
            GetWindowFrames(experiment, timeSeconds, windowSeconds, out var frameMin, out var frameMax);
            for (var i = 0; i < experiment.Electrodes.Length; i++)
                values[i] = CountSpikesInFrameRange(experiment.Electrodes[i].SpikeTimes, frameMin, frameMax);
            return values;
        }

        private static void GetWindowFrames(
            MeaExperiment experiment,
            double timeSeconds,
            double windowSeconds,
            out long frameMin,
            out long frameMax)
        {
            var samplingRate = experiment.DataAcquisitionSettings.SamplingRate;
            var nPoints = (long)experiment.DataAcquisitionSettings.nDataAcquisitionPoints;
            var half = windowSeconds / 2;
            frameMin = (long)Math.Round((timeSeconds - half) * samplingRate);
            frameMax = (long)Math.Round((timeSeconds + half) * samplingRate);
            if (nPoints <= 0) return;
            var last = nPoints - 1;
            frameMin = Math.Clamp(frameMin, 0, last);
            frameMax = Math.Clamp(frameMax, 0, last);
        }

        private static int CountSpikesInFrameRange(List<SpikeDetected> spikes, long frameMin, long frameMax)
        {
            if (spikes == null || spikes.Count == 0 || frameMax < frameMin)
                return 0;
            return Math.Max(0, UpperBound(spikes, frameMax) - LowerBound(spikes, frameMin));
        }

        private static double MaxTumblingBinCount(MeaExperiment experiment, double windowSeconds)
        {
            if (experiment?.Electrodes == null)
                return 1;

            var samplingRate = experiment.DataAcquisitionSettings.SamplingRate;
            var binFrames = Math.Max(1L, (long)Math.Round(samplingRate * windowSeconds));
            var max = 1.0;
            foreach (var electrode in experiment.Electrodes)
            {
                var spikes = electrode.SpikeTimes;
                if (spikes == null || spikes.Count == 0) continue;
                spikes.Sort((a, b) => a.Frameno.CompareTo(b.Frameno));

                var currentBin = spikes[0].Frameno / binFrames;
                var count = 0;
                foreach (var spike in spikes)
                {
                    var bin = spike.Frameno / binFrames;
                    if (bin != currentBin)
                    {
                        if (count > max) max = count;
                        currentBin = bin;
                        count = 1;
                    }
                    else
                    {
                        count++;
                    }
                }

                if (count > max) max = count;
            }

            return max;
        }

        private static int LowerBound(List<SpikeDetected> spikes, long frame)
        {
            var lo = 0;
            var hi = spikes.Count;
            while (lo < hi)
            {
                var mid = lo + (hi - lo) / 2;
                if (spikes[mid].Frameno < frame) lo = mid + 1;
                else hi = mid;
            }

            return lo;
        }

        private static int UpperBound(List<SpikeDetected> spikes, long frame)
        {
            var lo = 0;
            var hi = spikes.Count;
            while (lo < hi)
            {
                var mid = lo + (hi - lo) / 2;
                if (spikes[mid].Frameno <= frame) lo = mid + 1;
                else hi = mid;
            }

            return lo;
        }
    }
}

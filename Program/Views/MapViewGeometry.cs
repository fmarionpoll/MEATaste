using System;
using System.Collections.Generic;
using System.Linq;
using MEATaste.DataMEA.Models;
using ScottPlot;
using ScottPlot.Plottables;

namespace MEATaste.Views
{
    public static class MapViewGeometry
    {
        public const double SelectionPadUm = 150;
        public const double ZoomFactor = 0.7;
        public const float ElectrodeMarkerSize = 10;
        public const float ClickRadiusPx = 15;

        public static (double XMin, double XMax, double YMin, double YMax) FullLimits(MeaExperiment experiment)
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

        public static (double XMin, double XMax, double YMin, double YMax)? SelectionLimits(
            MeaExperiment experiment,
            IReadOnlyList<int> channels)
        {
            if (experiment?.Electrodes == null || channels == null || channels.Count == 0)
                return null;

            double? xMin = null, xMax = null, yMin = null, yMax = null;
            foreach (var channel in channels)
            {
                var match = experiment.Electrodes.FirstOrDefault(e => e.Electrode.Channel == channel);
                if (match == null) continue;
                var x = match.Electrode.XuM;
                var y = match.Electrode.YuM;
                xMin = xMin is null ? x : Math.Min(xMin.Value, x);
                xMax = xMax is null ? x : Math.Max(xMax.Value, x);
                yMin = yMin is null ? y : Math.Min(yMin.Value, y);
                yMax = yMax is null ? y : Math.Max(yMax.Value, y);
            }

            if (xMin is null)
                return null;

            var span = Math.Max(xMax.Value - xMin.Value, yMax.Value - yMin.Value);
            return (xMin.Value - SelectionPadUm, xMin.Value + span + SelectionPadUm,
                yMin.Value - SelectionPadUm, yMin.Value + span + SelectionPadUm);
        }

        public static void SetLimits(Plot plot, double xMin, double xMax, double yMin, double yMax)
        {
            plot.Axes.SetLimits(xMin, xMax, yMin, yMax);
        }

        public static void ZoomAboutCenter(Plot plot, double factor)
        {
            var limits = plot.Axes.GetLimits();
            var cx = (limits.Left + limits.Right) / 2;
            var cy = (limits.Bottom + limits.Top) / 2;
            var hx = (limits.Right - limits.Left) * factor / 2;
            var hy = (limits.Top - limits.Bottom) * factor / 2;
            plot.Axes.SetLimits(cx - hx, cx + hx, cy - hy, cy + hy);
        }

        public static void RemoveColorBars(Plot plot)
        {
            foreach (var panel in plot.Axes.GetPanels().ToArray())
            {
                if (panel.GetType().Name.Contains("ColorBar", StringComparison.OrdinalIgnoreCase))
                    plot.Axes.Remove(panel);
            }
        }

        public static Scatter ReplaceSelectionOverlay(
            Plot plot,
            Scatter overlay,
            MeaExperiment experiment,
            IReadOnlyList<int> channels)
        {
            if (overlay != null)
                plot.Remove(overlay);

            if (experiment?.Electrodes == null || channels == null || channels.Count == 0)
                return null;

            var xs = new List<double>();
            var ys = new List<double>();
            foreach (var channel in channels)
            {
                var match = experiment.Electrodes.FirstOrDefault(e => e.Electrode.Channel == channel);
                if (match == null) continue;
                xs.Add(match.Electrode.XuM);
                ys.Add(match.Electrode.YuM);
            }

            if (xs.Count == 0)
                return null;

            var scatter = plot.Add.Scatter(xs, ys);
            scatter.MarkerShape = MarkerShape.OpenCircle;
            scatter.MarkerSize = 12;
            scatter.MarkerLineWidth = 2;
            scatter.MarkerLineColor = Colors.Red;
            scatter.LineWidth = 0;
            return scatter;
        }

        public static void GetElectrodePositions(MeaExperiment experiment, out double[] xs, out double[] ys)
        {
            var n = experiment.Electrodes.Length;
            xs = new double[n];
            ys = new double[n];
            for (var i = 0; i < n; i++)
            {
                xs[i] = experiment.Electrodes[i].Electrode.XuM;
                ys[i] = experiment.Electrodes[i].Electrode.YuM;
            }
        }

        public static Color MapValue(IColormap colormap, double value, double min, double max)
        {
            if (double.IsNaN(value))
                return colormap.GetColor(double.NaN);
            if (max <= min)
                return colormap.GetColor(0.5);
            return colormap.GetColor((value - min) / (max - min));
        }

        public static Color[] MapValues(IColormap colormap, double[] values, double min, double max)
        {
            var colors = new Color[values.Length];
            for (var i = 0; i < values.Length; i++)
                colors[i] = MapValue(colormap, values[i], min, max);
            return colors;
        }

        public static Marker[] AddColoredMarkers(Plot plot, double[] xs, double[] ys, Color[] colors)
        {
            var markers = new Marker[xs.Length];
            for (var i = 0; i < xs.Length; i++)
            {
                var marker = plot.Add.Marker(xs[i], ys[i]);
                marker.Shape = MarkerShape.FilledCircle;
                marker.Size = ElectrodeMarkerSize;
                marker.Color = colors[i];
                markers[i] = marker;
            }

            return markers;
        }

        public static void UpdateMarkerColors(Marker[] markers, Color[] colors)
        {
            for (var i = 0; i < markers.Length; i++)
                markers[i].Color = colors[i];
        }

        public static Heatmap AddHiddenColorScale(Plot plot, IColormap colormap, double min, double max)
        {
            var heatmap = plot.Add.Heatmap(new[,] { { min, max } });
            heatmap.IsVisible = false;
            heatmap.Colormap = colormap;
            heatmap.ManualRange = new ScottPlot.Range(min, max);
            plot.Add.ColorBar(heatmap);
            return heatmap;
        }

        public static void SetColorScaleRange(Heatmap heatmap, IColormap colormap, double min, double max)
        {
            heatmap.Colormap = colormap;
            heatmap.ManualRange = new ScottPlot.Range(min, max);
            heatmap.Intensities = new[,] { { min, max } };
        }

        public static ElectrodeData FindNearestElectrode(Plot plot, float pixelX, float pixelY, MeaExperiment experiment)
        {
            if (experiment?.Electrodes == null || experiment.Electrodes.Length == 0)
                return null;

            Pixel mouse;
            try
            {
                mouse = plot.GetPixel(plot.GetCoordinates(pixelX, pixelY));
            }
            catch (Exception)
            {
                mouse = new Pixel(pixelX, pixelY);
            }

            ElectrodeData nearest = null;
            var best = ClickRadiusPx * ClickRadiusPx;
            foreach (var electrodeData in experiment.Electrodes)
            {
                Pixel pixel;
                try
                {
                    pixel = plot.GetPixel(new Coordinates(electrodeData.Electrode.XuM, electrodeData.Electrode.YuM));
                }
                catch (Exception)
                {
                    continue;
                }

                var dx = pixel.X - mouse.X;
                var dy = pixel.Y - mouse.Y;
                var d2 = dx * dx + dy * dy;
                if (d2 >= best) continue;
                best = d2;
                nearest = electrodeData;
            }

            return nearest;
        }
    }
}

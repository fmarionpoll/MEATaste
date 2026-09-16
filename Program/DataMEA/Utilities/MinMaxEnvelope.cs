using System;

namespace MEATaste.DataMEA.Utilities
{
    public static class MinMaxEnvelope
    {
        public static void Build(
            double[] data,
            double samplingRate,
            double xMin,
            double xMax,
            int pixelWidth,
            out double[] times,
            out double[] values)
        {
            times = Array.Empty<double>();
            values = Array.Empty<double>();
            if (data == null || data.Length == 0 || samplingRate <= 0)
                return;

            var startIndex = (int)Math.Floor(xMin * samplingRate);
            var endIndex = (int)Math.Ceiling(xMax * samplingRate);
            if (startIndex < 0) startIndex = 0;
            if (endIndex >= data.Length) endIndex = data.Length - 1;
            if (endIndex < startIndex)
                return;

            var nSamples = endIndex - startIndex + 1;
            if (pixelWidth < 2)
                pixelWidth = 2;

            var samplesPerPixel = nSamples / (double)pixelWidth;
            if (samplesPerPixel <= 2)
            {
                times = new double[nSamples];
                values = new double[nSamples];
                for (var i = 0; i < nSamples; i++)
                {
                    times[i] = (startIndex + i) / samplingRate;
                    values[i] = data[startIndex + i];
                }
                return;
            }

            times = new double[pixelWidth * 2];
            values = new double[pixelWidth * 2];
            for (var p = 0; p < pixelWidth; p++)
            {
                var i0 = startIndex + (int)(p * samplesPerPixel);
                var i1 = startIndex + (int)((p + 1) * samplesPerPixel) - 1;
                if (i0 > endIndex) i0 = endIndex;
                if (i1 > endIndex) i1 = endIndex;
                if (i1 < i0) i1 = i0;

                var min = data[i0];
                var max = data[i0];
                for (var i = i0 + 1; i <= i1; i++)
                {
                    var v = data[i];
                    if (v < min) min = v;
                    if (v > max) max = v;
                }

                var t0 = i0 / samplingRate;
                var t1 = i1 / samplingRate;
                var o = p * 2;
                times[o] = t0;
                values[o] = max;
                times[o + 1] = t1;
                values[o + 1] = min;
            }
        }
    }
}

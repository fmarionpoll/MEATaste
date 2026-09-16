using System;
using System.Linq;
using MEATaste.DataMEA.Models;

namespace MEATaste.DataMEA.MaxWell
{
    public class VoltageChunkBuffer
    {
        public int[] Channels { get; private set; } = Array.Empty<int>();
        public ushort[] Data { get; private set; } = Array.Empty<ushort>();
        public ulong Start { get; private set; }
        public int Length { get; private set; }

        public bool TryLoad(MeaExperiment experiment, ulong sampleIndex)
        {
            if (experiment?.Electrodes == null || experiment.Electrodes.Length == 0)
                return false;

            var chunk = (ulong)Math.Max(1, experiment.DataAcquisitionSettings.chunkSize);
            var nPoints = experiment.DataAcquisitionSettings.nDataAcquisitionPoints;
            if (nPoints == 0)
                return false;

            if (sampleIndex >= nPoints)
                sampleIndex = nPoints - 1;

            var aligned = sampleIndex / chunk * chunk;
            if (Data.Length > 0
                && Start == aligned
                && Channels.Length == experiment.Electrodes.Length)
                return true;

            Channels = experiment.Electrodes.Select(e => e.Electrode.Channel).ToArray();
            var end = aligned + chunk - 1;
            if (end >= nPoints)
                end = nPoints - 1;

            Data = H5FileReader.ReadDataPartChannels(Channels, aligned, end);
            Start = aligned;
            Length = (int)(end - aligned + 1);
            return Length > 0 && Data.Length >= Channels.Length * Length;
        }

        public double[] MeansMv(double lsbMilliVolt, ushort zero)
        {
            var means = new double[Channels.Length];
            if (Length <= 0)
                return means;

            for (var c = 0; c < Channels.Length; c++)
            {
                double sum = 0;
                var offset = c * Length;
                for (var s = 0; s < Length; s++)
                    sum += Data[offset + s];
                means[c] = (sum / Length - zero) * lsbMilliVolt;
            }

            return means;
        }
    }
}

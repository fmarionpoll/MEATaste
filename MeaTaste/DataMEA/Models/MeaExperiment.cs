using System;
using System.Collections.Generic;
using System.Linq;

namespace MeaTaste.DataMEA.Models
{
    // Models

    public record MeaExperiment
    {
        public string FileName { get; set; }
        public string FileVersion { get; set; } = "unknown";
        public Descriptors Descriptors;

        public MeaExperiment(string fileName, Descriptors descriptors)
        {
            FileName = fileName;
            Descriptors = descriptors;
        }
    }

    public class Descriptors
    {
        // time
        public DateTime TimeStart { get; set; }
        public DateTime TimeStop { get; set; }
        public double SamplingRate = 20000;
        // settings
        public double Gain { get; set; }
        public double Hpf { get; set; }
        public double Lsb { get; set; }
        // mapping
        public Electrode[] Electrodes { get; set; }

        public int[] GetChannelNumbers() =>
            Electrodes.Select(electrode => electrode.ChannelNumber).ToArray();

        public double[] GetArray_electrodes_XPos()
        {
            double[] xPos = new double[Electrodes.Length];
            for (int i=0; i< Electrodes.Length; i++)
                xPos[i] = Electrodes[i].XCoordinates_um;
            return xPos;
        }

        public double[] GetArray_electrodes_YPos()
        {
            double[] yPos = new double[Electrodes.Length];
            for (int i = 0; i < Electrodes.Length; i++)
                yPos[i] = Electrodes[i].YCoordinates_um;
            return yPos;
        }
    }

    public record Electrode(
        int ChannelNumber,
        int ElectrodeNumber,
        double XCoordinates_um,
        double YCoordinates_um);

}

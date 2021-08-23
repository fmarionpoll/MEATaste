using System;
using System.Collections.Generic;
using System.Linq;

namespace MEATaste.DataMEA.Models
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

        public int[] GetElectrodesChannelNumber() =>
            Electrodes.Select(electrode => electrode.ChannelNumber).ToArray();

        public int[] GetElectrodesElectrodeNumber() =>
            Electrodes.Select(electrode => electrode.ElectrodeNumber).ToArray();

        public double[] GetElectrodesXCoordinate() =>
            Electrodes.Select(electrode => electrode.XCoordinate).ToArray();
        
        public double[] GetElectrodesYCoordinate() =>
            Electrodes.Select(electrode => electrode.YCoordinate).ToArray();
      
    }

    public record Electrode(
        int ChannelNumber,
        int ElectrodeNumber,
        double XCoordinate,
        double YCoordinate);

}

using System;
using System.ComponentModel.DataAnnotations;


namespace MEATaste.DataMEA.Models
{
    // Models

    public record MeaExperiment
    {
        public string FileName { get; set; }
        public string FileVersion { get; set; } = "unknown";
        public ushort[] rawSignalUShort;
        public double[] rawSignalDouble;

        public Descriptors Descriptors;

        public MeaExperiment(string fileName, string fileVersion, Descriptors descriptors)
        {
            FileName = fileName;
            if (fileVersion != null)
                FileVersion = fileVersion;
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
        public ElectrodeRecord[] Electrodes { get; set; }

    }

    public record ElectrodeRecord(
        int Channel,
        int Electrode,
        double X_uM,
        double Y_uM);

}

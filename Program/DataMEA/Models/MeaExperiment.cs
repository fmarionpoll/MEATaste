using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MEATaste.DataMEA.Models
{
    // Models

    public record MeaExperiment
    {
        public string FileName { get; set; }
        public string FileVersion { get; set; } = "unknown";
        public ushort[] rawSignalUShort;
        public double[] rawSignalDouble;
        public AxesMaxMin axesMaxMin;

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
        double XuM,
        double YuM);

    public record AxesMaxMin(
        double XMin,
        double XMax,
        double YMin,
        double YMax);
}

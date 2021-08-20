using System;


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

    public record Descriptors
    {
        // time
        public DateTime TimeStart;
        public DateTime TimeStop;
        public double SamplingRate = 20000;
        // settings
        public double Gain;
        public double Hpf;
        public double Lsb;
        // mapping
        public Electrode[] electrodes;
    }

    public record Electrode(
        int ChannelNumber,
        int ElectrodeNumber,
        double XCoordinates_um,
        double YCoordinates_um);

}

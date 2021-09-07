using System;


namespace MEATaste.DataMEA.Models
{
    // Models

    public record MeaExperiment
    {
        public string FileName { get; set; }
        public string FileVersion { get; set; } = "unknown";
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
        public ElectrodeProperties[] Electrodes { get; set; }
    }

   

 
}

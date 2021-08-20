using System;
using System.Collections.Generic;

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
        public Electrode[] electrodes { get; set; }

        public List<string> electrodeChannels()
        {
            List<string> listChannels = new List<string>(electrodes.Length);
            foreach (Electrode element in electrodes)
                listChannels.Add(element.ChannelNumber.ToString());
            return listChannels;
        }
    }

    public record Electrode(
        int ChannelNumber,
        int ElectrodeNumber,
        double XCoordinates_um,
        double YCoordinates_um);

}

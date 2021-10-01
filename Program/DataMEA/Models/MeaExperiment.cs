
namespace MEATaste.DataMEA.Models
{
    public record MeaExperiment
    {
        public string FileName { get; set; }
        public string FileVersion { get; set; } = "unknown";
        public DataAcquisitionSettings DataAcquisitionSettings;
        public ElectrodeProperties[] Electrodes { get; set; }
        public SpikeDetected[] SpikeTimes { get; set; }

        public MeaExperiment(string fileName, string fileVersion, DataAcquisitionSettings dataAcquisitionSettings)
        {
            FileName = fileName;
            if (fileVersion != null)
                FileVersion = fileVersion;
            DataAcquisitionSettings = dataAcquisitionSettings;
        }
    }
}

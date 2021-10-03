
using System.Collections.Generic;

namespace MEATaste.DataMEA.Models
{
    public class MeaExperiment
    {
        public string FileName { get; set; }
        public string FileVersion { get; set; } = "unknown";
        public DataAcquisitionSettings DataAcquisitionSettings;
        public ElectrodeData[] Electrodes { get; set; }

        public MeaExperiment(string fileName, string fileVersion, DataAcquisitionSettings dataAcquisitionSettings)
        {
            FileName = fileName;
            if (fileVersion != null)
                FileVersion = fileVersion;
            DataAcquisitionSettings = dataAcquisitionSettings;
        }
    }
}

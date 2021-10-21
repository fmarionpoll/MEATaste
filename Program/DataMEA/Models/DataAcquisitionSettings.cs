using System;

namespace MEATaste.DataMEA.Models
{
    public class DataAcquisitionSettings
    {
        // time
        public DateTime TimeStart { get; set; }
        public DateTime TimeStop { get; set; }
        public double SamplingRate = 20000;
        // acquisition settings
        public double Gain { get; set; }
        public double Hpf { get; set; }
        public double Lsb { get; set; }
        // acquisition length
        public ulong nDataAcquisitionPoints { get; set; }
        public ulong nDataAcquisitionChannels { get; set; }
        public int chunkSize { get; set; }
    }
}
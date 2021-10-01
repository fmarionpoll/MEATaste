

namespace MEATaste.DataMEA.Models
{
    public class ElectrodeData
    {
        public ushort[] RawSignalUShort { get; set; }
        public double[] RawSignalDouble { get; set; }
        public long[]   SpikeTime { get; set; }
    }
}



namespace MEATaste.DataMEA.Models
{
    public class ElectrodeDataBuffer
    {
        public ushort[] RawSignalUShort { get; set; }
        public double[] RawSignalDouble { get; set; }
    }

    public record AxesExtrema(
        double XMin,
        double XMax,
        double YMin,
        double YMax);

    public record ElectrodeProperties(
        int Electrode,
        int Channel,
        double XuM,
        double YuM);
}

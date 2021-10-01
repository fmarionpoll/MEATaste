namespace MEATaste.DataMEA.Models
{
    public record SpikeDetected(
        long Frameno,
        int Channel,
        double Amplitude);
}

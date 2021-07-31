

namespace MeaTaste.Domain.MeaData.Models
{
    public class MeasureFileData
    {
        public MeaExperiment[] Points { get; set; }
    }

    public record Mapping(int Channel, int Electrode, double X, double Y);
    public record SpikeTime(long FrameNo, int Channel, double Amplitude);
    public record Gain(int Channel, double GainChannel);
    public record Hpf(int Channel, double HpfChannel);
    public record Lsb(int Channel, double LsbChannel);
    public record Time(int Channel, double TimeChannel);
    public record Version(int Channel, int VersionChannel);

    public record Wellplate(
        string Version,
        int Id,
        string Name, 
        Well[] Well);
    
    public record Well(
        IdWell[] IdWell,
        string CellType, 
        ControlWell[] Control, 
        string Group_color, 
        string Group_name, 
       string Name);

    public record ControlWell(int Channel, int Item);
    public record IdWell(int Channel, int Item);
}

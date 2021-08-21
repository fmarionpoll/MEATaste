

namespace TasteMEA.DataMEA.Models
{
    public class MEAWellPlate
    {
        public MeaExperiment[] Points { get; set; }
    }


    public record WellPlate(
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

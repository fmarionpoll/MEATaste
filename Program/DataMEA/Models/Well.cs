namespace MEATaste.DataMEA.Models
{
    public record Well(
        WellId[] IdWell,
        string CellType, 
        WellControl[] Control, 
        string GroupColor, 
        string GroupName, 
        string Name);
}
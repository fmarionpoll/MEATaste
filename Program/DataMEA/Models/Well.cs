namespace MEATaste.DataMEA.Models
{
    public record Well(
        IdWell[] IdWell,
        string CellType, 
        ControlWell[] Control, 
        string Group_color, 
        string Group_name, 
        string Name);
}
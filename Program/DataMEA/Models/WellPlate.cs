namespace MEATaste.DataMEA.Models
{
    public record WellPlate(
        string Version,
        int Id,
        string Name, 
        Well[] Well);
}
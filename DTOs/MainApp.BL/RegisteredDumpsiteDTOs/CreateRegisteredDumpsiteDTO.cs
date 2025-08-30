namespace DTOs.MainApp.BL.RegisteredDumpsiteDTOs;

public class CreateRegisteredDumpsiteDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsEnabled { get; set; }
    public string EnteredZonePolygon { get; set; }
    public Guid? RegisteredDumpsiteWasteTypeId { get; set; }
    public Guid? RegisteredDumpsiteRiskLevelId { get; set; }
    public string CreatedById { get; set; }
}

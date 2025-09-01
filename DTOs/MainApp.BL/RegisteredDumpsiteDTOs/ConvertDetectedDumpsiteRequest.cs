namespace DTOs.MainApp.BL.RegisteredDumpsiteDTOs;
public class ConvertDetectedDumpsiteRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid RegisteredDumpsiteWasteTypeId { get; set; }
    public Guid RegisteredDumpsiteRiskLevelId { get; set; }
    public string EnteredZonePolygon { get; set; } // GeoJSON string
    public bool IsEnabled { get; set; }
    public Guid? SourceDetectedDumpsiteId { get; set; } // Optional: to track the source
    public string ConversionNotes { get; set; }
}

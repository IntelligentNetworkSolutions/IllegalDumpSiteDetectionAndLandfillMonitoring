namespace DTOs.MainApp.BL.RegisteredDumpsiteDTOs;
public class CompleteInspectionDTO
{
    public Guid InspectionId { get; set; }
    public string Findings { get; set; } = string.Empty;
    public string? Recommendations { get; set; }
    public string? Notes { get; set; }
}

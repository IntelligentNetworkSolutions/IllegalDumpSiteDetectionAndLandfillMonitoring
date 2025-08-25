using System.ComponentModel.DataAnnotations;

namespace DTOs.MainApp.BL.RegisteredDumpsiteDTOs;

public class CreateRegisteredDumpsiteDTO
{
    [Required]
    public string Name { get; set; }
    public string? Description { get; set; }
    public string EnteredZonePolygon { get; set; }
    public bool IsEnabled { get; set; }
}

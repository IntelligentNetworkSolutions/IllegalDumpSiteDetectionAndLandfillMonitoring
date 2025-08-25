using DTOs.Helpers;
using DTOs.MainApp.BL;
using DTOs.MainApp.BL.RegisteredDumpsiteDTOs;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MainApp.MVC.ViewModels.IntranetPortal.RegisteredDumpsite;

public class RegisteredDumpsiteViewModel
{
    public Guid? Id { get; set; }

    [Required]
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool IsEnabled { get; set; }

    [JsonIgnore]
    public Polygon? Geom { get; set; }

    public string GeoJson
    {
        get
        {
            return GeoJsonHelpers.GeometryToGeoJson(Geom);
        }
    }
    public string? EnteredZonePolygon { get; set; }
    public string? CreatedById { get; set; }
    public DateTime? CreatedOn { get; set; } = DateTime.UtcNow;
    public virtual UserDTO? CreatedBy { get; set; }

    public Guid? RegisteredDumpsiteWasteTypeId { get; set; }
    public virtual RegisteredDumpsiteWasteTypeDTO? RegisteredDumpsiteType { get; set; }
    public Guid? RegisteredDumpsiteRiskLevelId { get; set; }
    public virtual RegisteredDumpsiteRiskLevelDTO? RegisteredDumpsiteRiskLevelType { get; set; }

}

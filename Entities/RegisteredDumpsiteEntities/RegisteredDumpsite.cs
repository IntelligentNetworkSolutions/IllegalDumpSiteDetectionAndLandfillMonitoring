using Entities.Intefaces;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities.RegisteredDumpsiteEntities;

public class RegisteredDumpsite : BaseEntity<Guid>, ICreatedByUser
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool IsEnabled { get; set; }
    public DateTime? DateOfResolvment { get; set; }

    [JsonIgnore]
    [Column(TypeName = "geometry(Polygon)")]
    public Polygon Geom { get; set; }

    [NotMapped]
    public string GeoJson { get; set; }
    public string CreatedById { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public ApplicationUser? CreatedBy { get; set; }

    public Guid RegisteredDumpsiteWasteTypeId { get; set; }
    public virtual RegisteredDumpsiteWasteType? RegisteredDumpsiteWasteType { get; set; }

    public int RegisteredDumpsiteStatusId { get; set; }
    public virtual RegisteredDumpsiteStatus? RegisteredDumpsiteStatus { get; set; }

    public Guid RegisteredDumpsiteRiskLevelId { get; set; }
    public virtual RegisteredDumpsiteRiskLevel? RegisteredDumpsiteRiskLevel { get; set; }

    public virtual ICollection<RegisteredDumpsiteFile> DumpsiteFiles { get; set; } = new List<RegisteredDumpsiteFile>();
    public virtual ICollection<RegisteredDumpsiteInspection> Inspections { get; set; } = new List<RegisteredDumpsiteInspection>();
}

using System.ComponentModel.DataAnnotations.Schema;
using Entities.Intefaces;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;

namespace Entities.DetectionEntities
{
    public class DetectionIgnoreZone : BaseEntity<Guid>, ICreatedByUser
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsEnabled { get; set; }

        [JsonIgnore]
        [Column(TypeName = "geometry(Polygon)")]
        public Polygon Geom { get; set; }

        [NotMapped]
        public string GeoJson { get; set; }

        public string CreatedById { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public virtual ApplicationUser? CreatedBy { get; set; }
    }
}

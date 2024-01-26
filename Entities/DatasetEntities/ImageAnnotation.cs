using System.ComponentModel.DataAnnotations;
using Entities.Intefaces;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;

namespace Entities.DatasetEntities
{
    public class ImageAnnotation : BaseEntity<Guid>, ICreatedByUser, IUpdatedByUser
    {
        public string AnnotationsGeoJson { get; set; }
        
        [JsonIgnore]
        public Geometry Geom { get; set; }

        public bool IsEnabled { get; set; } = false;

        public Guid? DatasetImageId { get; set; }
        public virtual DatasetImage? DatasetImage { get; set; }

        public string CreatedById { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public ApplicationUser? CreatedBy { get; set; }

        public string? UpdatedById { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public ApplicationUser? UpdatedBy { get; set; }
    }
}

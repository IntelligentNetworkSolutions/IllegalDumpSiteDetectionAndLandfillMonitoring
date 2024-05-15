using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Intefaces;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;

namespace Entities.DatasetEntities
{
    public class ImageAnnotation : BaseEntity<Guid>, ICreatedByUser, IUpdatedByUser
    {
        [NotMapped]
        public string? AnnotationJson { get; set; }
        
        [JsonIgnore]
        [Column(TypeName = "geometry(Polygon)")]
        public Polygon Geom { get; set; }

        [NotMapped]
        public string GeoJson
        {
            get
            {
                return Helpers.GeoJsonHelpers.GeometryToGeoJson(Geom);
            }
        }

        public bool IsEnabled { get; set; } = false;

        public Guid? DatasetImageId { get; set; }
        public virtual DatasetImage? DatasetImage { get; set; }

        public Guid? DatasetClassId { get; set; }
        public virtual DatasetClass? DatasetClass { get; set; }

        public string CreatedById { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public ApplicationUser? CreatedBy { get; set; }

        public string? UpdatedById { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public ApplicationUser? UpdatedBy { get; set; }
    }
}

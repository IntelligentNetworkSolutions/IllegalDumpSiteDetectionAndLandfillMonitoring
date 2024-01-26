using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;

namespace DTOs.MainApp.BL.DatasetDTOs
{
    public record ImageAnnotationDTO
    {
        public Guid Id { get; init; }
        public string AnnotationsGeoJson { get; init; }

        [JsonIgnore]
        public Geometry Geom { get; init;  }

        public bool IsEnabled { get; init; } = false;

        public Guid? DatasetImageId { get; init; }
        public virtual DatasetImageDTO? DatasetImage { get; init; }

        public string CreatedById { get; init; }
        public DateTime CreatedOn { get; init; } = DateTime.UtcNow;
        public UserDTO? CreatedBy { get; init; }

        public string? UpdatedById { get; init; }
        public DateTime? UpdatedOn { get; init; }
        public UserDTO? UpdatedBy { get; init; }
    }
}

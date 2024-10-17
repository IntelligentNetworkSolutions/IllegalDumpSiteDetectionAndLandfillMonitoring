using DTOs.Helpers;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;


namespace DTOs.MainApp.BL.DatasetDTOs
{
    public record ImageAnnotationDTO
    {
        public Guid? Id { get; init; }
        [NotMapped]
        public int IdInt { get; set; } // MMDetection Id

        public string? AnnotationJson { get; init; }

        [JsonIgnore]
        public Polygon Geom { get; init; }

        public string GeoJson { get => GeoJsonHelpers.GeometryToGeoJson(Geom); }

        public Dictionary<int, int>? TopLeftBottomRight { get => GeoJsonHelpers.GeometryBBoxToTopLeftBottomRight(Geom); }

        public Dictionary<int, int>? TopLeftWidthHeight { get => GeoJsonHelpers.GeometryBBoxToTopLeftWidthHeight(Geom); }

        public bool IsEnabled { get; init; } = false;

        public Guid? DatasetImageId { get; init; }
        [NotMapped]
        public int DatasetImageIdInt { get; set; } // MMDetection Id
        public virtual DatasetImageDTO? DatasetImage { get; init; }

        public Guid? DatasetClassId { get; set; }
        public virtual DatasetClassDTO? DatasetClass { get; set; }

        public string? CreatedById { get; init; }
        public DateTime CreatedOn { get; init; } = DateTime.UtcNow;
        public UserDTO? CreatedBy { get; init; }

        public string? UpdatedById { get; init; }
        public DateTime? UpdatedOn { get; init; }
        public UserDTO? UpdatedBy { get; init; }
    }
}
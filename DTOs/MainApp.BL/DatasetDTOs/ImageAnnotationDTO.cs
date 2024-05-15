using DTOs.Helpers;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;


namespace DTOs.MainApp.BL.DatasetDTOs
{
    public record ImageAnnotationDTO
    {
        public Guid? Id { get; init; }
        public string? AnnotationJson { get; init; }

        [JsonIgnore]
        public Polygon Geom { get; init; }

        public string GeoJson
        {
            get
            {
                return GeoJsonHelpers.GeometryToGeoJson(Geom);
            }            
        }

        public Dictionary<int, int>? TopLeftBottomRight
        {
            get
            {
                return GeoJsonHelpers.GeometryBBoxToTopLeftBottomRight(Geom);
            }
        }

        public Dictionary<int, int>? TopLeftWidthHeight
        {
            get
            {
                return GeoJsonHelpers.GeometryBBoxToTopLeftWidthHeight(Geom);
            }
        }

        public bool IsEnabled { get; init; } = false;

        public Guid? DatasetImageId { get; init; }
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

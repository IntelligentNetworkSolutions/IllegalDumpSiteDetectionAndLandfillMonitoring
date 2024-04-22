using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;

namespace DTOs.MainApp.BL.DetectionDTOs
{
    public class DetectionIgnoreZoneDTO
    {
        public Guid? Id { get; set; }

        public string? Name { get; set; }
        public string? Description { get; set; }

        [JsonIgnore]
        public Polygon? Geom { get; set; }

        public string GeoJson
        {
            get
            {
                return Helpers.GeoJsonHelpers.GeometryToGeoJson(Geom);
            }
        }

        public string? CreatedById { get; set; }
        public DateTime? CreatedOn { get; set; } = DateTime.UtcNow;
        public virtual UserDTO? CreatedBy { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DTOs.Helpers;
using NetTopologySuite.Geometries;

namespace DTOs.MainApp.BL.DetectionDTOs
{
    public class DetectionIgnoreZoneDTO
    {
        public Guid? Id { get; set; }

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
    }
}

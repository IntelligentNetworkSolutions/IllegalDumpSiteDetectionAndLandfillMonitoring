using DTOs.Helpers;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DTOs.MainApp.BL.DetectionDTOs
{
    public record DetectionRunsDTO
    {
        public Guid Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        [JsonIgnore]
        [Column(TypeName = "geometry")]
        public Polygon Geom { get; set; }
        public string GeoJson
        {
            get
            {
                return GeoJsonHelper.GeometryToGeoJson(Geom);
            }
        }

    }
}

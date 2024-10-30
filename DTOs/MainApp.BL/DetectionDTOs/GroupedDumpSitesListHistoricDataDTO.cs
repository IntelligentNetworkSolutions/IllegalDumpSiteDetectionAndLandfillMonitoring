using DTOs.Helpers;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MainApp.BL.DetectionDTOs
{
    public record GroupedDumpSitesListHistoricDataDTO
    {
        public string ClassName { get; set; }
        [JsonIgnore]
        public List<Polygon>? Geoms { get; set; }
        public List<string> GeoJsons
        {
            get
            {
                return GeoJsonHelpers.GeometryListToGeoJson(Geoms);
            }
        }
        public List<double> GeomAreas { get; set; }
        public double TotalGroupArea { get; set; }


        public List<Polygon>? GeomsInIgnoreZone { get; set; }
        public List<Polygon>? GeomsOutsideOfIgnoreZone { get; set; }

        public List<double> GeomAreasInIgnoreZone { get; set; }
        public List<double> GeomAreasOutsideOfIgnoreZone { get; set; }

        public double TotalGroupAreaInIgnoreZone { get; set; }
        public double TotalGroupAreaOutOfIgnoreZone { get; set; }

    }
}

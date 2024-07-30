using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.Helpers;
using DTOs.MainApp.BL.DatasetDTOs;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;

namespace DTOs.MainApp.BL.DetectionDTOs
{
    public class DetectedDumpSiteDTO
    {
        public Guid? Id { get; set; }

        public double? ConfidenceRate { get; set; }

        public Guid? DetectionRunId { get; set; }
        public virtual DetectionRunDTO? DetectionRun { get; set; }

        public Guid? DatasetClassId { get; set; }
        public virtual DatasetClassDTO? DatasetClass { get; set; }

        [JsonIgnore]
        public Polygon? Geom { get; set; }

        public string GeoJson
        {
            get
            {
                return GeoJsonHelpers.GeometryToGeoJson(Geom);
            }
        }
    }
}

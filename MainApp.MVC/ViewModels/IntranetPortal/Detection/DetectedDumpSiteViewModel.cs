using DTOs.Helpers;
using DTOs.MainApp.BL.DatasetDTOs;
using DTOs.MainApp.BL.DetectionDTOs;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;

namespace MainApp.MVC.ViewModels.IntranetPortal.Detection
{
    public class DetectedDumpSiteViewModel
    {
        public Guid? Id { get; set; }

        public double? ConfidenceRate { get; set; }

        public Guid? DetectionRunId { get; set; }
        public virtual DetectionRunViewModel? DetectionRun { get; set; }

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

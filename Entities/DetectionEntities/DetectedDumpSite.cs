using System.ComponentModel.DataAnnotations.Schema;
using Entities.DatasetEntities;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;

namespace Entities.DetectionEntities
{
    public class DetectedDumpSite : BaseEntity<Guid>
    {
        public double? ConfidenceRate { get; set; }

        public Guid DatasetClassId { get; set; }
        public virtual DatasetClass? DatasetClass { get; set; }

        public Guid DetectionRunId { get; set; }
        public virtual DetectionRun? DetectionRun { get; set; }

        [JsonIgnore]
        [Column(TypeName = "geometry(Polygon)")]
        public Polygon Geom { get; set; }

        [NotMapped]
        public string GeoJson { get; set; }
    }
}

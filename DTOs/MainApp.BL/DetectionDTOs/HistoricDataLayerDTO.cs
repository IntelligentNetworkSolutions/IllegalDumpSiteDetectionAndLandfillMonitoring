using NetTopologySuite.Geometries.Prepared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MainApp.BL.DetectionDTOs
{
    public record HistoricDataLayerDTO
    {
        public Guid? DetectionRunId { get; set; }
        public string? DetectionRunName { get; set; }
        public string? DetectionRunDescription { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool? IsCompleted { get; set; }
        public double? TotalAreaOfDetectionRun { get; set; }
        public double? AvgConfidenceRate { get; set; }
        public List<double?> AllConfidenceRates { get; set; }
        public List<GroupedDumpSitesListHistoricDataDTO> GroupedDumpSitesList { get; set; }

    }
}

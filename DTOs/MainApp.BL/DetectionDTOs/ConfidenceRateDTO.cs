using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MainApp.BL.DetectionDTOs
{
    public class ConfidenceRateDTO
    {
        public Guid detectionRunId { get; set; }
        public double confidenceRate { get; set; }
    }
}

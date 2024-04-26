using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MainApp.BL.DetectionDTOs
{
    public class HistoricDataDTO
    {
        public List<HistoricDataLayerDTO> DetectionRuns { get; set; }
        public Guid? SelectedDetectionRunId { get; set; }
    }
}

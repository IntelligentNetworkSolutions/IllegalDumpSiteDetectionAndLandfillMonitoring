using DTOs.MainApp.BL.DetectionDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MainApp.BL.LegalLandfillManagementDTOs
{
    public record LegalLandfillPointCloudFileDTO
    {
        public Guid Id { get; set; }
        public string? FileName { get; set; }
        public DateTime ScanDateTime { get; set; }
        public Guid LegalLandfillId { get; set; }
        public virtual LegalLandfillDTO? LegalLandfill { get; set; }

    }
}

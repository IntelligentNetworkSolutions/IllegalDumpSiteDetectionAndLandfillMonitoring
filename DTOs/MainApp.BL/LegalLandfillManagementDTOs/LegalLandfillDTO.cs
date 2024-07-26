using DTOs.MainApp.BL.DetectionDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MainApp.BL.LegalLandfillManagementDTOs
{
    public record LegalLandfillDTO
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<LegalLandfillPointCloudFileDTO>? LegalLandfillPointCloudFiles { get; set; }

        public LegalLandfillDTO()
        {
            LegalLandfillPointCloudFiles = new List<LegalLandfillPointCloudFileDTO>();
        }
    }
}

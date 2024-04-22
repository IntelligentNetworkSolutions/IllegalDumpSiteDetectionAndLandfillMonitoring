using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MainApp.BL.DetectionDTOs
{
    public class DetectionRunDTO
    {
        public Guid? Id { get; set; }

        public string? Name { get; set; }
        public string? Description { get; set; }

        public bool IsCompleted { get; set; } = false;

        public string? ImagePath { get; set; }
        public string? ImageFileName { get; set; }

        public string? CreatedById { get; set; }
        public DateTime? CreatedOn { get; set; }
        public virtual UserDTO? CreatedBy { get; set; }

        public virtual ICollection<DetectedDumpSiteDTO>? DetectedDumpSites { get; set; }

        public DetectionRunDTO()
        {
            DetectedDumpSites = new List<DetectedDumpSiteDTO>();
        }
    }
}

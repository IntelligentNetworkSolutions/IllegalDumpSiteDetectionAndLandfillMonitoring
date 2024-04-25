using System.ComponentModel.DataAnnotations.Schema;
using Entities.Intefaces;

namespace Entities.DetectionEntities
{
    public class DetectionRun : BaseEntity<Guid>, ICreatedByUser
    {
        public string Name { get; set; }
        public string? Description { get; set; }

        public bool IsCompleted { get; set; } = false;

        public string ImagePath { get; set; }
        public string ImageFileName { get; set; }

        public string CreatedById { get; set; }
        public DateTime CreatedOn { get; set; }
        public virtual ApplicationUser? CreatedBy { get; set; }

        public virtual ICollection<DetectedDumpSite>? DetectedDumpSites { get; set; }
    }
}

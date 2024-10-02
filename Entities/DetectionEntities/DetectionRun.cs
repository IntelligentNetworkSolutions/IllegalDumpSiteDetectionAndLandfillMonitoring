using Entities.Intefaces;

namespace Entities.DetectionEntities
{
    public class DetectionRun : BaseEntity<Guid>, ICreatedByUser
    {
        public string Name { get; set; }
        public string? Description { get; set; }

        public bool IsCompleted { get; set; } = false;

        public Guid DetectionInputImageId { get; set; }
        public virtual DetectionInputImage? DetectionInputImage { get; set; }

        public string CreatedById { get; set; }
        public DateTime CreatedOn { get; set; }
        public virtual ApplicationUser? CreatedBy { get; set; }

        public virtual ICollection<DetectedDumpSite>? DetectedDumpSites { get; set; }
    }
}

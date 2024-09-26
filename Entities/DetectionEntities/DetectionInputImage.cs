using Entities.Intefaces;

namespace Entities.DetectionEntities
{
    public class DetectionInputImage : BaseEntity<Guid>, ICreatedByUser, IUpdatedByUser
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime DateTaken { get; set; }

        public string ImagePath { get; set; }
        public string ImageFileName { get; set; }

        public string CreatedById { get; set; }
        public DateTime CreatedOn { get; set; }
        public virtual ApplicationUser? CreatedBy { get; set; }

        public string? UpdatedById { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public virtual ApplicationUser? UpdatedBy { get; set; }
    }
}

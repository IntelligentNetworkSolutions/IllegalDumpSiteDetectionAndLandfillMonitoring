using Entities.Intefaces;

namespace Entities.DatasetEntities
{
    public class DatasetImage : BaseEntity<Guid>, ICreatedByUser, IUpdatedByUser
    {
        public string FileName { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string? ThumbnailPath { get; set; }

        public bool IsEnabled { get; set; } = false;

        public Guid? DatasetId { get; set; }
        public virtual Dataset? Dataset { get; set; }

        public string CreatedById { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public ApplicationUser? CreatedBy { get; set; }
        public string? UpdatedById { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public ApplicationUser? UpdatedBy { get; set; }

        public virtual ICollection<ImageAnnotation> ImageAnnotations { get; set; } = new List<ImageAnnotation>();
    }
}
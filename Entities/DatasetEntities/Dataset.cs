using Entities.Intefaces;

namespace Entities.DatasetEntities
{
    public class Dataset : BaseEntity<Guid>, ICreatedByUser, IUpdatedByUser
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public bool IsPublished { get; set; } = false;

        public Guid? ParentDatasetId { get; set; }
        public virtual Dataset? ParentDataset { get; set; }

        public string CreatedById { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public ApplicationUser? CreatedBy { get; set; }

        public string? UpdatedById { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public ApplicationUser? UpdatedBy { get; set; }
        public bool? AnnotationsPerSubclass { get; set; }
        public virtual ICollection<Dataset_DatasetClass> DatasetClasses { get; set; }
    }
}

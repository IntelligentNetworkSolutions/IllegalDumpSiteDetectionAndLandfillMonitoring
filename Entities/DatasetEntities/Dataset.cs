using System.ComponentModel.DataAnnotations;
using Entities.Intefaces;

namespace Entities.DatasetEntities
{
    public class Dataset : BaseEntity<Guid>, ICreatedByUser, IUpdatedByUser
    {
        public required string Name { get; set; }
        public required string Description { get; set; }

        public bool IsPublished { get; set; } = false;

        public Guid? ParentDatasetId { get; set; }
        public virtual Dataset? ParentDataset { get; set; }

        public required string CreatedById { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public ApplicationUser? CreatedBy { get; set; }

        public string? UpdatedById { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public ApplicationUser? UpdatedBy { get; set; }
    }
}

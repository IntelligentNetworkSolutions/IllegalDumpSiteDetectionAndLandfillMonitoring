using System.ComponentModel.DataAnnotations;
using Entities.Intefaces;

namespace Entities.DatasetEntities
{
    public class DatasetClass : BaseEntity<Guid>, ICreatedByUser
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }

        public Guid DatasetId { get; set; }
        public virtual Dataset? Dataset { get; set; }

        public string CreatedById { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public ApplicationUser? CreatedBy { get; set; }
    }
}

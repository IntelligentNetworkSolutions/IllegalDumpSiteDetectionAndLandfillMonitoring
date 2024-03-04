using System.ComponentModel.DataAnnotations;
using Entities.Intefaces;

namespace Entities.DatasetEntities
{
    public class DatasetClass : BaseEntity<Guid>, ICreatedByUser
    {
        public Guid? ParentClassId { get; set; }
        public virtual DatasetClass? ParentClass { get; set; }
        public string ClassName { get; set; }
        public string CreatedById { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public ApplicationUser? CreatedBy { get; set; }

        public virtual ICollection<Dataset_DatasetClass> Datasets { get; set; }

    }
}

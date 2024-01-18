using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Intefaces;

namespace Entities.DatasetEntities
{
    public class DatasetClass : IBaseEntity<Guid>, ICreatedByUser
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public required int ClassId { get; set; }
        public required string ClassName { get; set; }

        public required Guid DatasetId { get; set; }
        public virtual Dataset? Dataset { get; set; }

        public required string CreatedById { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public ApplicationUser? CreatedBy { get; set; }
    }
}

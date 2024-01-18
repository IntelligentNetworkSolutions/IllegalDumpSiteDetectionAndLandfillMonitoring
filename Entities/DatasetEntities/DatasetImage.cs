using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Intefaces;

namespace Entities.DatasetEntities
{
    public class DatasetImage : IBaseEntity<Guid>, ICreatedByUser, IUpdatedByUser
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public required string FileName { get; set; }
        public required string ImagePath { get; set; }

        public bool IsEnabled { get; set; } = false;

        public Guid? DatasetId { get; set; }
        public virtual Dataset? Dataset { get; set; }

        public required string CreatedById { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public ApplicationUser? CreatedBy { get; set; }
        public string? UpdatedById { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public ApplicationUser? UpdatedBy { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Intefaces
{
    public interface IUpdatedByUser
    {
        public string? UpdatedById { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public ApplicationUser? UpdatedBy { get; set; }
    }
}

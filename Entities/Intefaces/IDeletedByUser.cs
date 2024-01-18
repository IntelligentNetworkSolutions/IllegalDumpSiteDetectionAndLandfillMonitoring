using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Intefaces
{
    public interface IDeletedByUser
    {
        public string? DeletedById { get; set; }
        public DateTime? DeletedOn { get; set; }
        public ApplicationUser? DeletedBy { get; set; }
    }
}

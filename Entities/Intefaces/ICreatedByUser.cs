using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Intefaces
{
    public interface ICreatedByUser
    {
        public string CreatedById { get; set; }
        public DateTime CreatedOn { get; set; }
        public ApplicationUser? CreatedBy { get; set; }
    }
}

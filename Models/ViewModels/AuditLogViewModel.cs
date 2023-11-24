using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class AuditLogViewModel
    {
        public List<AuditLogUserViewModel> InternalUsersList { get; set; }
        public List<string> AuditActionsList { get; set; }
    }
}

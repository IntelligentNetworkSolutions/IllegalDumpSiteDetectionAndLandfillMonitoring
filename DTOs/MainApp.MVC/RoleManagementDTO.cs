using SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MainApp.MVC
{
    public class RoleManagementDTO : RoleDTO
    {
        public ICollection<AuthClaim> Claims { get; set; }

        public List<string?> ClaimsInsert { get; set; }

        public RoleManagementDTO()
        {

            Claims = new List<AuthClaim>();

            ClaimsInsert = new List<string>();
        }
    }
}

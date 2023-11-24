using Microsoft.AspNetCore.Identity;
using SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class UserManagementEditRoleViewModel : IdentityRole
    {
        public ICollection<AuthClaim> Claims { get; set; }

        public List<string> ClaimsInsert { get; set; }

        public UserManagementEditRoleViewModel()
        {

            Claims = new List<AuthClaim>();

            ClaimsInsert = new List<string>();
        }
    }
}

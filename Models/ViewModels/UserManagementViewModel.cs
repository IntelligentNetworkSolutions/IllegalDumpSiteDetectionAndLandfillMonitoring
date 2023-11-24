using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class UserManagementViewModel
    {
        public List<UserManagementUserViewModel> Users { get; set; }
        public List<IdentityRole> Roles { get; set; }        

    }
}

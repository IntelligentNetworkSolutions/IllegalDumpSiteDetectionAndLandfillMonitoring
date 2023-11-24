using Microsoft.AspNetCore.Identity;
using SD;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class UserManagementCreateUserViewModel : ApplicationUser
    {
        public List<IdentityRole> Roles { get; set; }

        public List<string> RolesInsert { get; set; }

        public ICollection<AuthClaim> Claims { get; set; }

        public List<IdentityRoleClaim<string>> RoleClaims { get; set; }

        public List<string> ClaimsInsert { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public int PasswordMinLength { get; set; }

        public bool PasswordMustHaveNumbers { get; set; }

        public bool PasswordMustHaveLetters { get; set; }
        public bool PasswordMustHaveSpecialChar { get; set; }

        public IEnumerable<ApplicationUser> AllUsers { get; set; }

        public UserManagementCreateUserViewModel()
        {
            Roles = new List<IdentityRole>();
            RolesInsert = new List<string>();
            RoleClaims = new List<IdentityRoleClaim<string>>();
            ClaimsInsert = new List<string>();
            Claims = new List<AuthClaim>();
            AllUsers = new List<ApplicationUser>();

        }
    }
}

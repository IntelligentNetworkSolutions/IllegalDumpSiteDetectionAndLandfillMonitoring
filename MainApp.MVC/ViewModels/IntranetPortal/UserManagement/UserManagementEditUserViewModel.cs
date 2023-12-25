using Entities;
using Microsoft.AspNetCore.Identity;
using SD;
using System.ComponentModel.DataAnnotations;

namespace MainApp.MVC.ViewModels.IntranetPortal.UserManagement
{
    public class UserManagementEditUserViewModel : ApplicationUser
    {
        public List<IdentityRole> Roles { get; set; }

        public List<string> RolesInsert { get; set; }

        public ICollection<AuthClaim> Claims { get; set; }

        public List<IdentityRoleClaim<string>> RoleClaims { get; set; }

        public List<string> ClaimsInsert { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
        public int PasswordMinLength { get; set; }

        public bool PasswordMustHaveNumbers { get; set; }

        public bool PasswordMustHaveLetters { get; set; }
        public bool PasswordMustHaveSpecialChar { get; set; }
        public IEnumerable<ApplicationUser> AllUsersExceptCurrent { get; set; }

        public UserManagementEditUserViewModel()
        {
            Roles = new List<IdentityRole>();
            RolesInsert = new List<string>();
            RoleClaims = new List<IdentityRoleClaim<string>>();
            ClaimsInsert = new List<string>();
            Claims = new List<AuthClaim>();
            AllUsersExceptCurrent = new List<ApplicationUser>();

        }
    }
}

using DTOs.MainApp.BL;
using SD;
using System.ComponentModel.DataAnnotations;

namespace MainApp.MVC.ViewModels.IntranetPortal.UserManagement
{
    public class UserManagementEditUserViewModel
    {
        public List<RoleDTO> Roles { get; set; }

        public List<string> RolesInsert { get; set; }

        public ICollection<AuthClaim> Claims { get; set; }

        public List<RoleClaimDTO> RoleClaims { get; set; }

        public List<string> ClaimsInsert { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
        public int PasswordMinLength { get; set; }

        public bool PasswordMustHaveNumbers { get; set; }

        public bool PasswordMustHaveLetters { get; set; }
        public IEnumerable<UserDTO> AllUsersExceptCurrent { get; set; }
        public string Id { get; set; }
        public string Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool? IsActive { get; set; }
        public string? PhoneNumber { get; set; }
        public string UserName { get; set; }
        public string? NormalizedUserName { get; set; }
        public bool EmailConfirmed { get; set; }
        public UserManagementEditUserViewModel()
        {
            Roles = new List<RoleDTO>();
            RolesInsert = new List<string>();
            RoleClaims = new List<RoleClaimDTO>();
            ClaimsInsert = new List<string>();
            Claims = new List<AuthClaim>();
            AllUsersExceptCurrent = new List<UserDTO>();

        }
    }
}

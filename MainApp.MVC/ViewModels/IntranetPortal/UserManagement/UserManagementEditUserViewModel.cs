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

        [Required(ErrorMessage = "Email is a required property")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Email address is not valid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "First name is a required property")]
        [MinLength(2, ErrorMessage = "First name must contains at least 2 characters")]
        [MaxLength(15, ErrorMessage = "First name must contains maximum 15 characters")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last name is a required property")]
        [MinLength(2, ErrorMessage = "Last name must contains at least 2 characters")]
        [MaxLength(50, ErrorMessage = "Last name must contains maximum 50 characters")]
        public string? LastName { get; set; }

        public bool? IsActive { get; set; }

        [Required(ErrorMessage = "Phone number is a required property")]
        [MinLength(9, ErrorMessage = "Phone number must be at least 9 characters long")]
        [MaxLength(15, ErrorMessage = "Phone number cannot exceed 15 characters")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Username is a required property")]
        [MinLength(5, ErrorMessage = "Username must contains at least 5 characters")]
        [MaxLength(20, ErrorMessage = "Username must contains maximum 20 characters")]
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

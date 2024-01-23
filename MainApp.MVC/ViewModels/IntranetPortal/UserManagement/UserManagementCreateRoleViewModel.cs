using MainApp.MVC.Helpers;
using Microsoft.AspNetCore.Identity;
using SD;
using System.ComponentModel.DataAnnotations;

namespace MainApp.MVC.ViewModels.IntranetPortal.UserManagement
{
    public class UserManagementCreateRoleViewModel
    {
        public string? Id { get; set; }
        [Required(ErrorMessage ="Role name is a required property")]
        [MinLength(3, ErrorMessage = "Role name must contains at least 3 characters")]
        [MaxLength(30, ErrorMessage = "Role name must contains maximum 30 characters")]
        public string? Name { get; set; }
        public string? NormalizedName { get; set; }
        public ICollection<AuthClaim> Claims { get; set; }

        public List<string> ClaimsInsert { get; set; }

        public UserManagementCreateRoleViewModel()
        {

            Claims = new List<AuthClaim>();

            ClaimsInsert = new List<string>();
        }
    }
}

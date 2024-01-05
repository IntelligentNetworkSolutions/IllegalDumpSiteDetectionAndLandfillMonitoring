using Microsoft.AspNetCore.Identity;
using SD;

namespace MainApp.MVC.ViewModels.IntranetPortal.UserManagement
{
    public class UserManagementEditRoleViewModel
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? NormalizedName { get; set; }
        public ICollection<AuthClaim> Claims { get; set; }

        public List<string?> ClaimsInsert { get; set; }
       

        public UserManagementEditRoleViewModel()
        {

            Claims = new List<AuthClaim>();

            ClaimsInsert = new List<string?>();
        }
    }
}

using Microsoft.AspNetCore.Identity;
using SD;

namespace MainApp.MVC.ViewModels.IntranetPortal.UserManagement
{
    public class UserManagementCreateRoleViewModel : IdentityRole
    {       
        public ICollection<AuthClaim> Claims { get; set; }

        public List<string> ClaimsInsert { get; set; }

        public UserManagementCreateRoleViewModel()
        {

            Claims = new List<AuthClaim>();

            ClaimsInsert = new List<string>();
        }
    }
}

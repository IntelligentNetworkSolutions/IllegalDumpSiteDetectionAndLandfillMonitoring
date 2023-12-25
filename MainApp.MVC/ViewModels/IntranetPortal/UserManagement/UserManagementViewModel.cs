using Microsoft.AspNetCore.Identity;

namespace MainApp.MVC.ViewModels.IntranetPortal.UserManagement
{
    public class UserManagementViewModel
    {
        public List<UserManagementUserViewModel> Users { get; set; }
        public List<IdentityRole> Roles { get; set; }        

    }
}

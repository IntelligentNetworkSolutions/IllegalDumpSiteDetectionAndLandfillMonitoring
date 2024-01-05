using DTOs.MainApp.MVC;
using Microsoft.AspNetCore.Identity;

namespace MainApp.MVC.ViewModels.IntranetPortal.UserManagement
{
    public class UserManagementViewModel
    {
        public List<UserManagementUserViewModel> Users { get; set; }
        public List<RoleDTO> Roles { get; set; }

        public UserManagementViewModel()
        {
            Roles = new List<RoleDTO>();
            Users = new List<UserManagementUserViewModel>();
        }

    }
}

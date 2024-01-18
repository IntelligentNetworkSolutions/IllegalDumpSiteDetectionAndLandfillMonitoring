using DTOs.MainApp.BL;

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

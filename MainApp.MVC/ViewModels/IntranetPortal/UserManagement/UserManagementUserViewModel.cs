using DTOs.MainApp.MVC;
using Entities;
using Microsoft.AspNetCore.Identity;
using SD;
using System.ComponentModel.DataAnnotations;

namespace MainApp.MVC.ViewModels.IntranetPortal.UserManagement
{
    public class UserManagementUserViewModel
    {
        public string Id { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool? IsActive { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public List<RoleDTO> Roles { get; set; }
        

        public UserManagementUserViewModel()
        {
            Roles = new List<RoleDTO>();

        }
    }
}

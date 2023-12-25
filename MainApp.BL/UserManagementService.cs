using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dal;
using Microsoft.AspNetCore.Identity;
using Services.Interfaces.Services;

namespace Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManagementDa _userManagementDa;

        public UserManagementService(UserManagementDa userManagementDa)
        {
            _userManagementDa = userManagementDa;
        }

        public IQueryable<IdentityRole> GetRolesAsQueriable()
        {
            return _userManagementDa.GetRolesAsQueriable();
        }
    }
}

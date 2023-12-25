using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Services.Interfaces.Services
{
    public interface IUserManagementService
    {
        IQueryable<IdentityRole> GetRolesAsQueriable();
    }
}

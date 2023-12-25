using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace DAL.Interfaces.Repositories
{
    public interface IIntranetPortalUsersTokenDa
    {
        Task<int> CreateIntranetPortalUserToken(string token, string userId);

        Task<int> UpdateAndHashUserPassword(ApplicationUser user, string password);

        Task<int> UpdateIsTokenUsedForUser(string token, string userId);

        Task<ApplicationUser> GetUser(string userId);

        Task<bool> IsTokenNotUsed(string token, string userId);
    }
}

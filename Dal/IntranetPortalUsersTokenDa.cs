using Dal.ApplicationStorage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal
{
    public class IntranetPortalUsersTokenDa
    {
        private readonly ApplicationDbContext _db;
        private static ILogger<IntranetPortalUsersTokenDa> _logger;
        public IntranetPortalUsersTokenDa(ApplicationDbContext db, ILogger<IntranetPortalUsersTokenDa> logger)
        {
            _db = db;
            _logger = logger;
        }

        // TODO: Test
        public async Task<Int32> CreateIntranetPortalUserToken(string token, string userId)
        {
            try
            {
                var dbIntranetPortalUsersToken = await _db.IntranetPortalUsersTokens.Where(x => x.ApplicationUserId == userId).ToListAsync();
                if (dbIntranetPortalUsersToken != null && dbIntranetPortalUsersToken.Count > 0)
                {
                    foreach (var item in dbIntranetPortalUsersToken)
                    {
                        if (item.isTokenUsed == false)
                        {
                            item.isTokenUsed = true;
                        }

                    }
                }
                IntranetPortalUsersToken model = new IntranetPortalUsersToken();
                model.ApplicationUserId = userId;
                model.isTokenUsed = false;
                model.Token = token;
                await _db.IntranetPortalUsersTokens.AddAsync(model);
                return await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }
        public async Task<Int32> UpdateAndHashUserPassword(ApplicationUser user, string password)
        {
            try
            {
                var passwordHashed = new PasswordHasher<ApplicationUser>();
                user.PasswordHash = passwordHashed.HashPassword(user, password);
                _db.Users.Update(user);

                return await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        // TODO: Test
        public async Task<Int32> UpdateIsTokenUsedForUser(string token, string userId)
        {
            try
            {
                var intranetPortalUserToken = await _db.IntranetPortalUsersTokens.Where(x => x.Token == token && x.ApplicationUserId == userId && x.isTokenUsed == false).FirstOrDefaultAsync();
                intranetPortalUserToken.isTokenUsed = true;
                _db.IntranetPortalUsersTokens.Update(intranetPortalUserToken);
                return await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        // TODO: Change to Nullable
        public async Task<ApplicationUser> GetUser(string userId)
        {
            try
            {
                return await _db.Users.Where(z => z.Id == userId)
                                        .FirstOrDefaultAsync();
                //return await _db.Users.SingleOrDefaultAsync(z => z.Id == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }
        public async Task<bool> IsTokenNotUsed(string token, string userId)
        {
            try
            {
                var intranetPortalUsersToken = await _db.IntranetPortalUsersTokens.Where(x => x.Token == token && x.ApplicationUserId == userId && x.isTokenUsed == false).FirstOrDefaultAsync();
                if (intranetPortalUsersToken != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }

                //var intranetPortalUsersToken =
                //    await _db.IntranetPortalUsersTokens
                //                .Where(x => x.Token == token
                //                        && x.ApplicationUserId == userId
                //                        && x.isTokenUsed == false)
                //                .FirstOrDefaultAsync();

                //if (intranetPortalUsersToken != null)
                //    return true;

                //return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }
    }
}

using DAL.ApplicationStorage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using DAL.Interfaces.Repositories;

namespace DAL.Repositories
{
    public class IntranetPortalUsersTokenDa : IIntranetPortalUsersTokenDa
    {
        private readonly ApplicationDbContext _db;
        private static ILogger<IntranetPortalUsersTokenDa> _logger;
        public IntranetPortalUsersTokenDa(ApplicationDbContext db, ILogger<IntranetPortalUsersTokenDa> logger)
        {
            _db = db;
            _logger = logger;
        }

        // TODO: Test
        public async Task<int> CreateIntranetPortalUserToken(string token, string userId)
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
                throw;
            }
        }
        public async Task<int> UpdateAndHashUserPassword(ApplicationUser user, string password)
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
                throw;
            }
        }

        // TODO: Test
        public async Task<int> UpdateIsTokenUsedForUser(string token, string userId)
        {
            try
            {
                var intranetPortalUserToken = 
                    await _db.IntranetPortalUsersTokens.FirstOrDefaultAsync(x => x.Token == token 
                                                                            && x.ApplicationUserId == userId 
                                                                            && x.isTokenUsed == false);
                intranetPortalUserToken.isTokenUsed = true;
                _db.IntranetPortalUsersTokens.Update(intranetPortalUserToken);
                return await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<ApplicationUser?> GetUser(string userId)
        {
            try
            {
                return await _db.Users.SingleOrDefaultAsync(z => z.Id == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<bool> IsTokenNotUsed(string token, string userId)
        {
            try
            {
                var intranetPortalUsersToken = 
                    await _db.IntranetPortalUsersTokens.FirstOrDefaultAsync(x => x.Token == token 
                                                                            && x.ApplicationUserId == userId 
                                                                            && x.isTokenUsed == false);
                if (intranetPortalUsersToken != null)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}

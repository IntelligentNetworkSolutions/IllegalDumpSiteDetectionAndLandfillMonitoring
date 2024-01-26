using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DAL.Repositories
{
    public class ApplicationSettingsRepository : IApplicationSettingsRepo
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ApplicationSettingsRepository> _logger;

        public ApplicationSettingsRepository(ApplicationDbContext db, ILogger<ApplicationSettingsRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        #region Create
        public async Task<bool> CreateApplicationSetting(ApplicationSettings appSetting)
        {
            _db.ApplicationSettings.Add(appSetting);
            int addResult = await _db.SaveChangesAsync();
            return addResult > 0;
        }
        #endregion

        #region Delete
        public async Task<bool?> DeleteApplicationSettingByKey(string appSettingKey)
        {
            ApplicationSettings? applicationSetting = await GetApplicationSettingByKey(appSettingKey);
            if (applicationSetting is null)
                return null;
            return await DeleteApplicationSetting(applicationSetting);
        }

        private async Task<bool> DeleteApplicationSetting(ApplicationSettings? appSetting)
        {
            if (appSetting is null)
                return false;

            _db.ApplicationSettings.Remove(appSetting);

            int deleteResult = await _db.SaveChangesAsync();
            return deleteResult > 0;
        }
        #endregion

        #region Update
        public async Task<bool> UpdateApplicationSetting(ApplicationSettings appSetting)
        {
            _db.ApplicationSettings.Update(appSetting);
            int addResult = await _db.SaveChangesAsync();
            return addResult > 0;
        }
        #endregion

        #region Read
        public async Task<List<ApplicationSettings>> GetAllApplicationSettingsAsList()
            => await _db.ApplicationSettings.ToListAsync();

        public async Task<IQueryable<ApplicationSettings>> GetAllApplicationSettingsAsQueryable()
            => await Task.FromResult(_db.ApplicationSettings.AsSingleQuery());
            
        public async Task<List<string>> GetAllApplicationSettingsKeysAsList()
            => await _db.ApplicationSettings.Select(x => x.Key).ToListAsync();

        public async Task<ApplicationSettings?> GetApplicationSettingByKey(string appSettingKey)
            => await _db.ApplicationSettings.FindAsync(appSettingKey);
        #endregion
    }
}

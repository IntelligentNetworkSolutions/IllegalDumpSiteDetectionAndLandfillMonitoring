using DAL.ApplicationStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace DAL.Repositories
{
    public class ApplicationSettingsDa //: IApplicationSettingsDa
    {
        private readonly ApplicationDbContext _db;
        private static ILogger<ApplicationSettingsDa> _logger;

        public ApplicationSettingsDa(ApplicationDbContext db, ILogger<ApplicationSettingsDa> logger)
        {
            _db = db;
            _logger = logger;
        }
        /*
        TODO
        public async Task<ApplicationSettings> AddApplicationSettings(ApplicationSettingsCreateViewModel model)
        {
            try
            {
                ApplicationSettings appSettingDb = new ApplicationSettings();
                appSettingDb.Key = model.Key;
                appSettingDb.Value = model.Value;
                appSettingDb.Description = model.Description;
                appSettingDb.DataType = model.DataType;
                appSettingDb.Module = model.InsertedModule;

                _db.Add(appSettingDb);
                await _db.SaveChangesAsync();
                return appSettingDb;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }
        */
        public async Task<List<string>> GetAllApplicationSettingsKeys()
        {
            try
            {
                return await _db.ApplicationSettings.Select(x => x.Key).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task<ApplicationSettings> FindApplicationSetting(string applicationSettingsKey)
        {
            try
            {
                return await _db.ApplicationSettings.FindAsync(applicationSettingsKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task<List<ApplicationSettings>> GetApplicationSettings()
        {
            try
            {
                var app = await _db.ApplicationSettings.
                ToListAsync();
                return app;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }
        /*
        TODO
        public async Task<ApplicationSettings> EditApplicationSetting(ApplicationSettingsEditViewModel model)
        {
            try
            {
                ApplicationSettings appSettingDb = new ApplicationSettings();
                appSettingDb.Key = model.Key;
                appSettingDb.Value = model.Value;
                appSettingDb.Description = model.Description;
                appSettingDb.DataType = model.DataType;
                appSettingDb.Module = model.InsertedModule;

                _db.Update(appSettingDb);
                await _db.SaveChangesAsync();

                return appSettingDb;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }
        */
        public async Task<ApplicationSettings> DeleteApplicationSetting(string key)
        {
            try
            {
                var applicationSetting = await FindApplicationSetting(key);
                _db.ApplicationSettings.Remove(applicationSetting);

                await _db.SaveChangesAsync();
                return applicationSetting;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public ApplicationSettings GetApplicationConfiguration(string configurationName)
        {
            try
            {
                return _db.ApplicationSettings.Where(z => z.Key == configurationName).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }
    }
}

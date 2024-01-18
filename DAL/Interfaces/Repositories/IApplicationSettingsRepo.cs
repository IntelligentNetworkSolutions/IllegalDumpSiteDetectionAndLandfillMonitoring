using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities;

namespace DAL.Interfaces.Repositories
{
    public interface IApplicationSettingsRepo
    {
        Task<List<ApplicationSettings>> GetAllApplicationSettingsAsList();

        Task<IQueryable<ApplicationSettings>> GetAllApplicationSettingsAsQueryable();

        Task<List<string>> GetAllApplicationSettingsKeysAsList();

        Task<ApplicationSettings?> GetApplicationSettingByKey(string appSettingKey);

        Task<bool> CreateApplicationSetting(ApplicationSettings appSetting);

        Task<bool> UpdateApplicationSetting(ApplicationSettings appSetting);
        
        Task<bool?> DeleteApplicationSettingByKey(string appSettingKey);
    }
}

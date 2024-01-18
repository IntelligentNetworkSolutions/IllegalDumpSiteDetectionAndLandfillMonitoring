using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD;

namespace MainApp.BL.Interfaces.Services
{
    public interface IApplicationSettingsService
    {
        Task<ResultDTO> CreateApplicationSetting(AppSettingDTO appSettingDTO);

        Task<ResultDTO> UpdateApplicationSetting(AppSettingDTO appSettingDTO);

        Task<ResultDTO> DeleteApplicationSetting(string appSettingKey);

        Task<List<AppSettingDTO>?> GetAllApplicationSettingsAsList();

        Task<IQueryable<AppSettingDTO>?> GetAllApplicationSettingsAsQueryable();

        Task<List<string>?> GetAllApplicationSettingsKeysAsList();

        Task<AppSettingDTO?> GetApplicationSettingByKey(string appSettingKey);
    }
}

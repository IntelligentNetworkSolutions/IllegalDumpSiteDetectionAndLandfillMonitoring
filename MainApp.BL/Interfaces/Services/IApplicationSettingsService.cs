using DTOs.MainApp.BL;
using SD;

namespace MainApp.BL.Interfaces.Services
{
    public interface IApplicationSettingsService
    {
        Task<ResultDTO> CreateApplicationSetting(AppSettingDTO? appSettingDTO);

        Task<ResultDTO> UpdateApplicationSetting(AppSettingDTO appSettingDTO);

        Task<ResultDTO> DeleteApplicationSetting(string appSettingKey);

        Task<List<AppSettingDTO>?> GetAllApplicationSettingsAsList();

        Task<List<string>?> GetAllApplicationSettingsKeysAsList();

        Task<AppSettingDTO?> GetApplicationSettingByKey(string appSettingKey);
    }
}

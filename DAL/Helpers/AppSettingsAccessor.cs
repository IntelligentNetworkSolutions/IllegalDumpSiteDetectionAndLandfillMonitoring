using System.Threading.Tasks;
using DAL.Interfaces.Helpers;
using DAL.Interfaces.Repositories;
using Entities;
using SD;
using SD.Helpers;

namespace DAL.Helpers
{
    public class AppSettingsAccessor : IAppSettingsAccessor
    {
        private readonly IApplicationSettingsRepo _applicationSettingsRepo;

        public AppSettingsAccessor(IApplicationSettingsRepo applicationSettingsRepo)
        {
            _applicationSettingsRepo = applicationSettingsRepo;
        }

        private async Task<string?> GetApplicationSettingStoredValueByKey(string appSettingKey)
        {
            if (string.IsNullOrWhiteSpace(appSettingKey))
                return null;

            ApplicationSettings? appSetting = await _applicationSettingsRepo.GetApplicationSettingByKey(appSettingKey);

            if (appSetting is null)
                return null;

            return appSetting.Value;
        }

        public async Task<ResultDTO<AppSettingType?>> GetApplicationSettingValueByKey<AppSettingType>
            (string appSettingKey, object? defValue = null)
        {
            var appSettingValueStr = await GetApplicationSettingStoredValueByKey(appSettingKey);

            if (appSettingValueStr is null && defValue is null)
                return ResultDTO<AppSettingType?>.Fail($"Missing App setting with key: {appSettingKey}.");

            if (string.IsNullOrWhiteSpace(appSettingValueStr))
                return ResultDTO<AppSettingType?>.Ok((AppSettingType?)CommonHelper.CastTo<AppSettingType>(defValue));

            return ResultDTO<AppSettingType?>.Ok((AppSettingType?)CommonHelper.CastTo<AppSettingType>(appSettingValueStr));
        }
    }
}

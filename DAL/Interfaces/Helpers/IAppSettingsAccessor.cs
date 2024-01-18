using System.Threading.Tasks;
using SD;

namespace DAL.Interfaces.Helpers
{
    public interface IAppSettingsAccessor
    {
        Task<ResultDTO<AppSettingType?>> GetApplicationSettingValueByKey<AppSettingType>(string appSettingKey, object? defValue = null);
    }
}

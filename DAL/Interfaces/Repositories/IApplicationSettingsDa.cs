using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Dal.Interfaces.Repositories
{
    public interface IApplicationSettingsDa
    {
        // TODO: 🤔 ViewModels in Models/Domain hmmm 🤔🤔🤔 should be dtos then AutoMapper maybe
        //Task<ApplicationSettings> AddApplicationSettings(ApplicationSettingsCreateViewModel model);
        Task<List<string>> GetAllApplicationSettingsKeys();

        Task<ApplicationSettings> FindApplicationSetting(string applicationSettingsKey);

        Task<List<ApplicationSettings>> GetApplicationSettings();

        //Task<ApplicationSettings> EditApplicationSetting(ApplicationSettingsEditViewModel model);

        Task<ApplicationSettings> DeleteApplicationSetting(string key);

        ApplicationSettings GetApplicationConfiguration(string configurationName);

    }
}

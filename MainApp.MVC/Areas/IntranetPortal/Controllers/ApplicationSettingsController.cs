using Microsoft.AspNetCore.Mvc;
using MainApp.MVC.ViewModels.IntranetPortal.ApplicationSettings;
using DAL.Interfaces.Helpers;
using MainApp.BL.Interfaces.Services;
using SD;
using DTOs.MainApp.BL;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    //[TypeFilter(typeof(UserIsInsadminResourceFilter))]
    public class ApplicationSettingsController : Controller
    {
        private IApplicationSettingsService _applicationSettingsService;
        private IAppSettingsAccessor _appSettingsAccessor;
        private readonly IConfiguration _configuration;

        public ApplicationSettingsController(IApplicationSettingsService applicationSettingsService, IConfiguration configuration, IAppSettingsAccessor appSettingsAccessor)
        {
            _applicationSettingsService = applicationSettingsService;
            _configuration = configuration;
            _appSettingsAccessor = appSettingsAccessor;
        }

        public async Task<IActionResult> Index()
        {
            var genSet = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("Generic", "hehe");
            var genSetBool = await _appSettingsAccessor.GetApplicationSettingValueByKey<bool?>("GenericBool", false);
            // Missing
            var genSetBoolche = await _appSettingsAccessor.GetApplicationSettingValueByKey<bool?>("GenericBool4e");
            var genSetDateTime = await _appSettingsAccessor.GetApplicationSettingValueByKey<DateTime?>("GenericDateTime");

            var apps = await _applicationSettingsService.GetAllApplicationSettingsAsList();
            var appSettingsVM = apps.Select(a => new ApplicationSettingsViewModel()
            {
                Key = a.Key,
                Value = a.Value,
                DataType = a.DataType,
                Description = a.Description,
                Module = a.Module
            });
            return View(appSettingsVM);
        }

        public async Task<IActionResult> Create()
        {
            var allKeys = await _applicationSettingsService.GetAllApplicationSettingsKeysAsList();

            ApplicationSettingsCreateViewModel model = new ApplicationSettingsCreateViewModel()
            {
                Modules = SD.Modules.GetAll().ToList(),
                AllApplicationSettingsKeys = allKeys
            };

            return View(model);
        }


        //TODO
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationSettingsCreateViewModel model)
        {
            var allKeys = await _applicationSettingsService.GetAllApplicationSettingsKeysAsList();

            if (!ModelState.IsValid)
            {
                model.Modules = SD.Modules.GetAll().ToList();
                model.AllApplicationSettingsKeys = allKeys;
                return View(model);
            }

            AppSettingDTO dto = new AppSettingDTO()
            {
                Key = model.Key,
                Value = model.Value,
                DataType = model.DataType,
                Description = model.Description,
                Module = model.Modules.Count() > 0 ? model.Modules.FirstOrDefault().Value : null
            };

            var resultAdd = await _applicationSettingsService.CreateApplicationSetting(dto);
            if (!resultAdd.IsSuccess && ResultDTO.HandleError(resultAdd))
            {
                model.Modules = SD.Modules.GetAll().ToList();
                model.AllApplicationSettingsKeys = allKeys;
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string settingKey)
        {
            var appSettingDb = await _applicationSettingsService.GetApplicationSettingByKey(settingKey);
            if (appSettingDb == null)
            {
                // TODO: Handle Error 
                return View("Error");
            }

            ApplicationSettingsEditViewModel model = new ApplicationSettingsEditViewModel()
            {
                Key = appSettingDb.Key,
                Value = appSettingDb.Value,
                Description = appSettingDb.Description,
                DataType = appSettingDb.DataType,
                Modules = SD.Modules.GetAll().ToList(),
                InsertedModule = appSettingDb.Module,
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationSettingsEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Modules = SD.Modules.GetAll().ToList();
                return View(model);
            }

            AppSettingDTO appSettingDTO = new AppSettingDTO()
            {
                Key = model.Key,
                Value = model.Value,
                DataType = model.DataType,
                Description = model.Description,
                Module = model.Modules.Count() > 0 ? model.Modules.FirstOrDefault().Value : null
            };

            ResultDTO resUpdate = await _applicationSettingsService.UpdateApplicationSetting(appSettingDTO);
            if (!resUpdate.IsSuccess && ResultDTO.HandleError(resUpdate)) 
            {
                model.Modules = SD.Modules.GetAll().ToList();
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string? key)
        {
            if (key == null)
            {
                var errorPath = _configuration["ErrorViewsPath:Error404"];
                if (!string.IsNullOrEmpty(errorPath))
                    return Redirect(errorPath);
                else
                    return NotFound();
            }

            var appSetting = await _applicationSettingsService.GetApplicationSettingByKey(key);
            if (appSetting == null)
            {
                var errorPath = _configuration["ErrorViewsPath:Error404"];
                if (!string.IsNullOrEmpty(errorPath))
                    return Redirect(errorPath);
                else
                    return NotFound();
            }

            ApplicationSettingsDeleteViewModel model = new ApplicationSettingsDeleteViewModel()
            {
                Key = appSetting.Key,
                Value = appSetting.Value,
                Description = appSetting.Description,
                DataType = appSetting.DataType,
                InsertedModule = appSetting.Module
            };
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string key)
        {
            var resDelete = await _applicationSettingsService.DeleteApplicationSetting(key);

            if (!resDelete.IsSuccess)
            {
                return View();
                //resDelete.ErrMsg
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

using Dal.ApplicationStorage;
using Dal;
using MainApp.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.ViewModels;
using MainApp.Helpers;
using SD.Helpers;

namespace MainApp.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    [TypeFilter(typeof(UserIsInsadminResourceFilter))]
    public class ApplicationSettingsController : Controller
    {        
        private ApplicationSettingsDa _applicationSettingsDa;
        private readonly IConfiguration _configuration;

        public ApplicationSettingsController(ApplicationSettingsDa applicationSettingsDa, IConfiguration configuration)
        {           
            _applicationSettingsDa = applicationSettingsDa;
            _configuration = configuration;           
        }

        public async Task<IActionResult> Index()
        {            
            var apps = await _applicationSettingsDa.GetApplicationSettings();
            var appSettingsVM = apps.Select(a => new ApplicationSettingsViewModel()
            {
                Key = a.Key,
                Value = a.Value,
                DataType= a.DataType,
                Description= a.Description,
                Module = a.Module                
            });
            return View(appSettingsVM);
        }

        public async Task<IActionResult> Create()
        {            
            var allKeys = await _applicationSettingsDa.GetAllApplicationSettingsKeys();
            ApplicationSettingsCreateViewModel model = new ApplicationSettingsCreateViewModel();
            model.Modules = SD.Modules.GetAll().ToList();
            model.AllApplicationSettingsKeys = allKeys;
            return View(model);            
        }
               
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationSettingsCreateViewModel model)
        {           
            var allKeys = await _applicationSettingsDa.GetAllApplicationSettingsKeys();

            if (ModelState.IsValid)
            {
                await _applicationSettingsDa.AddApplicationSettings(model);
                return RedirectToAction(nameof(Index));
            }

            model.Modules = SD.Modules.GetAll().ToList();
            model.AllApplicationSettingsKeys = allKeys;
            return View(model);
        }
        
        public async Task<IActionResult> Edit(string settingKey)
        {           
            var appSettingDb = await _applicationSettingsDa.FindApplicationSetting(settingKey); 
            ApplicationSettingsEditViewModel model = new ApplicationSettingsEditViewModel();    
            
            model.Modules = SD.Modules.GetAll().ToList();
            model.InsertedModule = appSettingDb.Module;
            model.Key = appSettingDb.Key;
            model.Value = appSettingDb.Value;
            model.Description = appSettingDb.Description;
            model.DataType = appSettingDb.DataType;

            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationSettingsEditViewModel model)
        {            
            if (ModelState.IsValid)
            {
                await _applicationSettingsDa.EditApplicationSetting(model);
                return RedirectToAction(nameof(Index));
            }

            model.Modules = SD.Modules.GetAll().ToList();
            return View(model);
        }
        
        public async Task<IActionResult> Delete(string? key)
        {           
            if (key == null)
            {
                var errorPath = _configuration["ErrorViewsPath:Error404"];
                if (!string.IsNullOrEmpty(errorPath))
                {
                    return Redirect(errorPath);
                }
                else
                {
                    return NotFound();
                }
            }

            var appSetting = await _applicationSettingsDa.FindApplicationSetting(key);

            if (appSetting == null)
            {
                var errorPath = _configuration["ErrorViewsPath:Error404"];
                if (!string.IsNullOrEmpty(errorPath))
                {
                    return Redirect(errorPath);
                }
                else
                {
                    return NotFound();
                }
            }
            ApplicationSettingsDeleteViewModel model = new ApplicationSettingsDeleteViewModel();
            model.Key = appSetting.Key;
            model.Value = appSetting.Value;
            model.Description = appSetting.Description;
            model.DataType = appSetting.DataType;
            model.InsertedModule = appSetting.Module;
            return View(model);
        }
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string key)
        {            
            await _applicationSettingsDa.DeleteApplicationSetting(key);
            return RedirectToAction(nameof(Index));
        }
    }
}

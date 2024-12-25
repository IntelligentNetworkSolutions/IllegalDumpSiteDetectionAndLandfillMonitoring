using DAL.Interfaces.Helpers;
using DTOs.MainApp.BL;
using MainApp.BL.Interfaces.Services;
using MainApp.MVC.Filters;
using MainApp.MVC.ViewModels.IntranetPortal.ApplicationSettings;
using Microsoft.AspNetCore.Mvc;
using SD;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    //[TypeFilter(typeof(UserIsInsadminResourceFilter))]
    public class ApplicationSettingsController : Controller
    {
        private IApplicationSettingsService _applicationSettingsService;
        private IAppSettingsAccessor _appSettingsAccessor;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMMDetectionConfigurationService _mmDetectionService;

        public ApplicationSettingsController(IApplicationSettingsService applicationSettingsService, IConfiguration configuration, IAppSettingsAccessor appSettingsAccessor, IWebHostEnvironment webHostEnvironment, IMMDetectionConfigurationService mmDetectionService)
        {
            _applicationSettingsService = applicationSettingsService;
            _configuration = configuration;
            _appSettingsAccessor = appSettingsAccessor;
            _webHostEnvironment = webHostEnvironment;
            _mmDetectionService = mmDetectionService;
        }

        [HasAuthClaim(nameof(SD.AuthClaims.ViewApplicationSettings))]
        public async Task<IActionResult> Index()
        {
            try
            {
                var apps = await _applicationSettingsService.GetAllApplicationSettingsAsList();
                if (apps == null)
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                var appSettingsVM = apps.Select(a => new ApplicationSettingsViewModel()
                {
                    Key = a.Key,
                    Value = a.Value,
                    DataType = a.DataType,
                    Description = a.Description,
                    Module = a.Module
                });
                if(appSettingsVM == null)
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                return View(appSettingsVM);
            }
            catch (Exception)
            {
                return HandleErrorRedirect("ErrorViewsPath:Error", 400);
            }
           
        }

        [HasAuthClaim(nameof(SD.AuthClaims.CreateApplicationSettings))]
        public async Task<IActionResult> Create()
        {
            try
            {
                var allKeys = await _applicationSettingsService.GetAllApplicationSettingsKeysAsList();
                if(allKeys == null)
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                ApplicationSettingsCreateViewModel model = new ApplicationSettingsCreateViewModel()
                {
                    Modules = SD.Modules.GetAll().ToList(),
                    AllApplicationSettingsKeys = allKeys
                };

                return View(model);
            }
            catch (Exception)
            {
                return HandleErrorRedirect("ErrorViewsPath:Error", 400);
            }
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.CreateApplicationSettings))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationSettingsCreateViewModel model)
        {
            try
            {
                var allKeys = await _applicationSettingsService.GetAllApplicationSettingsKeysAsList();
                if (allKeys == null)
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

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
            catch (Exception)
            {
                return HandleErrorRedirect("ErrorViewsPath:Error", 400);
            }
        }

        [HasAuthClaim(nameof(SD.AuthClaims.UpdateApplicationSettings))]
        public async Task<IActionResult> Edit(string settingKey)
        {
            try
            {
                var appSettingDb = await _applicationSettingsService.GetApplicationSettingByKey(settingKey);
                if (appSettingDb == null)
                {
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);
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
            catch (Exception)
            {
                return HandleErrorRedirect("ErrorViewsPath:Error", 400);
            }
        }


        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.UpdateApplicationSettings))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationSettingsEditViewModel model)
        {
            try
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
            catch (Exception)
            {
                return HandleErrorRedirect("ErrorViewsPath:Error", 400);
            }
        }

        [HasAuthClaim(nameof(SD.AuthClaims.DeleteApplicationSettings))]
        public async Task<IActionResult> Delete(string? key)
        {
            try
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
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

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
            catch (Exception)
            {
                return HandleErrorRedirect("ErrorViewsPath:Error", 400);
            }
        }

        [HttpPost, ActionName("Delete")]
        [HasAuthClaim(nameof(SD.AuthClaims.DeleteApplicationSettings))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string key)
        {
            try
            {
                var resDelete = await _applicationSettingsService.DeleteApplicationSetting(key);

                if (!resDelete.IsSuccess)
                {
                    return View();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return HandleErrorRedirect("ErrorViewsPath:Error", 400);
            }
        }

        public async Task<IActionResult> DiskSize()
        {
            try
            {
                var datasetImagesFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DatasetImagesFolder", "DatasetImages");
                var datasetThumbnailsFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DatasetThumbnailsFolder", "DatasetThumbnails");
                var pointCloudFileConverts = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("LegalLandfillPointCloudFileConverts", "Uploads\\LegalLandfillUploads\\PointCloudConverts");
                var pointCloudFileUploads = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("LegalLandfillPointCloudFileUploads", "Uploads\\LegalLandfillUploads\\PointCloudUploads");
                var detectionInputImagesFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DetectionInputImagesFolder", "Uploads\\DetectionUploads\\InputImages");
                var detectionInputImagesThumbnailsFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DetectionInputImageThumbnailsFolder", "Uploads\\DetectionUploads\\InputImagesThumbnails");
                var mmDetectionTrainingFolder = _mmDetectionService.GetTrainingRunsBaseOutDirAbsPath();

                if ((datasetImagesFolder.IsSuccess == false && datasetImagesFolder.HandleError()) ||
                    (datasetThumbnailsFolder.IsSuccess == false && datasetThumbnailsFolder.HandleError()) ||
                    (pointCloudFileConverts.IsSuccess == false && pointCloudFileConverts.HandleError()) ||
                    (pointCloudFileUploads.IsSuccess == false && pointCloudFileUploads.HandleError()) ||
                    (detectionInputImagesFolder.IsSuccess == false && detectionInputImagesFolder.HandleError()) ||
                    (detectionInputImagesThumbnailsFolder.IsSuccess == false && detectionInputImagesThumbnailsFolder.HandleError()))
                {
                    return View(new List<FolderSizeViewModel>());
                }

                var folderSizes = new List<FolderSizeViewModel>
            {
                new FolderSizeViewModel
                {
                    FolderName = "Dataset Images",
                    Size = GetFolderSize(Path.Combine(_webHostEnvironment.WebRootPath, datasetImagesFolder.Data))
                },
                new FolderSizeViewModel
                {
                    FolderName = "Dataset Thumbnails",
                    Size = GetFolderSize(Path.Combine(_webHostEnvironment.WebRootPath, datasetThumbnailsFolder.Data))
                },
                new FolderSizeViewModel
                {
                    FolderName = "Point Cloud File Converts",
                    Size = GetFolderSize(Path.Combine(_webHostEnvironment.WebRootPath, pointCloudFileConverts.Data))
                },
                new FolderSizeViewModel
                {
                    FolderName = "Point Cloud File Uploads",
                    Size = GetFolderSize(Path.Combine(_webHostEnvironment.WebRootPath, pointCloudFileUploads.Data))
                },
                new FolderSizeViewModel
                {
                    FolderName = "Detection Input Image",
                    Size = GetFolderSize(Path.Combine(_webHostEnvironment.WebRootPath, detectionInputImagesFolder.Data))
                },
                new FolderSizeViewModel
                {
                    FolderName = "Detection Input Image Thumbnails",
                    Size = GetFolderSize(Path.Combine(_webHostEnvironment.WebRootPath, detectionInputImagesThumbnailsFolder.Data))
                },
                new FolderSizeViewModel
                {
                    FolderName = "Training Runs",
                    Size = GetFolderSize(mmDetectionTrainingFolder)
                }
            };

                return View(folderSizes);
            }
            catch (Exception)
            {
                return HandleErrorRedirect("ErrorViewsPath:Error", 400);
            }
        }


        private long GetFolderSize(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
                return 0;

            var files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);

            long totalSize = 0;
            foreach (var file in files)
            {
                try
                {
                    totalSize += new FileInfo(file).Length;
                }
                catch
                {
                    // TODO Handle access exceptions if needed
                }
            }

            return totalSize;
        }
        private IActionResult HandleErrorRedirect(string configKey, int statusCode)
        {
            string? errorPath = _configuration[configKey];
            if (string.IsNullOrEmpty(errorPath))
            {
                return statusCode switch
                {
                    404 => NotFound(),
                    403 => Forbid(),
                    405 => StatusCode(405),
                    _ => BadRequest()
                };
            }
            return Redirect(errorPath);
        }
    }
}

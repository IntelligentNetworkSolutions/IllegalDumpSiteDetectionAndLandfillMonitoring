using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL.ApplicationStorage;
using Entities.DatasetEntities;
using AutoMapper;
using MainApp.BL.Interfaces.Services.DatasetServices;
using DTOs.MainApp.BL.DatasetDTOs;
using MainApp.MVC.Helpers;
using Humanizer;
using System.Drawing;
using System.Drawing.Imaging;
using ImageMagick;
using DAL.Interfaces.Helpers;
using System.Data;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    public class DatasetImagesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IDatasetService _datasetService;
        private readonly IDatasetImagesService _datasetImagesService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IAppSettingsAccessor _appSettingsAccessor;

        public DatasetImagesController(IConfiguration configuration, 
                                       IMapper mapper, 
                                       IDatasetService datasetService, 
                                       IDatasetImagesService datasetImagesService, 
                                       IWebHostEnvironment webHostEnvironment,
                                       IAppSettingsAccessor appSettingsAccessor)
        {
            _configuration = configuration;
            _mapper = mapper;
            _datasetService = datasetService;
            _datasetImagesService = datasetImagesService;
            _webHostEnvironment = webHostEnvironment;
            _appSettingsAccessor = appSettingsAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDatasetImage(Guid datasetImageId, Guid datasetId)
        {
            // TODO: add check claim
            //if (!User.HasAuthClaim(SD.AuthClaims.DeleteDatasetImage) || !_modulesAndAuthClaims.HasModule(SD.Modules.Datasets))
            //{
            //    var errorPath = _configuration["ErrorViewsPath:Error403"];
            //    if (!string.IsNullOrEmpty(errorPath))
            //    {
            //        return Redirect(errorPath);
            //    }
            //    else
            //    {
            //        return StatusCode(403);
            //    }
            //}
            if (datasetId == Guid.Empty)
            {
                return Json(new { responseError = DbResHtml.T("Invalid dataset id", "Resources") });
            }
            if (datasetId == Guid.Empty)
            {
                return Json(new { responseError = DbResHtml.T("Invalid dataset id", "Resources") });
            }

            var datasetDb = await _datasetService.GetDatasetById(datasetId) ?? throw new Exception("Dataset not found");
            if (datasetDb.IsPublished == true)
            {
                return Json(new { responseErrorAlreadyPublished = DbResHtml.T("Dataset is already published. No changes allowed", "Resources") });
            }

            var datasetImagesFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DatasetImagesFolder", "DatasetImages");
            var datasetThumbnailsFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DatasetThumbnailsFolder", "DatasetThumbnails");

            var currentImageFileName = string.Format("{0}.jpg", datasetImageId);
            var currentThumbnailFileName = string.Format("{0}.jpg", datasetImageId);

            var currentImagePath = Path.Combine(_webHostEnvironment.WebRootPath, datasetImagesFolder.Data, datasetId.ToString(), currentImageFileName);
            var currentThumbnailPath = Path.Combine(_webHostEnvironment.WebRootPath, datasetThumbnailsFolder.Data, datasetId.ToString(), currentThumbnailFileName);

            var isImageDeleted = await _datasetImagesService.DeleteDatasetImage(datasetImageId);
            if (isImageDeleted == 1)
            {
                if (System.IO.File.Exists(currentImagePath))
                {
                    System.IO.File.Delete(currentImagePath);
                }
                if (System.IO.File.Exists(currentThumbnailPath))
                {
                    System.IO.File.Delete(currentThumbnailPath);
                }
                return Json(new { responseSuccess = DbResHtml.T("Successfully deleted dataset image", "Resources") });
            }
            else
            {
                return Json(new { responseError = DbResHtml.T("Dataset image was not deleted", "Resources") });
            }

        }

        [HttpPost]
        public async Task<IActionResult> EditDatasetImage(EditDatasetImageDTO model)
        {
            // TODO: add check claim
            //if (!User.HasAuthClaim(SD.AuthClaims.EditDatasetImage) || !_modulesAndAuthClaims.HasModule(SD.Modules.Datasets))
            //{
            //    var errorPath = _configuration["ErrorViewsPath:Error403"];
            //    if (!string.IsNullOrEmpty(errorPath))
            //    {
            //        return Redirect(errorPath);
            //    }
            //    else
            //    {
            //        return StatusCode(403);
            //    }
            //}
            if (!ModelState.IsValid)
            {
                return Json(new { responseError = DbResHtml.T("Entered model is not valid", "Resources") });
            }

            var datasetDb = await _datasetService.GetDatasetById(model.DatasetId) ?? throw new Exception("Dataset not found");
            if (datasetDb.IsPublished == true)
            {
                return Json(new { responseErrorAlreadyPublished = DbResHtml.T("Dataset is already published. No changes allowed", "Resources") });
            }

            model.FileName = string.Format("{0}.jpg", model.Name);
            var isImageUpdated = await _datasetImagesService.EditDatasetImage(model);
            if (isImageUpdated == 1)
            {
                return Json(new { responseSuccess = DbResHtml.T("Successfully updated dataset image", "Resources") });
            }
            else if (isImageUpdated == 2)
            {
                return Json(new { responseError = DbResHtml.T("You can not enable this image because there are not annotations!", "Resources") });
            }
            else
            {
                return Json(new { responseError = DbResHtml.T("Dataset image was not updated", "Resources") });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadDatasetImage(Guid datasetId, string imageCropped, string imageName)
        {
            // TODO: add check claim
            //if (!User.HasAuthClaim(SD.AuthClaims.AddDatasetImage) || !_modulesAndAuthClaims.HasModule(SD.Modules.Datasets))
            //{
            //    var errorPath = _configuration["ErrorViewsPath:Error403"];
            //    if (!string.IsNullOrEmpty(errorPath))
            //    {
            //        return Redirect(errorPath);
            //    }
            //    else
            //    {
            //        return StatusCode(403);
            //    }
            //}
            if (datasetId == Guid.Empty)
            {
                return Json(new { responseError = DbResHtml.T("Invalid dataset id", "Resources") });
            }
            if (string.IsNullOrEmpty(imageCropped))
            {
                return Json(new { responseError = DbResHtml.T("Invalid image", "Resources") });
            }
            if (string.IsNullOrEmpty(imageName))
            {
                return Json(new { responseError = DbResHtml.T("Invalid image name", "Resources") });
            }

            var datasetDb = await _datasetService.GetDatasetById(datasetId) ?? throw new Exception("Dataset not found");
            if (datasetDb.IsPublished == true)
            {
                return Json(new { responseErrorAlreadyPublished = DbResHtml.T("Dataset is already published. No changes allowed", "Resources") });
            }

            var datasetImagesFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DatasetImagesFolder", "DatasetImages");
            var datasetThumbnailsFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DatasetThumbnailsFolder", "DatasetThumbnails");
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, datasetImagesFolder.Data, datasetDb.Id.ToString());
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            DatasetImageDTO dto = new()
            {
                FileName = $"{imageName}.jpg",
                Name = imageName,
                DatasetId = datasetId,
                IsEnabled = false,
                CreatedOn = DateTime.UtcNow,
                CreatedById = "3ae81c64-f69f-4245-9c78-c315ca706a0b",
                ImagePath = string.Format("\\{0}\\{1}\\", datasetImagesFolder.Data, datasetDb.Id.ToString()),
                ThumbnailPath = string.Format("\\{0}\\{1}\\", datasetThumbnailsFolder.Data, datasetDb.Id.ToString()),
            };

            var isImageAdded = await _datasetImagesService.AddDatasetImage(dto);
            if (isImageAdded != Guid.Empty)
            {
                var imageFileName = string.Format("{0}.jpg", isImageAdded);
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, datasetImagesFolder.Data, datasetDb.Id.ToString(), imageFileName);
                string base64Datas = imageCropped.Split(',')[1];
                byte[] imageBytes = Convert.FromBase64String(base64Datas);

                using (MemoryStream ms = new(imageBytes))
                {
                    using (MagickImage image = new(ms))
                    {
                        image.Quality = 95;
                        image.Write(imagePath);
                    }
                }
                SaveTumbnailImage(datasetId, isImageAdded, imageCropped, datasetThumbnailsFolder.Data);
                return Json(new { responseSuccess = DbResHtml.T("Successfully added dataset image", "Resources") });
            }
            else
            {
                return Json(new { responseError = DbResHtml.T("Dataset image was not added", "Resources") });
            }
        }

        private void SaveTumbnailImage(Guid datasetId, Guid addedImageId, string originalImage, string thumbnailsFolderData)
        {
            string thumbnailsFolder = Path.Combine(_webHostEnvironment.WebRootPath, thumbnailsFolderData, datasetId.ToString());
            if (!Directory.Exists(thumbnailsFolder))
            {
                Directory.CreateDirectory(thumbnailsFolder);
            }

            var thumbnailFileName = string.Format("{0}.jpg", addedImageId);
            var thumbnailPath = Path.Combine(_webHostEnvironment.WebRootPath, thumbnailsFolder, thumbnailFileName);
            string base64DatasThumbnail = originalImage.Split(',')[1];
            byte[] thumbnailBytes = Convert.FromBase64String(base64DatasThumbnail);
            using (MemoryStream msThumbnail = new(thumbnailBytes))
            {
                using (MagickImage imageThumbnail = new(msThumbnail))
                {
                    imageThumbnail.Resize(192, 192);
                    imageThumbnail.Quality = 95;
                    imageThumbnail.Write(thumbnailPath);
                }
            }
        }
    }
}

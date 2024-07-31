using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using MainApp.BL.Interfaces.Services.DatasetServices;
using DTOs.MainApp.BL.DatasetDTOs;
using MainApp.MVC.Helpers;
using ImageMagick;
using DAL.Interfaces.Helpers;
using MainApp.MVC.Filters;
using System.Security.Claims;
using SD;

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
        [HasAuthClaim(nameof(SD.AuthClaims.DeleteDatasetImage))]
        public async Task<IActionResult> DeleteDatasetImage(Guid datasetImageId, Guid datasetId)
        {
            if (datasetId == Guid.Empty)
            {
                return Json(new { responseError = DbResHtml.T("Invalid dataset id", "Resources") });
            }
            if (datasetImageId == Guid.Empty)
            {
                return Json(new { responseError = DbResHtml.T("Invalid dataset image id", "Resources") });
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
            if (isImageDeleted.IsSuccess == true && isImageDeleted.Data == 1 && string.IsNullOrEmpty(isImageDeleted.ErrMsg))
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
            if (!string.IsNullOrEmpty(isImageDeleted.ErrMsg))
            {
                return Json(new { responseError = DbResHtml.T(isImageDeleted.ErrMsg, "Resources") });
            }
            return Json(new { responseError = DbResHtml.T("Error occured while deleting the image", "Resources") });
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.EditDatasetImage))]
        public async Task<IActionResult> EditDatasetImage(EditDatasetImageDTO model)
        {
            if (ModelState.IsValid == false)
                return Json(new { responseError = DbResHtml.T("Entered model is not valid", "Resources") });

            string? userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { responseError = DbResHtml.T("User id is not valid", "Resources") });

            DatasetDTO datasetDb = await _datasetService.GetDatasetById(model.DatasetId) ?? throw new Exception("Dataset not found");
            if (datasetDb.IsPublished == true)
                return Json(new { responseErrorAlreadyPublished = DbResHtml.T("Dataset is already published. No changes allowed", "Resources") });

            model.FileName = string.Format("{0}.jpg", model.Name);
            model.UpdatedById = userId;
            ResultDTO<int> resultImageUpdate = await _datasetImagesService.EditDatasetImage(model);
            if (resultImageUpdate.IsSuccess == true && resultImageUpdate.Data == 1 && string.IsNullOrEmpty(resultImageUpdate.ErrMsg))
                return Json(new { responseSuccess = DbResHtml.T("Successfully updated dataset image", "Resources") });

            if (!string.IsNullOrEmpty(resultImageUpdate.ErrMsg))
                return Json(new { responseError = DbResHtml.T(resultImageUpdate.ErrMsg, "Resources") });

            return Json(new { responseError = DbResHtml.T("Dataset image was not updated", "Resources") });
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.AddDatasetImage))]
        public async Task<IActionResult> UploadDatasetImage(Guid datasetId, string imageCropped, string imageName)
        {
            string? userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { responseError = DbResHtml.T("User id is not valid", "Resources") });

            if (datasetId == Guid.Empty)
                return Json(new { responseError = DbResHtml.T("Invalid dataset id", "Resources") });

            if (string.IsNullOrEmpty(imageCropped))
                return Json(new { responseError = DbResHtml.T("Invalid image", "Resources") });

            if (string.IsNullOrEmpty(imageName))
                return Json(new { responseError = DbResHtml.T("Invalid image name", "Resources") });

            DatasetDTO datasetDb = await _datasetService.GetDatasetById(datasetId) ?? throw new Exception("Dataset not found");
            if (datasetDb.IsPublished == true)
                return Json(new { responseErrorAlreadyPublished = DbResHtml.T("Dataset is already published. No changes allowed", "Resources") });

            ResultDTO<string?> datasetImagesFolder =
                await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DatasetImagesFolder", "DatasetImages");
            ResultDTO<string?> datasetThumbnailsFolder =
                await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DatasetThumbnailsFolder", "DatasetThumbnails");
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, datasetImagesFolder.Data, datasetDb.Id.ToString());
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            DatasetImageDTO dto = new()
            {
                FileName = $"{imageName}.jpg",
                Name = imageName,
                DatasetId = datasetId,
                IsEnabled = false,
                CreatedOn = DateTime.UtcNow,
                CreatedById = userId,
                ImagePath = string.Format("\\{0}\\{1}\\", datasetImagesFolder.Data, datasetDb.Id.ToString()),
                ThumbnailPath = string.Format("\\{0}\\{1}\\", datasetThumbnailsFolder.Data, datasetDb.Id.ToString()),
            };

            ResultDTO<Guid> resultImageAdd = await _datasetImagesService.AddDatasetImage(dto);
            if (resultImageAdd.IsSuccess && resultImageAdd.Data != Guid.Empty)
            {
                string imageFileName = string.Format("{0}.jpg", resultImageAdd.Data);
                string imagePath =
                    Path.Combine(_webHostEnvironment.WebRootPath, datasetImagesFolder.Data, datasetDb.Id.ToString(), imageFileName);
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
                SaveTumbnailImage(datasetId, resultImageAdd.Data, imageCropped, datasetThumbnailsFolder.Data);
                return Json(new { responseSuccess = DbResHtml.T("Successfully added dataset image", "Resources") });
            }

            return Json(new { responseError = DbResHtml.T(resultImageAdd.ErrMsg, "Resources") });
        }

        private ResultDTO SaveTumbnailImage(Guid datasetId, Guid addedImageId, string originalImage, string thumbnailsFolderData)
        {
            try
            {
                string thumbnailsFolder = Path.Combine(_webHostEnvironment.WebRootPath, thumbnailsFolderData, datasetId.ToString());
                if (Directory.Exists(thumbnailsFolder) == false)
                    Directory.CreateDirectory(thumbnailsFolder);

                string thumbnailFileName = string.Format("{0}.jpg", addedImageId);
                string thumbnailPath = Path.Combine(_webHostEnvironment.WebRootPath, thumbnailsFolder, thumbnailFileName);
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

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }
    }
}
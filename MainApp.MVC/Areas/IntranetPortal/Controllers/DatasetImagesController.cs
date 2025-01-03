﻿using AutoMapper;
using DAL.Interfaces.Helpers;
using DTOs.MainApp.BL.DatasetDTOs;
using ImageMagick;
using MainApp.BL.Interfaces.Services;
using MainApp.BL.Interfaces.Services.DatasetServices;
using MainApp.MVC.Filters;
using MainApp.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using SD;
using System.Security.Claims;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    public class DatasetImagesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IDatasetService _datasetService;
        private readonly IDatasetImagesService _datasetImagesService;
        private readonly IImageAnnotationsService _imageAnnotationsService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IAppSettingsAccessor _appSettingsAccessor;

        public DatasetImagesController(IConfiguration configuration,
                                       IMapper mapper,
                                       IDatasetService datasetService,
                                       IDatasetImagesService datasetImagesService,
                                       IImageAnnotationsService imageAnnotationsService,
                                       IWebHostEnvironment webHostEnvironment,
                                       IAppSettingsAccessor appSettingsAccessor)
        {
            _configuration = configuration;
            _mapper = mapper;
            _datasetService = datasetService;
            _datasetImagesService = datasetImagesService;
            _imageAnnotationsService = imageAnnotationsService;
            _webHostEnvironment = webHostEnvironment;
            _appSettingsAccessor = appSettingsAccessor;
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.DeleteDatasetImage))]
        public async Task<IActionResult> DeleteDatasetImage(Guid datasetImageId, Guid datasetId, bool deleteAnnotations = false)
        {
            if (datasetId == Guid.Empty)
            {
                return Json(new { responseError = DbResHtml.T("Invalid dataset id", "Resources") });
            }
            if (datasetImageId == Guid.Empty)
            {
                return Json(new { responseError = DbResHtml.T("Invalid dataset image id", "Resources") });
            }
            try
            {
                var datasetDb = await _datasetService.GetDatasetById(datasetId);

                if (datasetDb.IsSuccess == false && datasetDb.HandleError())
                    return Json(new { responseError = DbResHtml.T(datasetDb.ErrMsg!, "Resources") });

                if (datasetDb is null)
                    return Json(new { responseError = DbResHtml.T("Dataset not found", "Resources") });

                if (datasetDb.Data.IsPublished)
                {
                    return Json(new { responseErrorAlreadyPublished = DbResHtml.T("Dataset is already published. No changes allowed", "Resources") });
                }

                var datasetImageDb = await _datasetImagesService.GetDatasetImageById(datasetImageId);
                if (datasetImageDb.IsSuccess == false && datasetImageDb.HandleError())
                {
                    return Json(new { responseError = DbResHtml.T($"Dataset image not found.{datasetDb.ErrMsg}", "Resources") });
                }

                var activeAnnotations = await _imageAnnotationsService.GetImageAnnotationsByImageId(datasetImageId);
                if (activeAnnotations.IsSuccess == false && activeAnnotations.HandleError())
                {
                    return Json(new { responseError = DbResHtml.T($"Failed to retrive annotations.{activeAnnotations.ErrMsg}", "Resources") });

                }
                if (activeAnnotations.Data.Any() && !deleteAnnotations)
                {
                    return Json(new
                    {
                        responseAnnotated = true,
                        responseError = DbResHtml.T("This image has active annotations. Do you want to continue anyway?", "Resources")
                    });
                }

                ResultDTO<int> isImageDeleted = await _datasetImagesService.DeleteDatasetImage(datasetImageId, deleteAnnotations);
                if (isImageDeleted.IsSuccess == false && isImageDeleted.HandleError())
                {
                    return Json(new { responseError = DbResHtml.T(isImageDeleted.ErrMsg ?? "Error occurred while deleting the image", "Resources") });
                }

                return Json(new { responseSuccess = DbResHtml.T("Successfully deleted dataset image", "Resources") });
            }
            catch (Exception ex)
            {
                return Json(new { responseError = DbResHtml.T($"Error occurred while deleting the image. {ex.Message}", "Resources") });
            }
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

            try
            {
                ResultDTO<DatasetDTO> datasetDb = await _datasetService.GetDatasetById(model.DatasetId);
                if (datasetDb.IsSuccess == false && datasetDb.HandleError())
                    return Json(new { responseError = DbResHtml.T(datasetDb.ErrMsg!, "Resources") });

                if (datasetDb.Data.IsPublished == true)
                    return Json(new { responseErrorAlreadyPublished = DbResHtml.T("Dataset is already published. No changes allowed", "Resources") });

                model.FileName = string.Format("{0}.jpg", model.Name);
                model.UpdatedById = userId;

                ResultDTO<int> resultImageUpdate = await _datasetImagesService.EditDatasetImage(model);

                if (resultImageUpdate.IsSuccess && resultImageUpdate.Data == 1)
                    return Json(new { responseSuccess = DbResHtml.T("Successfully updated dataset image", "Resources") });

                if (resultImageUpdate.IsSuccess == false && resultImageUpdate.HandleError())
                    return Json(new { responseError = DbResHtml.T(resultImageUpdate.ErrMsg ?? "An error occurred while updating the dataset image", "Resources") });

                return Json(new { responseError = DbResHtml.T("Dataset image was not updated", "Resources") });
            }
            catch (Exception ex)
            {
                return Json(new { responseError = DbResHtml.T($"An unexpected error occurred: {ex.Message}", "Resources") });
            }
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

            try
            {
                ResultDTO<DatasetDTO> datasetDb = await _datasetService.GetDatasetById(datasetId);
                if (datasetDb.IsSuccess == false && datasetDb.HandleError())
                    return Json(new { responseError = DbResHtml.T(datasetDb.ErrMsg!, "Resources") });

                if (datasetDb.Data.IsPublished == true)
                    return Json(new { responseErrorAlreadyPublished = DbResHtml.T("Dataset is already published. No changes allowed", "Resources") });

                ResultDTO<string?> datasetImagesFolder =
                    await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DatasetImagesFolder", "DatasetImages");
                ResultDTO<string?> datasetThumbnailsFolder =
                    await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DatasetThumbnailsFolder", "DatasetThumbnails");

                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, datasetImagesFolder.Data, datasetDb.Data.Id.ToString());
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);
                // Extract image width and height
                int imageWidth = 0;
                int imageHeight = 0;
                using (var ms = new MemoryStream(Convert.FromBase64String(imageCropped.Split(',')[1])))
                {
                    using (var image = new MagickImage(ms))
                    {
                        imageWidth = image.Width;
                        imageHeight = image.Height;
                    }
                }

                DatasetImageDTO dto = new()
                {
                    FileName = $"{imageName}.jpg",
                    Name = imageName,
                    DatasetId = datasetId,
                    IsEnabled = false,
                    CreatedOn = DateTime.UtcNow,
                    CreatedById = userId,
                    ImagePath = Path.Combine(datasetImagesFolder.Data, datasetDb.Data.Id.ToString()),
                    ThumbnailPath = Path.Combine(datasetThumbnailsFolder.Data, datasetDb.Data.Id.ToString()),
                    Width = imageWidth,
                    Height = imageHeight
                };

                ResultDTO<Guid> resultImageAdd = await _datasetImagesService.AddDatasetImage(dto);
                if (resultImageAdd.IsSuccess && resultImageAdd.Data != Guid.Empty)
                {
                    string imageFileName = string.Format("{0}.jpg", resultImageAdd.Data);
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, datasetImagesFolder.Data, datasetDb.Data.Id.ToString(), imageFileName);
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
            catch (Exception ex)
            {
                return Json(new { responseError = DbResHtml.T($"An unexpected error occurred: {ex.Message}", "Resources") });
            }
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
using AutoMapper;
using DAL.Interfaces.Helpers;
using DTOs.MainApp.BL.DatasetDTOs;
using MainApp.BL.Interfaces.Services;
using MainApp.BL.Interfaces.Services.DatasetServices;
using MainApp.MVC.Filters;
using MainApp.MVC.ViewModels.IntranetPortal.Annotations;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;
using SD;
using System.Security.Claims;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    public class AnnotationsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IAppSettingsAccessor _appSettingsAccessor;
        private readonly IDatasetService _datasetService;
        private readonly IDatasetImagesService _datasetImagesService;
        private readonly IImageAnnotationsService _imageAnnotationsService;
        private readonly IDatasetClassesService _datasetClassesService;

        public AnnotationsController(IConfiguration configuration,
            IMapper mapper,
            IAppSettingsAccessor appSettingsAccessor,
            IDatasetService datasetServoce,
            IDatasetImagesService datasetImagesService,
            IImageAnnotationsService imageAnnotationsService,
            IDatasetClassesService datasetClassesService)
        {
            _configuration = configuration;
            _mapper = mapper;
            _appSettingsAccessor = appSettingsAccessor;
            _datasetImagesService = datasetImagesService;
            _imageAnnotationsService = imageAnnotationsService;
            _datasetService = datasetServoce;
            _datasetClassesService = datasetClassesService;
        }


        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Annotate(Guid datasetImageId)
        {
            try
            {
                if (datasetImageId == Guid.Empty)
                {
                    return Json(new { responseSuccess = false, responseError = "Invalid dataset image ID." });
                }

                var datasetImage = await _datasetImagesService.GetDatasetImageById(datasetImageId);
                if (datasetImage.IsSuccess == false && datasetImage.HandleError())
                {
                    return Json(new { responseSuccess = false, responseError = "An error occurred. Could not retrieve dataset image." });
                }

                var datasetAllImagesResult = await _datasetImagesService.GetImagesForDataset((Guid)datasetImage.Data.DatasetId);
                if (datasetAllImagesResult.IsSuccess == false && datasetAllImagesResult.HandleError())
                {
                    return Json(new { responseSuccess = false, responseError = "An error occurred. Could not retrieve dataset images." });
                }

                var dataset = await _datasetService.GetDatasetById(datasetImage.Data.DatasetId.GetValueOrDefault());
                if (dataset.IsSuccess == false && dataset.HandleError())
                {
                    return Json(new { responseSuccess = false, responseError = "An error occurred. Could not retrieve dataset." });
                }

                var datasetClasses = await _datasetClassesService.GetAllDatasetClassesByDatasetId(dataset.Data.Id);
                if (datasetClasses.IsSuccess == false && datasetClasses.HandleError())
                {
                    return Json(new { responseSuccess = false, responseError = "An error occurred. Could not retrieve dataset classes." });
                }

                var currentImagePositionInDataset = datasetAllImagesResult
                    .Data.IndexOf(datasetAllImagesResult.Data.First(x => x.Id == datasetImage.Data.Id));

                var nextImage = (currentImagePositionInDataset + 1) < datasetAllImagesResult.Data.Count
                                ? datasetAllImagesResult.Data[currentImagePositionInDataset + 1]
                                : datasetAllImagesResult.Data.First();

                var previousImage = (currentImagePositionInDataset > 0)
                                    ? datasetAllImagesResult.Data[currentImagePositionInDataset - 1]
                                    : datasetAllImagesResult.Data.Last();

                AnnotateViewModel model = new AnnotateViewModel
                {
                    DatasetImage = datasetImage.Data,
                    Dataset = dataset.Data,
                    DatasetClasses = datasetClasses.Data,
                    CurrentImagePositionInDataset = currentImagePositionInDataset + 1,
                    NextImage = nextImage,
                    PreviousImage = previousImage,
                    TotalImagesCount = datasetAllImagesResult.Data.Count
                };

                bool isAjaxRequest = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

                if (isAjaxRequest)
                {
                    return Json(new { responseSuccess = true, model });
                }

                return View(model);
            }
            catch (Exception ex)
            {
                return Json(new { responseSuccess = false, responseError = "An unexpected error occurred: " + ex.Message });
            }
        }


        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewDatasetImageAnnotations))]
        public async Task<IActionResult> GetImageAnnotations(Guid datasetImageId)
        {
            try
            {
                if (datasetImageId == Guid.Empty)
                {
                    return Json(new { responseSuccess = false, responseError = "Such image does not exist." });
                }

                var imageAnnotations = await _imageAnnotationsService.GetImageAnnotationsByImageId(datasetImageId);
                /*                if (imageAnnotations.IsSuccess == false && imageAnnotations.HandleError())
                                    return Json(new { responseSuccess = false, responseError = "No annotations found for this image." });*/ // could be empty

                return Ok(new { responseSuccess = true, data = imageAnnotations.Data });
            }
            catch (Exception ex)
            {
                return Json(new { responseSuccess = false, responseError = "An unexpected error occurred: " + ex.Message });
            }
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.EditDatasetImageAnnotations))]
        public async Task<IActionResult> SaveImageAnnotations(EditImageAnnotationsDTO editImageAnnotations)
        {
            try
            {
                List<ImageAnnotationDTO> newImageAnnotationsList = new List<ImageAnnotationDTO>();

                if (editImageAnnotations.ImageAnnotations != null)
                {
                    string? userId = User.FindFirstValue("UserId");

                    newImageAnnotationsList = editImageAnnotations.ImageAnnotations.Select(p => p with
                    {
                        DatasetImageId = editImageAnnotations.DatasetImageId,
                        Geom = (Polygon)DTOs.Helpers.GeoJsonHelpers.GeoJsonFeatureToGeometry(p.AnnotationJson),
                        CreatedById = p.Id.HasValue ? null : userId,
                        UpdatedById = p.Id.HasValue ? userId : null
                    }).ToList();
                }

                var newEditImageAnnotations = new EditImageAnnotationsDTO
                {
                    DatasetImageId = editImageAnnotations.DatasetImageId,
                    ImageAnnotations = newImageAnnotationsList
                };

                var res = await _imageAnnotationsService.BulkUpdateImageAnnotations(newEditImageAnnotations);
                if (res.IsSuccess == false && res.HandleError())
                    return Json(new { isSuccess = false, errMsg = res.ErrMsg! });

                return Ok(new { isSuccess = true, data = res });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, errMsg = "An unexpected error occurred: " + ex.Message });
            }
        }
    }
}

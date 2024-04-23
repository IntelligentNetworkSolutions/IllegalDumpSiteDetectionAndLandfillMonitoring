using AutoMapper;
using DAL.Interfaces.Helpers;
using DTOs.MainApp.BL.DatasetDTOs;
using Entities.DatasetEntities;
using MainApp.BL.Interfaces.Services;
using MainApp.BL.Interfaces.Services.DatasetServices;

using MainApp.MVC.Filters;
using MainApp.MVC.ViewModels.IntranetPortal.Annotations;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;
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

        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewDatasetImageAnnotations))]
        public async Task<IActionResult> Annotate(Guid datasetImageId)
        {
            if (datasetImageId ==  Guid.Empty)
            {
                throw new Exception("Object not found");
            }            

            var datasetImage = await _datasetImagesService.GetDatasetImageById(datasetImageId) ?? throw new Exception("Object not found");
            //var imageAnnotations = await _imageAnnotationsService.GetImageAnnotationsByImageId(datasetImageId);
            var dataset = await _datasetService.GetDatasetById(datasetImage.DatasetId.GetValueOrDefault());
            var datasetClasses = await _datasetClassesService.GetAllDatasetClassesByDatasetId(dataset.Id);

            AnnotateViewModel model = new AnnotateViewModel();
            model.DatasetImage = datasetImage;
            //model.ImageAnnotations = imageAnnotations;
            model.Dataset = dataset;
            model.DatasetClasses = datasetClasses;

            return View(model);
        }

        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewDatasetImageAnnotations))]
        public async Task<IActionResult> GetImageAnnotations(Guid datasetImageId)
        {
            if (datasetImageId == Guid.Empty)
            {
                throw new Exception("Object not found");
            }
            
            var imageAnnotations = await _imageAnnotationsService.GetImageAnnotationsByImageId(datasetImageId);

            return Ok(imageAnnotations);
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.EditDatasetImageAnnotations))]
        public async Task<IActionResult> SaveImageAnnotations(EditImageAnnotationsDTO editImageAnnotations)
        {
            List<ImageAnnotationDTO> newImageAnnotationsList = new List<ImageAnnotationDTO>();
            EditImageAnnotationsDTO newEditImageAnnotations = null;
            if (editImageAnnotations.ImageAnnotations != null)
            {
                string? userId = User.FindFirstValue("UserId");

                newImageAnnotationsList = editImageAnnotations.ImageAnnotations.Select(p => p with {
                    DatasetImageId = editImageAnnotations.DatasetImageId,
                    Geom = (Polygon)DTOs.Helpers.GeoJsonHelpers.GeoJsonFeatureToGeometry(p.AnnotationJson),
                    CreatedById = p.Id.HasValue ? null : userId,
                    UpdatedById = p.Id.HasValue ? userId : null                    
                }).ToList();                

            }

            newEditImageAnnotations = new EditImageAnnotationsDTO
            {
                DatasetImageId = editImageAnnotations.DatasetImageId,
                ImageAnnotations = newImageAnnotationsList
            };

            var res = await _imageAnnotationsService.BulkUpdateImageAnnotations(newEditImageAnnotations);
                        
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> VGGAnnotator(string? datasetImageId = null)
        {
            string imgToAnnotatePath = string.Empty;
            if (string.IsNullOrEmpty(datasetImageId))
            {
                imgToAnnotatePath = "";
            }

            imgToAnnotatePath = datasetImageId;

            return View((imgToAnnotatePath));
        }
    }
}

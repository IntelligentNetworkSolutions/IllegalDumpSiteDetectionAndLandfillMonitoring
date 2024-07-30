using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using DAL.Interfaces.Helpers;
using MainApp.MVC.Helpers;
using MainApp.BL.Interfaces.Services.DatasetServices;
using MainApp.MVC.ViewModels.IntranetPortal.Dataset;
using DTOs.MainApp.BL.DatasetDTOs;
using System.Data;
using Westwind.Globalization;
using X.PagedList;
using MainApp.MVC.Filters;
using System.Security.Claims;
using SD;
using DTOs.MainApp.BL;
using DTOs.ObjectDetection.API.CocoFormatDTOs;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    public class DatasetsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IDatasetService _datasetService;
        private readonly IDatasetImagesService _datasetImagesService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IAppSettingsAccessor _appSettingsAccessor;

        public DatasetsController(IConfiguration configuration,
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

        [HasAuthClaim(nameof(SD.AuthClaims.ViewDatasets))]
        public async Task<IActionResult> Index()
        {
            List<DatasetDTO> datasetsList = await _datasetService.GetAllDatasets();
            List<DatasetViewModel> model = _mapper.Map<List<DatasetViewModel>>(datasetsList) ?? throw new Exception("Model not found");

            List<DatasetViewModel> parentDatasetsList = model.Where(d => d.ParentDatasetId == null).OrderBy(x => x.Name).ToList();
            //string optionsHtml = GenerateDatasetOptionsHtml(parentDatasetsList, model,"");
            //ViewBag.OptionsHtml = optionsHtml;

            return View(model);
        }

        // TODO: Add Auth Claim
        public async Task<List<DatasetDTO>> GetAllDatasets()
        {
            var datasets = await _datasetService.GetAllDatasets() ?? throw new Exception("Object not found");
            return datasets;
        }

        [HttpPost]
        public async Task<IActionResult> GetParentAndChildrenDatasets(Guid currentDatasetId)
        {
            if (currentDatasetId == Guid.Empty)
                return Json(new { responseError = DbResHtml.T("Invalid dataset id", "Resources") });
            List<DatasetDTO> allDatasetsDb = await _datasetService.GetAllDatasets() ?? throw new Exception("Object not found");
            DatasetDTO currentDatasetDb = await _datasetService.GetDatasetById(currentDatasetId) ?? throw new Exception("Object not found");
            if (currentDatasetDb == null)
                return Json(new { responseError = DbResHtml.T("Invalid current dataset", "Resources") });

            List<DatasetDTO> childrenDatasets = allDatasetsDb.Where(x => x.ParentDatasetId == currentDatasetId).ToList() ?? throw new Exception("Object not found");
            DatasetDTO? parentDataset = currentDatasetDb.ParentDataset;
            if (parentDataset == null)
            {
                DatasetDTO datasetDTO = new();
                return Json(new { parent = datasetDTO, childrenList = childrenDatasets, currentDataset = currentDatasetDb });
            }

            return Json(new { parent = parentDataset, childrenList = childrenDatasets, currentDataset = currentDatasetDb });
        }


        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.AddDataset))]
        public async Task<IActionResult> CreateConfirmed(CreateDatasetViewModel datasetViewModel)
        {
            string? userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { responseError = DbResHtml.T("User id is not valid", "Resources") });

            if (!ModelState.IsValid)
                return Json(new { responseError = DbResHtml.T("Dataset model is not valid", "Resources") });

            DatasetDTO selectedParentDataset;
            if (datasetViewModel.ParentDatasetId != null)
            {
                selectedParentDataset = await _datasetService.GetDatasetById(datasetViewModel.ParentDatasetId.Value);
                if (selectedParentDataset.IsPublished == false)
                    return Json(new { responseError = DbResHtml.T("Selected parent dataset is not published. You can not add subdataset for unpublished datasets!", "Resources") });
            }

            datasetViewModel.CreatedById = userId;
            DatasetDTO dataSetDto = _mapper.Map<DatasetDTO>(datasetViewModel) ?? throw new Exception("Model not found");
            var insertedDatasetDTO = await _datasetService.CreateDataset(dataSetDto) ?? throw new Exception("Model not found");
            if (datasetViewModel.ParentDatasetId != null)
                await _datasetService.AddInheritedParentClasses(insertedDatasetDTO.Id, datasetViewModel.ParentDatasetId.Value);

            return Json(new { id = insertedDatasetDTO.Id });
        }

        // TODO: Create ViewModel
        [HasAuthClaim(nameof(SD.AuthClaims.ManageDataset))]
        public async Task<IActionResult> Edit(Guid datasetId, int? page, string? SearchByImageName, bool? SearchByIsAnnotatedImage, bool? SearchByIsEnabledImage, int? SearchByShowNumberOfImages, string? OrderByImages)
        {
            if (datasetId == Guid.Empty)
                return NotFound();

            DatasetDTO datasetDb = await _datasetService.GetDatasetById(datasetId) ?? throw new Exception("Object not found");
            if (datasetDb.IsPublished == true)
            {
                TempData["ErrorDatasetIsPublished"] = DbRes.T("This dataset is published and cannot be edited.", "Resources");
                return RedirectToAction(nameof(Index));
            }

            int pageSize = SearchByShowNumberOfImages ?? 20;
            int pageNumber = page ?? 1;

            EditDatasetDTO dto = await _datasetService.GetObjectForEditDataset(datasetId, SearchByImageName, SearchByIsAnnotatedImage, SearchByIsEnabledImage, OrderByImages);
            IPagedList<DatasetImageDTO> pagedImagesList = dto.ListOfDatasetImages.ToPagedList(pageNumber, pageSize);

            EditDatasetViewModel model = _mapper.Map<EditDatasetViewModel>(dto);
            model.PagedImagesList = pagedImagesList;
            model.SearchByImageName = SearchByImageName;
            model.SearchByIsAnnotatedImage = SearchByIsAnnotatedImage;
            model.SearchByIsEnabledImage = SearchByIsEnabledImage;
            model.SearchByShowNumberOfImages = SearchByShowNumberOfImages;
            model.OrderByImages = OrderByImages;

            return View(model);
        }


        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.PublishDataset))]
        public async Task<IActionResult> PublishDataset(Guid datasetId)
        {
            string? userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { responseError = DbResHtml.T("User id is not valid", "Resources") });

            if (datasetId == Guid.Empty)
                return Json(new { responseError = DbResHtml.T("Incorect dataset id", "Resources") });

            DatasetDTO datasetDb = await _datasetService.GetDatasetById(datasetId) ?? throw new Exception("Dataset not found");
            ResultDTO<int> nubmerOfImagesNeededToPublishDataset = await _appSettingsAccessor.GetApplicationSettingValueByKey<int>("NumberOfImagesNeededToPublishDataset", 100);
            ResultDTO<int> nubmerOfClassesNeededToPublishDataset = await _appSettingsAccessor.GetApplicationSettingValueByKey<int>("NumberOfClassesNeededToPublishDataset", 1);
            if (datasetDb.IsPublished == true)
                return Json(new { responseErrorAlreadyPublished = DbResHtml.T("Dataset is already published. No changes allowed", "Resources") });

            ResultDTO<int> isPublished = await _datasetService.PublishDataset(datasetId, userId);
            if (isPublished.IsSuccess == true && isPublished.Data == 1 && string.IsNullOrEmpty(isPublished.ErrMsg))
                return Json(new { responseSuccess = DbResHtml.T("Successfully published dataset", "Resources") });

            if (isPublished.IsSuccess == false && isPublished.Data == 2)
                return Json(new { responseError = DbResHtml.T($"Insert at least {nubmerOfImagesNeededToPublishDataset.Data} images, {nubmerOfClassesNeededToPublishDataset.Data} class/es and annotate all enabled images to publish the dataset", "Resources") });

            return Json(new { responseError = DbResHtml.T("Dataset was not published", "Resources") });
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.DeleteDatasetClass))]
        public async Task<IActionResult> DeleteDatasetConfirmed(Guid datasetId)
        {
            if (datasetId == Guid.Empty)
                return Json(new { responseError = DbResHtml.T("Invalid dataset id", "Resources") });

            var isDatasetDeleted = await _datasetService.DeleteDataset(datasetId);
            var listOfAllDatasets = await _datasetService.GetAllDatasets() ?? throw new Exception("Datasets not found");
            var childrenDatasetsList = listOfAllDatasets.Where(x => x.ParentDatasetId == datasetId).ToList() ?? throw new Exception("Object not found");
            if (isDatasetDeleted.IsSuccess == true && isDatasetDeleted.Data == 1 && string.IsNullOrEmpty(isDatasetDeleted.ErrMsg))
            {
                var datasetImagesFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DatasetImagesFolder", "DatasetImages");
                string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, datasetImagesFolder.Data, datasetId.ToString());
                if (Directory.Exists(folderPath))
                    Directory.Delete(folderPath, true);

                return Json(new { responseSuccess = DbResHtml.T("Successfully deleted dataset", "Resources") });
            }

            if (!string.IsNullOrEmpty(isDatasetDeleted.ErrMsg))
                return Json(new { responseError = DbResHtml.T(isDatasetDeleted.ErrMsg, "Resources") });

            return Json(new { responseError = DbResHtml.T("Some error occured", "Resources") });
        }

        // TODO: Change AuthClaim

        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.PublishDataset))]
        public async Task<IActionResult> ExportDataset(Guid datasetId)
        {
            if (datasetId == Guid.Empty)
                return Json(new { responseError = DbResHtml.T("Invalid dataset id", "Resources") });

            ResultDTO<CocoDatasetDTO> resultExportDatasetAsCoco =
                await _datasetService.ExportDatasetAsCOCOFormat(datasetId);
            if (resultExportDatasetAsCoco.IsSuccess == false)
                return Json(new { responseError = DbResHtml.T(resultExportDatasetAsCoco.ErrMsg, "Resources") });

            return Json(new
            {
                responseSuccess = DbResHtml.T("Successfully Exported Dataset in COCO Format", "Resources"),
                COCODataset = resultExportDatasetAsCoco.Data
            });
        }

        #region Dataset Classes
        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.AddDatasetClass))]
        public async Task<IActionResult> AddDatasetClass(Guid selectedClassId, Guid datasetId)
        {
            string? userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { responseError = DbResHtml.T("User id is not valid", "Resources") });

            if (datasetId == Guid.Empty)
                return Json(new { responseError = DbResHtml.T("Invalid dataset id", "Resources") });

            if (selectedClassId == Guid.Empty)
                return Json(new { responseError = DbResHtml.T("Invalid dataset class id", "Resources") });

            DatasetDTO datasetDb = await _datasetService.GetDatasetById(datasetId) ?? throw new Exception("Object not found");
            if (datasetDb.IsPublished == true)
                return Json(new { responseErrorAlreadyPublished = DbResHtml.T("Dataset is already published. No changes allowed", "Resources") });

            ResultDTO<int> isClassAdded = await _datasetService.AddDatasetClassForDataset(selectedClassId, datasetId, userId);
            if (isClassAdded.IsSuccess == true && isClassAdded.Data == 1 && string.IsNullOrEmpty(isClassAdded.ErrMsg))
                return Json(new { responseSuccess = DbResHtml.T("Successfully added dataset class", "Resources") });

            if (!string.IsNullOrEmpty(isClassAdded.ErrMsg))
                return Json(new { responseError = DbResHtml.T(isClassAdded.ErrMsg, "Resources") });

            return Json(new { responseError = DbResHtml.T("Dataset class was not added", "Resources") });
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.DeleteDatasetClass))]
        public async Task<IActionResult> DeleteDatasetClass(Guid datasetClassId, Guid datasetId)
        {
            string? userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { responseError = DbResHtml.T("User id is not valid", "Resources") });

            if (datasetId == Guid.Empty)
                return Json(new { responseError = DbResHtml.T("Invalid dataset id", "Resources") });

            if (datasetClassId == Guid.Empty)
                return Json(new { responseError = DbResHtml.T("Invalid dataset class id", "Resources") });

            var datasetDb = await _datasetService.GetDatasetById(datasetId) ?? throw new Exception("Object not found");
            if (datasetDb.IsPublished == true)
                return Json(new { responseErrorAlreadyPublished = DbResHtml.T("Dataset is already published. No changes allowed", "Resources") });

            var isClassDeleted = await _datasetService.DeleteDatasetClassForDataset(datasetClassId, datasetId, userId);
            if (isClassDeleted.IsSuccess == true && isClassDeleted.Data == 1 && string.IsNullOrEmpty(isClassDeleted.ErrMsg))
                return Json(new { responseSuccess = DbResHtml.T("Successfully deleted dataset class", "Resources") });

            if (!string.IsNullOrEmpty(isClassDeleted.ErrMsg))
                return Json(new { responseError = DbResHtml.T(isClassDeleted.ErrMsg, "Resources") });

            return Json(new { responseError = DbResHtml.T("Some error occured", "Resources") });
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.ChooseDatasetClassType))]
        public async Task<IActionResult> ChooseDatasetClassType(Guid datasetId, bool annotationsPerSubclass)
        {
            string? userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { responseError = DbResHtml.T("User id is not valid", "Resources") });

            if (datasetId == Guid.Empty)
                return Json(new { responseError = DbResHtml.T("Invalid dataset id", "Resources") });

            ResultDTO<int> isSetAnnontationsPerSubclass =
                await _datasetService.SetAnnotationsPerSubclass(datasetId, annotationsPerSubclass, userId);

            if (isSetAnnontationsPerSubclass.IsSuccess == true
                && isSetAnnontationsPerSubclass.Data == 1
                && string.IsNullOrEmpty(isSetAnnontationsPerSubclass.ErrMsg))
                return Json(new { responseSuccess = DbResHtml.T("Now you can add classes for this dataset", "Resources") });

            if (!string.IsNullOrEmpty(isSetAnnontationsPerSubclass.ErrMsg))
                return Json(new { responseError = DbResHtml.T(isSetAnnontationsPerSubclass.ErrMsg, "Resources") });

            return Json(new { responseError = DbResHtml.T("Choosed option was not saved", "Resources") });
        }
        #endregion

        //private string GetOptionDisplayName(DatasetViewModel item, string prefix)
        //{
        //    return $"{prefix}{item.Name}<br>";
        //}

        //private string GenerateDatasetOptionsHtml(List<DatasetViewModel> parentDatasetsList, List<DatasetViewModel> allDatasets,string prefix)
        //{
        //    StringBuilder options = new StringBuilder();
        //    int count = 1;

        //    foreach (var dataset in parentDatasetsList)
        //    {
        //        string optionPrefix = $"{prefix}{count}. ";
        //        options.AppendLine($"<option value=\"{dataset.Id}\">{GetOptionDisplayName(dataset, optionPrefix)}</option>");

        //        var children = allDatasets.Where(d => d.ParentDatasetId == dataset.Id).ToList();
        //        if (children.Any())
        //        {
        //            options.AppendLine(GenerateDatasetOptionsHtml(children, allDatasets, $"{optionPrefix}"));
        //        }
        //        count++;
        //    }

        //    return options.ToString();
        //}
    }
}
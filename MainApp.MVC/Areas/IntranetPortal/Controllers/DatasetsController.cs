using AutoMapper;
using DAL.Interfaces.Helpers;
using DTOs.MainApp.BL;
using DTOs.MainApp.BL.DatasetDTOs;
using DTOs.ObjectDetection.API;
using ImageMagick;
using MainApp.BL.Interfaces.Services.DatasetServices;
using MainApp.MVC.Filters;
using MainApp.MVC.Helpers;
using MainApp.MVC.ViewModels.IntranetPortal.Dataset;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SD;
using System.Data;
using System.IO.Compression;
using System.Security.Claims;
using System.Text;
using X.PagedList;

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
            try
            {
                var datasetListResult = await _datasetService.GetAllDatasets();

                if (datasetListResult.IsSuccess == false && datasetListResult.HandleError())
                {
                    var errorPath = _configuration["ErrorViewsPath:Error"];
                    if (errorPath != null)
                    {
                        return BadRequest();
                    }
                    return Redirect(errorPath);
                }

                List<DatasetViewModel> model = _mapper.Map<List<DatasetViewModel>>(datasetListResult.Data);

                if (model == null)
                {
                    var errorPath = _configuration["ErrorViewsPath:Error404"];
                    if (errorPath != null)
                    {
                        return BadRequest();
                    }
                    return Redirect(errorPath);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                var errorPath = _configuration["ErrorViewsPath:Error"];
                if (errorPath != null)
                {
                    return BadRequest();
                }
                return Redirect(errorPath);
            }
        }
        // Remove maybe ? 
        public async Task<List<DatasetDTO>> GetAllDatasets()
        {
            var datasets = await _datasetService.GetAllDatasets();
            return datasets.Data;
        }

        public async Task<ResultDTO<List<DatasetDTO>>> GetAllPublishedDatasets()
        {
            try
            {
                ResultDTO<List<DatasetDTO>>? resultGetEntities = await _datasetService.GetAllPublishedDatasets();

                if (resultGetEntities.IsSuccess == false && resultGetEntities.HandleError())
                    return ResultDTO<List<DatasetDTO>>.Fail(resultGetEntities.ErrMsg!);

                if (resultGetEntities.Data == null)
                    return ResultDTO<List<DatasetDTO>>.Fail("Published datasets not found");

                return ResultDTO<List<DatasetDTO>>.Ok(resultGetEntities.Data);
            }
            catch (Exception ex)
            {
                return ResultDTO<List<DatasetDTO>>.Fail("An unexpected error occurred while retrieving published datasets.");
            }
        }



        [HttpPost]
        public async Task<IActionResult> GetParentAndChildrenDatasets(Guid currentDatasetId)
        {
            try
            {
                if (currentDatasetId == Guid.Empty)
                    return Json(new { responseError = DbResHtml.T("Invalid dataset id", "Resources") });

                ResultDTO<List<DatasetDTO>> allDatasetsDb = await _datasetService.GetAllDatasets();

                if (allDatasetsDb.IsSuccess == false && allDatasetsDb.HandleError() || allDatasetsDb.Data is null)
                    return Json(new { responseError = DbResHtml.T("No datasets found.", "Resources") });

                ResultDTO<DatasetDTO> currentDatasetDb = await _datasetService.GetDatasetById(currentDatasetId);
                if (currentDatasetDb.IsSuccess == false && currentDatasetDb.HandleError() || currentDatasetDb == null)
                    return Json(new { responseError = DbResHtml.T("Invalid current dataset", "Resources") });

                List<DatasetDTO> childrenDatasets = allDatasetsDb.Data.Where(x => x.ParentDatasetId == currentDatasetId).ToList();
                if (!childrenDatasets.Any())
                    return Json(new { responseError = DbResHtml.T("No child datasets found.", "Resources") });

                DatasetDTO? parentDataset = currentDatasetDb.Data.ParentDataset;
                if (parentDataset == null)
                {
                    DatasetDTO datasetDTO = new();
                    return Json(new { parent = datasetDTO, childrenList = childrenDatasets, currentDataset = currentDatasetDb });
                }

                return Json(new { parent = parentDataset, childrenList = childrenDatasets, currentDataset = currentDatasetDb });
            }
            catch (Exception ex)
            {
                return Json(new { responseError = DbResHtml.T("An unexpected error occurred while retrieving datasets.", "Resources") });
            }
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.AddDataset))]
        public async Task<IActionResult> CreateConfirmed(CreateDatasetViewModel datasetViewModel)
        {
            try
            {
                string? userId = User.FindFirstValue("UserId");
                if (string.IsNullOrEmpty(userId))
                    return Json(new { responseError = DbResHtml.T("User id is not valid", "Resources") });

                if (!ModelState.IsValid)
                    return Json(new { responseError = DbResHtml.T("Dataset model is not valid", "Resources") });

                ResultDTO<DatasetDTO> selectedParentDataset;
                if (datasetViewModel.ParentDatasetId != null)
                {
                    selectedParentDataset = await _datasetService.GetDatasetById(datasetViewModel.ParentDatasetId.Value);
                    if (selectedParentDataset.IsSuccess == false && selectedParentDataset.HandleError())
                        return Json(new { responseError = DbResHtml.T(selectedParentDataset.ErrMsg!, "Resources") });

                    if (selectedParentDataset.Data.IsPublished == false)
                        return Json(new { responseError = DbResHtml.T("Selected parent dataset is not published. You can not add subdataset for unpublished datasets!", "Resources") });
                }

                datasetViewModel.CreatedById = userId;
                DatasetDTO dataSetDto = _mapper.Map<DatasetDTO>(datasetViewModel);
                if (dataSetDto is null)
                    return Json(new { responseError = DbResHtml.T("Mapping failed!", "Resources") });

                var insertedDatasetDTO = await _datasetService.CreateDataset(dataSetDto);

                if (insertedDatasetDTO.IsSuccess == false && insertedDatasetDTO.HandleError())
                    return Json(new { responseError = DbResHtml.T(insertedDatasetDTO.ErrMsg!, "Resources") });

                if (datasetViewModel.ParentDatasetId != null)
                {
                    var inheritedParrentClass = await _datasetService.AddInheritedParentClasses(insertedDatasetDTO.Data.Id, datasetViewModel.ParentDatasetId.Value);

                    if (inheritedParrentClass.IsSuccess == false && inheritedParrentClass.HandleError())
                        return Json(new { responseError = DbResHtml.T($"{inheritedParrentClass.ErrMsg}", "Resources") });
                }

                return Json(new { id = insertedDatasetDTO.Data.Id });
            }
            catch (Exception ex)
            {
                return Json(new { responseError = DbResHtml.T("An unexpected error occurred while creating the dataset. Please try again later.", "Resources") });
            }
        }

        // TODO: Create ViewModel
        //[HasAuthClaim(nameof(SD.AuthClaims.ManageDataset))]
        //public async Task<IActionResult> Edit(Guid datasetId, int? page, string? SearchByImageName, bool? SearchByIsAnnotatedImage, bool? SearchByIsEnabledImage, int? SearchByShowNumberOfImages, string? OrderByImages)
        //{
        //    if (datasetId == Guid.Empty)
        //        return NotFound();

        //    DatasetDTO datasetDb = await _datasetService.GetDatasetById(datasetId) ?? throw new Exception("Object not found");
        //    if (datasetDb.IsPublished == true)
        //    {
        //        TempData["ErrorDatasetIsPublished"] = DbRes.T("This dataset is published and cannot be edited.", "Resources");
        //        return RedirectToAction(nameof(Index));
        //    }

        //    int pageSize = SearchByShowNumberOfImages ?? 20;
        //    int pageNumber = page ?? 1;

        //    EditDatasetDTO dto = await _datasetService.GetObjectForEditDataset(datasetId, SearchByImageName, SearchByIsAnnotatedImage, SearchByIsEnabledImage, OrderByImages);
        //    IPagedList<DatasetImageDTO> pagedImagesList = dto.ListOfDatasetImages.ToPagedList(pageNumber, pageSize);

        //    EditDatasetViewModel model = _mapper.Map<EditDatasetViewModel>(dto);
        //    model.PagedImagesList = pagedImagesList;
        //    model.SearchByImageName = SearchByImageName;
        //    model.SearchByIsAnnotatedImage = SearchByIsAnnotatedImage;
        //    model.SearchByIsEnabledImage = SearchByIsEnabledImage;
        //    model.SearchByShowNumberOfImages = SearchByShowNumberOfImages;
        //    model.OrderByImages = OrderByImages;

        //    return View(model);
        //}
        public async Task<IActionResult> Edit(Guid datasetId, int? page, string? SearchByImageName, bool? SearchByIsAnnotatedImage,
                                              bool? SearchByIsEnabledImage, int? SearchByShowNumberOfImages, string? OrderByImages)
        {
            try
            {
                if (datasetId == Guid.Empty)
                {
                    TempData["EditDatasetErrorMessage"] = "Such dataset does not exist";
                    return RedirectToAction("Index");
                }

                var datasetDb = await _datasetService.GetDatasetById(datasetId);

                if (datasetDb.IsSuccess == false && datasetDb.HandleError())
                {
                    TempData["EditDatasetErrorMessage"] = datasetDb.ErrMsg;
                    return RedirectToAction("Index");
                }

                if (datasetDb.Data is null)
                {
                    TempData["EditDatasetErrorMessage"] = "Dataset is not found";
                    return RedirectToAction("Index");
                }

                int pageSize = SearchByShowNumberOfImages ?? 20;
                int pageNumber = page ?? 1;

                ResultDTO<EditDatasetDTO> resultGetObjectForEditDataset = await _datasetService.GetObjectForEditDataset(
                    datasetId, SearchByImageName, SearchByIsAnnotatedImage, SearchByIsEnabledImage, OrderByImages, pageNumber, pageSize);

                if (resultGetObjectForEditDataset.IsSuccess == false && resultGetObjectForEditDataset.HandleError())
                {
                    TempData["EditDatasetErrorMessage"] = resultGetObjectForEditDataset.ErrMsg;
                    return RedirectToAction("Index");
                }

                var dto = resultGetObjectForEditDataset.Data;

                IPagedList<DatasetImageDTO> pagedImagesList = new StaticPagedList<DatasetImageDTO>(
                    dto.ListOfDatasetImages, pageNumber, pageSize, dto.NumberOfDatasetImages);

                EditDatasetViewModel model = _mapper.Map<EditDatasetViewModel>(dto);
                model.PagedImagesList = pagedImagesList;
                model.SearchByImageName = SearchByImageName;
                model.SearchByIsAnnotatedImage = SearchByIsAnnotatedImage;
                model.SearchByIsEnabledImage = SearchByIsEnabledImage;
                model.SearchByShowNumberOfImages = SearchByShowNumberOfImages;
                model.OrderByImages = OrderByImages;

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["EditDatasetErrorMessage"] = "An unexpected error occurred while editing the dataset. Please try again later.";
                return RedirectToAction("Index");
            }
        }



        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.EditDatasetImage))]
        public async Task<IActionResult> EnableAllImages(Guid datasetId)
        {
            try
            {
                if (datasetId == Guid.Empty)
                {
                    return Json(new { responseError = DbResHtml.T($"Invalid dataset id.", "Resources") });
                }

                ResultDTO updateDataset = await _datasetService.EnableAllImagesInDataset(datasetId);

                if (updateDataset.IsSuccess == false && updateDataset.HandleError())
                {
                    return Json(new { responseError = DbResHtml.T("An error occurred while enabling images.", "Resources") });
                }

                return Json(new { responseSuccess = DbResHtml.T("All dataset images have been enabled", "Resources") });
            }
            catch (Exception ex)
            {
                return Json(new { responseError = DbResHtml.T("An unexpected error occurred while enabling the images. Please try again later.", "Resources") });
            }
        }


        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.PublishDataset))]
        public async Task<IActionResult> PublishDataset(Guid datasetId)
        {
            try
            {
                string? userId = User.FindFirstValue("UserId");
                if (string.IsNullOrEmpty(userId))
                    return Json(new { responseError = DbResHtml.T("User id is not valid", "Resources") });

                if (datasetId == Guid.Empty)
                    return Json(new { responseError = DbResHtml.T("Incorrect dataset id", "Resources") });

                ResultDTO<DatasetDTO> datasetDb = await _datasetService.GetDatasetById(datasetId);

                if (datasetDb.IsSuccess == false && datasetDb.HandleError())
                    return Json(new { responseError = DbResHtml.T(datasetDb.ErrMsg!, "Resources") });

                if (datasetDb.Data is null)
                    return Json(new { responseError = DbResHtml.T("Dataset is not found", "Resources") });

                ResultDTO<int> numberOfImagesNeededToPublishDataset = await _appSettingsAccessor.GetApplicationSettingValueByKey<int>("NumberOfImagesNeededToPublishDataset", 100);
                ResultDTO<int> numberOfClassesNeededToPublishDataset = await _appSettingsAccessor.GetApplicationSettingValueByKey<int>("NumberOfClassesNeededToPublishDataset", 1);

                if (datasetDb.Data.IsPublished == true)
                    return Json(new { responseErrorAlreadyPublished = DbResHtml.T("Dataset is already published. No changes allowed", "Resources") });

                ResultDTO<int> isPublished = await _datasetService.PublishDataset(datasetId, userId);
                if (isPublished.IsSuccess == true && isPublished.Data == 1 && string.IsNullOrEmpty(isPublished.ErrMsg))
                    return Json(new { responseSuccess = DbResHtml.T("Successfully published dataset", "Resources") });

                if (isPublished.IsSuccess == false && isPublished.Data == 2)
                    return Json(new { responseError = DbResHtml.T($"Insert at least {numberOfImagesNeededToPublishDataset.Data} images, {numberOfClassesNeededToPublishDataset.Data} class/es and annotate all enabled images to publish the dataset", "Resources") });

                return Json(new { responseError = DbResHtml.T("Dataset was not published", "Resources") });
            }
            catch (Exception ex)
            {
                return Json(new { responseError = DbResHtml.T("An unexpected error occurred while publishing the dataset. Please try again later.", "Resources") });
            }
        }


        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.DeleteDatasetClass))]
        public async Task<IActionResult> DeleteDatasetConfirmed(Guid datasetId)
        {
            if (datasetId == Guid.Empty)
                return Json(new { responseError = DbResHtml.T("Invalid dataset id", "Resources") });

            try
            {
                ResultDTO deleteDatasetCompletelyIncludedResult = await _datasetService.DeleteDatasetCompletelyIncluded(datasetId);
                if (deleteDatasetCompletelyIncludedResult.IsSuccess == false && ResultDTO.HandleError(deleteDatasetCompletelyIncludedResult))
                    return Json(new { responseError = DbResHtml.T(deleteDatasetCompletelyIncludedResult.ErrMsg!, "Resources") });

                ResultDTO<string?> datasetImagesFolder =
                    await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DatasetImagesFolder", "DatasetImages");
                ResultDTO<string?> datasetThumbnailFolder =
                    await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DatasetThumbnailsFolder", "DatasetThumbnails");


                string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, datasetImagesFolder.Data, datasetId.ToString());
                string thumbnailPath = Path.Combine(_webHostEnvironment.WebRootPath, datasetThumbnailFolder.Data, datasetId.ToString());
                if (Directory.Exists(folderPath))
                    Directory.Delete(folderPath, true);
                if (Directory.Exists(thumbnailPath))
                    Directory.Delete(thumbnailPath, true);

                return Json(new { responseSuccess = DbResHtml.T("Successfully deleted dataset", "Resources") });
            }
            catch (Exception ex)
            {
                // TODO: ADD Logger
                return Json(new { responseError = DbResHtml.T(ex.InnerException is not null ? ex.InnerException.Message : ex.Message, "Resources") });
            }
        }

        // TODO: Change AuthClaim

        #region Import / Export COCO Dataset
        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.PublishDataset))]
        public async Task<IActionResult> ExportDataset(Guid datasetId, string exportOption, string? downloadLocation, bool asSplit)
        {
            if (datasetId == Guid.Empty)
                return Json(new { responseError = DbResHtml.T("Invalid dataset id", "Resources") });

            string zipFilePath = string.Empty;
            FileStream fileStream = null;

            try
            {
                ResultDTO<string> resultExportDatasetAsCoco =
                    await _datasetService.ExportDatasetAsCOCOFormat(datasetId, exportOption, downloadLocation, asSplit);

                if (resultExportDatasetAsCoco.IsSuccess == false && resultExportDatasetAsCoco.HandleError())
                    return Json(new { responseError = DbResHtml.T(resultExportDatasetAsCoco.ErrMsg, "Resources") });

                zipFilePath = resultExportDatasetAsCoco.Data;
                fileStream = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read);
                var contentType = "application/zip";
                var fileName = $"{datasetId}.zip";

                Response.Headers.Add("Content-Disposition", zipFilePath);
                Response.Headers.Add("Download-Disposition", fileName);

                return File(fileStream, contentType, fileName);
            }
            catch (Exception ex)
            {
                return Json(new { responseError = DbResHtml.T(ex.Message, "Resources") });
            }
        }


        [HttpPost]
        public async Task<IActionResult> CleanupTempFilesFromExportDataset(string fileGuid)
        {
            try
            {
                var filePath = Path.Combine(Path.GetTempPath(), fileGuid);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return Json(new { responseError = DbResHtml.T(ex.Message, "Resources") });
            }
        }

        #endregion

        [HttpPost]
        [RequestSizeLimit(int.MaxValue)]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue)]
        [HasAuthClaim(nameof(SD.AuthClaims.PublishDataset))]
        public async Task<IActionResult> ImportDataset(IFormFile datasetFile, string datasetName, bool allowUnannotatedImages)
        {
            if (datasetFile == null || datasetFile.Length == 0)
            {
                return Json(new { responseError = DbResHtml.T("Invalid dataset file", "Resources") });
            }

            string? userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { responseError = DbResHtml.T("User id is not valid", "Resources") });
            }

            try
            {
                string tempFilePath = Path.Combine(Path.GetTempPath(), datasetFile.FileName);
                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await datasetFile.CopyToAsync(stream);
                }

                string absoluteOutputDir = ProcessZipFile(tempFilePath, _webHostEnvironment.WebRootPath);
                string outputDir = Path.GetRelativePath(_webHostEnvironment.WebRootPath, absoluteOutputDir);
                string outputJsonPath = Path.Combine(_webHostEnvironment.WebRootPath, outputDir);

                ResultDTO<DatasetDTO> importDatasetResult =
                    await _datasetService.ImportDatasetCocoFormatedAtDirectoryPath(datasetName, outputJsonPath, userId, _webHostEnvironment.WebRootPath, allowUnannotatedImages);

                if (importDatasetResult.IsSuccess == false && importDatasetResult.HandleError())
                {
                    CleanUpFoldersFromImportingDataset(tempFilePath, outputJsonPath);
                    return Json(new { responseError = DbResHtml.T("Error importing dataset file", "Resources") });
                }

                ResultDTO<string> generateThumbsResult = await GenerateThumbnailsForDatasetWithErrors(importDatasetResult.Data!.Id);
                if (generateThumbsResult.IsSuccess == false && generateThumbsResult.HandleError())
                {
                    CleanUpFoldersFromImportingDataset(tempFilePath, outputJsonPath);
                    return Json(new { responseError = DbResHtml.T(generateThumbsResult.ErrMsg!, "Resources") });
                }

                CleanUpFoldersFromImportingDataset(tempFilePath, outputJsonPath);

                return Json(new { responseSuccess = DbResHtml.T($"Dataset imported and {generateThumbsResult.Data}", "Resources"), dataset = importDatasetResult.Data });
            }
            catch (Exception ex)
            {
                return Json(new { responseError = DbResHtml.T(ex.Message, "Resources") });
            }
        }

        private void CleanUpFoldersFromImportingDataset(string tempFilePath, string outputJsonPath)
        {
            if (System.IO.File.Exists(tempFilePath))
            {
                System.IO.File.Delete(tempFilePath);
            }
            if (System.IO.Directory.Exists(outputJsonPath))
            {
                System.IO.Directory.Delete(outputJsonPath, true);
            }
        }

        #region Dataset Classes
        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.AddDatasetClass))]
        public async Task<IActionResult> AddDatasetClass(Guid selectedClassId, Guid datasetId)
        {
            try
            {
                string? userId = User.FindFirstValue("UserId");
                if (string.IsNullOrEmpty(userId))
                    return Json(new { responseError = DbResHtml.T("User id is not valid", "Resources") });

                if (datasetId == Guid.Empty)
                    return Json(new { responseError = DbResHtml.T("Invalid dataset id", "Resources") });

                if (selectedClassId == Guid.Empty)
                    return Json(new { responseError = DbResHtml.T("Invalid dataset class id", "Resources") });

                ResultDTO<DatasetDTO?> datasetDb = await _datasetService.GetDatasetById(datasetId);

                if (datasetDb.IsSuccess == false && datasetDb.HandleError())
                    return Json(new { responseError = DbResHtml.T(datasetDb.ErrMsg!, "Resources") });

                if (datasetDb.Data is null)
                    return Json(new { responseError = DbResHtml.T("Dataset is not found", "Resources") });

                if (datasetDb.Data.IsPublished == true)
                    return Json(new { responseErrorAlreadyPublished = DbResHtml.T("Dataset is already published. No changes allowed", "Resources") });

                ResultDTO<int> isClassAdded = await _datasetService.AddDatasetClassForDataset(selectedClassId, datasetId, userId);
                if (isClassAdded.IsSuccess == true && isClassAdded.Data == 1 && string.IsNullOrEmpty(isClassAdded.ErrMsg))
                    return Json(new { responseSuccess = DbResHtml.T("Successfully added dataset class", "Resources") });

                if (isClassAdded.IsSuccess == false && isClassAdded.HandleError())
                    return Json(new { responseError = DbResHtml.T("Dataset class was not added.", "Resources") });

                return Json(new { responseError = DbResHtml.T("Dataset class was not added", "Resources") });
            }
            catch (Exception ex)
            {
                return Json(new { responseError = DbResHtml.T("An unexpected error occurred while adding the dataset class. Please try again later.", "Resources") });
            }
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

            try
            {
                ResultDTO<DatasetDTO?> datasetDb = await _datasetService.GetDatasetById(datasetId);

                if (datasetDb.IsSuccess == false)
                    return Json(new { responseError = DbResHtml.T(datasetDb.ErrMsg!, "Resources") });

                if (datasetDb.Data is null)
                    return Json(new { responseError = DbResHtml.T("Dataset is not found", "Resources") });

                if (datasetDb.Data.IsPublished == true)
                    return Json(new { responseErrorAlreadyPublished = DbResHtml.T("Dataset is already published. No changes allowed", "Resources") });

                var isClassDeleted = await _datasetService.DeleteDatasetClassForDataset(datasetClassId, datasetId, userId);
                if (isClassDeleted.IsSuccess == false && isClassDeleted.HandleError() || isClassDeleted.Data != 1 || !string.IsNullOrEmpty(isClassDeleted.ErrMsg))
                {
                    return Json(new { responseError = DbResHtml.T(isClassDeleted.ErrMsg ?? "Error deleting dataset class", "Resources") });
                }

                return Json(new { responseSuccess = DbResHtml.T("Successfully deleted dataset class", "Resources") });

            }
            catch (Exception ex)
            {
                return Json(new { responseError = DbResHtml.T($"{ex.Message}", "Resources") });
            }
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

            try
            {
                ResultDTO<int> isSetAnnontationsPerSubclass =
                    await _datasetService.SetAnnotationsPerSubclass(datasetId, annotationsPerSubclass, userId);

                if (isSetAnnontationsPerSubclass.IsSuccess == true
                    && isSetAnnontationsPerSubclass.Data == 1
                    && string.IsNullOrEmpty(isSetAnnontationsPerSubclass.ErrMsg))
                {
                    return Json(new { responseSuccess = DbResHtml.T("Now you can add classes for this dataset", "Resources") });
                }

                if (!string.IsNullOrEmpty(isSetAnnontationsPerSubclass.ErrMsg) && isSetAnnontationsPerSubclass.IsSuccess == false && isSetAnnontationsPerSubclass.HandleError())
                {
                    return Json(new { responseError = DbResHtml.T(isSetAnnontationsPerSubclass.ErrMsg, "Resources") });
                }

                return Json(new { responseError = DbResHtml.T("Choosed option was not saved", "Resources") });
            }
            catch (Exception ex)
            {
                return Json(new { responseError = DbResHtml.T($"{ex.Message}", "Resources") });
            }
        }

        #endregion

        private string ProcessZipFile(string zipFilePath, string webRootPath)
        {
            //coco dataset
            string tempDir = Path.Combine(webRootPath, "temp" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            string extractDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            try
            {
                ZipFile.ExtractToDirectory(zipFilePath, extractDir);
            }
            catch (Exception ex)
            {
                throw;
            }

            List<JObject> images = new List<JObject>();
            List<JObject> annotations = new List<JObject>();
            Dictionary<int, int> imageIdMapping = new Dictionary<int, int>();

            string[] folders = { "train", "valid", "test" };
            int currentImageId = -1;
            int currentAnnotationId = -1;

            JObject? info = null;
            JArray? licenses = null;
            JArray? categories = null;

            foreach (var folder in folders)
            {
                string folderPath = Path.Combine(extractDir, folder);
                if (Directory.Exists(folderPath))
                {
                    string? jsonFileName = 
                        CocoTerminology.allowedAnnotationsFileNamesList.FirstOrDefault(fileName => System.IO.File.Exists(Path.Combine(folderPath, fileName)));
                    if (string.IsNullOrEmpty(jsonFileName) == false)
                    {
                        string jsonFilePath = Path.Combine(folderPath, jsonFileName);
                        JObject jsonData = JObject.Parse(System.IO.File.ReadAllText(jsonFilePath));

                        if (info == null && licenses == null && categories == null)
                        {
                            info = (JObject)jsonData["info"];
                            licenses = (JArray)jsonData["licenses"];
                            categories = (JArray)jsonData["categories"];
                        }

                        // Process images
                        foreach (var image in jsonData["images"])
                        {
                            int oldImageId = (int)image["id"];
                            currentImageId++;
                            image["id"] = currentImageId;

                            imageIdMapping[oldImageId] = currentImageId;

                            images.Add((JObject)image);
                        }

                        // Process annotations
                        foreach (var annotation in jsonData["annotations"])
                        {
                            currentAnnotationId++;
                            annotation["id"] = currentAnnotationId;

                            int oldImageId = (int)annotation["image_id"];
                            if (imageIdMapping.ContainsKey(oldImageId))
                            {
                                annotation["image_id"] = imageIdMapping[oldImageId];
                            }

                            annotations.Add((JObject)annotation);
                        }

                        foreach (var imageFile in Directory.GetFiles(folderPath, "*.jpg"))
                        {
                            string fileName = Path.GetFileName(imageFile);
                            string destFilePath = Path.Combine(tempDir, fileName);
                            System.IO.File.Copy(imageFile, destFilePath, true);
                        }
                    }
                }
            }

            string outputJsonPath = Path.Combine(tempDir, "annotations.json");
            JObject mergedJson = new JObject
            {
                ["info"] = info,
                ["licenses"] = licenses,
                ["categories"] = categories,
                ["images"] = JArray.FromObject(images),
                ["annotations"] = JArray.FromObject(annotations)
            };
            System.IO.File.WriteAllText(outputJsonPath, mergedJson.ToString());

            Directory.Delete(extractDir, true);

            return tempDir;
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.PublishDataset))]
        public async Task<IActionResult> GenerateThumbnailsForDataset(Guid datasetId)
        {
            string? userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { responseError = DbResHtml.T("User id is not valid", "Resources") });

            if (datasetId == Guid.Empty)
                return Json(new { responseError = DbResHtml.T("Invalid dataset id", "Resources") });

            try
            {
                ResultDTO<DatasetDTO> datasetDb = await _datasetService.GetDatasetById(datasetId);

                if (datasetDb.IsSuccess == false && datasetDb.HandleError())
                    return Json(new { responseError = DbResHtml.T(datasetDb.ErrMsg!, "Resources") });

                if (datasetDb.Data is null)
                    return Json(new { responseError = DbResHtml.T("Dataset not found", "Resources") });

                if (datasetDb.Data.IsPublished == true)
                    return Json(new { responseErrorAlreadyPublished = DbResHtml.T("Dataset is already published. No changes allowed", "Resources") });

                ResultDTO<string?> datasetThumbnailsFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DatasetThumbnailsFolder", "DatasetThumbnails");
                if (datasetThumbnailsFolder.IsSuccess == false || datasetThumbnailsFolder.Data == null)
                    return Json(new { responseError = DbResHtml.T("Could not retrieve thumbnail folder", "Resources") });

                string thumbnailsFolder = Path.Combine(_webHostEnvironment.WebRootPath, datasetThumbnailsFolder.Data, datasetDb.Data.Id.ToString());
                if (!Directory.Exists(thumbnailsFolder))
                    Directory.CreateDirectory(thumbnailsFolder);

                ResultDTO<List<DatasetImageDTO>> datasetImages = await _datasetImagesService.GetImagesForDataset(datasetId);
                if (datasetImages.IsSuccess == false && datasetImages.HandleError())
                    return Json(new { responseError = DbResHtml.T($"{datasetImages.ErrMsg}", "Resources") });

                foreach (var datasetImage in datasetImages.Data)
                {
                    string imageFilePath =
                        Path.Combine(_webHostEnvironment.WebRootPath, datasetImage.ImagePath.TrimStart(Path.DirectorySeparatorChar), datasetImage.FileName);

                    string thumbnailFileName = string.Format("{0}.jpg", datasetImage.Id);
                    string thumbnailFilePath = Path.Combine(thumbnailsFolder, thumbnailFileName);

                    if (!System.IO.File.Exists(imageFilePath))
                        continue;

                    using (var image = new MagickImage(imageFilePath))
                    {
                        image.Resize(192, 192);
                        image.Quality = 95;
                        image.Write(thumbnailFilePath);
                    }
                }

                return Json(new { responseSuccess = DbResHtml.T("Thumbnails generated successfully", "Resources") });
            }
            catch (Exception ex)
            {
                // TODO: Add logging
                return Json(new { responseError = DbResHtml.T(ex.Message, "Resources") });
            }
        }

        public async Task<ResultDTO<string>> GenerateThumbnailsForDatasetWithErrors(Guid datasetId)
        {
            string? userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
                return ResultDTO<string>.Fail(DbResHtml.T("User id is not valid", "Resources").Value!);

            if (datasetId == Guid.Empty)
                return ResultDTO<string>.Fail(DbResHtml.T("Invalid dataset id", "Resources").Value!);

            StringBuilder stringBuilder = new StringBuilder();

            try
            {
                ResultDTO<DatasetDTO> getDatasetResult = await _datasetService.GetDatasetDTOFullyIncluded(datasetId);
                if (getDatasetResult.IsSuccess == false && getDatasetResult.HandleError())
                    return ResultDTO<string>.Fail(getDatasetResult.ErrMsg!);

                DatasetDTO datasetDTO = getDatasetResult.Data!;

                if (datasetDTO.IsPublished == true)
                    return ResultDTO<string>.Fail(DbResHtml.T("Dataset is already published. No changes allowed", "Resources").Value!);

                ResultDTO<string?> datasetThumbnailsFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DatasetThumbnailsFolder", "DatasetThumbnails");
                if (datasetThumbnailsFolder.IsSuccess == false || datasetThumbnailsFolder.Data == null)
                    return ResultDTO<string>.Fail(DbResHtml.T("Could not retrieve thumbnail folder", "Resources").Value!);

                string thumbnailsFolder = Path.Combine(_webHostEnvironment.WebRootPath, datasetThumbnailsFolder.Data, datasetDTO.Id.ToString());
                if (Directory.Exists(thumbnailsFolder) == false)
                    Directory.CreateDirectory(thumbnailsFolder);

                ResultDTO<List<DatasetImageDTO>> datasetImages = await _datasetImagesService.GetImagesForDataset(datasetId);
                if (datasetImages.IsSuccess == false && datasetImages.HandleError())
                    return ResultDTO<string>.Fail(DbResHtml.T($"{datasetImages.ErrMsg!}", "Resources").Value!);


                foreach (DatasetImageDTO datasetImage in datasetImages.Data)
                {
                    string imageFilePath =
                        Path.Combine(_webHostEnvironment.WebRootPath, datasetImage.ImagePath.TrimStart(Path.DirectorySeparatorChar), datasetImage.FileName);

                    string thumbnailFileName = string.Format("{0}.jpg", datasetImage.Id);
                    string thumbnailFilePath = Path.Combine(thumbnailsFolder, thumbnailFileName);

                    if (System.IO.File.Exists(imageFilePath) == false)
                    {
                        stringBuilder.AppendLine($"Image does not exist at path: {imageFilePath}");
                        continue;
                        //return ResultDTO<string>.Fail($"Image does not exist at path: {imageFilePath}");
                    }

                    try
                    {
                        using (MagickImage image = new MagickImage(imageFilePath))
                        {
                            image.Resize(192, 192);
                            image.Quality = 95;
                            image.Write(thumbnailFilePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        stringBuilder.AppendLine($"Thumbnails Generation failed for Image at path: {imageFilePath}");
                        stringBuilder.AppendLine(ex.Message);
                        continue;
                        //return ResultDTO<string>.Fail($"Image does not exist at path: {imageFilePath}");
                    }
                }

                return ResultDTO<string>.Ok(
                    DbResHtml.T("Thumbnails generated successfully", "Resources").Value!
                    + (stringBuilder.Length == 0
                        ? ""
                        : " \n with Errors: " + stringBuilder.ToString()));
            }
            catch (Exception ex)
            {
                return ResultDTO<string>.ExceptionFail(ex.Message, ex);
            }
        }
    }
}
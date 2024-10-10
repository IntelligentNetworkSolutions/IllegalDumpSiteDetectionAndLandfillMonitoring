using AutoMapper;
using DAL.Interfaces.Helpers;
using DTOs.MainApp.BL;
using DTOs.MainApp.BL.DatasetDTOs;
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
using Westwind.Globalization;
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

            EditDatasetDTO dto = await _datasetService.GetObjectForEditDataset(datasetId, SearchByImageName, SearchByIsAnnotatedImage, SearchByIsEnabledImage, OrderByImages, pageNumber, pageSize);
            IPagedList<DatasetImageDTO> pagedImagesList = new StaticPagedList<DatasetImageDTO>(dto.ListOfDatasetImages, pageNumber, pageSize, dto.NumberOfDatasetImages);

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
        public async Task<IActionResult> PublishDataset(Guid datasetId, bool continueWithDisabledImages = false)
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

            // Call service and pass the continueWithDisabledImages flag
            ResultDTO<int> isPublished = await _datasetService.PublishDataset(datasetId, userId, continueWithDisabledImages);
            if (isPublished.IsSuccess == true && isPublished.Data == 1 && string.IsNullOrEmpty(isPublished.ErrMsg))
                return Json(new { responseSuccess = DbResHtml.T("Successfully published dataset", "Resources") });
            if (isPublished.IsSuccess == false && isPublished.Data == 2)
                return Json(new { responseError = DbResHtml.T($"Insert at least {nubmerOfImagesNeededToPublishDataset.Data} images, {nubmerOfClassesNeededToPublishDataset.Data} class/es and annotate at least 90% enabled images to publish the dataset", "Resources") });
            if (isPublished.IsSuccess == false && isPublished.Data == 4)
                return Json(new { responseError = DbResHtml.T("One or more images do not exist physically.", "Resources") });
            if (isPublished.IsSuccess == false && isPublished.Data == 5)
                return Json(new { responseWarningDisabledImages = DbResHtml.T("There are disabled images. Do you want to continue?", "Resources") });

            return Json(new { responseError = DbResHtml.T("Dataset was not published", "Resources") });
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
                return Json(new { responseError = DbResHtml.T(ex.Message, "Resources") });
            }
        }

        // TODO: Change AuthClaim

        #region Import / Export COCO Dataset
        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.PublishDataset))]
        public async Task<IActionResult> ExportDataset(Guid datasetId, string exportOption, string? downloadLocation)
        {
            if (datasetId == Guid.Empty)
                return Json(new { responseError = DbResHtml.T("Invalid dataset id", "Resources") });

            try
            {
                // Pass the download location to the service
                ResultDTO<string> resultExportDatasetAsCoco =
                    await _datasetService.ExportDatasetAsCOCOFormat(datasetId, exportOption, downloadLocation);

                if (!resultExportDatasetAsCoco.IsSuccess)
                    return Json(new { responseError = DbResHtml.T(resultExportDatasetAsCoco.ErrMsg, "Resources") });

                string zipFilePath = resultExportDatasetAsCoco.Data;
                var fileStream = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read);
                var contentType = "application/zip";
                var fileName = $"{datasetId}-{exportOption}.zip";

                return File(fileStream, contentType, fileName);
            }
            catch (Exception ex)
            {
                return Json(new { responseError = DbResHtml.T(ex.Message, "Resources") });
            }
        }
        #endregion




        //[HttpGet]
        //[HasAuthClaim(nameof(SD.AuthClaims.PublishDataset))]
        //public async Task<IActionResult> ImportDataset(string cocoDatasetDirectoryPath)
        //{
        //    string? userId = User.FindFirstValue("UserId");
        //    if (string.IsNullOrEmpty(userId))
        //        return Json(new { responseError = DbResHtml.T("User id is not valid", "Resources") });

        //    try
        //    {
        //        ResultDTO<DatasetDTO> importDatasetResult =
        //            await _datasetService.ImportDatasetCocoFormatedAtDirectoryPath("Test Dataset with Save Images v12", cocoDatasetDirectoryPath,
        //                                                                            userId, _webHostEnvironment.WebRootPath);
        //        if (importDatasetResult.IsSuccess == false && ResultDTO<DatasetDTO>.HandleError(importDatasetResult))
        //            return Json(new { responseError = DbResHtml.T(importDatasetResult.ErrMsg!, "Resources") });

        //        return Json(new { importDatasetResult.Data });
        //    }
        //    catch (Exception ex)
        //    {
        //        // TODO: ADD Logger
        //        return Json(new { responseError = DbResHtml.T(ex.Message, "Resources") });
        //    }
        //}

        //[HttpPost]
        //[RequestSizeLimit(int.MaxValue)]
        //[RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue)]
        //[HasAuthClaim(nameof(SD.AuthClaims.PublishDataset))]
        //public async Task<IActionResult> UploadAndProcessDataset(IFormFile datasetFile, string datasetName)
        //{
        //    if (datasetFile == null || datasetFile.Length == 0)
        //    {
        //        return Json(new { responseError = DbResHtml.T("Invalid dataset file", "Resources") });
        //    }

        //    string? userId = User.FindFirstValue("UserId");
        //    if (string.IsNullOrEmpty(userId))
        //        return Json(new { responseError = DbResHtml.T("User id is not valid", "Resources") });

        //    string tempFilePath = null;
        //    string outputDir = null;

        //    try
        //    {
        //        // Step 1: Save the uploaded zip file temporarily
        //        tempFilePath = Path.Combine(Path.GetTempPath(), datasetFile.FileName);
        //        using (var stream = new FileStream(tempFilePath, FileMode.Create))
        //        {
        //            await datasetFile.CopyToAsync(stream);
        //        }

        //        // Step 2: Process the zip file and get the path to the temp folder under datasetImagesFolder
        //        string absoluteOutputDir = ProcessZipFile(tempFilePath, _webHostEnvironment.WebRootPath);

        //        // Get the relative path from the web root to the output directory
        //        outputDir = Path.GetRelativePath(_webHostEnvironment.WebRootPath, absoluteOutputDir);

        //        string outputJsonPath = Path.Combine(_webHostEnvironment.WebRootPath, outputDir);

        //        // Step 3: Call the ImportDataset method to import the dataset
        //        ResultDTO<DatasetDTO> importDatasetResult =
        //            await _datasetService.ImportDatasetCocoFormatedAtDirectoryPath(datasetName, outputJsonPath, userId, _webHostEnvironment.WebRootPath);

        //        if (!importDatasetResult.IsSuccess && ResultDTO<DatasetDTO>.HandleError(importDatasetResult))
        //        {
        //            return Json(new { responseError = DbResHtml.T(importDatasetResult.ErrMsg!, "Resources") });
        //        }

        //        return Json(new { importDatasetResult.Data });
        //    }
        //    catch (Exception ex)
        //    {
        //        // TODO: Add logging
        //        return Json(new { responseError = DbResHtml.T(ex.Message, "Resources") });
        //    }
        //    finally
        //    {
        //        // Clean up the temporary zip file
        //        if (System.IO.File.Exists(tempFilePath))
        //        {
        //            System.IO.File.Delete(tempFilePath);
        //        }
        //    }
        //}
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

                if (!importDatasetResult.IsSuccess)
                {
                    CleanUp(tempFilePath, outputJsonPath);

                    return Json(new { responseError = DbResHtml.T("Error importing dataset file", "Resources") });
                }

                await GenerateThumbnailsForDataset(importDatasetResult.Data.Id);

                CleanUp(tempFilePath, outputJsonPath);

                return Json(new { responseSuccess = DbResHtml.T("Dataset imported and thumbnails generated", "Resources"), dataset = importDatasetResult.Data });
            }
            catch (Exception ex)
            {
                return Json(new { responseError = DbResHtml.T(ex.Message, "Resources") });
            }
        }

        private void CleanUp(string tempFilePath, string outputJsonPath)
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

        private string ProcessZipFile(string zipFilePath, string webRootPath)
        {
            //coco dataset
            string tempDir = Path.Combine(webRootPath, "temp");
            Directory.CreateDirectory(tempDir);

            string extractDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            ZipFile.ExtractToDirectory(zipFilePath, extractDir);

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
                    string jsonFilePath = Path.Combine(folderPath, "_annotations.coco.json");
                    if (System.IO.File.Exists(jsonFilePath))
                    {
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
                DatasetDTO datasetDb = await _datasetService.GetDatasetById(datasetId)
                                       ?? throw new Exception("Dataset not found");

                if (datasetDb.IsPublished == true)
                    return Json(new { responseErrorAlreadyPublished = DbResHtml.T("Dataset is already published. No changes allowed", "Resources") });

                ResultDTO<string?> datasetThumbnailsFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DatasetThumbnailsFolder", "DatasetThumbnails");
                if (datasetThumbnailsFolder.IsSuccess == false || datasetThumbnailsFolder.Data == null)
                    return Json(new { responseError = DbResHtml.T("Could not retrieve thumbnail folder", "Resources") });

                string thumbnailsFolder = Path.Combine(_webHostEnvironment.WebRootPath, datasetThumbnailsFolder.Data, datasetDb.Id.ToString());
                if (!Directory.Exists(thumbnailsFolder))
                    Directory.CreateDirectory(thumbnailsFolder);

                List<DatasetImageDTO> datasetImages = await _datasetImagesService.GetImagesForDataset(datasetId);

                foreach (var datasetImage in datasetImages)
                {
                    string imageFilePath = Path.Combine(_webHostEnvironment.WebRootPath, datasetImage.ImagePath.TrimStart('\\'), datasetImage.FileName);

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


    }
}
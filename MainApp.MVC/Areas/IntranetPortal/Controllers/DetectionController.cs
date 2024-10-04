using AutoMapper;
using DAL.Interfaces.Helpers;
using DocumentFormat.OpenXml.Spreadsheet;
using DTOs.MainApp.BL;
using DTOs.MainApp.BL.DetectionDTOs;
using DTOs.ObjectDetection.API.Responses.DetectionRun;
using Entities.DetectionEntities;
using MainApp.BL.Interfaces.Services.DetectionServices;
using MainApp.MVC.Helpers;
using MainApp.MVC.ViewModels.IntranetPortal.Detection;
using Microsoft.AspNetCore.Mvc;
using SD;
using Services.Interfaces.Services;
using System.Security.Claims;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    public class DetectionController : Controller
    {
        private readonly IDetectionRunService _detectionRunService;
        private readonly IUserManagementService _userManagementService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IAppSettingsAccessor _appSettingsAccessor;

        private string _baseDetectionRunInputImagesSaveDirectoryPath = "detection-runs\\input-images";
        private string _baseDetectionRunCopyVisualizedOutputImagesDirectoryPathMVC = "detection-runs\\outputs\\visualized-images";
        private string _baseSaveMMDetectionDirectoryAbsPath = "C:\\vs_code_workspaces\\mmdetection\\mmdetection\\ins_development";

        public DetectionController(IUserManagementService userManagementService, IConfiguration configuration, IMapper mapper, IWebHostEnvironment webHostEnvironment, IAppSettingsAccessor appSettingsAccessor,IDetectionRunService detectionRunService)
        {
            _userManagementService = userManagementService;
            _configuration = configuration;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _appSettingsAccessor = appSettingsAccessor;
            _detectionRunService = detectionRunService;

            string? baseSaveMMDetectionDirectoryAbsPath =
                _configuration["AppSettings:MMDetection:BaseSaveMMDetectionDirectoryAbsPath"];
            if (string.IsNullOrEmpty(baseSaveMMDetectionDirectoryAbsPath))
                throw new Exception($"{nameof(baseSaveMMDetectionDirectoryAbsPath)} is missing");

            string? baseDetectionRunCopyVisualizedOutputDirPathMVC =
                _configuration["AppSettings:MVC:BaseDetectionRunCopyVisualizedOutputImagesDirectoryPath"];
            if (string.IsNullOrEmpty(baseDetectionRunCopyVisualizedOutputDirPathMVC))
                throw new Exception($"{nameof(baseDetectionRunCopyVisualizedOutputDirPathMVC)} is missing");

            string? baseDetectionRunInputImagesSaveDirPathMVC =
                _configuration["AppSettings:MVC:BaseDetectionRunInputImagesSaveDirectoryPath"];
            if (string.IsNullOrEmpty(baseDetectionRunInputImagesSaveDirPathMVC))
                throw new Exception($"{nameof(baseDetectionRunInputImagesSaveDirPathMVC)} is missing");

            _baseDetectionRunInputImagesSaveDirectoryPath = baseDetectionRunInputImagesSaveDirPathMVC;
            _baseDetectionRunCopyVisualizedOutputImagesDirectoryPathMVC = baseDetectionRunCopyVisualizedOutputDirPathMVC;
            _baseSaveMMDetectionDirectoryAbsPath = baseSaveMMDetectionDirectoryAbsPath;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DetectedZones()
        {
            var detectionRunsListDTOs = await _detectionRunService.GetDetectionRunsWithClasses() ?? throw new Exception("Object not found");
            var detectionRunsViewModelList = _mapper.Map<List<DetectionRunViewModel>>(detectionRunsListDTOs) ?? throw new Exception("Object not found");
            return View(detectionRunsViewModelList);
        }

        private async Task<List<DetectionRunDTO>> GetDummyDetectionRunDTOsAsync()
        {
            var id = User.FindFirstValue("UserId");
            var appUser = await _userManagementService.GetUserById(id);
            if (appUser is null)
                throw new Exception("User not found");
            //return ResultDTO<UserManagementDTO>.Fail("User not found");

            UserDTO dtoUser = _mapper.Map<UserDTO>(appUser);

            return new List<DetectionRunDTO>
                {
                    new DetectionRunDTO
                    {
                        Id = Guid.Parse("0935966d-a036-448a-8b6b-963d494345dc"), IsCompleted = true,
                        Name = "Volkovo new park 2024-03-22", Description = "Volkovo new park area drone imagery taken on 2024-03-22",
                        CreatedById = dtoUser.Id, CreatedBy = dtoUser, CreatedOn = DateTime.UtcNow.AddDays(-1),
                        DetectionInputImageId = Guid.Parse("0935966d-a036-448a-8b6b-963d494345dc"),
                        DetectionInputImage = new DetectionInputImageDTO
                        {
                            Id = Guid.Parse("0935966d-a036-448a-8b6b-963d494345dc"),
                            Name = "Volkovo_Drone",
                            ImageFileName = "Volkovo_Drone.tif",
                            ImagePath = "/detection-runs/input-images/0935966d-a036-448a-8b6b-963d494345dc/Volkovo_Drone.tif",
                            DateTaken = DateTime.UtcNow.AddDays(-1),
                            CreatedById = dtoUser.Id,
                            CreatedOn = DateTime.UtcNow.AddDays(-1)
                        }

                    },
                    new DetectionRunDTO
                    {
                        Id = Guid.Parse("cf4d9a42-8cf6-4c3b-918f-05851f0f553c"), IsCompleted = true,
                        Name = "Zlokukjani river bridge 2024-03-20", Description = "Zlokukjani river bridge area drone imagery taken on 2024-03-20",
                        CreatedById = dtoUser.Id, CreatedBy = dtoUser, CreatedOn = DateTime.UtcNow.AddDays(-2),
                        DetectionInputImageId = Guid.Parse("cf4d9a42-8cf6-4c3b-918f-05851f0f553c"),
                        DetectionInputImage = new DetectionInputImageDTO
                        {
                            Id = Guid.Parse("cf4d9a42-8cf6-4c3b-918f-05851f0f553c"),
                            Name = "Zlokukjani_Drone",
                            ImageFileName = "Zlokukjani_Drone.tif",
                            ImagePath = "/detection-runs/input-images/cf4d9a42-8cf6-4c3b-918f-05851f0f553c/Zlokukjani_Drone.tif",
                            DateTaken = DateTime.UtcNow.AddDays(-2),
                            CreatedById = dtoUser.Id,
                            CreatedOn = DateTime.UtcNow.AddDays(-2)
                        }
                    },
                    //new DetectionRunDTO
                    //{
                    //    Id = Guid.NewGuid(), IsCompleted = true,
                    //    Name = "Saraj near Vardar 2024-02-26", Description = "Saraj near Vardar river area drone imagery taken on 2024-02-26",
                    //    CreatedById = dtoUser.Id, CreatedBy = dtoUser, CreatedOn = DateTime.UtcNow.AddDays(-3),
                    //    ImageFileName = "", ImagePath = ""
                    //}
                };
        }

        [HttpGet]
        public async Task<IActionResult> CreateDetectionRun()
        {

            var detectionRuns = await GetDummyDetectionRunDTOsAsync();

            return View(detectionRuns);
        }

        [HttpPost]
        [RequestSizeLimit(int.MaxValue)]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<ResultDTO<(string, string)>> StartDetectionRun
            (string name, string description, string imgName, IFormFile imgFile)
        {
            if (string.IsNullOrEmpty(name))
                return ResultDTO<(string, string)>.Fail($"{nameof(name)} is null or empty");

            if (string.IsNullOrEmpty(description))
                return ResultDTO<(string, string)>.Fail($"{nameof(description)} is null or empty");

            if (string.IsNullOrEmpty(imgName))
                return ResultDTO<(string, string)>.Fail($"{nameof(imgName)} is null or empty");

            if (imgFile is null)
                return ResultDTO<(string, string)>.Fail($"{nameof(imgFile)} is null");

            // Generate Detection Run Id
            Guid detectionRunId = Guid.NewGuid();
            string detectionRunIdStr = detectionRunId.ToString();

            string detectionImgfileExtension = Path.GetExtension(imgFile.FileName);

            string saveDirectoryPath = Path.Combine(_baseDetectionRunInputImagesSaveDirectoryPath, detectionRunIdStr);

            try
            {
                // Save Detection Run Input Image
                ResultDTO<(string absoluteFilePath, string relativeFilePath)> resultSave =
                    SaveFormFileToPathAtRootWithName(imgFile, saveDirectoryPath, detectionRunIdStr);
                if (resultSave.IsSuccess == false && resultSave.HandleError())
                    return ResultDTO<(string, string)>.Fail("Error saving input image");
                (string absoluteFilePath, string relativeFilePath) = resultSave.Data;

                // Get Current Logged In User
                var userId = User.FindFirstValue("UserId");
                var currUserDTO = await _userManagementService.GetUserById(userId);
                if (currUserDTO is null)
                    throw new Exception("User not found");

                // Create DetectionInputImage
                DetectionInputImageDTO detectionInputImageDTO = new DetectionInputImageDTO()
                {
                    Id = detectionRunId,
                    Name = imgName,
                    ImageFileName = imgName,
                    ImagePath = absoluteFilePath,
                    CreatedById = currUserDTO.Id,
                    CreatedBy = currUserDTO,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedById = currUserDTO.Id
                };

                // Create Detection Run DTO
                DetectionRunDTO detectionRunDTO = new DetectionRunDTO()
                {
                    Id = detectionRunId,
                    Name = name,
                    Description = description,
                    CreatedById = currUserDTO.Id,
                    CreatedBy = currUserDTO,
                    DetectionInputImage = detectionInputImageDTO,
                    DetectionInputImageId = detectionRunId,
                };

                // Create Detection Run
                ResultDTO resultCreate = await _detectionRunService.CreateDetectionRun(detectionRunDTO);
                if (resultCreate.IsSuccess == false && resultCreate.HandleError())
                {
                    DeleteFileAtPath(absoluteFilePath);
                    return ResultDTO<(string, string)>.Fail(resultCreate.ErrMsg!);
                }

                // Start Detection Run SYNCHRONIZED WAITS FOR DETECTION RESULTS
                ResultDTO resultDetection = await _detectionRunService.StartDetectionRun(detectionRunDTO);
                if (resultDetection.IsSuccess == false && resultDetection.HandleError())
                    return ResultDTO<(string, string)>.Fail(resultDetection.ErrMsg!);

                // Update Is Completed
                ResultDTO resultIsCompletedUpdate = await _detectionRunService.IsCompleteUpdateDetectionRun(detectionRunDTO);
                if (resultIsCompletedUpdate.IsSuccess == false && resultIsCompletedUpdate.HandleError())
                    return ResultDTO<(string, string)>.Fail(resultIsCompletedUpdate.ErrMsg!);

                // Get and Check if Detection Run Success Output Files Exist
                ResultDTO<(string visualizedFilePath, string bboxesFilePath)> resultGetDetectionResultFiles =
                    await _detectionRunService.GetRawDetectionRunResultPathsByRunId(detectionRunId, detectionImgfileExtension);
                if (resultGetDetectionResultFiles.IsSuccess == false && resultGetDetectionResultFiles.HandleError())
                    return ResultDTO<(string, string)>.Fail(resultGetDetectionResultFiles.ErrMsg!);

                // Copy Visualized Output Image to MVC wwwroot to be able to show
                ResultDTO<(string absVisualizedFilePathMVC, string relVisualizedFilePathMVC)> resultCopyFile =
                    CopyFileFromPathToRootAtDir(resultGetDetectionResultFiles.Data.visualizedFilePath,
                                                _baseDetectionRunCopyVisualizedOutputImagesDirectoryPathMVC);
                if (resultCopyFile.IsSuccess == false && resultCopyFile.HandleError())
                    return ResultDTO<(string, string)>.Fail(resultGetDetectionResultFiles.ErrMsg!);

                // Get BBox Prediction Results from JSON as Deserialized DetectionRunFinishedResponse
                ResultDTO<DetectionRunFinishedResponse> resultBBoxDeserialization =
                    await _detectionRunService.GetBBoxResultsDeserialized(resultGetDetectionResultFiles.Data.bboxesFilePath);
                if (resultBBoxDeserialization.IsSuccess == false && resultBBoxDeserialization.HandleError())
                    return ResultDTO<(string, string)>.Fail(resultBBoxDeserialization.ErrMsg!);

                // TODO: Convert BBoxes to Projection 
                ResultDTO<DetectionRunFinishedResponse> resultBBoxConversionToProjection =
                    await _detectionRunService.ConvertBBoxResultToImageProjection(absoluteFilePath, resultBBoxDeserialization.Data!);
                if (resultBBoxDeserialization.IsSuccess == false && resultBBoxDeserialization.HandleError())
                    return ResultDTO<(string, string)>.Fail(resultBBoxDeserialization.ErrMsg!);

                // TODO: Save Detected Dump Sites -> ! Each row is a single polygon  
                ResultDTO<List<DetectedDumpSite>> resultCreateDetectedDumpSites =
                    await _detectionRunService.CreateDetectedDumpsSitesFromDetectionRun(detectionRunId, resultBBoxConversionToProjection.Data!);
                if (resultCreateDetectedDumpSites.IsSuccess == false && resultCreateDetectedDumpSites.HandleError())
                    return ResultDTO<(string, string)>.Fail(resultCreateDetectedDumpSites.ErrMsg!);

                return ResultDTO<(string, string)>.Ok((detectionRunIdStr, resultCopyFile.Data.relVisualizedFilePathMVC));
            }
            catch (Exception ex)
            {
                return ResultDTO<(string, string)>.ExceptionFail(ex.Message, ex);
            }
        }

        private ResultDTO DeleteFileAtPath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return ResultDTO.Fail($"{nameof(filePath)} is null or empty");

            try
            {
                // Check if the file exists
                if (System.IO.File.Exists(filePath) == false)
                    return ResultDTO.Fail("File does not exist.");

                System.IO.File.Delete(filePath);
                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                return ResultDTO.Fail($"Error deleting file: {ex.Message}");
            }
        }

        private ResultDTO<(string absolutePath, string relativePath)> SaveFormFileToPathAtRootWithName(IFormFile imgFile,
            string directoryRelativePath, string fileName)
        {
            if (imgFile is null)
                return ResultDTO<(string, string)>.Fail($"{nameof(imgFile)} is null");

            if (string.IsNullOrEmpty(directoryRelativePath))
                return ResultDTO<(string, string)>.Fail($"{nameof(directoryRelativePath)} is null or empty");

            if (string.IsNullOrEmpty(fileName))
                return ResultDTO<(string, string)>.Fail($"{nameof(fileName)} is null or empty");

            try
            {
                string baseDirectoryMVC = _webHostEnvironment.WebRootPath;


                string fileNameWithExt = Path.GetFileNameWithoutExtension(fileName) + Path.GetExtension(imgFile.FileName);
                string relativeFilePath = Path.Combine(directoryRelativePath, fileNameWithExt);

                string absoluteDirectoryPathMVC = Path.Combine(baseDirectoryMVC, directoryRelativePath);
                string absoluteFilePathMVC = Path.Combine(absoluteDirectoryPathMVC, fileNameWithExt);

                string absoluteDirectoryPathMMDetection = Path.Combine(_baseSaveMMDetectionDirectoryAbsPath, directoryRelativePath);
                string absoluteFilePathMMDetection = Path.Combine(absoluteDirectoryPathMMDetection, fileNameWithExt);

                // Create the directory if it doesn't already exist
                if (!Directory.Exists(absoluteDirectoryPathMMDetection))
                    Directory.CreateDirectory(absoluteDirectoryPathMMDetection!);

                // Save the file to the path
                using (var stream = new FileStream(absoluteFilePathMMDetection, FileMode.Create))
                {
                    imgFile.CopyTo(stream);
                }

                return ResultDTO<(string absolutePath, string relativePath)>.Ok((absoluteFilePathMMDetection, relativeFilePath));
            }
            catch (Exception ex)
            {
                return ResultDTO<(string, string)>.Fail($"Error saving file: {ex.Message}");
            }
        }

        private ResultDTO<(string absolutePath, string relativePath)> CopyFileFromPathToRootAtDir(string absOriginalFilePath, string directoryRelativePath)
        {
            if (string.IsNullOrEmpty(absOriginalFilePath))
                return ResultDTO<(string, string)>.Fail($"{nameof(absOriginalFilePath)} is null or empty");

            if (string.IsNullOrEmpty(directoryRelativePath))
                return ResultDTO<(string, string)>.Fail($"{nameof(directoryRelativePath)} is null or empty");

            try
            {
                if (System.IO.File.Exists(absOriginalFilePath) == false)
                    return ResultDTO<(string, string)>.Fail($"{nameof(directoryRelativePath)} - Original File can't be accessed or found");

                string baseDirectoryMVC = _webHostEnvironment.WebRootPath;
                string absCopyDirPath = Path.Combine(baseDirectoryMVC, directoryRelativePath);

                if (!Directory.Exists(absCopyDirPath))
                    Directory.CreateDirectory(absCopyDirPath!);

                string fileNameWithExt = Path.GetFileName(absOriginalFilePath);
                string relativeFilePath = Path.Combine(directoryRelativePath, fileNameWithExt);
                string absFilePath = Path.Combine(absCopyDirPath, fileNameWithExt);

                // Code to copy the file from the original path to the new path
                System.IO.File.Copy(absOriginalFilePath, absFilePath, overwrite: true);
                if (System.IO.File.Exists(absFilePath) == false)
                    return ResultDTO<(string, string)>.Fail($"{nameof(absFilePath)} - File not copied");

                return ResultDTO<(string absolutePath, string relativePath)>.Ok((absFilePath, relativeFilePath));
            }
            catch (Exception ex)
            {
                return ResultDTO<(string, string)>.Fail($"Error saving file: {ex.Message}");
            }
        }


        public async Task<List<DetectionRunViewModel>> GetAllDetectionRuns()
        {
            var detectionRunsListDTOs = await _detectionRunService.GetDetectionRunsWithClasses() ?? throw new Exception("Object not found");
            var detectionRunsViewModelList = _mapper.Map<List<DetectionRunViewModel>>(detectionRunsListDTOs) ?? throw new Exception("Object not found");
            return detectionRunsViewModelList;
        }

        [HttpPost]
        public async Task<ResultDTO<List<DetectionRunDTO>>> ShowDumpSitesOnMap(List<Guid> selectedDetectionRunsIds)
        {
            var selectedDetectionRuns = await _detectionRunService.GetSelectedDetectionRunsIncludingDetectedDumpSites(selectedDetectionRunsIds);
            if (selectedDetectionRuns.IsSuccess == false && selectedDetectionRuns.HandleError())
                return ResultDTO<List<DetectionRunDTO>>.Fail(selectedDetectionRuns.ErrMsg!);
            if (selectedDetectionRuns.Data == null)
                return ResultDTO<List<DetectionRunDTO>>.Fail("Data is null");

            return ResultDTO<List<DetectionRunDTO>>.Ok(selectedDetectionRuns.Data);
        }

        [HttpPost]
        public async Task<List<AreaComparisonAvgConfidenceRateReportDTO>> GenerateAreaComparisonAvgConfidenceRateReport(List<Guid> selectedDetectionRunsIds)
        {
            return await _detectionRunService.GenerateAreaComparisonAvgConfidenceRateData(selectedDetectionRunsIds);
        }

        //images
        [HttpGet]
        public async Task<IActionResult> ViewAllImages()
        {
            var resultDtoList = await _detectionRunService.GetAllImages();

            if (!resultDtoList.IsSuccess && resultDtoList.HandleError())
            {
                var errorPath = _configuration["ErrorViewsPath:Error"];
                if (errorPath == null)
                {
                    return BadRequest();
                }
                return Redirect(errorPath);
            }

            var vmList = _mapper.Map<List<DetectionInputImageViewModel>>(resultDtoList.Data);

            return View(vmList);
        }

        [HttpPost]
        [RequestSizeLimit(int.MaxValue)]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<ResultDTO> AddImage(DetectionInputImageViewModel detectionInputImageViewModel, IFormFile? file)
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return ResultDTO.Fail(error);
            }

            var detectionInputImagesFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DetectionInputImagesFolder", "Uploads\\DetectionUploads\\InputImages");
            if (!detectionInputImagesFolder.IsSuccess && detectionInputImagesFolder.HandleError()){
                return ResultDTO.Fail("Can not get the application settings");
            }

            if(string.IsNullOrEmpty(detectionInputImagesFolder.Data))
            {
                return ResultDTO.Fail("Image folder path not found");
            }
            // string saveDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "detection", "input-images");
            string saveDirectory = Path.Combine(_webHostEnvironment.WebRootPath, detectionInputImagesFolder.Data);
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string filePath = Path.Combine(saveDirectory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                detectionInputImageViewModel.ImageFileName = fileName;
                // detectionInputImageViewModel.ImagePath = Path.Combine("detection", "input-images", fileName);
                detectionInputImageViewModel.ImagePath = string.Format("{0}\\{1}", detectionInputImagesFolder.Data, fileName);
            }
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return ResultDTO.Fail("User id not found");
            }
            var currUserDTO = await _userManagementService.GetUserById(userId);

            if (currUserDTO is null)
            {
                return ResultDTO.Fail("User is not found");
            }
            detectionInputImageViewModel.CreatedById = userId;
            detectionInputImageViewModel.UpdatedById = userId;

            var dto = _mapper.Map<DetectionInputImageDTO>(detectionInputImageViewModel);
            if (dto == null)
            {
                var error = DbResHtml.T("Mapping failed", "Resources");
                return ResultDTO.Fail(error.ToString());
            }

            ResultDTO resultCreate = await _detectionRunService.CreateDetectionInputImage(dto);
            if (!resultCreate.IsSuccess && resultCreate.HandleError())
            {
                return ResultDTO.Fail(resultCreate.ErrMsg!);
            }

            return ResultDTO.Ok();
        }

        [HttpPost]
        public async Task<ResultDTO> EditDetectionImageInput(DetectionInputImageViewModel detectionInputImageViewModel)
        {           
            if (!ModelState.IsValid)
            {
                var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return ResultDTO.Fail(error);
            }

            var id = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(id))
            {
                return ResultDTO.Fail("User id not found");
            }
            var appUser = await _userManagementService.GetUserById(id);
            if (appUser == null)
            {
                var error = DbResHtml.T("User not found", "Resources");
                return ResultDTO.Fail(error.ToString());
            }

            detectionInputImageViewModel.UpdatedById = appUser.Id;
            detectionInputImageViewModel.UpdatedOn = DateTime.UtcNow;

            var dto = _mapper.Map<DetectionInputImageDTO>(detectionInputImageViewModel);
            if (dto == null)
            {
                var error = DbResHtml.T("Mapping failed", "Resources");
                return ResultDTO.Fail(error.ToString());
            }

            ResultDTO resultEdit = await _detectionRunService.EditDetectionInputImage(dto);
            if (!resultEdit.IsSuccess && resultEdit.HandleError())
            {
                return ResultDTO.Fail(resultEdit.ErrMsg!);
            }

            return ResultDTO.Ok();
        }

        [HttpPost]
        public async Task<ResultDTO> DeleteDetectionImageInput(DetectionInputImageViewModel detectionInputImageViewModel)
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return ResultDTO.Fail(error);
            }

            var resultCheckForFiles = await _detectionRunService.GetDetectionInputImageByDetectionRunId(detectionInputImageViewModel.Id);
            if (!resultCheckForFiles.IsSuccess && resultCheckForFiles.HandleError())
            {
                return ResultDTO.Fail(resultCheckForFiles.ErrMsg!);
            }
            if (resultCheckForFiles.Data == null)
            {
                return ResultDTO.Fail("Data is null");
            }

            if (resultCheckForFiles.Data.Count > 0)
            {
                var error = DbResHtml.T("This image is still used in the detection run. Delete the detection run first!", "Resources");
                return ResultDTO.Fail(error.ToString());
            }

            var dto = _mapper.Map<DetectionInputImageDTO>(detectionInputImageViewModel);
            if (dto == null)
            {
                var error = DbResHtml.T("Mapping failed", "Resources");
                return ResultDTO.Fail(error.ToString());
            }

            ResultDTO resultDelete = await _detectionRunService.DeleteDetectionInputImage(dto);
            if (!resultDelete.IsSuccess && resultDelete.HandleError())
            {
                return ResultDTO.Fail(resultDelete.ErrMsg!);
            }

            //string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", detectionInputImageViewModel.ImagePath ?? string.Empty);
            string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, detectionInputImageViewModel.ImagePath!);
            if (System.IO.File.Exists(imagePath))
            {
                try
                {
                    System.IO.File.Delete(imagePath);
                }
                catch (Exception ex)
                {
                    var fileDeleteError = DbResHtml.T("Failed to delete the image file", "Resources");
                    return ResultDTO.Fail($"{fileDeleteError}: {ex.Message}");
                }
            }

            return ResultDTO.Ok();
        }

        [HttpPost]
        public async Task<ResultDTO<DetectionInputImageDTO>> GetDetectionInputImageById(Guid detectionInputImageId)
        {
            ResultDTO<DetectionInputImageDTO> resultGetEntity = await _detectionRunService.GetDetectionInputImageById(detectionInputImageId);
            if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())
            {
                return ResultDTO<DetectionInputImageDTO>.Fail(resultGetEntity.ErrMsg!);
            }
            if (resultGetEntity.Data == null)
            {
                return ResultDTO<DetectionInputImageDTO>.Fail("Image Input is null");

            }
            return ResultDTO<DetectionInputImageDTO>.Ok(resultGetEntity.Data);
        }



        [HttpGet]
        public async Task<ResultDTO<List<DetectionInputImageDTO>>> GetAllDetectionInputImages()
        {
            ResultDTO<List<DetectionInputImageDTO>> resultGetEntities = await _detectionRunService.GetAllImages();

            if (!resultGetEntities.IsSuccess && resultGetEntities.HandleError())
            {
                return ResultDTO<List<DetectionInputImageDTO>>.Fail(resultGetEntities.ErrMsg!);
            }

            if (resultGetEntities.Data == null)
            {
                return ResultDTO<List<DetectionInputImageDTO>>.Fail("Detection input images are not found");
            }
            return ResultDTO<List<DetectionInputImageDTO>>.Ok(resultGetEntities.Data);
        }

        [HttpPost]
        public async Task<ResultDTO<List<DetectionInputImageDTO>>> GetSelectedDetectionInputImages(List<Guid> selectedImagesIds)
        {
            ResultDTO<List<DetectionInputImageDTO>> resultGetEntities = await _detectionRunService.GetSelectedInputImagesById(selectedImagesIds);

            if (!resultGetEntities.IsSuccess && resultGetEntities.HandleError())
            {
                return ResultDTO<List<DetectionInputImageDTO>>.Fail(resultGetEntities.ErrMsg!);
            }

            if (resultGetEntities.Data == null)
            {
                return ResultDTO<List<DetectionInputImageDTO>>.Fail("Detection input images are not found");
            }

            foreach (var item in resultGetEntities.Data)
            {
                item.ImagePath = "/" + item.ImagePath?.Replace("\\", "/");
            }
            return ResultDTO<List<DetectionInputImageDTO>>.Ok(resultGetEntities.Data);
        }
    }
}

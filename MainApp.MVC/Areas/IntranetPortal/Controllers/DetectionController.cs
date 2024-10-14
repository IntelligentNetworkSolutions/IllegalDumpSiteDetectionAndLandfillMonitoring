using AutoMapper;
using CliWrap;
using CliWrap.Buffered;
using DAL.Interfaces.Helpers;
using DocumentFormat.OpenXml.Vml;
using DTOs.MainApp.BL;
using DTOs.MainApp.BL.DetectionDTOs;
using DTOs.ObjectDetection.API.Responses.DetectionRun;
using Entities.DetectionEntities;
using MainApp.BL.Interfaces.Services.DetectionServices;
using MainApp.MVC.Helpers;
using MainApp.MVC.ViewModels.IntranetPortal.Detection;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Mvc;
using OSGeo.GDAL;
using SD;
using Services.Interfaces.Services;
using System.Security.Claims;
using System.IO;
using Hangfire.States;
using Hangfire;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Office2010.Excel;
using MainApp.MVC.Filters;
using SD.Enums;
using System.Runtime.InteropServices;
using System.Text;
using Hangfire.Server;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using MimeKit;

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
        private readonly IBackgroundJobClient _backgroundJobClient;

        private string _baseDetectionRunInputImagesSaveDirectoryPath = "detection-runs\\input-images";
        private string _baseDetectionRunCopyVisualizedOutputImagesDirectoryPathMVC = "detection-runs\\outputs\\visualized-images";
        private string _baseSaveMMDetectionDirectoryAbsPath = "C:\\vs_code_workspaces\\mmdetection\\mmdetection\\ins_development";

        public DetectionController(IUserManagementService userManagementService, IConfiguration configuration, IMapper mapper, IWebHostEnvironment webHostEnvironment, IAppSettingsAccessor appSettingsAccessor,IDetectionRunService detectionRunService, IBackgroundJobClient backgroundJobClient)
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
            _backgroundJobClient = backgroundJobClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewDetectedZones))]
        public async Task<IActionResult> DetectedZones()
        {
            var detectionRunsListDTOs = await _detectionRunService.GetDetectionRunsWithClasses() ?? throw new Exception("Object not found");
            var detectionRunsViewModelList = _mapper.Map<List<DetectionRunViewModel>>(detectionRunsListDTOs) ?? throw new Exception("Object not found");
            detectionRunsViewModelList = detectionRunsViewModelList.Where(x => x.IsCompleted == true && x.Status == "Success").ToList();
            return View(detectionRunsViewModelList);
        }

        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.CreateDetectionRun))]
        public async Task<IActionResult> CreateDetectionRun()
        {           
            return View();
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.ScheduleDetectionRun))]
        public async Task<ResultDTO> ScheduleDetectionRun(DetectionRunViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return ResultDTO.Fail(error);
            }

            //get selected image input from database
            ResultDTO<DetectionInputImageDTO> resultGetInputImage = await _detectionRunService.GetDetectionInputImageById(viewModel.SelectedInputImageId);
            if (resultGetInputImage.IsSuccess == false && resultGetInputImage.HandleError())
                return ResultDTO.Fail(resultGetInputImage.ErrMsg!);
            if (resultGetInputImage.Data == null)
                return ResultDTO.Fail("Image not found");

            //find current user
            var userId = User.FindFirstValue("UserId");
            if (userId is null)
                return ResultDTO.Fail("User id is null");

            var currUserDTO = await _userManagementService.GetUserById(userId);
            if (currUserDTO is null)
                return ResultDTO.Fail("User not found");

            string absInputImgPath = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, resultGetInputImage.Data.ImagePath!);
            string inputImgExtension = System.IO.Path.GetExtension(resultGetInputImage.Data.ImageFileName!);

            DetectionRunDTO detectionRunDTO = new()
            {
                Id = Guid.NewGuid(),
                Name = viewModel.Name,
                Description = viewModel.Description,
                CreatedById = currUserDTO.Id,
                CreatedBy = currUserDTO,
                DetectionInputImageId = viewModel.SelectedInputImageId,
                InputImgPath = absInputImgPath,
                Status = nameof(ScheduleRunsStatus.Waiting)
            };
            // Create Detection Run
            ResultDTO resultCreate = await _detectionRunService.CreateDetectionRun(detectionRunDTO);
            if (resultCreate.IsSuccess == false && resultCreate.HandleError())
                return ResultDTO.Fail(resultCreate.ErrMsg!);

            string jobId = _backgroundJobClient.Enqueue(() => StartDetectionRun(detectionRunDTO));
            using (var connection = JobStorage.Current.GetConnection())
            {
                connection.SetJobParameter(jobId, "detectionRunId", detectionRunDTO.Id.Value.ToString());
            }

            return ResultDTO.Ok();
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.StartDetectionRun))]
        [RequestSizeLimit(int.MaxValue)]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<ResultDTO<string>> StartDetectionRun(DetectionRunDTO detectionRunDTO)
        {                   
            try
            {
                //SATUS: PROCESSING
                ResultDTO resultStatusUpdate = await _detectionRunService.UpdateStatus(detectionRunDTO.Id.Value, nameof(ScheduleRunsStatus.Processing));
                if (resultStatusUpdate.IsSuccess == false && resultStatusUpdate.HandleError())
                {
                    ResultDTO<string> resHandleErrorProcess = await HandleErrorProcess(detectionRunDTO.Id.Value, resultStatusUpdate.ErrMsg!);
                    return ResultDTO<string>.Fail(resHandleErrorProcess.Data!);
                }

                // Start Detection Run SYNCHRONIZED WAITS FOR DETECTION RESULTS
                ResultDTO resultDetection = await _detectionRunService.StartDetectionRun(detectionRunDTO);
                if (resultDetection.IsSuccess == false && resultDetection.HandleError())
                {
                    ResultDTO<string> resHandleErrorProcess = await HandleErrorProcess(detectionRunDTO.Id.Value, resultDetection.ErrMsg!);
                    return ResultDTO<string>.Fail(resHandleErrorProcess.Data!);
                }

                // Update Is Completed
                ResultDTO resultIsCompletedUpdate = await _detectionRunService.IsCompleteUpdateDetectionRun(detectionRunDTO);
                if (resultIsCompletedUpdate.IsSuccess == false && resultIsCompletedUpdate.HandleError())
                {
                    ResultDTO<string> resHandleErrorProcess = await HandleErrorProcess(detectionRunDTO.Id.Value, resultIsCompletedUpdate.ErrMsg!);
                    return ResultDTO<string>.Fail(resHandleErrorProcess.Data!);
                }

                // Get and Check if Detection Run Success Output Files Exist
                ResultDTO<string> resultGetDetectionResultFiles = await _detectionRunService.GetRawDetectionRunResultPathsByRunId(detectionRunDTO.Id.Value);
                if (resultGetDetectionResultFiles.IsSuccess == false && resultGetDetectionResultFiles.HandleError())
                {
                    ResultDTO<string> resHandleErrorProcess = await HandleErrorProcess(detectionRunDTO.Id.Value, resultGetDetectionResultFiles.ErrMsg!);
                    return ResultDTO<string>.Fail(resHandleErrorProcess.Data!);
                }

                // Get BBox Prediction Results from JSON as Deserialized DetectionRunFinishedResponse
                ResultDTO<DetectionRunFinishedResponse> resultBBoxDeserialization = await _detectionRunService.GetBBoxResultsDeserialized(resultGetDetectionResultFiles.Data!);
                if (resultBBoxDeserialization.IsSuccess == false && resultBBoxDeserialization.HandleError())
                {
                    ResultDTO<string> resHandleErrorProcess = await HandleErrorProcess(detectionRunDTO.Id.Value, resultBBoxDeserialization.ErrMsg!);
                    return ResultDTO<string>.Fail(resHandleErrorProcess.Data!);
                }

                //Convert BBoxes to Projection 
                ResultDTO<DetectionRunFinishedResponse> resultBBoxConversionToProjection =await _detectionRunService.ConvertBBoxResultToImageProjection(detectionRunDTO.InputImgPath, resultBBoxDeserialization.Data!);
                if (resultBBoxConversionToProjection.IsSuccess == false && resultBBoxConversionToProjection.HandleError())
                {
                    ResultDTO<string> resHandleErrorProcess = await HandleErrorProcess(detectionRunDTO.Id.Value, resultBBoxConversionToProjection.ErrMsg!);
                    return ResultDTO<string>.Fail(resHandleErrorProcess.Data!);
                }

                //Create detected dump sites in db
                ResultDTO<List<DetectedDumpSite>> resultCreateDetectedDumpSites = await _detectionRunService.CreateDetectedDumpsSitesFromDetectionRun(detectionRunDTO.Id.Value, resultBBoxConversionToProjection.Data!);
                if (resultCreateDetectedDumpSites.IsSuccess == false && resultCreateDetectedDumpSites.HandleError())
                {
                    ResultDTO<string> resHandleErrorProcess = await HandleErrorProcess(detectionRunDTO.Id.Value, resultCreateDetectedDumpSites.ErrMsg!);
                    return ResultDTO<string>.Fail(resHandleErrorProcess.Data!);
                }

                //STATUS: SUCCESS
                await _detectionRunService.UpdateStatus(detectionRunDTO.Id.Value, nameof(ScheduleRunsStatus.Success));

                return ResultDTO<string>.Ok(detectionRunDTO.Id.Value.ToString());
            }
            catch (Exception ex)
            {
                return ResultDTO<string>.ExceptionFail(ex.Message, ex);
            }
        }

        private async Task<ResultDTO<string>> HandleErrorProcess(Guid detectionRunId, string inputErrMsg)
        {
            StringBuilder errMsgBuilder = new();
            errMsgBuilder.AppendLine(inputErrMsg);
            //UPDATE STATUS: ERROR
            ResultDTO resStatusUpdate = await _detectionRunService.UpdateStatus(detectionRunId, nameof(ScheduleRunsStatus.Error));
            if (resStatusUpdate.IsSuccess == false && resStatusUpdate.HandleError())
                errMsgBuilder.AppendLine(resStatusUpdate.ErrMsg!);

            //Create error file
            ResultDTO resultFileCreating = await CreateErrMsgFile(detectionRunId, errMsgBuilder.ToString());
            if (resultFileCreating.IsSuccess == false && resultFileCreating.HandleError())
                errMsgBuilder.AppendLine(resultFileCreating.ErrMsg!);

            //Mark job as failed
            ResultDTO resMarkJobAsFailed = await MarkJobAsFailed(errMsgBuilder.ToString(), detectionRunId);
            if (resMarkJobAsFailed.IsSuccess == false && resMarkJobAsFailed.HandleError())
                errMsgBuilder.AppendLine(resMarkJobAsFailed.ErrMsg!);

            return ResultDTO<string>.Ok(errMsgBuilder.ToString());
        }
        private async Task<ResultDTO> MarkJobAsFailed(string errorMessage, Guid detectionRunId)
        {
            string jobId = string.Empty;
            bool isStateChanged = false;

            //Get the job
            var monitoringApi = JobStorage.Current.GetMonitoringApi();
            var processingJobs = monitoringApi.ProcessingJobs(0, int.MaxValue);

            foreach (var job in processingJobs)
            {              
                using (var connection = JobStorage.Current.GetConnection())
                {
                    var storedKey = connection.GetJobParameter(job.Key, "detectionRunId");
                    if (storedKey == detectionRunId.ToString())
                    {
                        jobId = job.Key;
                    }
                }
            }

            if (!string.IsNullOrEmpty(jobId) && errorMessage != null)
            {
                //Set the job state to failed
                Exception exception = new(errorMessage);
                isStateChanged = _backgroundJobClient.ChangeState(jobId, new FailedState(exception)
                {
                    Reason = errorMessage
                });
            } 

            if (!isStateChanged)
            {
                return ResultDTO.Fail("Job is not transferred in failed jobs");
            }

            return ResultDTO.Ok();
        }
        private async Task<ResultDTO> CreateErrMsgFile(Guid detectionRunId, string errMsg)
        {
            var detectionRunsErrorLogsFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DetectionRunsErrorLogsFolder", "Uploads\\DetectionUploads\\DetectionRunsErrorLogs");
            if (!detectionRunsErrorLogsFolder.IsSuccess && detectionRunsErrorLogsFolder.HandleError())
            {
                return ResultDTO.Fail("Can not get the application settings");
            }

            if (string.IsNullOrEmpty(detectionRunsErrorLogsFolder.Data))
            {
                return ResultDTO.Fail("Directory path not found");
            }

            try
            {
                string filePath = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, detectionRunsErrorLogsFolder.Data);
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                string fileName = $"{detectionRunId}_errMsg.txt";
                string fullFilePath = System.IO.Path.Combine(filePath, fileName);

                await System.IO.File.WriteAllTextAsync(fullFilePath, errMsg);

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                 return ResultDTO.Fail($"Failed to create the file: {ex.Message}");
            }

        }


        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewDetectionRuns))]
        public async Task<ResultDTO<string>> GetDetectionRunErrorLogMessage(Guid detectionRunId)
        {
            var detectionRunsErrorLogsFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DetectionRunsErrorLogsFolder", "Uploads\\DetectionUploads\\DetectionRunsErrorLogs");
            if (!detectionRunsErrorLogsFolder.IsSuccess && detectionRunsErrorLogsFolder.HandleError())
            {
                return ResultDTO<string>.Fail("Can not get the application settings");
            }

            if (string.IsNullOrEmpty(detectionRunsErrorLogsFolder.Data))
            {
                return ResultDTO<string>.Fail("Directory path not found");
            }

            string filePath = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, detectionRunsErrorLogsFolder.Data);
            string fileName = $"{detectionRunId}_errMsg.txt";
            string fullFilePath = System.IO.Path.Combine(filePath, fileName);

            // If file exists, read the error message from the txt file
            if (System.IO.File.Exists(fullFilePath))
            {
                string fileContent = await System.IO.File.ReadAllTextAsync(fullFilePath);
                return ResultDTO<string>.Ok(fileContent);
            }
            else
            {
                string jobId = string.Empty;
                StringBuilder errMsg = new();

                var monitoringApi = JobStorage.Current.GetMonitoringApi();
                var failedJobs = monitoringApi.FailedJobs(0, int.MaxValue);

                // Check failed jobs first
                foreach (var job in failedJobs)
                {
                    using (var connection = JobStorage.Current.GetConnection())
                    {
                        var storedKey = connection.GetJobParameter(job.Key, "detectionRunId");
                        if (storedKey == detectionRunId.ToString())
                        {
                            jobId = job.Key;
                            break;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(jobId))
                {
                    var jobDetails = monitoringApi.JobDetails(jobId);
                    var historyData = jobDetails?.History.FirstOrDefault()?.Data;
                    if (historyData != null)
                    {
                        foreach (var data in historyData)
                        {
                            errMsg.Append(data.Value);
                        }
                    }
                    else
                    {
                        errMsg.Append(jobDetails?.History.FirstOrDefault()?.Reason);
                    }

                    if (!string.IsNullOrEmpty(errMsg.ToString()))
                    {
                        return ResultDTO<string>.Ok(errMsg.ToString());
                    }
                }
                else
                {
                    // If job is not found in failed jobs, check successful jobs
                    var succeededJobs = monitoringApi.SucceededJobs(0, int.MaxValue);
                    foreach (var job in succeededJobs)
                    {
                        using (var connection = JobStorage.Current.GetConnection())
                        {
                            var storedKey = connection.GetJobParameter(job.Key, "detectionRunId");
                            if (storedKey == detectionRunId.ToString())
                            {
                                jobId = job.Key;
                                break;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(jobId))
                    {
                        var jobDetails = monitoringApi.JobDetails(jobId);
                        var historyData = jobDetails?.History.FirstOrDefault()?.Data;
                        if (historyData != null)
                        {
                            foreach (var data in historyData)
                            {
                                errMsg.Append(data.Value);
                            }
                        }
                        else
                        {
                            errMsg.Append(jobDetails?.History.FirstOrDefault()?.Reason);
                        }

                        if (!string.IsNullOrEmpty(errMsg.ToString()))
                        {
                            return ResultDTO<string>.Ok(errMsg.ToString());
                        }
                    }
                }

                return ResultDTO<string>.Ok("File and background scheduled job not found. Cannot read the error message!");
            }
        }

       
        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewDetectionRuns))]
        public async Task<List<DetectionRunViewModel>> GetAllDetectionRuns()
        {
            var detectionRunsListDTOs = await _detectionRunService.GetDetectionRunsWithClasses() ?? throw new Exception("Object not found");
            var detectionRunsViewModelList = _mapper.Map<List<DetectionRunViewModel>>(detectionRunsListDTOs) ?? throw new Exception("Object not found");
            detectionRunsViewModelList = detectionRunsViewModelList.Where(x => x.IsCompleted == true && x.Status == "Success").ToList();
            return detectionRunsViewModelList;
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.DeleteDetectionRun))]
        public async Task<ResultDTO> DeleteDetectionRun(Guid detectionRunId)
        {     
            if (detectionRunId == Guid.Empty)
                return ResultDTO.Fail("Invalid detection run id");

            var monitoringApi = JobStorage.Current.GetMonitoringApi();
            var enqueuedJobs = monitoringApi.EnqueuedJobs("default", 0, int.MaxValue);

            foreach (var job in enqueuedJobs)
            {
                var jobId = job.Key;

                using (var connection = JobStorage.Current.GetConnection())
                {
                    var storedKey = connection.GetJobParameter(jobId, "detectionRunId");
                    if (storedKey == detectionRunId.ToString())
                    {
                        BackgroundJob.Delete(jobId);
                    }
                }
            }

            var resultGetEntity = await _detectionRunService.GetDetectionRunById(detectionRunId);
            if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())
            {
                return ResultDTO.Fail(resultGetEntity.ErrMsg!);
            }
            if(resultGetEntity.Data == null)
            {
                return ResultDTO.Fail("Detection run not found");
            }
            if(resultGetEntity.Data.Status != nameof(ScheduleRunsStatus.Waiting))
            {
                return ResultDTO.Fail("Can not delete detection run because it is in process or already finished");
            }
                                    
            ResultDTO resultDeleteEntity = await _detectionRunService.DeleteDetectionRun(resultGetEntity.Data);
            if (!resultDeleteEntity.IsSuccess && resultDeleteEntity.HandleError())
            {
                return ResultDTO.Fail(resultDeleteEntity.ErrMsg!);
            }

           
            return ResultDTO.Ok();
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewDetectionRuns))]
        public async Task<ResultDTO<DetectionRunDTO>> GetDetectionRunById(Guid detectionRunId)
        {
            ResultDTO<DetectionRunDTO> resultGetEntity = await _detectionRunService.GetDetectionRunById(detectionRunId);
            if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())
            {
                return ResultDTO<DetectionRunDTO>.Fail(resultGetEntity.ErrMsg!);
            }
            if (resultGetEntity.Data == null)
            {
                return ResultDTO<DetectionRunDTO>.Fail("Detection run not found");

            }
            
            return ResultDTO<DetectionRunDTO>.Ok(resultGetEntity.Data);
        }


        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewDetectedZones))]
        public async Task<ResultDTO<List<DetectionRunDTO>>> ShowDumpSitesOnMap([FromBody] DetectionRunShowOnMapViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return ResultDTO<List<DetectionRunDTO>>.Fail(error);
            }

            if(model.selectedDetectionRunsIds == null || model.selectedDetectionRunsIds.Count == 0)
            {
                return ResultDTO<List<DetectionRunDTO>>.Fail("No detection run selected");
            }

            if (model.selectedConfidenceRates == null || model.selectedConfidenceRates.Count == 0)
            {
                return ResultDTO<List<DetectionRunDTO>>.Fail("No confidence rates selected");
            }

            var selectedDetectionRuns = await _detectionRunService.GetSelectedDetectionRunsIncludingDetectedDumpSites(model.selectedDetectionRunsIds, model.selectedConfidenceRates);
            if (selectedDetectionRuns.IsSuccess == false && selectedDetectionRuns.HandleError())
                return ResultDTO<List<DetectionRunDTO>>.Fail(selectedDetectionRuns.ErrMsg!);
            if (selectedDetectionRuns.Data == null)
                return ResultDTO<List<DetectionRunDTO>>.Fail("Data is null");

            return ResultDTO<List<DetectionRunDTO>>.Ok(selectedDetectionRuns.Data);
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewDetectedZones))]
        public async Task<List<AreaComparisonAvgConfidenceRateReportDTO>> GenerateAreaComparisonAvgConfidenceRateReport(List<Guid> selectedDetectionRunsIds)
        {
            return await _detectionRunService.GenerateAreaComparisonAvgConfidenceRateData(selectedDetectionRunsIds);
        }

        //images
        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.PreviewDetectionInputImages))]
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
        [HasAuthClaim(nameof(SD.AuthClaims.AddDetectionInputImage))]
        [RequestSizeLimit(int.MaxValue)]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<ResultDTO<string>> AddImage(DetectionInputImageViewModel detectionInputImageViewModel, IFormFile? file)
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return ResultDTO<string>.Fail(error);
            }

            var detectionInputImagesFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DetectionInputImagesFolder", "Uploads\\DetectionUploads\\InputImages");
            if (!detectionInputImagesFolder.IsSuccess && detectionInputImagesFolder.HandleError()){
                return ResultDTO<string>.Fail("Can not get the application settings");
            }

            if(string.IsNullOrEmpty(detectionInputImagesFolder.Data))
            {
                return ResultDTO<string>.Fail("Image folder path not found");
            }
            // string saveDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "detection", "input-images");
            string saveDirectory = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, detectionInputImagesFolder.Data);
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);
                string filePath = System.IO.Path.Combine(saveDirectory, fileName);

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
                return ResultDTO<string>.Fail("User id not found");
            }
            var currUserDTO = await _userManagementService.GetUserById(userId);

            if (currUserDTO is null)
            {
                return ResultDTO<string>.Fail("User is not found");
            }
            detectionInputImageViewModel.CreatedById = userId;
            detectionInputImageViewModel.UpdatedById = userId;

            var dto = _mapper.Map<DetectionInputImageDTO>(detectionInputImageViewModel);
            if (dto == null)
            {
                var error = DbResHtml.T("Mapping failed", "Resources");
                return ResultDTO<string>.Fail(error.ToString());
            }

            ResultDTO<DetectionInputImageDTO> resultCreateAndReturnEntity = await _detectionRunService.CreateDetectionInputImage(dto);
            if (!resultCreateAndReturnEntity.IsSuccess && resultCreateAndReturnEntity.HandleError())
            {
                return ResultDTO<string>.Fail(resultCreateAndReturnEntity.ErrMsg!);
            }

            string absGeoTiffPath = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, dto.ImagePath!);
            return ResultDTO<string>.Ok(absGeoTiffPath);
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.AddDetectionInputImage))]
        public async Task<IActionResult> GenerateThumbnail([FromBody] string absGeoTiffPath)
        {
            if (string.IsNullOrEmpty(absGeoTiffPath))
            {
                return Json(new { isSuccess = false, errMsg = "Invalid image path." });
            }

            var appSettingDetectionInputImageThumbnailsFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DetectionInputImageThumbnailsFolder", "Uploads\\DetectionUploads\\InputImageThumbnails");
            if (!appSettingDetectionInputImageThumbnailsFolder.IsSuccess && appSettingDetectionInputImageThumbnailsFolder.HandleError())
            {
                return Json(new { isSuccess = false, errMsg = "Cannot get the application setting for thumbnails folder" });
            }
            if (appSettingDetectionInputImageThumbnailsFolder.Data == null)
            {
                return Json(new { isSuccess = false, errMsg = "Detection input image thumbnails folder value is null" });
            }

            var thumbnailsFolder = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, appSettingDetectionInputImageThumbnailsFolder.Data);
            if (!Directory.Exists(thumbnailsFolder))
            {
                Directory.CreateDirectory(thumbnailsFolder);
            }

            string thumbnailPath = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, appSettingDetectionInputImageThumbnailsFolder.Data, System.IO.Path.GetFileNameWithoutExtension(absGeoTiffPath) + "_thumbnail.jpg");
            try
            {
                var result = await Cli.Wrap("gdal_translate")
                                      .WithArguments($"-of JPEG -outsize 500 0 \"{absGeoTiffPath}\" \"{thumbnailPath}\"")
                                      .WithValidation(CommandResultValidation.None)
                                      .ExecuteBufferedAsync();

                if (result.ExitCode != 0)
                {
                    return Json(new { isSuccess = false, errMsg = $"Error in generating thumbnail. Exit code: {result.ExitCode}, Error: {result.StandardError}" });
                }
                if (!System.IO.File.Exists(thumbnailPath))
                {
                    return Json(new { isSuccess = false, errMsg = "Thumbnail was not created." });
                }

                var xmlFilePath = System.IO.Path.ChangeExtension(thumbnailPath, ".jpg.aux.xml");
                if (System.IO.File.Exists(xmlFilePath))
                {
                    System.IO.File.Delete(xmlFilePath);
                }

                return Json(new { isSuccess = true, thumbnailPath = thumbnailPath });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, errMsg = "An error occurred: " + ex.Message });
            }
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.EditDetectionInputImage))]
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
        [HasAuthClaim(nameof(SD.AuthClaims.DeleteDetectionInputImage))]
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
            string imagePath = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, detectionInputImageViewModel.ImagePath!);
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

            //delete thumbnail
            var appSettingDetectionInputImageThumbnailsFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DetectionInputImageThumbnailsFolder", "Uploads\\DetectionUploads\\InputImageThumbnails");
            if (!appSettingDetectionInputImageThumbnailsFolder.IsSuccess && appSettingDetectionInputImageThumbnailsFolder.HandleError())
            {
                ResultDTO.Fail("Cannot get the application setting for thumbnails folder");                
            }
            if (appSettingDetectionInputImageThumbnailsFolder.Data == null)
            {
                ResultDTO.Fail("Detection input image thumbnails folder value is null");
            }

            string imgFileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(detectionInputImageViewModel.ImageFileName!);
            string thumbnailPath = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, appSettingDetectionInputImageThumbnailsFolder.Data!,imgFileNameWithoutExtension + "_thumbnail.jpg");
            if (System.IO.File.Exists(thumbnailPath))
            {
                System.IO.File.Delete(thumbnailPath);
            }

            return ResultDTO.Ok();
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.PreviewDetectionInputImages))]
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

            var appSettingDetectionInputImageThumbnailsFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DetectionInputImageThumbnailsFolder", "Uploads\\DetectionUploads\\InputImageThumbnails");
            if (!appSettingDetectionInputImageThumbnailsFolder.IsSuccess && appSettingDetectionInputImageThumbnailsFolder.HandleError())
            {
                return ResultDTO<DetectionInputImageDTO>.Fail("Cannot get the application setting for thumbnails folder");
            }
            if (appSettingDetectionInputImageThumbnailsFolder.Data == null)
            {
                return ResultDTO<DetectionInputImageDTO>.Fail("Detection input image thumbnails folder value is null");
            }

            var imageFileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(resultGetEntity.Data.ImageFileName);
            var thumbnailPath = "/" + System.IO.Path.Combine(appSettingDetectionInputImageThumbnailsFolder.Data, imageFileNameWithoutExtension + "_thumbnail.jpg").Replace("\\", "/");
            resultGetEntity.Data.ThumbnailFilePath = thumbnailPath;

            return ResultDTO<DetectionInputImageDTO>.Ok(resultGetEntity.Data);
        }

        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.PreviewDetectionInputImages))]
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
        [HasAuthClaim(nameof(SD.AuthClaims.PreviewDetectionInputImages))]
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

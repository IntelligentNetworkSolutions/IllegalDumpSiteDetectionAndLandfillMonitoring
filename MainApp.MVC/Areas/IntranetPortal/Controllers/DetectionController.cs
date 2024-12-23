using AutoMapper;
using CliWrap;
using CliWrap.Buffered;
using DAL.Interfaces.Helpers;
using DTOs.MainApp.BL;
using DTOs.MainApp.BL.DetectionDTOs;
using DTOs.MainApp.BL.TrainingDTOs;
using DTOs.ObjectDetection.API.Responses.DetectionRun;
using Entities.DetectionEntities;
using Entities.TrainingEntities;
using Hangfire;
using Hangfire.States;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using MainApp.BL.Interfaces.Services.DetectionServices;
using MainApp.BL.Interfaces.Services.TrainingServices;
using MainApp.BL.Services.TrainingServices;
using MainApp.MVC.Filters;
using MainApp.MVC.Helpers;
using MainApp.MVC.ViewModels.IntranetPortal.Detection;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg.OpenPgp;
using SD;
using SD.Enums;
using Services.Interfaces.Services;
using System.Security.Claims;
using System.Text;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    public class DetectionController : Controller
    {
        private readonly IDetectionRunService _detectionRunService;
        private readonly IUserManagementService _userManagementService;
        private readonly IDetectionIgnoreZoneService _detectionIgnoreZoneService;
        private readonly ITrainedModelService _trainedModelService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IAppSettingsAccessor _appSettingsAccessor;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public DetectionController(IUserManagementService userManagementService,
                                    IConfiguration configuration,
                                    IMapper mapper,
                                    IWebHostEnvironment webHostEnvironment,
                                    IAppSettingsAccessor appSettingsAccessor,
                                    IDetectionRunService detectionRunService,
                                    IBackgroundJobClient backgroundJobClient,
                                    IDetectionIgnoreZoneService detectionIgnoreZoneService,
                                    ITrainedModelService trainedModelService)
        {
            _userManagementService = userManagementService;
            _configuration = configuration;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _appSettingsAccessor = appSettingsAccessor;
            _detectionRunService = detectionRunService;
            _detectionIgnoreZoneService = detectionIgnoreZoneService;
            _backgroundJobClient = backgroundJobClient;
            _trainedModelService = trainedModelService;
        }

        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewDetectedZones))]
        public async Task<IActionResult> DetectedZones()
        {
            try
            {
                ResultDTO<List<DetectionRunDTO>>? resultGetDetectionRuns = await _detectionRunService.GetDetectionRunsWithClasses();
                if (resultGetDetectionRuns.IsSuccess == false && resultGetDetectionRuns.HandleError())
                    return HandleErrorRedirect("ErrorViewsPath:Error", 400);
                if (resultGetDetectionRuns.Data == null)
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                List<DetectionRunViewModel> detectionRunsViewModelList = _mapper.Map<List<DetectionRunViewModel>>(resultGetDetectionRuns.Data);
                if (detectionRunsViewModelList == null)
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                detectionRunsViewModelList = detectionRunsViewModelList.Where(x => x.IsCompleted == true && x.Status == "Success").ToList();
                return View(detectionRunsViewModelList);
            }
            catch (Exception)
            {
                return HandleErrorRedirect("ErrorViewsPath:Error", 400);
            }            
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
            try
            {
                if (!ModelState.IsValid)
                {
                    string error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return ResultDTO.Fail(error);
                }

                //get selected image input from database
                ResultDTO<DetectionInputImageDTO>? resultGetInputImage = await _detectionRunService.GetDetectionInputImageById(viewModel.SelectedInputImageId);
                if (resultGetInputImage.IsSuccess == false && resultGetInputImage.HandleError())
                    return ResultDTO.Fail(resultGetInputImage.ErrMsg!);
                if (resultGetInputImage.Data == null)
                    return ResultDTO.Fail("Image not found");

                //find current user
                string? userId = User.FindFirstValue("UserId");
                if (userId is null)
                    return ResultDTO.Fail("User id is null");

                ResultDTO<UserDTO>? resutGetCurrUserDTO = await _userManagementService.GetUserById(userId);
                if (resutGetCurrUserDTO.IsSuccess == false && resutGetCurrUserDTO.HandleError())
                    return ResultDTO.Fail(resutGetCurrUserDTO.ErrMsg!);
                if (resutGetCurrUserDTO.Data is null)
                    return ResultDTO.Fail("User not found");

                string absInputImgPath = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, resultGetInputImage.Data.ImagePath!);
                string inputImgExtension = System.IO.Path.GetExtension(resultGetInputImage.Data.ImageFileName!);

                //get trained model
                ResultDTO<TrainedModelDTO>? getTrainedModelResult = await _trainedModelService.GetTrainedModelById(viewModel.SelectedTrainedModelId);
                if (getTrainedModelResult.IsSuccess == false && getTrainedModelResult.HandleError())
                    return ResultDTO.Fail(getTrainedModelResult.ErrMsg!);
                if (getTrainedModelResult.Data == null)
                    return ResultDTO.Fail("Trained model not found");

                DetectionRunDTO detectionRunDTO = new()
                {
                    Id = Guid.NewGuid(),
                    Name = viewModel.Name,
                    Description = viewModel.Description,

                    DetectionInputImageId = viewModel.SelectedInputImageId,
                    InputImgPath = absInputImgPath,

                    IsCompleted = false,
                    Status = nameof(ScheduleRunsStatus.Waiting),

                    TrainedModelId = getTrainedModelResult.Data.Id,
                    TrainedModel = null,

                    CreatedOn = DateTime.UtcNow,
                    CreatedById = resutGetCurrUserDTO.Data.Id,
                    CreatedBy = resutGetCurrUserDTO.Data,
                };
                // Create Detection Run
                ResultDTO? resultCreate = await _detectionRunService.CreateDetectionRun(detectionRunDTO);
                if (resultCreate.IsSuccess == false && resultCreate.HandleError())
                    return ResultDTO.Fail(resultCreate.ErrMsg!);

                string? jobId = _backgroundJobClient.Enqueue(() => StartDetectionRun(detectionRunDTO));
                if (jobId == null)
                    return ResultDTO.Fail("Failed to enqueue the job, no job id returned");

                using (IStorageConnection connection = JobStorage.Current.GetConnection())
                    connection.SetJobParameter(jobId, "detectionRunId", detectionRunDTO.Id.Value.ToString());

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
            
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.StartDetectionRun))]
        [RequestSizeLimit(int.MaxValue)]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<ResultDTO<string>> StartDetectionRun(DetectionRunDTO detectionRunDTO)
        {
            try
            {
                //STATUS: PROCESSING
                ResultDTO? resultStatusUpdate = await _detectionRunService.UpdateStatus(detectionRunDTO.Id.Value, nameof(ScheduleRunsStatus.Processing));
                if (resultStatusUpdate.IsSuccess == false && resultStatusUpdate.HandleError())
                {
                    ResultDTO<string> resHandleErrorProcess = await HandleErrorProcess(detectionRunDTO.Id.Value, resultStatusUpdate.ErrMsg!);
                    return ResultDTO<string>.Fail(resHandleErrorProcess.Data!);
                }

                ResultDTO<TrainedModelDTO>? getTrainedModelResult = await _trainedModelService.GetTrainedModelById(detectionRunDTO.TrainedModelId.Value);
                if (getTrainedModelResult.IsSuccess == false && getTrainedModelResult.HandleError())
                    return ResultDTO<string>.Fail(getTrainedModelResult.ErrMsg!);

                detectionRunDTO.TrainedModel = getTrainedModelResult.Data;

                // Start Detection Run SYNCHRONIZED WAITS FOR DETECTION RESULTS
                ResultDTO? resultDetection = await _detectionRunService.StartDetectionRun(detectionRunDTO);
                if (resultDetection.IsSuccess == false && resultDetection.HandleError())
                {
                    ResultDTO<string>? resHandleErrorProcess = await HandleErrorProcess(detectionRunDTO.Id.Value, resultDetection.ErrMsg!);
                    return ResultDTO<string>.Fail(resHandleErrorProcess.Data!);
                }

                // Update Is Completed
                ResultDTO? resultIsCompletedUpdate = await _detectionRunService.IsCompleteUpdateDetectionRun(detectionRunDTO);
                if (resultIsCompletedUpdate.IsSuccess == false && resultIsCompletedUpdate.HandleError())
                {
                    ResultDTO<string>? resHandleErrorProcess = await HandleErrorProcess(detectionRunDTO.Id.Value, resultIsCompletedUpdate.ErrMsg!);
                    return ResultDTO<string>.Fail(resHandleErrorProcess.Data!);
                }

                // Get and Check if Detection Run Success Output Files Exist
                ResultDTO<string>? resultGetDetectionResultFiles = await _detectionRunService.GetRawDetectionRunResultPathsByRunId(detectionRunDTO.Id.Value);
                if (resultGetDetectionResultFiles.IsSuccess == false && resultGetDetectionResultFiles.HandleError())
                {
                    ResultDTO<string>? resHandleErrorProcess = await HandleErrorProcess(detectionRunDTO.Id.Value, resultGetDetectionResultFiles.ErrMsg!);
                    return ResultDTO<string>.Fail(resHandleErrorProcess.Data!);
                }

                // Get BBox Prediction Results from JSON as Deserialized DetectionRunFinishedResponse
                ResultDTO<DetectionRunFinishedResponse>? resultBBoxDeserialization = await _detectionRunService.GetBBoxResultsDeserialized(resultGetDetectionResultFiles.Data!);
                if (resultBBoxDeserialization.IsSuccess == false && resultBBoxDeserialization.HandleError())
                {
                    ResultDTO<string> resHandleErrorProcess = await HandleErrorProcess(detectionRunDTO.Id.Value, resultBBoxDeserialization.ErrMsg!);
                    return ResultDTO<string>.Fail(resHandleErrorProcess.Data!);
                }

                //Convert BBoxes to Projection 
                ResultDTO<DetectionRunFinishedResponse>? resultBBoxConversionToProjection = await _detectionRunService.ConvertBBoxResultToImageProjection(detectionRunDTO.InputImgPath, resultBBoxDeserialization.Data!);
                if (resultBBoxConversionToProjection.IsSuccess == false && resultBBoxConversionToProjection.HandleError())
                {
                    ResultDTO<string> resHandleErrorProcess = await HandleErrorProcess(detectionRunDTO.Id.Value, resultBBoxConversionToProjection.ErrMsg!);
                    return ResultDTO<string>.Fail(resHandleErrorProcess.Data!);
                }

                //Create detected dump sites in db
                ResultDTO<List<DetectedDumpSite>>? resultCreateDetectedDumpSites = await _detectionRunService.CreateDetectedDumpsSitesFromDetectionRun(detectionRunDTO.Id.Value, resultBBoxConversionToProjection.Data!);
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
            try
            {
                StringBuilder errMsgBuilder = new();
                errMsgBuilder.AppendLine(inputErrMsg);
                //UPDATE STATUS: ERROR
                ResultDTO? resStatusUpdate = await _detectionRunService.UpdateStatus(detectionRunId, nameof(ScheduleRunsStatus.Error));
                if (resStatusUpdate.IsSuccess == false && resStatusUpdate.HandleError())
                    errMsgBuilder.AppendLine(resStatusUpdate.ErrMsg!);

                //Create error file
                ResultDTO? resultFileCreating = await CreateErrMsgFile(detectionRunId, errMsgBuilder.ToString());
                if (resultFileCreating.IsSuccess == false && resultFileCreating.HandleError())
                    errMsgBuilder.AppendLine(resultFileCreating.ErrMsg!);

                //Mark job as failed
                ResultDTO? resMarkJobAsFailed = await MarkJobAsFailed(errMsgBuilder.ToString(), detectionRunId);
                if (resMarkJobAsFailed.IsSuccess == false && resMarkJobAsFailed.HandleError())
                    errMsgBuilder.AppendLine(resMarkJobAsFailed.ErrMsg!);

                return ResultDTO<string>.Ok(errMsgBuilder.ToString());
            }
            catch (Exception ex)
            {
                return ResultDTO<string>.ExceptionFail(ex.Message, ex);
            }           
        }
        private async Task<ResultDTO> MarkJobAsFailed(string errorMessage, Guid detectionRunId)
        {
            try
            {
                string jobId = string.Empty;
                bool isStateChanged = false;

                //Get the job
                IMonitoringApi? monitoringApi = JobStorage.Current.GetMonitoringApi();
                if (monitoringApi == null)
                    return ResultDTO.Fail("Monitoring Api not found");

                JobList<ProcessingJobDto>? processingJobs = monitoringApi.ProcessingJobs(0, int.MaxValue);
                if (processingJobs == null)
                    return ResultDTO.Fail("Processing jobs not found");

                foreach (var job in processingJobs)
                {
                    using (var connection = JobStorage.Current.GetConnection())
                    {
                        string? storedKey = connection.GetJobParameter(job.Key, "detectionRunId");
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
                    return ResultDTO.Fail("Job is not transferred in failed jobs");

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }
        private async Task<ResultDTO> CreateErrMsgFile(Guid detectionRunId, string errMsg)
        {                  
            try
            {
                ResultDTO<string?>? detectionRunsErrorLogsFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DetectionRunsErrorLogsFolder", "Uploads\\DetectionUploads\\DetectionRunsErrorLogs");
                if (!detectionRunsErrorLogsFolder.IsSuccess && detectionRunsErrorLogsFolder.HandleError())
                    return ResultDTO.Fail("Can not get the application settings");

                if (string.IsNullOrEmpty(detectionRunsErrorLogsFolder.Data))
                    return ResultDTO.Fail("Directory path not found");

                string filePath = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, detectionRunsErrorLogsFolder.Data);
                if (!Directory.Exists(filePath))                
                    Directory.CreateDirectory(filePath);
                
                string fileName = $"{detectionRunId}_errMsg.txt";
                string fullFilePath = System.IO.Path.Combine(filePath, fileName);

                await System.IO.File.WriteAllTextAsync(fullFilePath, errMsg);

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewDetectionRuns))]
        public async Task<ResultDTO<string>> GetDetectionRunErrorLogMessage(Guid detectionRunId)
        {
            try
            {
                ResultDTO<string?>? detectionRunsErrorLogsFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DetectionRunsErrorLogsFolder", "Uploads\\DetectionUploads\\DetectionRunsErrorLogs");
                if (!detectionRunsErrorLogsFolder.IsSuccess && detectionRunsErrorLogsFolder.HandleError())
                    return ResultDTO<string>.Fail("Can not get the application settings");
                if (string.IsNullOrEmpty(detectionRunsErrorLogsFolder.Data))
                    return ResultDTO<string>.Fail("Directory path not found");

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

                    IMonitoringApi? monitoringApi = JobStorage.Current.GetMonitoringApi();
                    if (monitoringApi == null)
                        return ResultDTO<string>.Fail("Monitoring API not found");
                    JobList<FailedJobDto>? failedJobs = monitoringApi.FailedJobs(0, int.MaxValue);
                    if (failedJobs == null)
                        return ResultDTO<string>.Fail("Failed jobs not found");

                    // Check failed jobs first
                    foreach (var job in failedJobs)
                    {
                        using (var connection = JobStorage.Current.GetConnection())
                        {
                            string? storedKey = connection.GetJobParameter(job.Key, "detectionRunId");
                            if (storedKey == detectionRunId.ToString())
                            {
                                jobId = job.Key;
                                break;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(jobId))
                    {
                        JobDetailsDto? jobDetails = monitoringApi.JobDetails(jobId);
                        IDictionary<string, string>? historyData = jobDetails?.History.FirstOrDefault()?.Data;
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
                            return ResultDTO<string>.Ok(errMsg.ToString());

                    }
                    else
                    {
                        // If job is not found in failed jobs, check successful jobs
                        JobList<SucceededJobDto>? succeededJobs = monitoringApi.SucceededJobs(0, int.MaxValue);
                        if (succeededJobs == null)
                            return ResultDTO<string>.Fail("Succeded jobs not found");

                        foreach (var job in succeededJobs)
                        {
                            using (var connection = JobStorage.Current.GetConnection())
                            {
                                string? storedKey = connection.GetJobParameter(job.Key, "detectionRunId");
                                if (storedKey == detectionRunId.ToString())
                                {
                                    jobId = job.Key;
                                    break;
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(jobId))
                        {
                            JobDetailsDto? jobDetails = monitoringApi.JobDetails(jobId);
                            IDictionary<string, string>? historyData = jobDetails?.History.FirstOrDefault()?.Data;
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
            catch (Exception ex)
            {
                return ResultDTO<string>.ExceptionFail(ex.Message, ex);
            }
        }

        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewDetectionRuns))]
        public async Task<ResultDTO<List<DetectionRunViewModel>>> GetAllDetectionRuns()
        {
            try
            {
                ResultDTO<List<DetectionRunDTO>>? resultGetDetectionRuns = await _detectionRunService.GetDetectionRunsWithClasses();
                if (resultGetDetectionRuns.IsSuccess == false && resultGetDetectionRuns.HandleError())
                    return ResultDTO<List<DetectionRunViewModel>>.Fail(resultGetDetectionRuns.ErrMsg!);
                if (resultGetDetectionRuns.Data == null)
                    return ResultDTO<List<DetectionRunViewModel>>.Fail("Detection run list not found");

                List<DetectionRunViewModel> detectionRunsViewModelList = _mapper.Map<List<DetectionRunViewModel>>(resultGetDetectionRuns.Data);
                if (detectionRunsViewModelList == null)
                    return ResultDTO<List<DetectionRunViewModel>>.Fail("Mapping to list of detection run view model failed");

                detectionRunsViewModelList = detectionRunsViewModelList.Where(x => x.IsCompleted == true && x.Status == "Success").ToList();

                return ResultDTO<List<DetectionRunViewModel>>.Ok(detectionRunsViewModelList);
            }
            catch (Exception ex)
            {
                return ResultDTO<List<DetectionRunViewModel>>.ExceptionFail(ex.Message, ex);
            }
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.DeleteDetectionRun))]
        public async Task<ResultDTO> DeleteDetectionRun(Guid detectionRunId)
        {
            try
            {
                if (detectionRunId == Guid.Empty)
                    return ResultDTO.Fail("Invalid detection run id");

                IMonitoringApi monitoringApi = JobStorage.Current.GetMonitoringApi();
                JobList<EnqueuedJobDto> enqueuedJobs = monitoringApi.EnqueuedJobs("default", 0, int.MaxValue);

                foreach (KeyValuePair<string, EnqueuedJobDto> job in enqueuedJobs)
                {
                    string jobId = job.Key;

                    using (IStorageConnection connection = JobStorage.Current.GetConnection())
                    {
                        string storedKey = connection.GetJobParameter(jobId, "detectionRunId");
                        if (storedKey == detectionRunId.ToString())
                            BackgroundJob.Delete(jobId);
                    }
                }

                JobList<ProcessingJobDto>? processingJobs = monitoringApi.ProcessingJobs(0, int.MaxValue);
                if (processingJobs == null)
                    return ResultDTO.Fail("Processing jobs not found");

                foreach (KeyValuePair<string, ProcessingJobDto> job in processingJobs)
                {
                    string jobId = job.Key;
                    using (IStorageConnection connection = JobStorage.Current.GetConnection())
                    {
                        string storedKey = connection.GetJobParameter(jobId, "detectionRunId");
                        if (storedKey == detectionRunId.ToString())
                            return ResultDTO.Fail("Can not delete detection run because it is in process");
                    }
                }

                ResultDTO resultDeleteEntity = await _detectionRunService.DeleteDetectionRun(detectionRunId, _webHostEnvironment.WebRootPath);
                if (!resultDeleteEntity.IsSuccess && resultDeleteEntity.HandleError())
                    return ResultDTO.Fail(resultDeleteEntity.ErrMsg!);

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
           
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewDetectionRuns))]
        public async Task<ResultDTO<DetectionRunDTO>> GetDetectionRunById(Guid detectionRunId)
        {
            try
            {
                ResultDTO<DetectionRunDTO> resultGetEntity = await _detectionRunService.GetDetectionRunById(detectionRunId);
                if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())
                    return ResultDTO<DetectionRunDTO>.Fail(resultGetEntity.ErrMsg!);

                if (resultGetEntity.Data == null)
                    return ResultDTO<DetectionRunDTO>.Fail("Detection run not found");

                return ResultDTO<DetectionRunDTO>.Ok(resultGetEntity.Data);
            }
            catch (Exception ex)
            {
                return ResultDTO<DetectionRunDTO>.ExceptionFail(ex.Message, ex);
            }            
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewDetectedZones))]
        public async Task<ResultDTO<List<DetectionRunDTO>>> ShowDumpSitesOnMap([FromBody] DetectionRunShowOnMapViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    string error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return ResultDTO<List<DetectionRunDTO>>.Fail(error);
                }

                if (model.selectedDetectionRunsIds == null || model.selectedDetectionRunsIds.Count == 0)
                    return ResultDTO<List<DetectionRunDTO>>.Fail("No detection run selected");

                if (model.selectedConfidenceRates == null || model.selectedConfidenceRates.Count == 0)
                    return ResultDTO<List<DetectionRunDTO>>.Fail("No confidence rates selected");

                ResultDTO<List<DetectionRunDTO>> selectedDetectionRuns = await _detectionRunService.GetSelectedDetectionRunsIncludingDetectedDumpSites(model.selectedDetectionRunsIds, model.selectedConfidenceRates);
                if (selectedDetectionRuns.IsSuccess == false && selectedDetectionRuns.HandleError())
                    return ResultDTO<List<DetectionRunDTO>>.Fail(selectedDetectionRuns.ErrMsg!);
                if (selectedDetectionRuns.Data == null)
                    return ResultDTO<List<DetectionRunDTO>>.Fail("Data is null");

                string? baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}";

                foreach (var item in selectedDetectionRuns.Data)
                {
                    item.DetectionInputImage.ImagePath = $"{baseUrl}/{item.DetectionInputImage.ImagePath.Replace("\\", "/")}";
                }

                ResultDTO<List<DetectionIgnoreZoneDTO>> ignoreZones = await _detectionIgnoreZoneService.GetAllIgnoreZonesDTOs();
                if (ignoreZones.IsSuccess == false && ignoreZones.HandleError())
                    return ResultDTO<List<DetectionRunDTO>>.Fail(ignoreZones.ErrMsg!);
                if (ignoreZones.Data == null)
                    return ResultDTO<List<DetectionRunDTO>>.Fail("Ignore zones are null");

                foreach (DetectionRunDTO detectionRun in selectedDetectionRuns.Data)
                {
                    foreach (DetectedDumpSiteDTO dumpSite in detectionRun.DetectedDumpSites!)
                    {
                        if (dumpSite.Geom != null)
                        {
                            dumpSite.IsInsideIgnoreZone = ignoreZones.Data.Any(zone =>
                                zone.Geom != null && zone.Geom.Intersects(dumpSite.Geom));
                        }
                    }
                }

                return ResultDTO<List<DetectionRunDTO>>.Ok(selectedDetectionRuns.Data);
            }
            catch (Exception ex)
            {
                return ResultDTO<List<DetectionRunDTO>>.ExceptionFail(ex.Message, ex);
            }
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewDetectedZones))]
        public async Task<ResultDTO<List<AreaComparisonAvgConfidenceRateReportDTO>>> GenerateAreaComparisonAvgConfidenceRateReport(List<Guid> selectedDetectionRunsIds, int selectedConfidenceRate)
        {
            try
            {
                ResultDTO<List<AreaComparisonAvgConfidenceRateReportDTO>> resultGenerateReport = await _detectionRunService.GenerateAreaComparisonAvgConfidenceRateData(selectedDetectionRunsIds, selectedConfidenceRate);
                if (resultGenerateReport.IsSuccess == false && resultGenerateReport.HandleError())
                    return ResultDTO<List<AreaComparisonAvgConfidenceRateReportDTO>>.Fail(resultGenerateReport.ErrMsg!);
                if (resultGenerateReport.Data == null)
                    return ResultDTO<List<AreaComparisonAvgConfidenceRateReportDTO>>.Fail("Area comparison confidence rate report data not found");

                return ResultDTO<List<AreaComparisonAvgConfidenceRateReportDTO>>.Ok(resultGenerateReport.Data);
            }
            catch (Exception ex)
            {
                return ResultDTO<List<AreaComparisonAvgConfidenceRateReportDTO>>.ExceptionFail(ex.Message, ex);                
            }            
        }

        //images
        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.PreviewDetectionInputImages))]
        public async Task<IActionResult> DetectionInputImages()
        {
            try
            {
                ResultDTO<List<DetectionInputImageDTO>> resultDtoList = await _detectionRunService.GetAllImages();
                if (!resultDtoList.IsSuccess && resultDtoList.HandleError())
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                List<DetectionInputImageViewModel>? vmList = _mapper.Map<List<DetectionInputImageViewModel>>(resultDtoList.Data);
                if (vmList == null)
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                return View(vmList);
            }
            catch (Exception)
            {
                return HandleErrorRedirect("ErrorViewsPath:Error", 400);
            }
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.AddDetectionInputImage))]
        [RequestSizeLimit(int.MaxValue)]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<ResultDTO<string>> AddImage(DetectionInputImageViewModel detectionInputImageViewModel, IFormFile? file)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    string error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return ResultDTO<string>.Fail(error);
                }

                ResultDTO<string?> detectionInputImagesFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DetectionInputImagesFolder", "Uploads\\DetectionUploads\\InputImages");
                if (!detectionInputImagesFolder.IsSuccess && detectionInputImagesFolder.HandleError())
                    return ResultDTO<string>.Fail("Can not get the application settings");

                if (string.IsNullOrEmpty(detectionInputImagesFolder.Data))
                    return ResultDTO<string>.Fail("Image folder path not found");

                // string saveDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "detection", "input-images");
                string saveDirectory = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, detectionInputImagesFolder.Data);
                if (!Directory.Exists(saveDirectory))
                    Directory.CreateDirectory(saveDirectory);

                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);
                    string filePath = System.IO.Path.Combine(saveDirectory, fileName);

                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    detectionInputImageViewModel.ImageFileName = fileName;
                    //detectionInputImageViewModel.ImagePath = string.Format("{0}\\{1}", detectionInputImagesFolder.Data, fileName);
                    detectionInputImageViewModel.ImagePath = System.IO.Path.Combine(detectionInputImagesFolder.Data, fileName);
                }

                string? userId = User.FindFirstValue("UserId");
                if (string.IsNullOrEmpty(userId))
                    return ResultDTO<string>.Fail("User id not found");

                detectionInputImageViewModel.CreatedById = userId;
                detectionInputImageViewModel.UpdatedById = userId;

                DetectionInputImageDTO? dto = _mapper.Map<DetectionInputImageDTO>(detectionInputImageViewModel);
                if (dto == null)
                {
                    HtmlString error = DbResHtml.T("Mapping failed", "Resources");
                    return ResultDTO<string>.Fail(error.ToString());
                }

                ResultDTO<DetectionInputImageDTO>? resultCreateAndReturnEntity = await _detectionRunService.CreateDetectionInputImage(dto);
                if (!resultCreateAndReturnEntity.IsSuccess && resultCreateAndReturnEntity.HandleError())
                    return ResultDTO<string>.Fail(resultCreateAndReturnEntity.ErrMsg!);

                string absGeoTiffPath = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, dto.ImagePath!);
                return ResultDTO<string>.Ok(absGeoTiffPath);
            }
            catch (Exception ex)
            {
                return ResultDTO<string>.ExceptionFail(ex.Message, ex);
            }           
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.AddDetectionInputImage))]
        public async Task<IActionResult> GenerateThumbnail([FromBody] string absGeoTiffPath)
        {
            if (string.IsNullOrEmpty(absGeoTiffPath))
                return Json(new { isSuccess = false, errMsg = "Invalid image path." });

            ResultDTO<string?>? appSettingDetectionInputImageThumbnailsFolder =
                await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DetectionInputImageThumbnailsFolder",
                                                                                    "Uploads\\DetectionUploads\\InputImageThumbnails");
            if (!appSettingDetectionInputImageThumbnailsFolder.IsSuccess && appSettingDetectionInputImageThumbnailsFolder.HandleError())
                return Json(new { isSuccess = false, errMsg = "Cannot get the application setting for thumbnails folder" });

            if (appSettingDetectionInputImageThumbnailsFolder.Data == null)
                return Json(new { isSuccess = false, errMsg = "Detection input image thumbnails folder value is null" });

            string thumbnailsFolder = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, appSettingDetectionInputImageThumbnailsFolder.Data);
            if (Directory.Exists(thumbnailsFolder) == false)
                Directory.CreateDirectory(thumbnailsFolder);

            string? thumbnailPath = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, appSettingDetectionInputImageThumbnailsFolder.Data, System.IO.Path.GetFileNameWithoutExtension(absGeoTiffPath) + "_thumbnail.jpg");
            if (string.IsNullOrWhiteSpace(thumbnailPath))
                return Json(new { isSuccess = false, errMsg = $"Invalid thumbnail path: {thumbnailPath}"});
            if (!thumbnailPath.StartsWith(_webHostEnvironment.WebRootPath, StringComparison.OrdinalIgnoreCase))
                return Json(new { isSuccess = false, errMsg = $"Invalid thumbnail path: {thumbnailPath}"});
            if (!string.Equals(Path.GetExtension(thumbnailPath), ".jpg", StringComparison.OrdinalIgnoreCase))
                return Json(new { isSuccess = false, errMsg = $"Invalid thumbnail extension: {thumbnailPath}" });

            try
            {
                BufferedCommandResult result = await Cli.Wrap("gdal_translate")
                                      .WithArguments($"-of JPEG -outsize 500 0 \"{absGeoTiffPath}\" \"{thumbnailPath}\"")
                                      .WithValidation(CommandResultValidation.None)
                                      .ExecuteBufferedAsync();

                if (result.ExitCode != 0)
                    return Json(new { isSuccess = false, errMsg = $"Error in generating thumbnail. Exit code: {result.ExitCode}, Error: {result.StandardError}" });

                if (!System.IO.File.Exists(thumbnailPath))
                    return Json(new { isSuccess = false, errMsg = "Thumbnail was not created." });

                string? xmlFilePath = System.IO.Path.ChangeExtension(thumbnailPath, ".jpg.aux.xml");
                if (string.IsNullOrWhiteSpace(xmlFilePath))
                    return Json(new { isSuccess = false, errMsg = $"File not found: {xmlFilePath}" });
                if (!xmlFilePath.StartsWith(_webHostEnvironment.WebRootPath, StringComparison.OrdinalIgnoreCase))
                    return Json(new { isSuccess = false, errMsg = $"Wrong file path: {xmlFilePath}" });
                if (!string.Equals(Path.GetExtension(xmlFilePath), ".xml", StringComparison.OrdinalIgnoreCase))
                    return Json(new { isSuccess = false, errMsg = $"Invalid file extension: {xmlFilePath}" });

                if (System.IO.File.Exists(xmlFilePath))
                    System.IO.File.Delete(xmlFilePath);

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
            try
            {
                if (!ModelState.IsValid)
                {
                    string error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return ResultDTO.Fail(error);
                }

                string? id = User.FindFirstValue("UserId");
                if (string.IsNullOrEmpty(id))
                    return ResultDTO.Fail("User id not found");

                ResultDTO<UserDTO>? resultGetAppUser = await _userManagementService.GetUserById(id);
                if (resultGetAppUser.IsSuccess == false && resultGetAppUser.HandleError())
                    return ResultDTO.Fail(resultGetAppUser.ErrMsg!);
                if (resultGetAppUser.Data == null)
                {
                    HtmlString error = DbResHtml.T("User not found", "Resources");
                    return ResultDTO.Fail(error.ToString());
                }

                detectionInputImageViewModel.UpdatedById = resultGetAppUser.Data.Id;
                detectionInputImageViewModel.UpdatedOn = DateTime.UtcNow;

                DetectionInputImageDTO? dto = _mapper.Map<DetectionInputImageDTO>(detectionInputImageViewModel);
                if (dto == null)
                {
                    HtmlString? error = DbResHtml.T("Mapping failed", "Resources");
                    return ResultDTO.Fail(error.ToString());
                }

                ResultDTO? resultEdit = await _detectionRunService.EditDetectionInputImage(dto);
                if (!resultEdit.IsSuccess && resultEdit.HandleError())
                    return ResultDTO.Fail(resultEdit.ErrMsg!);

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.DeleteDetectionInputImage))]
        public async Task<ResultDTO> DeleteDetectionImageInput(DetectionInputImageViewModel detectionInputImageViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    string? error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return ResultDTO.Fail(error);
                }

                string? imgExtension = Path.GetExtension(detectionInputImageViewModel.ImageFileName);
                if (!string.Equals(imgExtension, ".tif", StringComparison.OrdinalIgnoreCase))
                    return ResultDTO.Fail($"Invalid image extension {imgExtension}");

                ResultDTO<List<DetectionRunDTO>>? resultCheckForFiles =
                    await _detectionRunService.GetDetectionInputImageByDetectionRunId(detectionInputImageViewModel.Id);
                if (!resultCheckForFiles.IsSuccess && resultCheckForFiles.HandleError())
                    return ResultDTO.Fail(resultCheckForFiles.ErrMsg!);

                if (resultCheckForFiles.Data == null)
                    return ResultDTO.Fail("Data is null");

                if (resultCheckForFiles.Data.Count > 0)
                {
                    HtmlString error = DbResHtml.T("This image is still used in the detection run. Delete the detection run first!", "Resources");
                    return ResultDTO.Fail(error.ToString());
                }

                DetectionInputImageDTO? dto = _mapper.Map<DetectionInputImageDTO>(detectionInputImageViewModel);
                if (dto == null)
                {
                    HtmlString error = DbResHtml.T("Mapping failed", "Resources");
                    return ResultDTO.Fail(error.ToString());
                }

                ResultDTO? resultDelete = await _detectionRunService.DeleteDetectionInputImage(dto);
                if (!resultDelete.IsSuccess && resultDelete.HandleError())
                    return ResultDTO.Fail(resultDelete.ErrMsg!);

                //string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", detectionInputImageViewModel.ImagePath ?? string.Empty);
                if (string.IsNullOrWhiteSpace(detectionInputImageViewModel.ImagePath))
                    return ResultDTO.Fail($"Invalid file path provided: {detectionInputImageViewModel.ImagePath}");
                string? imagePath = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, detectionInputImageViewModel.ImagePath);
                if (string.IsNullOrWhiteSpace(imagePath))
                    return ResultDTO.Fail($"Invalid file path provided: {imagePath}");
                if (!imagePath.StartsWith(_webHostEnvironment.WebRootPath, StringComparison.OrdinalIgnoreCase))
                    return ResultDTO.Fail($"Invalid file path provided: {imagePath}");
                if (!string.Equals(Path.GetExtension(imagePath), ".tif", StringComparison.OrdinalIgnoreCase))
                    return ResultDTO.Fail($"Invalid file extension {imagePath}");

                if (System.IO.File.Exists(imagePath))
                {
                    try
                    {
                        System.IO.File.Delete(imagePath);
                    }
                    catch (Exception ex)
                    {
                        HtmlString fileDeleteError = DbResHtml.T("Failed to delete the image file", "Resources");
                        return ResultDTO.Fail($"{fileDeleteError}: {ex.Message}");
                    }
                }

                //delete thumbnail
                ResultDTO<string?>? appSettingDetectionInputImageThumbnailsFolder =
                    await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DetectionInputImageThumbnailsFolder","Uploads\\DetectionUploads\\InputImageThumbnails");
                if (!appSettingDetectionInputImageThumbnailsFolder.IsSuccess && appSettingDetectionInputImageThumbnailsFolder.HandleError())
                    return ResultDTO.Fail("Cannot get the application setting for thumbnails folder");
                if (appSettingDetectionInputImageThumbnailsFolder.Data == null)
                    return ResultDTO.Fail("Detection input image thumbnails folder value is null");

                string? imgFileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(detectionInputImageViewModel.ImageFileName!);
                if (string.IsNullOrWhiteSpace(imgFileNameWithoutExtension))
                    return ResultDTO.Fail($"Invalid image name provided: {imgFileNameWithoutExtension}");
                               
                string? thumbnailPath = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, appSettingDetectionInputImageThumbnailsFolder.Data, imgFileNameWithoutExtension + "_thumbnail.jpg");
                if (string.IsNullOrWhiteSpace(thumbnailPath))
                    return ResultDTO.Fail($"Invalid thumbnail path provided: {thumbnailPath}");
                if (!thumbnailPath.StartsWith(_webHostEnvironment.WebRootPath, StringComparison.OrdinalIgnoreCase))
                    return ResultDTO.Fail($"Invalid thumbnail path provided: {thumbnailPath}");
                if (!string.Equals(Path.GetExtension(thumbnailPath), ".jpg", StringComparison.OrdinalIgnoreCase))
                    return ResultDTO.Fail($"Invalid image extension {thumbnailPath}");


                if (System.IO.File.Exists(thumbnailPath))
                    System.IO.File.Delete(thumbnailPath);

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.PreviewDetectionInputImages))]
        public async Task<ResultDTO<DetectionInputImageDTO>> GetDetectionInputImageById(Guid detectionInputImageId)
        {
            try
            {
                ResultDTO<DetectionInputImageDTO>? resultGetEntity = await _detectionRunService.GetDetectionInputImageById(detectionInputImageId);
                if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())
                    return ResultDTO<DetectionInputImageDTO>.Fail(resultGetEntity.ErrMsg!);
                if (resultGetEntity.Data == null)
                    return ResultDTO<DetectionInputImageDTO>.Fail("Image Input is null");

                ResultDTO<string?>? appSettingDetectionInputImageThumbnailsFolder =
                    await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DetectionInputImageThumbnailsFolder",
                                                                                        "Uploads\\DetectionUploads\\InputImageThumbnails");
                if (!appSettingDetectionInputImageThumbnailsFolder.IsSuccess && appSettingDetectionInputImageThumbnailsFolder.HandleError())
                    return ResultDTO<DetectionInputImageDTO>.Fail("Cannot get the application setting for thumbnails folder");
                if (appSettingDetectionInputImageThumbnailsFolder.Data == null)
                    return ResultDTO<DetectionInputImageDTO>.Fail("Detection input image thumbnails folder value is null");

                string? imageFileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(resultGetEntity.Data.ImageFileName);
                if (string.IsNullOrEmpty(imageFileNameWithoutExtension))
                    return ResultDTO<DetectionInputImageDTO>.Fail("Image file name not found");

                string? baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}";
                if (string.IsNullOrEmpty(baseUrl))
                    return ResultDTO<DetectionInputImageDTO>.Fail("Base url not found");

                string? thumbnailPath = System.IO.Path.Combine("/", appSettingDetectionInputImageThumbnailsFolder.Data, imageFileNameWithoutExtension + "_thumbnail.jpg").Replace("\\", "/");
                if (string.IsNullOrEmpty(thumbnailPath))
                    return ResultDTO<DetectionInputImageDTO>.Fail("Thumbnail path not found");

                thumbnailPath = $"{baseUrl}/{thumbnailPath}";
                resultGetEntity.Data.ThumbnailFilePath = thumbnailPath;
                return ResultDTO<DetectionInputImageDTO>.Ok(resultGetEntity.Data);
            }
            catch (Exception ex)
            {
                return ResultDTO<DetectionInputImageDTO>.ExceptionFail(ex.Message, ex);
            }
        }

        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.PreviewDetectionInputImages))]
        public async Task<ResultDTO<List<DetectionInputImageDTO>>> GetAllDetectionInputImages()
        {
            try
            {
                ResultDTO<List<DetectionInputImageDTO>>? resultGetEntities = await _detectionRunService.GetAllImages();
                if (!resultGetEntities.IsSuccess && resultGetEntities.HandleError())
                    return ResultDTO<List<DetectionInputImageDTO>>.Fail(resultGetEntities.ErrMsg!);
                if (resultGetEntities.Data == null)
                    return ResultDTO<List<DetectionInputImageDTO>>.Fail("Detection input images are not found");

                return ResultDTO<List<DetectionInputImageDTO>>.Ok(resultGetEntities.Data);
            }
            catch (Exception ex)
            {
                return ResultDTO<List<DetectionInputImageDTO>>.ExceptionFail(ex.Message, ex);
            }
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.PreviewDetectionInputImages))]
        public async Task<ResultDTO<List<DetectionInputImageDTO>>> GetSelectedDetectionInputImages(List<Guid> selectedImagesIds)
        {
            try
            {
                ResultDTO<List<DetectionInputImageDTO>>? resultGetEntities = await _detectionRunService.GetSelectedInputImagesById(selectedImagesIds);
                if (!resultGetEntities.IsSuccess && resultGetEntities.HandleError())
                    return ResultDTO<List<DetectionInputImageDTO>>.Fail(resultGetEntities.ErrMsg!);
                if (resultGetEntities.Data == null)
                    return ResultDTO<List<DetectionInputImageDTO>>.Fail("Detection input images are not found");

                string? baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}";
                if (string.IsNullOrEmpty(baseUrl))
                    return ResultDTO<List<DetectionInputImageDTO>>.Fail("Base url not found");

                foreach (var item in resultGetEntities.Data)
                {
                    item.ImagePath = $"{baseUrl}/{item.ImagePath?.Replace("\\", "/")}";
                }
                return ResultDTO<List<DetectionInputImageDTO>>.Ok(resultGetEntities.Data);
            }
            catch (Exception ex)
            {
                return ResultDTO<List<DetectionInputImageDTO>>.ExceptionFail(ex.Message, ex);
            }
        }

        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.PreviewDetectionInputImages))]
        public async Task<ResultDTO<List<TrainedModelDTO>>> GetPublishedTrainedModels()
        {
            try
            {
                ResultDTO<List<TrainedModelDTO>>? getPublishedTrainedModelsResult =
                    await _trainedModelService.GetPublishedTrainedModelsIncludingTrainRuns();
                if (getPublishedTrainedModelsResult.IsSuccess == false && getPublishedTrainedModelsResult.HandleError())
                    ResultDTO<List<TrainedModelDTO>>.Fail(getPublishedTrainedModelsResult.ErrMsg!);
                if (getPublishedTrainedModelsResult.Data == null)
                    return ResultDTO<List<TrainedModelDTO>>.Fail("Trained model list not found");

                return ResultDTO<List<TrainedModelDTO>>.Ok(getPublishedTrainedModelsResult.Data!);
            }
            catch (Exception ex)
            {
                return ResultDTO<List<TrainedModelDTO>>.ExceptionFail(ex.Message, ex);
            }
        }

        private IActionResult HandleErrorRedirect(string configKey, int statusCode)
        {
            string? errorPath = _configuration[configKey];
            if (string.IsNullOrEmpty(errorPath))
            {
                return statusCode switch
                {
                    404 => NotFound(),
                    403 => Forbid(),
                    405 => StatusCode(405),
                    _ => BadRequest()
                };
            }
            return Redirect(errorPath);
        }
    }
}

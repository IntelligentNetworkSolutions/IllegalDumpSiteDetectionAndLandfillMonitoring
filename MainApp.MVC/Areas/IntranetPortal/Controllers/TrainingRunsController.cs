using AutoMapper;
using DAL.Interfaces.Helpers;
using DTOs.MainApp.BL;
using DTOs.MainApp.BL.DatasetDTOs;
using DTOs.MainApp.BL.TrainingDTOs;
using Hangfire;
using Hangfire.States;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using MainApp.BL.Interfaces.Services;
using MainApp.BL.Interfaces.Services.DatasetServices;
using MainApp.BL.Interfaces.Services.TrainingServices;
using MainApp.MVC.Filters;
using MainApp.MVC.ViewModels.IntranetPortal.Training;
using Microsoft.AspNetCore.Mvc;
using SD;
using SD.Enums;
using Services.Interfaces.Services;
using System.Security.Claims;
using System.Text;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    public class TrainingRunsController : Controller
    {
        private readonly ITrainingRunService _trainingRunService;
        private readonly ITrainingRunTrainParamsService _trainingRunTrainParamsService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IUserManagementService _userManagementService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IAppSettingsAccessor _appSettingsAccessor;
        private readonly IDatasetService _datasetService;
        protected readonly IMMDetectionConfigurationService _MMDetectionConfiguration;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;


        public TrainingRunsController(ITrainingRunService trainingRunService,
                                      ITrainingRunTrainParamsService trainingRunTrainParamsService,
                                      IWebHostEnvironment webHostEnvironment,
                                      IUserManagementService userManagementService,
                                      IBackgroundJobClient backgroundJobClient,
                                      IAppSettingsAccessor appSettingsAccessor,
                                      IDatasetService datasetService,
                                      IMMDetectionConfigurationService MMDetectionConfiguration,
                                      IConfiguration configuration,
                                      IMapper mapper)
        {
            _trainingRunService = trainingRunService;
            _trainingRunTrainParamsService = trainingRunTrainParamsService;
            _webHostEnvironment = webHostEnvironment;
            _userManagementService = userManagementService;
            _backgroundJobClient = backgroundJobClient;
            _appSettingsAccessor = appSettingsAccessor;
            _datasetService = datasetService;
            _MMDetectionConfiguration = MMDetectionConfiguration;
            _configuration = configuration;
            _mapper = mapper;
        }

        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewTrainingRuns))]
        public async Task<IActionResult> Index()
        {
            var resultDtoList = await _trainingRunService.GetAllTrainingRuns();
            if (!resultDtoList.IsSuccess && resultDtoList.HandleError())
            {
                var errorPath = _configuration["ErrorViewsPath:Error"];
                if (errorPath == null)
                {
                    return BadRequest();
                }
                return Redirect(errorPath);
            }
            if (resultDtoList.Data == null)
            {
                var errorPath = _configuration["ErrorViewsPath:Error404"];
                if (errorPath == null)
                {
                    return NotFound();
                }
                return Redirect(errorPath);
            }

            var vmList = _mapper.Map<List<TrainingRunIndexViewModel>>(resultDtoList.Data);
            if (vmList == null)
            {
                var errorPath = _configuration["ErrorViewsPath:Error404"];
                if (errorPath == null)
                {
                    return NotFound();
                }
                return Redirect(errorPath);
            }

            return View(vmList);
        }

        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.CreateTrainingRun))]
        public async Task<IActionResult> CreateTrainingRun()
        {
            return View();
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.EditTrainingRun))]
        public async Task<ResultDTO> EditTrainingRun(TrainingRunIndexViewModel trainingRunIndexViewModel)
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return ResultDTO.Fail(error);
            }

            ResultDTO resultEdit = await _trainingRunService.UpdateTrainingRunEntity(trainingRunIndexViewModel.Id, isCompleted: trainingRunIndexViewModel.IsCompleted, name: trainingRunIndexViewModel.Name);

            if (!resultEdit.IsSuccess && resultEdit.HandleError())
            {
                return ResultDTO.Fail(resultEdit.ErrMsg!);
            }

            return ResultDTO.Ok();

        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.ScheduleTrainingRun))]
        public async Task<ResultDTO> ScheduleTrainingRun(TrainingRunViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return ResultDTO.Fail(error);
            }

            string? userId = User.FindFirstValue("UserId");
            if (userId is null)
                return ResultDTO.Fail("User id is null");

            UserDTO? currUserDTO = await _userManagementService.GetUserById(userId);
            if (currUserDTO is null)
                return ResultDTO.Fail("User not found");

            TrainingRunDTO trainingRunDTO = new()
            {
                Id = Guid.NewGuid(),
                Name = viewModel.Name,
                CreatedById = currUserDTO.Id,
                DatasetId = viewModel.DatasetId,
                TrainedModelId = viewModel.TrainedModelId,
                Status = nameof(ScheduleRunsStatus.Waiting),
                TrainParamsId = Guid.NewGuid()
            };
                 
            ResultDTO<TrainingRunDTO> resultScheduleTrainingRun = await _trainingRunService.CreateTrainingRunWithBaseModel(trainingRunDTO);
            if (resultScheduleTrainingRun.IsSuccess == false && resultScheduleTrainingRun.HandleError())
                return ResultDTO.Fail(resultScheduleTrainingRun.ErrMsg!);
            if (resultScheduleTrainingRun.Data == null)
                return ResultDTO.Fail("Failed to crete the training run");

            ResultDTO<TrainingRunTrainParamsDTO> resultCreateParams = await _trainingRunTrainParamsService.CreateTrainingRunTrainParams(viewModel.NumEpochs, viewModel.BatchSize, viewModel.NumFrozenStages, trainingRunDTO.Id.Value, trainingRunDTO.TrainParamsId.Value);
            if (resultCreateParams.IsSuccess == false && resultCreateParams.HandleError())
                return ResultDTO.Fail(resultCreateParams.ErrMsg!);
            if (resultCreateParams.Data == null)
                return ResultDTO.Fail("Failed to crete the training run train params");

            string jobId = _backgroundJobClient.Enqueue(() => ExecuteTrainingRunProcess(resultScheduleTrainingRun.Data, resultCreateParams.Data));
            using (IStorageConnection? connection = JobStorage.Current.GetConnection())
            {
                connection.SetJobParameter(jobId, "trainingRunId", trainingRunDTO.Id.Value.ToString());
            }

            return ResultDTO.Ok();
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.ScheduleTrainingRun))]
        public async Task<ResultDTO> ExecuteTrainingRunProcess(TrainingRunDTO trainingRunDTO, TrainingRunTrainParamsDTO trainingRunTrainParamsDTO)
        {
            try
            {
                //int numEpochs = 1;
                //int numBatchSize = 1;
                //int numFrozenStages = 4;

                // Update Training Run only status PROCESSING
                ResultDTO updateTrainRunResult = await _trainingRunService.UpdateTrainingRunEntity(trainingRunDTO.Id!.Value, null, nameof(ScheduleRunsStatus.Processing), isCompleted: false);
                if (updateTrainRunResult.IsSuccess == false && updateTrainRunResult.HandleError())
                {
                    ResultDTO<string> resHandleErrorProcess = await HandleErrorProcess(trainingRunDTO.Id.Value, updateTrainRunResult.ErrMsg!);
                    return ResultDTO.Fail(resHandleErrorProcess.Data!);
                }

                //get dataset
                ResultDTO<DatasetDTO> resultDatasetIncludeThenAll = await _datasetService.GetDatasetDTOFullyIncluded(trainingRunDTO.DatasetId!.Value, track: false);
                if (resultDatasetIncludeThenAll.IsSuccess == false && resultDatasetIncludeThenAll.HandleError())
                {
                    ResultDTO<string> resHandleErrorProcess = await HandleErrorProcess(trainingRunDTO.Id.Value, resultDatasetIncludeThenAll.ErrMsg!);
                    return ResultDTO.Fail(resHandleErrorProcess.Data!);
                }

                // Export Dataset
                string datasetExportAbsPathForTrainingRunId = _MMDetectionConfiguration.GetTrainingRunDatasetDirAbsPath(trainingRunDTO.Id.Value);
                ResultDTO<string> exportDatasetResult = await _datasetService.ExportDatasetAsCOCOFormat(trainingRunDTO.DatasetId.Value, "AllImages", datasetExportAbsPathForTrainingRunId, true);
                if (exportDatasetResult.IsSuccess == false && exportDatasetResult.HandleError())
                {
                    ResultDTO<string> resHandleErrorProcess = await HandleErrorProcess(trainingRunDTO.Id.Value, exportDatasetResult.ErrMsg!);
                    return ResultDTO.Fail(resHandleErrorProcess.Data!);
                }

                // Generate Training Config
                ResultDTO<string> generateTrainRunConfigResult = await _trainingRunService.GenerateTrainingRunConfigFile(trainingRunDTO.Id.Value, resultDatasetIncludeThenAll.Data!, trainingRunDTO.BaseModel!, trainingRunTrainParamsDTO.NumEpochs, trainingRunTrainParamsDTO.NumFrozenStages, trainingRunTrainParamsDTO.BatchSize);
                if (generateTrainRunConfigResult.IsSuccess == false && generateTrainRunConfigResult.HandleError())
                {
                    ResultDTO<string> resHandleErrorProcess = await HandleErrorProcess(trainingRunDTO.Id.Value, generateTrainRunConfigResult.ErrMsg!);
                    return ResultDTO.Fail(resHandleErrorProcess.Data!);
                }

                //Start training run
                ResultDTO startTrainRunResult = await _trainingRunService.StartTrainingRun(trainingRunDTO.Id.Value);
                if (startTrainRunResult.IsSuccess == false && startTrainRunResult.HandleError())
                {
                    ResultDTO<string> resHandleErrorProcess = await HandleErrorProcess(trainingRunDTO.Id.Value, startTrainRunResult.ErrMsg!);
                    return ResultDTO.Fail(resHandleErrorProcess.Data!);
                }

                // Create Trained Model
                ResultDTO<Guid> createTrainedModelResultReturnId = await _trainingRunService.CreateTrainedModelByTrainingRunId(trainingRunDTO.Id!.Value);
                if (createTrainedModelResultReturnId.IsSuccess == false && createTrainedModelResultReturnId.HandleError())
                    return ResultDTO.Fail(createTrainedModelResultReturnId.ErrMsg!);

                // Update Training Run SUCCESS, TRAINED MODEL ID, ISCOMPLETED
                ResultDTO updateTrainRunResultSuccess = await _trainingRunService.UpdateTrainingRunEntity(trainingRunDTO.Id!.Value, createTrainedModelResultReturnId.Data!, nameof(ScheduleRunsStatus.Success), isCompleted: true);
                if (updateTrainRunResultSuccess.IsSuccess == false && updateTrainRunResultSuccess.HandleError())
                    return ResultDTO.Fail(updateTrainRunResultSuccess.ErrMsg!);

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
            finally
            {
                // TODO: Clean Up Training Run Files, Later
            }
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewTrainingRuns))]
        public async Task<ResultDTO<string>> GetTrainingRunErrorLogMessage(Guid trainingRunId)
        {
            if (trainingRunId == Guid.Empty)
            {
                return ResultDTO<string>.Fail("Invalid training run id");
            }

            ResultDTO<string?>? trainingRunsErrorLogsFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("TrainingRunsErrorLogsFolder", "Uploads\\TrainingUploads\\TrainingRunsErrorLogs");
            if (!trainingRunsErrorLogsFolder.IsSuccess && trainingRunsErrorLogsFolder.HandleError())
            {
                return ResultDTO<string>.Fail("Can not get the application settings");
            }

            if (string.IsNullOrEmpty(trainingRunsErrorLogsFolder.Data))
            {
                return ResultDTO<string>.Fail("Directory path not found");
            }

            string? filePath = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, trainingRunsErrorLogsFolder.Data);
            string? fileName = $"{trainingRunId}_errMsg.txt";
            string? fullFilePath = System.IO.Path.Combine(filePath, fileName);

            // If file exists, read the error message from the txt file
            if (System.IO.File.Exists(fullFilePath))
            {
                string? fileContent = await System.IO.File.ReadAllTextAsync(fullFilePath);
                return ResultDTO<string>.Ok(fileContent);
            }
            else
            {
                string jobId = string.Empty;
                StringBuilder errMsg = new();

                IMonitoringApi monitoringApi = JobStorage.Current.GetMonitoringApi();
                JobList<FailedJobDto> failedJobs = monitoringApi.FailedJobs(0, int.MaxValue);

                // Check failed jobs first
                foreach (var job in failedJobs)
                {
                    using (var connection = JobStorage.Current.GetConnection())
                    {
                        string storedKey = connection.GetJobParameter(job.Key, "trainingRunId");
                        if (storedKey == trainingRunId.ToString())
                        {
                            jobId = job.Key;
                            break;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(jobId))
                {
                    JobDetailsDto? jobDetails = monitoringApi.JobDetails(jobId);
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
                    JobList<SucceededJobDto> succeededJobs = monitoringApi.SucceededJobs(0, int.MaxValue);
                    foreach (var job in succeededJobs)
                    {
                        using (var connection = JobStorage.Current.GetConnection())
                        {
                            string storedKey = connection.GetJobParameter(job.Key, "trainingRunId");
                            if (storedKey == trainingRunId.ToString())
                            {
                                jobId = job.Key;
                                break;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(jobId))
                    {
                        JobDetailsDto? jobDetails = monitoringApi.JobDetails(jobId);
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

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewTrainingRuns))]
        public async Task<ResultDTO<TrainingRunDTO>> GetTrainingRunById(Guid trainingRunId)
        {
            ResultDTO<TrainingRunDTO> resultGetEntity = await _trainingRunService.GetTrainingRunById(trainingRunId);
            if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())
                return ResultDTO<TrainingRunDTO>.Fail(resultGetEntity.ErrMsg!);

            if (resultGetEntity.Data == null)
                return ResultDTO<TrainingRunDTO>.Fail("Training run not found");

            return ResultDTO<TrainingRunDTO>.Ok(resultGetEntity.Data);
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.DeleteTrainingRun))]
        public async Task<ResultDTO> DeleteTrainingRun(Guid trainingRunId)
        {
            if (trainingRunId == Guid.Empty)
                return ResultDTO.Fail("Invalid training run id");

            IMonitoringApi monitoringApi = JobStorage.Current.GetMonitoringApi();
            JobList<EnqueuedJobDto> enqueuedJobs = monitoringApi.EnqueuedJobs("default", 0, int.MaxValue);

            foreach (KeyValuePair<string, EnqueuedJobDto> job in enqueuedJobs)
            {
                string jobId = job.Key;

                using (IStorageConnection connection = JobStorage.Current.GetConnection())
                {
                    string storedKey = connection.GetJobParameter(jobId, "trainingRunId");
                    if (storedKey == trainingRunId.ToString())
                        BackgroundJob.Delete(jobId);
                }
            }

            ResultDTO resultDeleteEntity = await _trainingRunService.DeleteTrainingRun(trainingRunId, _webHostEnvironment.WebRootPath);
            if (!resultDeleteEntity.IsSuccess && resultDeleteEntity.HandleError())
                return ResultDTO.Fail(resultDeleteEntity.ErrMsg!);

            return ResultDTO.Ok();
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.PublishTrainingRunTrainedModel))]
        public async Task<ResultDTO> PublishTrainingRunTrainedModel(Guid trainingRunId)
        {
            if (trainingRunId == Guid.Empty)
                return ResultDTO.Fail("Invalid training run id");

            ResultDTO? resultPublisheTrainedModel = await _trainingRunService.PublishTrainingRunTrainedModel(trainingRunId);
            if (resultPublisheTrainedModel.IsSuccess == false && resultPublisheTrainedModel.HandleError())
                return ResultDTO.Fail(resultPublisheTrainedModel.ErrMsg!);

            return ResultDTO.Ok();
        }

        private async Task<ResultDTO<string>> HandleErrorProcess(Guid trainingRunId, string inputErrMsg)
        {
            StringBuilder errMsgBuilder = new();
            errMsgBuilder.AppendLine(inputErrMsg);
            //UPDATE STATUS: ERROR
            ResultDTO resStatusUpdate = await _trainingRunService.UpdateTrainingRunEntity(trainingRunId, null, nameof(ScheduleRunsStatus.Error), isCompleted:false);
            if (resStatusUpdate.IsSuccess == false && resStatusUpdate.HandleError())
                errMsgBuilder.AppendLine(resStatusUpdate.ErrMsg!);

            //Create error file
            ResultDTO resultFileCreating = await CreateErrMsgFile(trainingRunId, errMsgBuilder.ToString());
            if (resultFileCreating.IsSuccess == false && resultFileCreating.HandleError())
                errMsgBuilder.AppendLine(resultFileCreating.ErrMsg!);

            //Mark job as failed
            ResultDTO resMarkJobAsFailed = await MarkJobAsFailed(errMsgBuilder.ToString(), trainingRunId);
            if (resMarkJobAsFailed.IsSuccess == false && resMarkJobAsFailed.HandleError())
                errMsgBuilder.AppendLine(resMarkJobAsFailed.ErrMsg!);

            return ResultDTO<string>.Ok(errMsgBuilder.ToString());
        }

        private async Task<ResultDTO> CreateErrMsgFile(Guid trainingRunId, string errMsg)
        {
            ResultDTO<string?>? trainingRunsErrorLogsFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("TrainingRunsErrorLogsFolder", "Uploads\\TrainingUploads\\TrainingRunsErrorLogs");
            if (!trainingRunsErrorLogsFolder.IsSuccess && trainingRunsErrorLogsFolder.HandleError())
            {
                return ResultDTO.Fail("Can not get the application settings");
            }

            if (string.IsNullOrEmpty(trainingRunsErrorLogsFolder.Data))
            {
                return ResultDTO.Fail("Directory path not found");
            }

            try
            {
                string? filePath = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, trainingRunsErrorLogsFolder.Data);
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                string fileName = $"{trainingRunId}_errMsg.txt";
                string fullFilePath = System.IO.Path.Combine(filePath, fileName);

                await System.IO.File.WriteAllTextAsync(fullFilePath, errMsg);

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                return ResultDTO.Fail($"Failed to create the file: {ex.Message}");
            }
        }

        private async Task<ResultDTO> MarkJobAsFailed(string errorMessage, Guid trainingRunId)
        {
            string jobId = string.Empty;
            bool isStateChanged = false;

            //Get the job
            IMonitoringApi monitoringApi = JobStorage.Current.GetMonitoringApi();
            JobList<ProcessingJobDto> processingJobs = monitoringApi.ProcessingJobs(0, int.MaxValue);

            foreach (KeyValuePair<string, ProcessingJobDto> job in processingJobs)
            {
                using (IStorageConnection connection = JobStorage.Current.GetConnection())
                {
                    string storedKey = connection.GetJobParameter(job.Key, "trainingRunId");
                    if (storedKey == trainingRunId.ToString())
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

    }


}

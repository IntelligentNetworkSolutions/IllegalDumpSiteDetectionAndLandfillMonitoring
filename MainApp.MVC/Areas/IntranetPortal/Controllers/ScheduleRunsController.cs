using AutoMapper;
using DAL.Interfaces.Helpers;
using DTOs.MainApp.BL.DetectionDTOs;
using DTOs.MainApp.BL.TrainingDTOs;
using Entities.DetectionEntities;
using Entities.TrainingEntities;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using MainApp.BL.Interfaces.Services.DetectionServices;
using MainApp.BL.Interfaces.Services.TrainingServices;
using MainApp.MVC.Filters;
using MainApp.MVC.ViewModels.IntranetPortal.ScheduleRuns;
using Microsoft.AspNetCore.Mvc;
using SD;
using SD.Enums;
using Services.Interfaces.Services;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    public class ScheduleRunsController : Controller
    {
        private readonly IDetectionRunService _detectionRunService;
        private readonly ITrainingRunService _trainingRunService;
        private readonly IConfiguration _configuration;

        public ScheduleRunsController(IDetectionRunService detectionRunService, ITrainingRunService trainingRunService, IConfiguration configuration)
        {
            _detectionRunService = detectionRunService;
            _trainingRunService = trainingRunService;
            _configuration = configuration;
        }

        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.ManageScheduledRuns))]
        public async Task<IActionResult> ViewScheduledRuns()
        {
            try
            {
                ResultDTO<List<DetectionRunDTO>> resultGetDetectionRuns = await _detectionRunService.GetAllDetectionRuns();
                if (!resultGetDetectionRuns.IsSuccess && resultGetDetectionRuns.HandleError())
                {
                    return HandleErrorRedirect("ErrorViewsPath:Error", 400);
                }
                if (resultGetDetectionRuns.Data == null)
                {
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);
                }

                ResultDTO<List<TrainingRunDTO>> resultGetTrainingRuns = await _trainingRunService.GetAllTrainingRuns();
                if (!resultGetTrainingRuns.IsSuccess && resultGetTrainingRuns.HandleError())
                {
                    return HandleErrorRedirect("ErrorViewsPath:Error", 400);
                }
                if (resultGetTrainingRuns.Data == null)
                {
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);
                }

                //check if the detection run status is stuck in PROCESSING status for too long, change status to error or success
                bool hasProcessingStatus = resultGetDetectionRuns.Data.Any(d => d.Status == nameof(ScheduleRunsStatus.Processing));
                if (hasProcessingStatus)
                {
                    List<DetectionRunDTO> allProcessingRuns = resultGetDetectionRuns.Data.Where(x => x.Status.Equals(nameof(ScheduleRunsStatus.Processing))).ToList();

                    var monitoringApi = JobStorage.Current.GetMonitoringApi();
                    var processingJobs = monitoringApi.ProcessingJobs(0, int.MaxValue);
                    var succeededJobs = monitoringApi.SucceededJobs(0, int.MaxValue);
                    var failedJobs = monitoringApi.FailedJobs(0, int.MaxValue);

                    //Get jobs parameter detectionRunId 
                    List<string> processingJobsDetectionRunIds = GetProcessingJobDetectionRunIds(processingJobs);

                    foreach (var run in allProcessingRuns)
                    {
                        if (!processingJobsDetectionRunIds.Contains(run.Id.Value.ToString()))
                        {
                            //check if the job is in failed jobs 
                            bool jobFoundInFailedJobs = await CheckFailedJobsAndUpdateStatus(run, failedJobs);
                            if (!jobFoundInFailedJobs)
                            {
                                //check if the job is in success jobs 
                                await CheckSucceededJobsAndUpdateStatus(run, succeededJobs, monitoringApi);
                            }
                        }
                    }

                    //re-fetch detectionRuns after status updated to ERROR or SUCCESS
                    resultGetDetectionRuns = await _detectionRunService.GetAllDetectionRuns();
                    if (!resultGetDetectionRuns.IsSuccess && resultGetDetectionRuns.HandleError())
                    {
                        return HandleErrorRedirect("ErrorViewsPath:Error", 400);
                    }
                    if (resultGetDetectionRuns.Data == null)
                    {
                        return HandleErrorRedirect("ErrorViewsPath:Error404", 404);
                    }
                }

                //check if the training run status is stuck in PROCESSING status for too long, change status to error or success
                bool isProcessingStatus = resultGetTrainingRuns.Data.Any(d => d.Status == nameof(ScheduleRunsStatus.Processing));
                if (isProcessingStatus)
                {
                    List<TrainingRunDTO> allProcessingRuns = resultGetTrainingRuns.Data.Where(x => x.Status.Equals(nameof(ScheduleRunsStatus.Processing))).ToList();

                    var monitoringApi = JobStorage.Current.GetMonitoringApi();
                    var processingJobs = monitoringApi.ProcessingJobs(0, int.MaxValue);
                    var succeededJobs = monitoringApi.SucceededJobs(0, int.MaxValue);
                    var failedJobs = monitoringApi.FailedJobs(0, int.MaxValue);

                    //Get jobs parameter trainingRunId 
                    List<string> processingJobsDetectionRunIds = GetProcessingJobTrainingRunIds(processingJobs);

                    foreach (var run in allProcessingRuns)
                    {
                        if (!processingJobsDetectionRunIds.Contains(run.Id.Value.ToString()))
                        {
                            //check if the job is in failed jobs 
                            bool jobFoundInFailedJobs = await CheckFailedJobsAndUpdateStatusForTrainingRuns(run, failedJobs);
                            if (!jobFoundInFailedJobs)
                            {
                                //check if the job is in success jobs 
                                await CheckSucceededJobsAndUpdateStatusForTrainingRuns(run, succeededJobs, monitoringApi);
                            }
                        }
                    }

                    //re-fetch detectionRuns after status updated to ERROR or SUCCESS
                    resultGetTrainingRuns = await _trainingRunService.GetAllTrainingRuns();
                    if (!resultGetTrainingRuns.IsSuccess && resultGetTrainingRuns.HandleError())
                    {
                        return HandleErrorRedirect("ErrorViewsPath:Error", 400);
                    }
                    if (resultGetTrainingRuns.Data == null)
                    {
                        return HandleErrorRedirect("ErrorViewsPath:Error404", 404);
                    }
                }

                ViewScheduledRunsViewModel vm = new()
                {
                    DetectionRuns = resultGetDetectionRuns.Data,
                    TrainingRuns = resultGetTrainingRuns.Data
                };

                return View(vm);
            }
            catch (Exception)
            {
                return HandleErrorRedirect("ErrorViewsPath:Error", 400);
            }
            
        }

        //detection runs
        private List<string> GetProcessingJobDetectionRunIds(JobList<ProcessingJobDto> processingJobs)
        {
            List<string> processingJobsDetectionRunIds = new();
            foreach (var job in processingJobs)
            {
                using (var connection = JobStorage.Current.GetConnection())
                {
                    var storedKey = connection.GetJobParameter(job.Key, "detectionRunId");
                    if (!string.IsNullOrEmpty(storedKey))
                    {
                        processingJobsDetectionRunIds.Add(storedKey);
                    }
                }
            }
            return processingJobsDetectionRunIds;
        }

        private async Task<bool> CheckFailedJobsAndUpdateStatus(DetectionRunDTO run, JobList<FailedJobDto> failedJobs)
        {
            foreach (var fJob in failedJobs)
            {
                using (var connection = JobStorage.Current.GetConnection())
                {
                    var storedKey = connection.GetJobParameter(fJob.Key, "detectionRunId");
                    if (!string.IsNullOrEmpty(storedKey) && storedKey == run.Id.Value.ToString())
                    {
                        await _detectionRunService.UpdateStatus(run.Id.Value, nameof(ScheduleRunsStatus.Error));
                        return true;
                    }
                }
            }
            return false;
        }

        private async Task CheckSucceededJobsAndUpdateStatus(DetectionRunDTO run, JobList<SucceededJobDto> succeededJobs, IMonitoringApi monitoringApi)
        {
            foreach (var succJob in succeededJobs)
            {
                using (var connection = JobStorage.Current.GetConnection())
                {
                    var storedKey = connection.GetJobParameter(succJob.Key, "detectionRunId");
                    if (!string.IsNullOrEmpty(storedKey) && storedKey == run.Id.Value.ToString())
                    {
                        var jobDetails = monitoringApi.JobDetails(succJob.Key);
                        var historyData = jobDetails?.History.FirstOrDefault()?.Data;
                        var resValue = historyData.FirstOrDefault().Value;

                        if (resValue.Contains("\"IsSuccess\":true"))
                        {
                            await _detectionRunService.UpdateStatus(run.Id.Value, nameof(ScheduleRunsStatus.Success));
                        }
                        else
                        {
                            await _detectionRunService.UpdateStatus(run.Id.Value, nameof(ScheduleRunsStatus.Error));
                        }
                        return;
                    }
                }
            }
        }

        //training runs
        private List<string> GetProcessingJobTrainingRunIds(JobList<ProcessingJobDto> processingJobs)
        {
            List<string> processingJobsTrainingRunIds = new();
            foreach (var job in processingJobs)
            {
                using (var connection = JobStorage.Current.GetConnection())
                {
                    var storedKey = connection.GetJobParameter(job.Key, "trainingRunId");
                    if (!string.IsNullOrEmpty(storedKey))
                    {
                        processingJobsTrainingRunIds.Add(storedKey);
                    }
                }
            }
            return processingJobsTrainingRunIds;
        }
        private async Task<bool> CheckFailedJobsAndUpdateStatusForTrainingRuns(TrainingRunDTO run, JobList<FailedJobDto> failedJobs)
        {
            foreach (var fJob in failedJobs)
            {
                using (var connection = JobStorage.Current.GetConnection())
                {
                    var storedKey = connection.GetJobParameter(fJob.Key, "trainingRunId");
                    if (!string.IsNullOrEmpty(storedKey) && storedKey == run.Id.Value.ToString())
                    {
                        await _trainingRunService.UpdateTrainingRunEntity(run.Id.Value, null, nameof(ScheduleRunsStatus.Error), false);
                        return true;
                    }
                }
            }
            return false;
        }
        private async Task CheckSucceededJobsAndUpdateStatusForTrainingRuns(TrainingRunDTO run, JobList<SucceededJobDto> succeededJobs, IMonitoringApi monitoringApi)
        {
            foreach (var succJob in succeededJobs)
            {
                using (var connection = JobStorage.Current.GetConnection())
                {
                    var storedKey = connection.GetJobParameter(succJob.Key, "trainingRunId");
                    if (!string.IsNullOrEmpty(storedKey) && storedKey == run.Id.Value.ToString())
                    {
                        var jobDetails = monitoringApi.JobDetails(succJob.Key);
                        var historyData = jobDetails?.History.FirstOrDefault()?.Data;
                        var resValue = historyData.FirstOrDefault().Value;

                        if (resValue.Contains("\"IsSuccess\":true"))
                        {
                            await _trainingRunService.UpdateTrainingRunEntity(run.Id.Value, run.TrainedModelId.Value, nameof(ScheduleRunsStatus.Success), true);
                        }
                        else
                        {
                            await _trainingRunService.UpdateTrainingRunEntity(run.Id.Value, null, nameof(ScheduleRunsStatus.Error), false);
                        }
                        return;
                    }
                }
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

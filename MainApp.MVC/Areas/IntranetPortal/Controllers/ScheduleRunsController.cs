using AutoMapper;
using DAL.Interfaces.Helpers;
using DTOs.MainApp.BL.DetectionDTOs;
using Entities.DetectionEntities;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using MainApp.BL.Interfaces.Services.DetectionServices;
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
        private readonly IConfiguration _configuration;

        public ScheduleRunsController(IDetectionRunService detectionRunService, IConfiguration configuration)
        {
            _detectionRunService = detectionRunService;
            _configuration = configuration;
        }

        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.ManageScheduledRuns))]
        public async Task<IActionResult> ViewScheduledRuns()
        {
            ResultDTO<List<DetectionRunDTO>> resultGetDetectionRuns = await _detectionRunService.GetAllDetectionRuns();
            if (!resultGetDetectionRuns.IsSuccess && resultGetDetectionRuns.HandleError())
            {
                return RedirectToErrorPage("Error");
            }
            if (resultGetDetectionRuns.Data == null)
            {
                return RedirectToErrorPage("Error404");
            }

            //check if the detection run status is stuck in PROCESSING status for too long, chage status to error or success
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
                    return RedirectToErrorPage("Error");
                }
                if (resultGetDetectionRuns.Data == null)
                {
                   return RedirectToErrorPage("Error404");
                }
            }

            ViewScheduledRunsViewModel vm = new()
            {
                DetectionRuns = resultGetDetectionRuns.Data
            };

            return View(vm);
        }
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

        private IActionResult RedirectToErrorPage(string errorType)
        {
            var errorPath = _configuration[$"ErrorViewsPath:{errorType}"];
            return string.IsNullOrEmpty(errorPath) ? errorType == "Error" ? BadRequest() : NotFound() : Redirect(errorPath);
        }
    }
}

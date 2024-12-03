# Hangfire Integration

- [Official Page](https://www.hangfire.io/)
- [Documentation](https://docs.hangfire.io/en/latest/)

## Configuration

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Add Hangfire services
    services.AddHangfire((serviceProvider, configuration) => {
        configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(configuration.GetConnectionString("HangfireConnection"), new PostgreSqlStorageOptions
            {
                QueuePollInterval = TimeSpan.FromSeconds(15),
                JobExpirationCheckInterval = TimeSpan.FromHours(1),
                CountersAggregateInterval = TimeSpan.FromMinutes(5),
                PrepareSchemaIfNecessary = true,
                DashboardJobListLimit = 50000,
                TransactionTimeout = TimeSpan.FromMinutes(1),
                SchemaName = "hangfire"
            });
    });

    // Configure job server
    services.AddHangfireServer(options =>
    {
        options.WorkerCount = Environment.ProcessorCount * 2;
        options.Queues = new[] { "detection", "default" };
        options.ServerName = "DetectionServer";
    });
}
```

## Job Scheduling and Monitoring

```csharp
public class DetectionJobManager
{
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IDetectionRunService _detectionRunService;

    public async Task<string> ScheduleDetectionRun(DetectionRunDTO detectionRunDTO)
    {
        // Schedule with retry policy
        var jobId = BackgroundJob.Schedule(
            () => _detectionRunService.StartDetectionRun(detectionRunDTO),
            TimeSpan.FromSeconds(10));

        // Add continuation job for cleanup
        BackgroundJob.ContinueJobWith(
            jobId,
            () => CleanupDetectionResources(detectionRunDTO.Id));

        // Add job metadata
        using (var connection = JobStorage.Current.GetConnection())
        {
            connection.SetJobParameter(jobId, "detectionRunId", 
                detectionRunDTO.Id.ToString());
            connection.SetJobParameter(jobId, "priority", "high");
        }

        return jobId;
    }

    public async Task MonitorDetectionJob(string jobId)
    {
        var monitoringApi = JobStorage.Current.GetMonitoringApi();
        var job = monitoringApi.JobDetails(jobId);

        switch (job.State)
        {
            case "Processing":
                await HandleProcessingState(job);
                break;
            case "Failed":
                await HandleFailedState(job);
                break;
            case "Succeeded":
                await HandleSucceededState(job);
                break;
        }
    }

    private async Task HandleFailedState(JobDetailsDto job)
    {
        var detectionRunId = job.Properties["detectionRunId"];
        var exception = job.History
            .FirstOrDefault(x => x.StateName == "Failed")?.ExceptionDetails;

        await _detectionRunService.HandleFailedRun(
            Guid.Parse(detectionRunId), exception);
    }
}
```

## Error Recovery

```csharp
public class DetectionJobRetryAttribute : JobFilterAttribute, IElectStateFilter
{
    public void OnStateElection(ElectStateContext context)
    {
        var failedState = context.CandidateState as FailedState;
        if (failedState == null) return;

        var retryAttempt = context.GetRetryCount();
        if (retryAttempt < 3)
        {
            // Exponential backoff
            var delay = TimeSpan.FromMinutes(Math.Pow(2, retryAttempt));
            context.SetRetryAttempt(retryAttempt + 1);
            context.CandidateState = new ScheduledState(delay);
        }
    }
}
```
# Job processing in detection and training runs
## Single queue management
When a detection or training run is scheduled, the application ensures sequential processing:
- If a Hangfire job is already executing a detection/training run, newly created runs will wait in the queue.
- The next run starts only after the previous job completes.
```csharp
//start processing this job when the previous one finish
string jobId = _backgroundJobClient.Enqueue(() => StartDetectionRun(detectionRunDTO));
```

## Processing status monitoring
The application implements a robust status checking mechanism:
- When accessing the <i>View Scheduled Runs page</i>, background checks are performed.
- If a run is stuck in <i>processing</i> status and no corresponding Hangfire job is active(processing), the status is automatically updated to either <i>error</i> or <i>success</i> based on the job's final state.
```csharp
 //Get jobs parameter 
 List<string> processingJobsRunIds = GetProcessingJobRunIds(processingJobs);

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
``` 

## Cancellation Scenarios
Cancellation/deletion depend on the Hangfire job status:
1. Queue cancellation
- Training/Detection runs with <i>waiting</i> status can be canceled/deleted.
- Applicable when the Hangfire job is still in the queue.

2. Processing training/detection run restrictions
- Generally, runs with <i>processing</i> status cannot be canceled/deleted. 
```csharp
//frontend validation
 if (item.Status != nameof(ScheduleRunsStatus.Processing) && User.HasAuthClaim(SD.AuthClaims.DeleteTrainingRun))
 {
     <button class="mb-1 btn bg-gradient-danger btn-xs" id="delete_@item.Id" onclick="deleteTrainingRunModalFunction('@item.Id')" title="@DbResHtml.T("Delete Training Run", "Resources")">
         <i class="fas fa-trash"></i>
     </button>
 }
//backend validation
 if (resultGetDetectionRunDb.Data.Status == nameof(ScheduleRunsStatus.Processing))
     return ResultDTO.Fail("Can not delete detection run because it is in process");
```

3. Post-Execution Deletion
- After job completion (with <i>error</i> or <i>success</i> status), deletion is possible.
- Deletes related files (created during the execution process) to free up disk space.
```csharp
//delete err msg txt file from wwwroot ONLY IF status is ERROR
if (resultGetDetectionRunDb.Data.Status == nameof(ScheduleRunsStatus.Error))
{
    ResultDTO<string?>? detectionRunsErrorLogsFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DetectionRunsErrorLogsFolder", "Uploads\\DetectionUploads\\DetectionRunsErrorLogs");
    if (!detectionRunsErrorLogsFolder.IsSuccess && detectionRunsErrorLogsFolder.HandleError())
    {
        return ResultDTO.Fail("Can not get the application settings");
    }

    if (string.IsNullOrEmpty(detectionRunsErrorLogsFolder.Data))
    {
        return ResultDTO.Fail("Directory path not found");
    }

    string filePath = System.IO.Path.Combine(wwwrootPath, detectionRunsErrorLogsFolder.Data);
    if (Directory.Exists(filePath))
    {
        string fileName = $"{detectionRunId}_errMsg.txt";
        string fullFilePath = System.IO.Path.Combine(filePath, fileName);
        if (File.Exists(fullFilePath))
        {
            File.Delete(fullFilePath);
        }
    }
}

//delete error and success logs from DetectionRunCliOutDirAbsPath:
string? successLogFile = Path.Combine(_MMDetectionConfiguration.GetDetectionRunCliOutDirAbsPath(), $"succ_{detectionRunId}.txt");
if (File.Exists(successLogFile))
{
    File.Delete(successLogFile);
}
string? errorLogFile = Path.Combine(_MMDetectionConfiguration.GetDetectionRunCliOutDirAbsPath(), $"error_{detectionRunId}.txt");
if (File.Exists(errorLogFile))
{
    File.Delete(errorLogFile);
}

//delete all files from mmdetection ins-development detections detectionRunId folder
string? detectionRunFolderPath = Path.Combine(_MMDetectionConfiguration.GetDetectionRunOutputDirAbsPath(), detectionRunId.ToString());
if (Directory.Exists(detectionRunFolderPath))
{
    try
    {
        Directory.Delete(detectionRunFolderPath, recursive: true);
    }
    catch (Exception ex)
    {
        return ResultDTO.Fail($"Failed to delete folder: {ex.Message}");
    }
}
```

4. Manual Cancellation (Super Admin Only)
> [!WARNING]
> <b>Caution</b>: Manual cancellation via Hangfire dashboard
> - Not recommended in Production
> - Requires stopping associated Task Manager background processes 
> - May cause significant functionality issues

Important Considerations
- Only accessible to super admin users
- Uses Hangfire dashboard UI
- Terminates  ex. Python.exe, Conda.exe, and other related processes

Best Practices:
- Prefer built-in cancellation mechanisms
- Avoid manual intervention during critical operations



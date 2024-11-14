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

# Training Controllers Documentation

## TrainingRunsController

Provides HTTP endpoints for managing training runs and executing training operations.

### Endpoints

#### Create Training Run

```csharp
[HttpGet]
[HasAuthClaim(nameof(SD.AuthClaims.CreateTrainingRun))]
public async Task<IActionResult> CreateTrainingRun()
```

- Initializes training run creation view
- Requires CreateTrainingRun claim
- Returns view for training setup

#### Schedule Training Run

```csharp
[HttpPost]
[HasAuthClaim(nameof(SD.AuthClaims.ScheduleTrainingRun))]
public async Task<ResultDTO> ScheduleTrainingRun(TrainingRunViewModel viewModel)
```

- Validates training parameters
- Creates training run entity
- Schedules background job
- Returns operation result

#### Execute Training Process

```csharp
[HttpPost]
[HasAuthClaim(nameof(SD.AuthClaims.ScheduleTrainingRun))]
public async Task<ResultDTO> ExecuteTrainingRunProcess(TrainingRunDTO trainingRunDTO)
```

- Handles main training workflow
- Updates training status
- Manages error handling
- Creates trained model

#### Delete Training Run

```csharp
[HttpPost]
[HasAuthClaim(nameof(SD.AuthClaims.DeleteTrainingRun))]
public async Task<ResultDTO> DeleteTrainingRun(Guid trainingRunId)
```

- Removes training run
- Cleans up resources
- Cancels pending jobs

#### Publish Model

```csharp
[HttpPost]
[HasAuthClaim(nameof(SD.AuthClaims.PublishTrainingRunTrainedModel))]
public async Task<ResultDTO> PublishTrainingRunTrainedModel(Guid trainingRunId)
```

- Publishes trained model
- Updates model status
- Validates completion

## TrainedModelsController

Manages trained model operations and queries.

### Endpoints

#### Get All Trained Models

```csharp
[HttpGet]
[HasAuthClaim(nameof(SD.AuthClaims.ViewTrainingRuns))]
public async Task<ResultDTO<List<TrainedModelDTO>>> GetAllTrainedModels()
```

- Retrieves all trained models
- Includes related data
- Requires view permission

## Authorization

### Required Claims

- CreateTrainingRun
- ScheduleTrainingRun
- DeleteTrainingRun
- PublishTrainingRunTrainedModel
- ViewTrainingRuns

### Claim Enforcement

```csharp
[HasAuthClaim(nameof(SD.AuthClaims.ClaimName))]
```

- Declarative authorization
- Claim-based access control
- Role inheritance support

## Error Handling

### Error Response Format

```csharp
public class ResultDTO
{
    public bool IsSuccess { get; }
    public string? ErrMsg { get; }
    public object? ExObj { get; }
}
```

### Error Logging

- Structured logging
- Error file creation
- Job status updates
- Client notification

## Background Job Integration

### Job Management

```csharp
private readonly IBackgroundJobClient _backgroundJobClient;
```

- Hangfire integration
- Job scheduling
- Status tracking
- Error handling

### Job Parameters

```csharp
connection.SetJobParameter(jobId, "trainingRunId", trainingRunId.ToString());
```

- Job identification
- Parameter passing
- State management

## File Management

### Error Log Creation

```csharp
private async Task<ResultDTO> CreateErrMsgFile(Guid trainingRunId, string errMsg)
```

- Error documentation
- File system operations
- Path management

### Configuration Management

```csharp
private readonly IMMDetectionConfigurationService _MMDetectionConfiguration;
```

- Path resolution
- Configuration generation
- File organization

## View Models

### TrainingRunViewModel

```csharp
public class TrainingRunViewModel
{
    public string Name { get; set; }
    public Guid DatasetId { get; set; }
    public Guid TrainedModelId { get; set; }
}
```

- Data validation
- Model binding
- Client interaction

# Training Entities Documentation

## TrainingRun Entity

Represents a single training session in the system, tracking all aspects of the training process from initialization to completion.

### Properties

```csharp
public class TrainingRun
{
    public Guid Id { get; set; }                    // Unique identifier
    public string Name { get; set; }                // Descriptive name
    public bool IsCompleted { get; set; }           // Training completion status
    public string? Status { get; set; }             // Current status of training
    public Guid DatasetId { get; set; }             // Associated dataset
    public Guid? TrainedModelId { get; set; }       // Resulting model (if successful)
    public Guid? BaseModelId { get; set; }          // Base model for transfer learning
    public Guid? TrainParamsId { get; set; }        // Training parameters
    public string CreatedById { get; set; }         // User who initiated training
    public DateTime CreatedOn { get; set; }         // Creation timestamp
}
```

### Status Values

- `Waiting`: Initial state, pending execution
- `Processing`: Training is actively running
- `Success`: Training completed successfully
- `Error`: Training failed with errors

### Relationships

- **Dataset**: One-to-one relationship with training data
- **TrainedModel**: One-to-one relationship with resulting model
- **BaseModel**: One-to-one relationship with source model
- **CreatedBy**: One-to-one relationship with user

## TrainedModel Entity

Represents a trained model artifact, storing model files and metadata about the training process.

### Properties

```csharp
public class TrainedModel
{
    public Guid Id { get; set; }                    // Unique identifier
    public string Name { get; set; }                // Model name
    public string ModelConfigPath { get; set; }     // Path to config file
    public string ModelFilePath { get; set; }       // Path to model weights
    public bool IsPublished { get; set; }           // Publication status
    public Guid DatasetId { get; set; }             // Training dataset
    public Guid? TrainingRunId { get; set; }        // Associated training run
    public Guid? BaseModelId { get; set; }          // Parent model
    public string CreatedById { get; set; }         // Creator
    public DateTime CreatedOn { get; set; }         // Creation timestamp
}
```

### States

- **Unpublished**: Initial state, model is still under review
- **Published**: Model is approved for production use

### Relationships

- **Dataset**: One-to-one with training dataset
- **TrainingRun**: One-to-one with training process
- **BaseModel**: Self-referential for model lineage
- **CreatedBy**: One-to-one with creator user

## Entity Lifecycle

### TrainingRun Lifecycle

1. Created with `Waiting` status
2. Moves to `Processing` during execution
3. Transitions to `Success` or `Error`
4. Links to TrainedModel on success

### TrainedModel Lifecycle

1. Created after successful training
2. Initially unpublished
3. Can be published after review
4. Maintains references to training artifacts

## Usage Guidelines

### Creating New Training Runs

```csharp
var trainingRun = new TrainingRun
{
    Name = "Training Run Name",
    DatasetId = datasetGuid,
    BaseModelId = baseModelGuid,
    CreatedById = userId,
    Status = "Waiting"
};
```

### Managing TrainedModels

```csharp
var trainedModel = new TrainedModel
{
    Name = "Model Name",
    ModelConfigPath = configPath,
    ModelFilePath = modelPath,
    DatasetId = datasetGuid,
    TrainingRunId = trainingRunGuid,
    CreatedById = userId
};
```

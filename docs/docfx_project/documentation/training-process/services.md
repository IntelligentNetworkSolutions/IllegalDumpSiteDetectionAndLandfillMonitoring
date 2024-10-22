# Training Services Documentation

## TrainingRunService

Manages the execution and lifecycle of training runs, handling the core training workflow from initialization to completion.

### Key Methods

#### CreateTrainingRunWithBaseModel

```csharp
public async Task<ResultDTO<TrainingRunDTO>> CreateTrainingRunWithBaseModel(TrainingRunDTO inputTrainingRunDTO)
```

- Creates a new training run with transfer learning setup
- Validates base model and dataset
- Returns training run DTO with configuration

#### ExecuteTrainingRunProcess

```csharp
public async Task<ResultDTO> ExecuteTrainingRunProcess(TrainingRunDTO trainingRunDTO)
```

- Handles main training execution
- Manages dataset preparation
- Generates configurations
- Executes MMDetection training
- Creates trained model entity

#### GenerateTrainingRunConfigFile

```csharp
public async Task<ResultDTO<string>> GenerateTrainingRunConfigFile(
    Guid trainingRunId, 
    DatasetDTO datasetDTO, 
    TrainedModelDTO baseTrainedModelDTO, 
    int? numEpochs = null, 
    int? numFrozenStages = null, 
    int? numBatchSize = null)
```

- Generates MMDetection configuration
- Sets up training parameters
- Configures model architecture
- Returns path to config file

#### StartTrainingRun

```csharp
public async Task<ResultDTO> StartTrainingRun(Guid trainingRunId)
```

- Initiates training process
- Executes MMDetection commands
- Monitors training progress

## TrainedModelService

Manages trained model entities, handling model metadata, storage, and lifecycle.

### Key Methods

#### GetTrainedModelById

```csharp
public async Task<ResultDTO<TrainedModelDTO>> GetTrainedModelById(Guid id, bool track = false)
```

- Retrieves model information
- Includes related entities
- Returns model DTO

#### GetPublishedTrainedModelsIncludingTrainRuns

```csharp
public async Task<ResultDTO<List<TrainedModelDTO>>> GetPublishedTrainedModelsIncludingTrainRuns()
```

- Retrieves published models
- Includes training history
- Returns list of model DTOs

#### GetBestEpochForTrainedModelById

```csharp
public async Task<ResultDTO<TrainingRunResultsDTO>> GetBestEpochForTrainedModelById(Guid trainedModelId)
```

- Analyzes training metrics
- Identifies best performing epoch
- Returns training results

## Service Integration

### Training Workflow

1. **Initialization**

   ```csharp
   var trainingRunDTO = await _trainingRunService.CreateTrainingRunWithBaseModel(inputDTO);
   ```

2. **Configuration**

   ```csharp
   var configPath = await _trainingRunService.GenerateTrainingRunConfigFile(
       trainingRunDTO.Id,
       datasetDTO,
       baseModelDTO
   );
   ```

3. **Execution**

   ```csharp
   await _trainingRunService.StartTrainingRun(trainingRunDTO.Id);
   ```

4. **Model Creation**

   ```csharp
   var modelId = await _trainingRunService.CreateTrainedModelByTrainingRunId(trainingRunDTO.Id);
   ```

### Error Handling

- Services use ResultDTO pattern
- Comprehensive error logging
- Automatic cleanup on failure
- Status updates for monitoring

### Integration Points

- MMDetection framework
- Dataset management
- File system operations
- Background job processing

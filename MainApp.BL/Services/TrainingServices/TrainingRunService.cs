using AutoMapper;
using CliWrap;
using CliWrap.Buffered;
using DAL.Interfaces.Repositories.DatasetRepositories;
using DAL.Interfaces.Repositories.TrainingRepositories;
using DTOs.MainApp.BL.TrainingDTOs;
using Entities.DatasetEntities;
using Entities.TrainingEntities;
using MainApp.BL.Interfaces.Services;
using MainApp.BL.Interfaces.Services.TrainingServices;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SD;

namespace MainApp.BL.Services.TrainingServices
{
    public class TrainingRunService : ITrainingRunService
    {
        protected readonly IMMDetectionConfigurationService _MMDetectionConfiguration;
        private readonly ITrainingRunsRepository _trainingRunsRepository;
        private readonly ITrainedModelsRepository _trainedModelsRepository;
        private readonly IDatasetsRepository _datasetsRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<TrainingRunService> _logger;

        public TrainingRunService(IMMDetectionConfigurationService mMDetectionConfiguration,
            ITrainingRunsRepository trainingRunsRepository, ITrainedModelsRepository trainedModelsRepository,
            IDatasetsRepository datasetsRepository,
            IMapper mapper, ILogger<TrainingRunService> logger)
        {
            _MMDetectionConfiguration = mMDetectionConfiguration;
            _trainingRunsRepository = trainingRunsRepository;
            _trainedModelsRepository = trainedModelsRepository;
            _mapper = mapper;
            _logger = logger;
            _datasetsRepository = datasetsRepository;
        }

        // TODO: Refactor to return DTO
        public async Task<ResultDTO<TrainingRun>> GetTrainingRunById(Guid id, bool track = false)
        {
            try
            {
                ResultDTO<TrainingRun?> resultGetById =
                    await _trainingRunsRepository.GetById(id, track: track, includeProperties: "CreatedBy");

                if (resultGetById.IsSuccess == false && resultGetById.HandleError())
                    return ResultDTO<TrainingRun>.Fail(resultGetById.ErrMsg!);

                //DetectionRunDTO dto = _mapper.Map<DetectionRunDTO>(resultGetAllEntites.Data);

                return ResultDTO<TrainingRun>.Ok(resultGetById.Data!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<TrainingRun>.ExceptionFail(ex.Message, ex);
            }
        }

        // TODO: Implement
        public async Task<ResultDTO<List<TrainingRunDTO>>> GetAllTrainingRuns()
        {
            throw new NotImplementedException();
        }

        public async Task<ResultDTO> ExecuteDummyTrainingRunProcess()
        {
            //string modelConfigPath = "C:\\vs_code_workspaces\\mmdetection\\mmdetection\\ins_development\\resources\\ins_best_pretrained_2x2_model_files\\compiled_config\\ins_ann_v9_faster_r101_2_stages_base_add300_2_stages_e9.py";
            //string modelFilePath = "C:\\vs_code_workspaces\\mmdetection\\mmdetection\\ins_development\\resources\\ins_best_pretrained_2x2_model_files\\epoch_9.pth";

            string createdById = "bba10742-90ea-4ee7-ba41-b322099f01d8";
            string trainingRunName = "CowboyRun";

            ResultDTO<Dataset?> resultDatasetIncludeThenAll =
                await _datasetsRepository.GetByIdIncludeThenAll(Guid.Parse("cf84ac82-f28d-439d-98c2-d11eed031ac6"), track: false,
                    includeProperties:
                    [(d => d.CreatedBy, null), (d => d.UpdatedBy, null), (d => d.ParentDataset, null),
                    (d => d.DatasetClasses, [ddc => ((Dataset_DatasetClass)ddc).DatasetClass, dc => ((DatasetClass)dc).ParentClass]),
                    (d => d.DatasetImages, [di => ((DatasetImage)di).ImageAnnotations,]),
                    (d => d.DatasetImages, [ di => ((DatasetImage)di).ImageAnnotations]),
                    (d => d.DatasetImages, [ di => ((DatasetImage)di).ImageAnnotations, ia => ((ImageAnnotation)ia).DatasetClass,
                                                dc => ((DatasetClass)dc).Datasets]),
                    ]
                );

            if (resultDatasetIncludeThenAll.IsSuccess == false)
                return ResultDTO.Fail(resultDatasetIncludeThenAll.ErrMsg!);

            if (resultDatasetIncludeThenAll.Data is null)
                return ResultDTO.Fail("Error getting Dataset");

            ResultDTO<TrainedModel?> getInsBestTrainedModelResult =
                await _trainedModelsRepository.GetByIdInclude(Guid.Parse("a83f2202-f919-4b79-a181-b2747219ed46"), track: false);

            int numEpochs = 1;
            int numBatchSize = 1;
            int numFrozenStages = 4;

            ResultDTO result =
                await ExecuteTrainingRunProcess(trainingRunName, resultDatasetIncludeThenAll.Data, getInsBestTrainedModelResult.Data!, createdById,
                                                numEpochs, numFrozenStages, numBatchSize);

            return ResultDTO.Ok();
        }

        public async Task<ResultDTO> ExecuteTrainingRunProcess(string trainRunName, Dataset dataset, TrainedModel baseTrainedModel,
            string createdById, int? numEpochs = null, int? numFrozenStages = null, int? numBatchSize = null)
        {
            try
            {
                ResultDTO<TrainingRun> createTrainingRunResult = await CreateTrainingRun(trainRunName, dataset, baseTrainedModel, createdById);
                if (createTrainingRunResult.IsSuccess == false && ResultDTO<TrainingRun>.HandleError(createTrainingRunResult))
                    return ResultDTO.Fail(createTrainingRunResult.ErrMsg!);

                TrainingRun trainingRun = createTrainingRunResult.Data!;

                // TODO: Export Dataset at _MMDetectionConfiguration.GetTrainingRunDatasetDirAbsPath(trainingRun.Id)

                ResultDTO<string> generateTrainRunResult =
                    await GenerateTrainingRunConfig(trainingRun.Id, dataset, baseTrainedModel, numEpochs, numFrozenStages, numBatchSize);

                ResultDTO startTrainRunResult = await StartTrainingRun(trainingRun.Id);

                ResultDTO<TrainedModel> createTrainedModelResult = await CreateTrainedModelFromTrainingRun(trainingRun);
                if (createTrainedModelResult.IsSuccess == false && createTrainedModelResult.HandleError())
                    return ResultDTO.Fail(createTrainedModelResult.ErrMsg!);

                if (createTrainedModelResult.Data is null)
                    return ResultDTO.Fail("Create Trained Model returned Null Trained Model");

                // TODO: Update Training Run IsSuccuss
                ResultDTO<TrainingRun> updateTrainRunResult =
                    await UpdateTrainingRunAfterSuccessfullTraining(trainingRun, createTrainedModelResult.Data.Id);
                if (updateTrainRunResult.IsSuccess == false && updateTrainRunResult.HandleError())
                    return ResultDTO.Fail(updateTrainRunResult.ErrMsg!);

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
            finally
            {
                // TODO: Clean Up Training Run Files, Later
            }
        }

        public async Task<ResultDTO<TrainedModel>> CreateTrainedModelFromTrainingRun(TrainingRun trainingRun)
        {
            if (trainingRun is null)
                return ResultDTO<TrainedModel>.Fail("Training Run is null when Creating Trained Model");

            ResultDTO<string> getTrainedModelConfigFilePathResult =
                _MMDetectionConfiguration.GetTrainedModelConfigFileAbsPath(trainingRun.Id);
            if (getTrainedModelConfigFilePathResult.IsSuccess == false)
                return ResultDTO<TrainedModel>.Fail(getTrainedModelConfigFilePathResult.ErrMsg!);

            //TODO : Get BEst Epoch and call with NumBestEpoch

            ResultDTO<string> getTrainedModelBestEpochFilePathResult =
                _MMDetectionConfiguration.GetTrainedModelBestEpochFileAbsPath(trainingRun.Id);
            if (getTrainedModelBestEpochFilePathResult.IsSuccess == false)
                return ResultDTO<TrainedModel>.Fail(getTrainedModelBestEpochFilePathResult.ErrMsg!);

            TrainedModel trainedModel = new TrainedModel()
            {
                ModelConfigPath = getTrainedModelConfigFilePathResult.Data!,
                ModelFilePath = getTrainedModelBestEpochFilePathResult.Data!,

                TrainingRunId = trainingRun.Id,
                DatasetId = trainingRun.DatasetId,
                BaseModelId = trainingRun.BaseModelId,
                Name = trainingRun.Name,

                IsPublished = false,

                CreatedOn = DateTime.UtcNow,
                CreatedById = trainingRun.CreatedById,
            };

            ResultDTO<TrainedModel> createTrainedModelResult = await _trainedModelsRepository.CreateAndReturnEntity(trainedModel);
            if (createTrainedModelResult.IsSuccess == false && createTrainedModelResult.HandleError())
                return ResultDTO<TrainedModel>.Fail(createTrainedModelResult.ErrMsg!);

            return ResultDTO<TrainedModel>.Ok(createTrainedModelResult.Data!);
        }

        public async Task<ResultDTO<TrainingRun>> CreateTrainingRun(string name, Dataset dataset, TrainedModel baseTrainedModel, string createdById)
        {
            // TODO: refactor to accept TrainingRunDTO
            try
            {
                TrainingRun trainingRunEntity = new TrainingRun()
                {
                    Name = name,
                    IsCompleted = false,

                    DatasetId = dataset.Id,
                    Dataset = null,

                    BaseModelId = baseTrainedModel.Id,
                    BaseModel = null,

                    TrainParamsId = null,
                    TrainParams = null,

                    TrainedModelId = null,
                    TrainedModel = null,

                    CreatedById = createdById,
                    CreatedBy = null,
                    CreatedOn = DateTime.UtcNow,
                };

                ResultDTO<TrainingRun> createTrainRunResult = await _trainingRunsRepository.CreateAndReturnEntity(trainingRunEntity);
                if (createTrainRunResult.IsSuccess == false && createTrainRunResult.HandleError())
                    return ResultDTO<TrainingRun>.Fail(createTrainRunResult.ErrMsg!);

                if (createTrainRunResult.Data is null)
                    return ResultDTO<TrainingRun>.Fail("Error Creating Training Run");

                return ResultDTO<TrainingRun>.Ok(createTrainRunResult.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<TrainingRun>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<TrainingRun>> UpdateTrainingRunAfterSuccessfullTraining(TrainingRun trainingRun, Guid trainedModelId)
        {
            trainingRun.TrainedModelId = trainedModelId;
            trainingRun.IsCompleted = true;
            //trainingRun.IsSuccess = true;

            ResultDTO updateRunResult = await _trainingRunsRepository.Update(trainingRun);
            if (updateRunResult.IsSuccess == false && updateRunResult.HandleError())
                return ResultDTO<TrainingRun>.Fail(updateRunResult.ErrMsg!);

            return ResultDTO<TrainingRun>.Ok(trainingRun);
        }

        private string GeneratePythonTrainingCommandByRunId(Guid trainingRunId)
        {
            string detectionCommandStr = string.Empty;

            string scriptName = Path.Combine("tools", "train.py");
            string scriptFileAbsPath = Path.Combine(_MMDetectionConfiguration.GetRootDirAbsPath(), scriptName);
            // TODO: Update Script with INS script
            //string scriptFileAbsPath = Path.Combine(_MMDetectionConfiguration.GetScriptsDirAbsPath(), scriptName);

            string trainingConfigDirAbsPath = Path.Combine(_MMDetectionConfiguration.GetConfigsDirAbsPath(), trainingRunId.ToString());
            string trainingConfigFileAbsPath = Path.Combine(trainingConfigDirAbsPath, $"{trainingRunId}.py");

            // Save/Output Directory
            string workDirCommandParamStr = "--work-dir";
            string workDirAbsPath = Path.Combine(_MMDetectionConfiguration.GetTrainingRunsBaseOutDirAbsPath(), trainingRunId.ToString());

            detectionCommandStr =
                $"run -n openmmlab python " +
                $"{scriptFileAbsPath} " +
                $"\"{trainingConfigFileAbsPath}\" "
                + $"{workDirCommandParamStr} \"{workDirAbsPath}\"";

            return detectionCommandStr;
        }

        private async Task<ResultDTO> StartTrainingRun(Guid trainingRunId)
        {
            string trainingCommand = GeneratePythonTrainingCommandByRunId(trainingRunId);

            BufferedCommandResult powerShellResults = await Cli.Wrap(_MMDetectionConfiguration.GetCondaExeAbsPath())
                        .WithWorkingDirectory(_MMDetectionConfiguration.GetRootDirAbsPath())
                        .WithValidation(CommandResultValidation.None)
                        .WithArguments(trainingCommand.ToLower())
                        .WithStandardOutputPipe(PipeTarget.ToFile(Path.Combine(_MMDetectionConfiguration.GetTrainingRunCliOutDirAbsPath(), $"succ_{DateTime.Now.Ticks}.txt")))
                        .WithStandardErrorPipe(PipeTarget.ToFile(Path.Combine(_MMDetectionConfiguration.GetTrainingRunCliOutDirAbsPath(), $"error_{DateTime.Now.Ticks}.txt")))
                        .ExecuteBufferedAsync();

            if (powerShellResults.IsSuccess == false)
                return ResultDTO.Fail(powerShellResults.StandardError);

            return ResultDTO.Ok();
        }

        private async Task<ResultDTO<string>> GenerateTrainingRunConfig(Guid trainingRunId,
            Dataset dataset, TrainedModel baseTrainedModel, int? numEpochs = null, int? numFrozenStages = null, int? numBatchSize = null)
        {
            if (Guid.Empty == trainingRunId)
                return ResultDTO<string>.Fail("TrainingRunStartParams must have a Training Run Id");

            if ((baseTrainedModel is not null
                && dataset is not null
                && dataset.DatasetClasses is not null
                && dataset.DatasetClasses.Count != 0) == false)
                return ResultDTO<string>.Fail("TrainingRunStartParams must have a BaseTrainedModel with a fully included dataset " +
                    "i.e. DatasetClasses with DatasetClasses");

            if (string.IsNullOrEmpty(baseTrainedModel.ModelConfigPath)
                || string.IsNullOrEmpty(baseTrainedModel.ModelFilePath))
                return ResultDTO<string>.Fail("TrainingRunStartParams must have a BaseTrainedModel with Model File and Config paths");

            string[] classNames = dataset.DatasetClasses.Select(dc => dc.DatasetClass.ClassName).ToArray();

            string datasetRootPath = "C:/vs_code_workspaces/mmdetection/mmdetection/data/ins/v2";

            string trainingRunConfigContentStr =
                TrainingConfigGenerator.GenerateConfigOverrideStr(
                    backboneCheckpointAbsPath: _MMDetectionConfiguration.GetBackboneCheckpointAbsPath(),
                    //dataRootAbsPath: _MMDetectionConfiguration.GetTrainingRunDatasetDirAbsPath(trainingRunId),
                    dataRootAbsPath: datasetRootPath, // TODO: Return previous line once Export is available
                    classNames: classNames,
                    baseModelConfigFilePath: baseTrainedModel.ModelConfigPath,
                    baseModelFileAbsPath: baseTrainedModel.ModelFilePath,
                    numDatasetClasses: classNames.Length,
                    numBatchSize: numBatchSize,
                    numEpochs: numEpochs,
                    numFrozenStages: numFrozenStages);

            // Save to MMDetection
            string trainingRunConfigDirAbsPath = _MMDetectionConfiguration.GetTrainingRunConfigDirAbsPathByRunId(trainingRunId);
            string trainingRunConfigPythonFileAbsPath = _MMDetectionConfiguration.GetTrainingRunConfigFileAbsPathByRunId(trainingRunId);

            // Check if Config Directory Exists i.e. has permissions
            if (Directory.Exists(trainingRunConfigDirAbsPath) == false)
                Directory.CreateDirectory(trainingRunConfigDirAbsPath);

            // Check if Config File Exists i.e. has permissions
            //if (File.Exists(trainingRunConfigPythonFileAbsPath))
            //    return ResultDTO<string>.Fail("Training Config already Exists or inssuficient access permissions");

            try
            {
                await File.WriteAllTextAsync(trainingRunConfigPythonFileAbsPath, trainingRunConfigContentStr);
            }
            catch (Exception ex)
            {
                return ResultDTO<string>.ExceptionFail($"Training Config file Not Generated. \n{ex.Message}", ex);
            }

            return ResultDTO<string>.Ok(trainingRunConfigPythonFileAbsPath);
        }

        public async Task<ResultDTO<TrainingRunResultsDTO>> GetBestEpochForTrainingRun(Guid trainingRunId)
        {
            try
            {

                ResultDTO<string> getLogFilePathResult = _MMDetectionConfiguration.GetTrainingRunResultLogFileAbsPath(trainingRunId);
                if (getLogFilePathResult.IsSuccess == false && getLogFilePathResult.HandleError())
                    return ResultDTO<TrainingRunResultsDTO>.Fail(getLogFilePathResult.ErrMsg!);

                Dictionary<int, TrainingEpochMetricsDTO> epochMetrics = new Dictionary<int, TrainingEpochMetricsDTO>();
                using (StreamReader reader = new StreamReader(getLogFilePathResult.Data!))
                {
                    string jsonContent = "";
                    int currentEpoch = 0;
                    int lineNumber = 0;

                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        lineNumber++;

                        if (string.IsNullOrWhiteSpace(line) || line.Trim() == "{")
                            continue; // Skip empty lines or lines with only '{'

                        jsonContent += line;

                        if (line.Trim().EndsWith("}"))
                        {
                            try
                            {
                                JObject jsonObject = JObject.Parse(jsonContent);
                                TrainingEpochStepMetricsDTO stepMetrics = new TrainingEpochStepMetricsDTO();

                                if (jsonObject.TryGetValue("epoch", out JToken? epochToken))
                                    currentEpoch = epochToken.Value<int>();

                                if (jsonObject.TryGetValue("step", out JToken? stepToken))
                                    stepMetrics.Step = stepToken.Value<int>();

                                if (!epochMetrics.ContainsKey(currentEpoch))
                                    epochMetrics[currentEpoch] = new TrainingEpochMetricsDTO { Epoch = currentEpoch };

                                foreach (var property in jsonObject.Properties())
                                {
                                    if (property.Value.Type == JTokenType.Float || property.Value.Type == JTokenType.Integer)
                                    {
                                        double value = property.Value.Value<double>();
                                        stepMetrics.Metrics[property.Name] = value;
                                    }
                                }

                                epochMetrics[currentEpoch].Steps.Add(stepMetrics);

                                jsonContent = ""; // Reset for the next object
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error parsing JSON at line {lineNumber}: {jsonContent}");
                                Console.WriteLine($"Error message: {ex.Message}");
                                jsonContent = ""; // Reset for the next object
                            }
                        }
                    }
                }

                if (epochMetrics.Count == 0)
                    return ResultDTO<TrainingRunResultsDTO>.Fail("No valid data found in the file.");

                TrainingEpochMetricsDTO? bestEpoch = 
                    epochMetrics.Values.Where(e => e.Steps.Any(s => s.Metrics.ContainsKey("coco/bbox_mAP")))
                                        .OrderByDescending(e => e.Steps.Max(s => s.Metrics.GetValueOrDefault("coco/bbox_mAP", 0)))
                                        .FirstOrDefault();

                if (bestEpoch is null)
                    return ResultDTO<TrainingRunResultsDTO>.Fail("No epoch found with coco/bbox_mAP metric.");

                TrainingRunResultsDTO trainingRunResults = new TrainingRunResultsDTO()
                {
                    BestEpochMetrics = bestEpoch,
                    EpochMetrics = epochMetrics,
                };

                return ResultDTO<TrainingRunResultsDTO>.Ok(trainingRunResults);
            }
            catch (Exception ex)
            {
                return ResultDTO<TrainingRunResultsDTO>.ExceptionFail(ex.Message, ex);
            }
        }

        // TODO: Implement
        public async Task<ResultDTO> ScheduleTrainingRun()
        {
            throw new NotImplementedException();
        }

        // TODO: Implement, maybe one retry Clean up files regardless of outcome, someday
        public async Task<ResultDTO> CleanUpTrainingRunFiles()
        {
            throw new NotImplementedException();
        }

        // TODO: Implement 
        public async Task<ResultDTO<TrainingRunDTO>> EditTrainingRunById(Guid trainingRunId)
        {
            // Allowed Update only for Name and IsPublished
            throw new NotImplementedException();
        }

        // TODO: Implement , Delete files as well
        public async Task<ResultDTO> DeleteTrainingRunById(Guid trainingRunId)
        {
            throw new NotImplementedException();
        }
    }
}

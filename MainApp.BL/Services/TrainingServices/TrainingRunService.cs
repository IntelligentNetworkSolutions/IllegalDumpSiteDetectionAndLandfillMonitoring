using AutoMapper;
using CliWrap;
using CliWrap.Buffered;
using DAL.Interfaces.Helpers;
using DAL.Interfaces.Repositories.TrainingRepositories;
using DTOs.MainApp.BL.DatasetDTOs;
using DTOs.MainApp.BL.DetectionDTOs;
using DTOs.MainApp.BL.TrainingDTOs;
using Entities.DatasetEntities;
using Entities.TrainingEntities;
using MainApp.BL.Interfaces.Services;
using MainApp.BL.Interfaces.Services.DetectionServices;
using MainApp.BL.Interfaces.Services.TrainingServices;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SD;
using SD.Enums;
using SD.Helpers;

namespace MainApp.BL.Services.TrainingServices
{
    public class TrainingRunService : ITrainingRunService
    {
        protected readonly IMMDetectionConfigurationService _MMDetectionConfiguration;
        private readonly ITrainingRunsRepository _trainingRunsRepository;
        private readonly ITrainedModelsRepository _trainedModelsRepository;
        private readonly IDetectionRunService _detectionRunService;
        private readonly IMapper _mapper;
        private readonly ILogger<TrainingRunService> _logger;
        private readonly IAppSettingsAccessor _appSettingsAccessor;


        public TrainingRunService(IMMDetectionConfigurationService mMDetectionConfiguration,
            ITrainingRunsRepository trainingRunsRepository, ITrainedModelsRepository trainedModelsRepository,
            IMapper mapper, ILogger<TrainingRunService> logger, IDetectionRunService detectionRunService,
            IAppSettingsAccessor appSettingsAccessor)
        {
            _MMDetectionConfiguration = mMDetectionConfiguration;
            _trainingRunsRepository = trainingRunsRepository;
            _trainedModelsRepository = trainedModelsRepository;
            _mapper = mapper;
            _logger = logger;
            _detectionRunService = detectionRunService;
            _appSettingsAccessor = appSettingsAccessor;
        }

        public async Task<ResultDTO<TrainingRunDTO>> GetTrainingRunById(Guid id, bool track = false)
        {
            try
            {
                ResultDTO<TrainingRun?> resultGetById =
                    await _trainingRunsRepository.GetById(id, track: track, includeProperties: "CreatedBy");

                if (resultGetById.IsSuccess == false && resultGetById.HandleError())
                    return ResultDTO<TrainingRunDTO>.Fail(resultGetById.ErrMsg!);

                TrainingRunDTO dto = _mapper.Map<TrainingRunDTO>(resultGetById.Data);

                return ResultDTO<TrainingRunDTO>.Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<TrainingRunDTO>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<List<TrainingRunDTO>>> GetAllTrainingRuns()
        {
            try
            {
                ResultDTO<IEnumerable<TrainingRun>>? resultGetEntities =  await _trainingRunsRepository.GetAll(includeProperties: "CreatedBy,TrainedModel");

                if (resultGetEntities.IsSuccess == false && resultGetEntities.HandleError())
                    return ResultDTO<List<TrainingRunDTO>>.Fail(resultGetEntities.ErrMsg!);

                if (resultGetEntities.Data == null)
                    return ResultDTO<List<TrainingRunDTO>>.Fail("Training runs not found");

                List<TrainingRunDTO> dto = _mapper.Map<List<TrainingRunDTO>>(resultGetEntities.Data);

                return ResultDTO<List<TrainingRunDTO>>.Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<List<TrainingRunDTO>>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<TrainingRunDTO>> CreateTrainingRunWithBaseModel(TrainingRunDTO inputTrainingRunDTO)
        {
            try
            {
                //get base trained model
                ResultDTO<TrainedModel?> getInsBestTrainedModelResult = 
                    await _trainedModelsRepository.GetByIdInclude(inputTrainingRunDTO.TrainedModelId.Value, track: false);
                if (getInsBestTrainedModelResult.IsSuccess == false)
                    return ResultDTO<TrainingRunDTO>.Fail(getInsBestTrainedModelResult.ErrMsg!);
                if (getInsBestTrainedModelResult.Data == null)
                    return ResultDTO<TrainingRunDTO>.Fail($"Base model not found, for id: {inputTrainingRunDTO.TrainedModelId}");

                inputTrainingRunDTO.BaseModelId = getInsBestTrainedModelResult.Data.Id;

                //map trained model entity to dto 
                TrainedModelDTO trainedModelDTO = _mapper.Map<TrainedModelDTO>(getInsBestTrainedModelResult.Data);
                if (trainedModelDTO == null)
                    return ResultDTO<TrainingRunDTO>.Fail("Mapping failed");

                //map inputDTO to training run entity
                TrainingRun entity = _mapper.Map<TrainingRun>(inputTrainingRunDTO);
                if (entity == null)
                    return ResultDTO<TrainingRunDTO>.Fail("Mapping failed");

                //create training run 
                ResultDTO<TrainingRun> createTrainingRunResult = await _trainingRunsRepository.CreateAndReturnEntity(entity);
                if (createTrainingRunResult.IsSuccess == false && ResultDTO<TrainingRun>.HandleError(createTrainingRunResult))
                    return ResultDTO<TrainingRunDTO>.Fail(createTrainingRunResult.ErrMsg!);

                //map training run entity to dto
                TrainingRunDTO? outputTrainingRunDTO = _mapper.Map<TrainingRunDTO>(createTrainingRunResult.Data);
                if (outputTrainingRunDTO == null)
                    return ResultDTO<TrainingRunDTO>.Fail("Mapping failed");
                
                outputTrainingRunDTO.BaseModel = trainedModelDTO;

                return ResultDTO<TrainingRunDTO>.Ok(outputTrainingRunDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<TrainingRunDTO>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<Guid>> CreateTrainedModelByTrainingRunId(Guid trainingRunId)
        {
            try
            {
                if (trainingRunId == Guid.Empty)
                    return ResultDTO<Guid>.Fail("Training run id is null");

                //get training run from db
                ResultDTO<TrainingRun?> resultTrainingRunEntity = await _trainingRunsRepository.GetById(trainingRunId);
                if (resultTrainingRunEntity.IsSuccess == false)
                    return ResultDTO<Guid>.Fail(resultTrainingRunEntity.ErrMsg!);
                if (resultTrainingRunEntity.Data == null)
                    return ResultDTO<Guid>.Fail("Training run not found");

                ResultDTO<string> getTrainedModelConfigFilePathResult = _MMDetectionConfiguration.GetTrainedModelConfigFileAbsPath(resultTrainingRunEntity.Data.Id);
                if (getTrainedModelConfigFilePathResult.IsSuccess == false)
                    return ResultDTO<Guid>.Fail(getTrainedModelConfigFilePathResult.ErrMsg!);

                ResultDTO<TrainingRunResultsDTO> getBestEpochResult = GetBestEpochForTrainingRun(resultTrainingRunEntity.Data.Id);
                if (getBestEpochResult.IsSuccess == false && getBestEpochResult.HandleError())
                    return ResultDTO<Guid>.Fail(getBestEpochResult.ErrMsg!);

                ResultDTO<string> getTrainedModelBestEpochFilePathResult = _MMDetectionConfiguration.GetTrainedModelBestEpochFileAbsPath(resultTrainingRunEntity.Data.Id, getBestEpochResult.Data!.BestEpochMetrics.Epoch);
                if (getTrainedModelBestEpochFilePathResult.IsSuccess == false)
                    return ResultDTO<Guid>.Fail(getTrainedModelBestEpochFilePathResult.ErrMsg!);

                TrainedModel trainedModel = new TrainedModel()
                {
                    ModelConfigPath = getTrainedModelConfigFilePathResult.Data!,
                    ModelFilePath = getTrainedModelBestEpochFilePathResult.Data!,

                    TrainingRunId = resultTrainingRunEntity.Data.Id,
                    DatasetId = resultTrainingRunEntity.Data.DatasetId,
                    BaseModelId = resultTrainingRunEntity.Data.BaseModelId,
                    Name = resultTrainingRunEntity.Data.Name,

                    IsPublished = false,

                    CreatedOn = DateTime.UtcNow,
                    CreatedById = resultTrainingRunEntity.Data.CreatedById,
                };

                ResultDTO<TrainedModel> createTrainedModelResult = await _trainedModelsRepository.CreateAndReturnEntity(trainedModel);
                if (createTrainedModelResult.IsSuccess == false && createTrainedModelResult.HandleError())
                    return ResultDTO<Guid>.Fail(createTrainedModelResult.ErrMsg!);

                return ResultDTO<Guid>.Ok(createTrainedModelResult.Data!.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<Guid>.ExceptionFail(ex.Message, ex);
            }
        }
        
        public async Task<ResultDTO> UpdateTrainingRunEntity(Guid trainingRunId, Guid? trainedModelId = null, string? status = null, bool? isCompleted = null)
        {
            try
            {
                ResultDTO<TrainingRun?> resultGetEntity = await _trainingRunsRepository.GetById(trainingRunId, track: true);
                if (resultGetEntity.IsSuccess == false && resultGetEntity.HandleError())
                    return ResultDTO.Fail(resultGetEntity.ErrMsg!);

                if (resultGetEntity.Data is null)
                    return ResultDTO.Fail($"No Training Run found with ID: {trainingRunId}");

                if (trainedModelId != null)
                {
                    resultGetEntity.Data.TrainedModelId = trainedModelId;
                }
                if (status != null)
                {
                    resultGetEntity.Data.Status = status;
                }
                if (isCompleted != null)
                {
                    resultGetEntity.Data.IsCompleted = true;
                }

                ResultDTO updateRunResult = await _trainingRunsRepository.Update(resultGetEntity.Data);
                if (updateRunResult.IsSuccess == false && updateRunResult.HandleError())
                    return ResultDTO.Fail(updateRunResult.ErrMsg!);

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
                        
        }

        private string GeneratePythonTrainingCommandByRunId(Guid trainingRunId)
        {
            try
            {
                string detectionCommandStr = string.Empty;

                string scriptName = Path.Combine("tools", "train.py");
                string scriptFileAbsPath =
                    CommonHelper.PathToLinuxRegexSlashReplace(
                        Path.Combine(_MMDetectionConfiguration.GetRootDirAbsPath(), scriptName));
                // TODO: Update Script with INS script
                //string scriptFileAbsPath = Path.Combine(_MMDetectionConfiguration.GetScriptsDirAbsPath(), scriptName);

                string trainingConfigDirAbsPath =
                    CommonHelper.PathToLinuxRegexSlashReplace(
                        Path.Combine(_MMDetectionConfiguration.GetConfigsDirAbsPath(), trainingRunId.ToString()));
                string trainingConfigFileAbsPath =
                    CommonHelper.PathToLinuxRegexSlashReplace(
                        Path.Combine(trainingConfigDirAbsPath, $"{trainingRunId}.py"));

                // Save/Output Directory
                string workDirCommandParamStr = "--work-dir";
                string workDirAbsPath = 
                    CommonHelper.PathToLinuxRegexSlashReplace(
                        Path.Combine(_MMDetectionConfiguration.GetTrainingRunsBaseOutDirAbsPath(), trainingRunId.ToString()));

                string openmmlabPath = _MMDetectionConfiguration.GetOpenMMLabAbsPath();

                detectionCommandStr =
                    $"run -p {openmmlabPath} python " +
                    $"{scriptFileAbsPath} " +
                    $"\"{trainingConfigFileAbsPath}\" "
                    + $"{workDirCommandParamStr} \"{workDirAbsPath}\"";

                return detectionCommandStr;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ex.Message;
            }
        }

        public async Task<ResultDTO> StartTrainingRun(Guid trainingRunId)
        {
            try
            {
                string trainingCommand = GeneratePythonTrainingCommandByRunId(trainingRunId);
                string? trainingCliLogsAbsPath = _MMDetectionConfiguration.GetTrainingRunCliOutDirAbsPath();
                string? detectionCliLogsAbsPath = _MMDetectionConfiguration.GetDetectionRunCliOutDirAbsPath();

                if (!Directory.Exists(trainingCliLogsAbsPath))
                {
                    Directory.CreateDirectory(trainingCliLogsAbsPath);
                }

                if (!Directory.Exists(detectionCliLogsAbsPath))
                {
                    Directory.CreateDirectory(detectionCliLogsAbsPath);
                }

                BufferedCommandResult powerShellResults = await Cli.Wrap(_MMDetectionConfiguration.GetCondaExeAbsPath())
                            .WithWorkingDirectory(_MMDetectionConfiguration.GetRootDirAbsPath())
                            .WithValidation(CommandResultValidation.None)
                            .WithArguments(trainingCommand.ToLower())
                            .WithStandardOutputPipe(PipeTarget.ToFile(Path.Combine(_MMDetectionConfiguration.GetTrainingRunCliOutDirAbsPath(), $"succ_{trainingRunId}.txt")))
                            .WithStandardErrorPipe(PipeTarget.ToFile(Path.Combine(_MMDetectionConfiguration.GetTrainingRunCliOutDirAbsPath(), $"error_{trainingRunId}.txt")))
                            .ExecuteBufferedAsync();

                if (powerShellResults.IsSuccess == false)
                    return ResultDTO.Fail(powerShellResults.StandardError);

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<string>> GenerateTrainingRunConfigFile(Guid trainingRunId, DatasetDTO datasetDTO, TrainedModelDTO baseTrainedModelDTO, int? numEpochs = null, int? numFrozenStages = null, int? numBatchSize = null)
        {
            if (Guid.Empty == trainingRunId)
                return ResultDTO<string>.Fail("TrainingRunStartParams must have a Training Run Id");

            Dataset? dataset = _mapper.Map<Dataset>(datasetDTO);
            if(dataset == null)
                return ResultDTO<string>.Fail("Mapping failed");

            TrainedModel? baseTrainedModel = _mapper.Map<TrainedModel>(baseTrainedModelDTO);
            if (dataset == null)
                return ResultDTO<string>.Fail("Mapping failed");

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

            //string datasetRootPath = "C:/vs_code_workspaces/mmdetection/mmdetection/data/ins/v2";

            string trainingRunConfigContentStr =
                TrainingConfigGenerator.GenerateConfigOverrideStr(
                    backboneCheckpointAbsPath: _MMDetectionConfiguration.GetBackboneCheckpointAbsPath(),
                    dataRootAbsPath: _MMDetectionConfiguration.GetTrainingRunDatasetDirAbsPath(trainingRunId),
                    //dataRootAbsPath: datasetRootPath, // TODO: Return previous line once Export is available
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

        public ResultDTO<TrainingRunResultsDTO> GetBestEpochForTrainingRun(Guid trainingRunId)
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

        public async Task<ResultDTO> PublishTrainingRunTrainedModel(Guid trainingRunId)
        {
            try
            {
                //get training run entity
                ResultDTO<TrainingRun?> resultGetEntity = await _trainingRunsRepository.GetById(trainingRunId, includeProperties:"TrainedModel");
                if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())
                    return ResultDTO.Fail(resultGetEntity.ErrMsg!);

                if (resultGetEntity.Data == null)
                    return ResultDTO.Fail("Training run not found");

                //get trained model
                ResultDTO<TrainedModel?> resultGetTrainedModel = await _trainedModelsRepository.GetById(resultGetEntity.Data.TrainedModelId!.Value, track: true);
                if (!resultGetTrainedModel.IsSuccess && resultGetTrainedModel.HandleError())
                    return ResultDTO.Fail(resultGetTrainedModel.ErrMsg!);
                if (resultGetTrainedModel.Data == null)
                    return ResultDTO.Fail("Training model not found");

                //set isPubliesh to true
                resultGetTrainedModel.Data.IsPublished = true;

                //update trained model
                ResultDTO? updateTrainedModel = await _trainedModelsRepository.Update(resultGetTrainedModel.Data);
                if (!updateTrainedModel.IsSuccess && updateTrainedModel.HandleError())
                    return ResultDTO.Fail(updateTrainedModel.ErrMsg!);

                return ResultDTO.Ok();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO> DeleteTrainingRun(Guid trainingRunId, string wwwrootPath)
        {
            try
            {
                //get training run entity
                ResultDTO<TrainingRunDTO> resultGetEntity = await GetTrainingRunById(trainingRunId);
                if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())
                    return ResultDTO.Fail(resultGetEntity.ErrMsg!);

                if (resultGetEntity.Data == null)
                    return ResultDTO.Fail("Training run not found");

                //check status if is PROCESSING
                if (resultGetEntity.Data.Status == nameof(ScheduleRunsStatus.Processing))
                    return ResultDTO.Fail("Can not delete training run because it is in process");

                //check if status is waitnig then DELETE ONLY the training run from db because there are no files or trained model entered
                if (resultGetEntity.Data.Status == nameof(ScheduleRunsStatus.Waiting))
                {
                    //delete training run from db
                    TrainingRun trainingRunEntity = _mapper.Map<TrainingRun>(resultGetEntity.Data);
                    ResultDTO resultDeleteTrainingRun = await _trainingRunsRepository.Delete(trainingRunEntity);
                    if (resultDeleteTrainingRun.IsSuccess == false && resultDeleteTrainingRun.HandleError())
                        return ResultDTO.Fail(resultDeleteTrainingRun.ErrMsg!);
                    return ResultDTO.Ok();
                }

                //Get deteciton runs to check if there is detection run for the trained model from this training run
                ResultDTO<List<DetectionRunDTO>> resultGetDetections = await _detectionRunService.GetAllDetectionRuns();
                if (resultGetDetections.IsSuccess == false && resultGetDetections.HandleError())
                    return ResultDTO.Fail(resultGetDetections.ErrMsg!);
                if (resultGetDetections.Data == null)
                    return ResultDTO.Fail("Detection runs not found");

                bool hasDetectionRun = resultGetDetections.Data.Any(x => x.TrainedModelId == resultGetEntity.Data.TrainedModelId);
                if (hasDetectionRun)
                {
                    return ResultDTO.Fail("There is already detection run/s for this trained model, delete first detection run/s");
                }

                //delete err msg txt file from wwwroot ONLY IF status is ERROR
                if (resultGetEntity.Data.Status == nameof(ScheduleRunsStatus.Error))
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

                    string filePath = System.IO.Path.Combine(wwwrootPath, trainingRunsErrorLogsFolder.Data);
                    if (Directory.Exists(filePath))
                    {
                        string fileName = $"{trainingRunId}_errMsg.txt";
                        string fullFilePath = System.IO.Path.Combine(filePath, fileName);
                        File.Delete(fullFilePath);
                    }
                }

                //delete error and success logs from TrainingRunCliOutDirAbsPath:
                string? successLogFile = Path.Combine(_MMDetectionConfiguration.GetTrainingRunCliOutDirAbsPath(), $"succ_{trainingRunId}.txt");
                if (File.Exists(successLogFile))
                {
                    File.Delete(successLogFile);
                }
                string? errorLogFile = Path.Combine(_MMDetectionConfiguration.GetTrainingRunCliOutDirAbsPath(), $"error_{trainingRunId}.txt");
                if (File.Exists(errorLogFile))
                {
                    File.Delete(errorLogFile);
                }

                //delete all files from mmdetection ins-development trainings trainingRunId folder
                string? trainingRunFolderPath = Path.Combine(_MMDetectionConfiguration.GetTrainingRunsBaseOutDirAbsPath(), trainingRunId.ToString());
                if (Directory.Exists(trainingRunFolderPath))
                {
                    try
                    {
                        Directory.Delete(trainingRunFolderPath, recursive: true);
                    }
                    catch (Exception ex)
                    {
                        return ResultDTO.Fail($"Failed to delete folder: {ex.Message}");
                    }
                }

                //get trained model entity
                ResultDTO<TrainedModel?> resultGetTrainedModel = await _trainedModelsRepository.GetById(resultGetEntity.Data.TrainedModelId!.Value, track: true);
                if (!resultGetTrainedModel.IsSuccess && resultGetTrainedModel.HandleError())
                    return ResultDTO.Fail(resultGetTrainedModel.ErrMsg!);
                if (resultGetTrainedModel.Data == null)
                    return ResultDTO.Fail("Training model not found");
                
                //get all training runs
                ResultDTO<IEnumerable<TrainingRun>> resultGetAllTrainingRuns = await _trainingRunsRepository.GetAll();
                if (!resultGetAllTrainingRuns.IsSuccess && resultGetAllTrainingRuns.HandleError())
                    return ResultDTO.Fail(resultGetAllTrainingRuns.ErrMsg!);
                if (resultGetAllTrainingRuns.Data == null)
                    return ResultDTO.Fail("Training runs not found");

                //all trained model ids from training runs
                var trainingModelIdsList = resultGetAllTrainingRuns.Data.Select(x => x.TrainedModelId).ToList();
                if (!trainingModelIdsList.Contains(resultGetTrainedModel.Data.Id))
                {
                    //detele trained model from db if it is not contained in other training runs
                    ResultDTO resultDeleteTrainedModel = await _trainedModelsRepository.Delete(resultGetTrainedModel.Data);
                    if (!resultDeleteTrainedModel.IsSuccess && resultDeleteTrainedModel.HandleError())
                        return ResultDTO.Fail(resultDeleteTrainedModel.ErrMsg!);
                } 

                //delete training run from db
                TrainingRun trainingRun = _mapper.Map<TrainingRun>(resultGetEntity.Data);
                ResultDTO resultDeleteEntity = await _trainingRunsRepository.Delete(trainingRun);
                if (resultDeleteEntity.IsSuccess == false && resultDeleteEntity.HandleError())
                    return ResultDTO.Fail(resultDeleteEntity.ErrMsg!);

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }

        }
    }
}

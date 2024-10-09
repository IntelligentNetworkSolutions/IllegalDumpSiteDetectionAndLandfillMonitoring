using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CliWrap;
using CliWrap.Buffered;
using DAL.Interfaces.Repositories.DatasetRepositories;
using DAL.Interfaces.Repositories.TrainingRepositories;
using Entities.DatasetEntities;
using Entities.TrainingEntities;
using MainApp.BL.Interfaces.Services;
using MainApp.BL.Interfaces.Services.TrainingServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

                if(createTrainedModelResult.Data is null )
                    return ResultDTO.Fail("Create Trained Model returned Null Trained Model");

                // TODO: Update Training Run
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
                // TODO: Clean Up Training Run Files
            }
        }

        public async Task<ResultDTO<TrainedModel>> CreateTrainedModelFromTrainingRun(TrainingRun trainingRun)
        {
            if (trainingRun is null)
                return ResultDTO<TrainedModel>.Fail("Training Run is null when Creating Trained Model");

            ResultDTO<string> getTrainedModelConfigFilePathResult = 
                _MMDetectionConfiguration.GetTrainedModelConfigFileAbsPath(trainingRun.Id);
            if(getTrainedModelConfigFilePathResult.IsSuccess == false)
                return ResultDTO<TrainedModel>.Fail(getTrainedModelConfigFilePathResult.ErrMsg!);

            ResultDTO<string> getTrainedModelBestEpochFilePathResult = 
                _MMDetectionConfiguration.GetTrainedModelBestEpochFileAbsPath(trainingRun.Id);
            if(getTrainedModelBestEpochFilePathResult.IsSuccess == false)
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

            if(powerShellResults.IsSuccess == false)
                return ResultDTO.Fail(powerShellResults.StandardError);

            return ResultDTO.Ok();
        }

        private async Task<ResultDTO<string>> GenerateTrainingRunConfig(Guid trainingRunId, 
            Dataset  dataset, TrainedModel baseTrainedModel, int? numEpochs = null, int? numFrozenStages = null, int? numBatchSize = null)
        {
            if(Guid.Empty == trainingRunId)
                return ResultDTO<string>.Fail("TrainingRunStartParams must have a Training Run Id");

            if ((baseTrainedModel is not null
                && dataset is not null
                && dataset.DatasetClasses is not null
                && dataset.DatasetClasses.Count != 0) == false)
                return ResultDTO<string>.Fail("TrainingRunStartParams must have a BaseTrainedModel with a fully included dataset " +
                    "i.e. DatasetClasses with DatasetClasses");

            if(string.IsNullOrEmpty(baseTrainedModel.ModelConfigPath)
                || string.IsNullOrEmpty(baseTrainedModel.ModelFilePath))
                return ResultDTO<string>.Fail("TrainingRunStartParams must have a BaseTrainedModel with Model File and Config paths");

            string[] classNames = dataset.DatasetClasses.Select(dc => dc.DatasetClass.ClassName).ToArray();

            string datasetRootPath = "C:/vs_code_workspaces/mmdetection/mmdetection/data/ins/v2";

            string trainingRunConfigContentStr = 
                TrainingRunConfigGenerator.GenerateConfigOverrideStr(
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

        // TODO: Implement
        public async Task<ResultDTO> ScheduleTrainingRun()
        {
            throw new NotImplementedException();
        }

        // TODO: Implement, maybe one retry Clean up files regardless of outcome
        public async Task<ResultDTO> CleanUpTrainingRunFiles()
        {
            throw new NotImplementedException();
        }
    }

    public static class TrainingRunConfigGenerator
    {
        public static string GenerateConfigOverrideStr(string backboneCheckpointAbsPath, 
            string dataRootAbsPath, string[] classNames, 
            string baseModelConfigFilePath, string baseModelFileAbsPath, 
            int numDatasetClasses, int? numBatchSize = null, int? numEpochs = null, int? numFrozenStages = null)
        {
            string configStr =
                $"{GenerateConfigVariablesOverrideStr(numDatasetClasses, numBatchSize, numEpochs, numFrozenStages)}\r\n" +
                $"\r\n" +
                $"{GenerateConfigBaseModelOverrideStr(baseModelConfigFilePath)}\r\n" +
                $"\r\n" +
                $"{GenerateConfigDataRootOverrideStr(dataRootAbsPath)}\r\n" +
                $"\r\n" +
                $"{GenerateConfigMetaInfoOverrideStr(classNames)}\r\n" +
                $"\r\n" +
                $"{GenerateConfigModelOverrideStr(backboneCheckpointAbsPath)}\r\n" +
                $"\r\n" +
                $"{GenerateConfigTrainCfgOverrideStr}\r\n" +
                $"{GenerateConfigTrainDataloaderOverrideStr}\r\n" +
                $"\r\n" +
                $"{GenerateConfigValCfgOverrideStr}\r\n" +
                $"{GenerateConfigValDataloaderOverrideStr}\r\n" +
                $"{GenerateConfigValEvaluatorOverrideStr}\r\n" +
                $"\r\n" +
                $"{GenerateConfigTestCfgOverrideStr}\r\n" +
                $"{GenerateConfigTestDataloaderOverrideStr}\r\n" +
                $"{GenerateConfigTestEvaluatorOverrideStr}\r\n" +
                $"\r\n" +
                $"{GenerateConfigLoadFromOverrideStr(baseModelFileAbsPath)}\r\n" +
                $"\r\n";

            return configStr;
        }

        public static string GenerateConfigVariablesOverrideStr(int numDatasetClasses, 
            int? numBatchSize = null, int? numEpochs = null, int? numFrozenStages = null)
        {
            string configVariablesOverrideStr =
                $"num_dataset_classes = {numDatasetClasses}\r\n" +
                $"num_batch_size = {(numBatchSize.HasValue ? numBatchSize.Value : 2)}\r\n" +
                $"num_epochs = {(numEpochs.HasValue ? numEpochs.Value : 15)}\r\n" +
                $"num_frozen_stages = {(numFrozenStages.HasValue ? numFrozenStages.Value : 2)}\r\n";

            return configVariablesOverrideStr;
        }

        public static string GenerateConfigBaseModelOverrideStr(string baseModelConfigFilePath)
            => $"_base_ = ['{baseModelConfigFilePath}']";

        public static string GenerateConfigDataRootOverrideStr(string dataRootAbsPath)
            => $"data_root = '{dataRootAbsPath}'\r\n";

        public static string GenerateConfigMetaInfoOverrideStr(string[] classNames)
        {
            string classesListOverrideStr = "";
            string paletteArrOverrideStr = "";
            const int redVal = 220;
            const int greenVal = 20;
            const int blueVal = 60;
            for (int i = 0; i < classNames.Length; i++)
            {
                string className = classNames[i];
                classesListOverrideStr += $"'{className}', ";
                paletteArrOverrideStr += $"({redVal + i * -10}, {greenVal + i * 10}, {blueVal + i * 10}), ";
            }

            string metaInfoOverrideStr = $"metainfo = dict(classes=({classesListOverrideStr}), palette=[{paletteArrOverrideStr}])\r\n";

            return metaInfoOverrideStr;
        }

        public static string GenerateConfigModelOverrideStr(string backboneCheckpointAbsPath)
        {
            string configModelOverrideStr =
                $"model = dict(\r\n" +
                $"\tbackbone=dict(\r\n" +
                $"\t\tfrozen_stages=num_frozen_stages,\r\n" +
                $"\t\tinit_cfg=dict(checkpoint=r'{backboneCheckpointAbsPath}', type='Pretrained'),\r\n" +
                $"\t\tnum_stages=4),\r\n" +
                $"\troi_head=dict(\r\n" +
                $"\t\tbbox_head=dict(\r\n" +
                $"\t\t\tnum_classes=num_dataset_classes\r\n" +
                $"\t\t\t),\r\n" +
                $"\t\t),\r\n" +
                $"\ttype='FasterRCNN')\r\n";

            return configModelOverrideStr;
        }

        public const string GenerateConfigTrainCfgOverrideStr =
            "train_cfg = dict(max_epochs=num_epochs, type='EpochBasedTrainLoop', val_interval=1)\t\n";
        public const string GenerateConfigTrainDataloaderOverrideStr = 
                "train_dataloader = dict(\r\n" +
                "\tbatch_size=num_batch_size,\r\n" +
                "\tdataset=dict(\r\n" +
                "\t\tdata_root=data_root,\r\n" +
                "\t\tmetainfo=metainfo,\r\n" +
                "\t\tann_file='train/annotations_coco.json',\r\n" +
                "\t\tdata_prefix=dict(img='train/'),),\r\n" +
                "\tnum_workers=2,)\r\n";

        public const string GenerateConfigValCfgOverrideStr = 
            "val_cfg = dict(type='ValLoop')\r\n";
        public const string GenerateConfigValDataloaderOverrideStr =
                "val_dataloader = dict(\r\n" +
                "\tbatch_size=num_batch_size,\r\n" +
                "\tdataset=dict(\r\n" +
                "\t\tdata_root=data_root,\r\n" +
                "\t\tmetainfo=metainfo,\r\n" +
                "\t\tann_file='valid/annotations_coco.json',\r\n" +
                "\t\tdata_prefix=dict(img='valid/'),),\r\n" +
                "\tnum_workers=2,)\r\n";
        public const string GenerateConfigValEvaluatorOverrideStr = 
            "val_evaluator = dict(ann_file=data_root + '/valid/annotations_coco.json',)\r\n";

        public const string GenerateConfigTestCfgOverrideStr = 
            "test_cfg = dict(type='TestLoop')\r\n";
        public const string GenerateConfigTestDataloaderOverrideStr = 
                "test_dataloader = dict(\r\n" +
                "\tbatch_size=num_batch_size,\r\n" +
                "\tdataset=dict(\r\n" +
                "\t\tdata_root=data_root,\r\n" +
                "\t\tmetainfo=metainfo,\r\n" +
                "\t\tann_file='test/annotations_coco.json',\r\n" +
                "\t\tdata_prefix=dict(img='test/'),),\r\n" +
                "\tnum_workers=2,)\r\n";
        public const string GenerateConfigTestEvaluatorOverrideStr = 
            "test_evaluator = dict(ann_file=data_root + '/test/annotations_coco.json',)\r\n";

        public static string GenerateConfigLoadFromOverrideStr(string baseModelFileAbsPath)
            => $"load_from = '{baseModelFileAbsPath}'\r\n";
    }
}

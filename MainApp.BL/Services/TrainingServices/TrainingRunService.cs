using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.TrainingEntities;
using SD;

namespace MainApp.BL.Services.TrainingServices
{
    public class TrainingRunService
    {
        // TODO: Implement, maybe one retry Clean up files regardless of outcome
        public async Task<ResultDTO> CleanUpTrainingRunFiles()
        {
            throw new NotImplementedException();
        }

        public async Task<ResultDTO<string>> GenerateTrainingRunConfig(TrainingRunStartParams trainingRunStartParams)
        {
            if(string.IsNullOrEmpty(trainingRunStartParams.TrainingRunId))
                return ResultDTO<string>.Fail("TrainingRunStartParams must have a Training Run Id");

            if ((trainingRunStartParams.BaseTrainedModel is not null
                && trainingRunStartParams.BaseTrainedModel.Dataset is not null
                && trainingRunStartParams.BaseTrainedModel.Dataset.DatasetClasses is not null
                && trainingRunStartParams.BaseTrainedModel.Dataset.DatasetClasses.Count != 0) == false)
                return ResultDTO<string>.Fail("TrainingRunStartParams must have a BaseTrainedModel with a fully included dataset i.e. DatasetClasses with DatasetClasses");

            if(string.IsNullOrEmpty(trainingRunStartParams.BaseTrainedModel.ModelConfigPath)
                || string.IsNullOrEmpty(trainingRunStartParams.BaseTrainedModel.ModelFilePath))
                return ResultDTO<string>.Fail("TrainingRunStartParams must have a BaseTrainedModel with Model File and Config paths");

            TrainingConfigParamsOverrideStringGenerator trainingConfigParamsStrGenerator =
                new TrainingConfigParamsOverrideStringGenerator(trainingRunId: trainingRunStartParams.TrainingRunId,
                    classNames: trainingRunStartParams.BaseTrainedModel.Dataset.DatasetClasses.Select(dc => dc.DatasetClass.ClassName).ToArray(),
                    baseModelConfigMMDetectionRelativePath: trainingRunStartParams.BaseTrainedModel.ModelConfigPath, 
                    baseModelMMDetectionBaseRelativePath: trainingRunStartParams.BaseTrainedModel.ModelFilePath);

            string trainingConfigContentStr = trainingConfigParamsStrGenerator.GetFullOverridenTrainingRunConfigContentStr();

            // Save to MMDetection
            string mmDetectionBasePath = "";
            string mmDetectionConfigDirPath = Path.Combine(mmDetectionBasePath, "config");
            string trainingRunConfigDirAbsPath = Path.Combine(mmDetectionConfigDirPath, trainingRunStartParams.TrainingRunId);
            string trainingRunConfigPythonFileAbsPath = Path.Combine(trainingRunConfigDirAbsPath, trainingRunStartParams.TrainingRunId + ".py");

            // Check if Config File Exists i.e. has permissions
            if (File.Exists(trainingRunConfigPythonFileAbsPath))
                return ResultDTO<string>.Fail("Training Config already Exists or inssuficient access permissions");

            try
            {
                await File.WriteAllTextAsync(trainingRunConfigPythonFileAbsPath, trainingConfigContentStr);
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
    }

    public record TrainingRunStartParams
    {
        public string TrainingRunId { get; init; }
        public TrainedModel BaseTrainedModel { get; init; }
        public int? NumEpochs { get; init; }
        public int? NumFrozenStages { get; init; }
        public int? BatchSize { get; init; }
    }

    public record TrainingConfigParamsOverrideStringGenerator
    {
        public string DataRoot { get; init; }
        public string NumClasses { get; init; }
        public string BaseModelConfig { get; init; }
        public string BaseModelLoadFrom { get; init; }
        public string? NumEpochs { get; init; }
        public string? BatchSize { get; init; }
        public string? NumFrozenStagesBackbone { get; init; }

        public TrainingConfigParamsOverrideStringGenerator(string trainingRunId, string[] classNames, 
            string baseModelConfigMMDetectionRelativePath, string baseModelMMDetectionBaseRelativePath, 
            int? numEpochs = null, int? batchSize = null, int? numFrozenStagesBackbone = null)
        {
            DataRoot = GetDataRootOverrideStr(trainingRunId);

            NumClasses = GetNumClassesOverrideStr(classNames.Length);

            BaseModelConfig = GetBaseModelConfigOverrideStr(baseModelConfigMMDetectionRelativePath);

            BaseModelLoadFrom = GetBaseModelLoadFromPathOverrideStr(baseModelMMDetectionBaseRelativePath);

            if (batchSize.HasValue == false)
                batchSize = 1; // TODO: Read from AppSetting
            BatchSize = GetBatchSizeOverrideStr(batchSize.Value);

            if (numEpochs.HasValue == false)
                numEpochs = 15; // TODO: Read from AppSetting
            NumEpochs = GetNumEpochsOverrideStr(numEpochs.Value);

            if(numFrozenStagesBackbone.HasValue == false)
                numFrozenStagesBackbone = 3; // TODO: Read from AppSetting
            NumFrozenStagesBackbone = GetNumFrozenStagesBackboneOverrideStr(numFrozenStagesBackbone.Value);
        }

        public string GetFullOverridenTrainingRunConfigContentStr()
            => $"{BaseModelConfig}\n{NumFrozenStagesBackbone}\n{NumClasses}\n{DataRoot}\n{NumEpochs}\n{BatchSize}\n{BaseModelLoadFrom}\n";

        private static string? GetBatchSizeOverrideStr(int batchSize)
            => $"train_dataloader = dict(batch_size={batchSize},)\r\nval_dataloader = dict(batch_size={batchSize},)\r\ntest_dataloader = dict(batch_size={batchSize},)";

        private static string GetDataRootOverrideStr(string trainingRunId) => $"data_root = '{Path.Combine("data", trainingRunId)}'";

        private static string GetNumClassesOverrideStr(int numClasses)
            => $"model = dict(\r\n\troi_head=dict(\r\n\t\tbbox_head=dict(num_classes={numClasses})))";

        private static string GetBaseModelConfigOverrideStr(string baseModelConfigMMDetectionRelativePath)
            => $"_base_ = ['{baseModelConfigMMDetectionRelativePath}',]";

        private string GetBaseModelLoadFromPathOverrideStr(string baseModelMMDetectionBaseRelativePath)
            => $"load_from = '{baseModelMMDetectionBaseRelativePath}'";

        private static string GetNumEpochsOverrideStr(int numEpochs)
            => $"train_cfg = dict(max_epochs={numEpochs}, type='EpochBasedTrainLoop', val_interval=1)";

        private static string GetNumFrozenStagesBackboneOverrideStr(int numFrozenStagesBackbone)
            => $"model = dict(backbone=dict(frozen_stages={numFrozenStagesBackbone}, num_stages=4,)";
    }
}

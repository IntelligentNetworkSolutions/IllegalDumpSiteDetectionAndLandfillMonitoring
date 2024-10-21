using DTOs.ObjectDetection.API;
using SD;

namespace MainApp.BL.Interfaces.Services
{
    public interface IMMDetectionConfigurationService
    {
        MMDetectionConfiguration GetConfiguration();

        string GetRootDirAbsPath();
        string GetCondaExeAbsPath();
        string GetScriptsDirAbsPath();
        string GetOpenMMLabAbsPath();
     


        string GetBackboneCheckpointAbsPath();
        string GetConfigsDirAbsPath();
        string GetTrainingRunConfigFileAbsPathByRunId(Guid trainingRunId);
        string GetTrainingRunConfigDirAbsPathByRunId(Guid trainingRunId);
        string GetDatasetsDirAbsPath();
        string GetTrainingRunDatasetDirAbsPath(Guid trainingRunId);
        string GetTrainingRunsBaseOutDirAbsPath();
        ResultDTO<string> GetTrainedModelConfigFileAbsPath(Guid trainingRunId);
        ResultDTO<string> GetTrainedModelBestEpochFileAbsPath(Guid trainingRunId, int? bestEpochNum = null);
        ResultDTO<string> GetTrainingRunResultLogFileAbsPath(Guid trainingRunId);
        string GetTrainingRunCliOutDirAbsPath();

        string GetDetectionRunOutputDirAbsPath();
        string GetDetectionRunCliOutDirAbsPath();
        string GetDetectionRunOutputDirAbsPathByRunId(Guid detectionRunId);
        string GetDetectionRunOutputAnnotationsFileAbsPathByRunId(Guid detectionRunId);
    }
}

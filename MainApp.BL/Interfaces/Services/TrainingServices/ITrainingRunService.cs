using DTOs.MainApp.BL.DatasetDTOs;
using DTOs.MainApp.BL.TrainingDTOs;
using Entities.DatasetEntities;
using Entities.TrainingEntities;
using SD;

namespace MainApp.BL.Interfaces.Services.TrainingServices
{
    public interface ITrainingRunService
    {       
        Task<ResultDTO<List<TrainingRunDTO>>> GetAllTrainingRuns();
        ResultDTO<TrainingRunResultsDTO> GetBestEpochForTrainingRun(Guid trainingRunId);
        Task<ResultDTO<TrainingRunDTO>> GetTrainingRunById(Guid id, bool track = false);
        Task<ResultDTO<TrainingRunDTO>> CreateTrainingRunWithBaseModel(TrainingRunDTO inputTrainingRunDTO);        
        Task<ResultDTO> UpdateTrainingRunEntity(Guid trainingRunId, Guid? trainedModelId = null, string? status = null, bool? isCompleted = null);
        Task<ResultDTO<string>> GenerateTrainingRunConfigFile(Guid trainingRunId, DatasetDTO datasetDTO, TrainedModelDTO baseTrainedModelDTO, int? numEpochs = null, int? numFrozenStages = null, int? numBatchSize = null);
        Task<ResultDTO> StartTrainingRun(Guid trainingRunId);
        Task<ResultDTO<Guid>> CreateTrainedModelByTrainingRunId(Guid trainingRunId);
        Task<ResultDTO> DeleteTrainingRun(Guid trainingRunId, string wwwrootPath);
        Task<ResultDTO> PublishTrainingRunTrainedModel(Guid trainingRunId);
    }
}

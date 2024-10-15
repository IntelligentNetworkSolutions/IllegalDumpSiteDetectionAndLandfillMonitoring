using DTOs.MainApp.BL.TrainingDTOs;
using Entities.DatasetEntities;
using Entities.TrainingEntities;
using SD;

namespace MainApp.BL.Interfaces.Services.TrainingServices
{
    public interface ITrainingRunService
    {
        Task<ResultDTO> ExecuteDummyTrainingRunProcess();
        Task<ResultDTO> ExecuteTrainingRunProcess(string trainRunName, Dataset dataset, TrainedModel baseTrainedModel,
            string createdById, int? numEpochs = null, int? numFrozenStages = null, int? numBatchSize = null);
        Task<ResultDTO<List<TrainingRunDTO>>> GetAllTrainingRuns();
        Task<ResultDTO<TrainingRunResultsDTO>> GetBestEpochForTrainingRun(Guid trainingRunId);
        Task<ResultDTO<TrainingRun>> GetTrainingRunById(Guid id, bool track = false);
    }
}

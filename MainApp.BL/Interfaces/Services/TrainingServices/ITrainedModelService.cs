using DTOs.MainApp.BL.TrainingDTOs;
using SD;

namespace MainApp.BL.Interfaces.Services.TrainingServices
{
    public interface ITrainedModelService
    {
        Task<ResultDTO<TrainedModelDTO>> GetTrainedModelById(Guid id, bool track = false);
        Task<ResultDTO<List<TrainedModelDTO>>> GetAllTrainedModels();       
        Task<ResultDTO> DeleteTrainedModelById(Guid trainedModelId);
        Task<ResultDTO> EditTrainedModelById(Guid trainedModelId, string? name = null, bool? isPublished = null);
        Task<ResultDTO<TrainingRunResultsDTO>> GetBestEpochForTrainedModelById(Guid trainedModelId);
        Task<ResultDTO<List<TrainedModelDTO>>> GetPublishedTrainedModelsIncludingTrainRuns();
    }
}

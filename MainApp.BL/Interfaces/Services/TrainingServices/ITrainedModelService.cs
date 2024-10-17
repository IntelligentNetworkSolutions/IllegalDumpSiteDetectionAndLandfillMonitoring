using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.MainApp.BL.TrainingDTOs;
using Entities.TrainingEntities;
using SD;

namespace MainApp.BL.Interfaces.Services.TrainingServices
{
    public interface ITrainedModelService
    {
        Task<ResultDTO<TrainedModelDTO>> GetTrainedModelById(Guid id, bool track = false);
        Task<ResultDTO<List<TrainedModelDTO>>> GetAllTrainedModels();
        Task<ResultDTO<List<TrainedModelDTO>>> GetAllTrainedModelsIncludingTrainRuns();

        Task<ResultDTO> DeleteTrainedModelById(Guid trainedModelId);
        Task<ResultDTO<TrainedModel>> EditTrainedModelById(Guid trainedModelId);

        Task<ResultDTO<TrainingRunResultsDTO>> GetBestEpochForTrainedModelById(Guid trainedModelId);
    }
}

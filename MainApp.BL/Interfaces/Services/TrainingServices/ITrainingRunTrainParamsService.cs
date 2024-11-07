using DTOs.MainApp.BL.TrainingDTOs;
using SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.BL.Interfaces.Services.TrainingServices
{
    public interface ITrainingRunTrainParamsService
    {
        Task<ResultDTO<TrainingRunTrainParamsDTO>> CreateTrainingRunTrainParams(int numEpochs, int numBatchSize, int numFrozenStages, Guid trainingRunId, Guid trainParamsId);
    }
}

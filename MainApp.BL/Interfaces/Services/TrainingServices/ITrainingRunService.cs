using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}

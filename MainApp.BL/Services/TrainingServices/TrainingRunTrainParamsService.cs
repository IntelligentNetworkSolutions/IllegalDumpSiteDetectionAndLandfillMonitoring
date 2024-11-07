using AutoMapper;
using DAL.Interfaces.Repositories.TrainingRepositories;
using DTOs.MainApp.BL.DatasetDTOs;
using DTOs.MainApp.BL.TrainingDTOs;
using Entities.TrainingEntities;
using MainApp.BL.Interfaces.Services;
using MainApp.BL.Interfaces.Services.TrainingServices;
using Microsoft.Extensions.Logging;
using SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.BL.Services.TrainingServices
{
    public class TrainingRunTrainParamsService : ITrainingRunTrainParamsService
    {
        private readonly ITrainingRunTrainParamsRepository _trainingRunTrainParamsRepository;

        private readonly IMapper _mapper;
        private readonly ILogger<TrainingRunTrainParamsService> _logger;

        public TrainingRunTrainParamsService(ITrainingRunTrainParamsRepository trainingRunTrainParamsRepository, IMapper mapper, ILogger<TrainingRunTrainParamsService> logger)
        {
            _trainingRunTrainParamsRepository = trainingRunTrainParamsRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResultDTO<TrainingRunTrainParamsDTO>> CreateTrainingRunTrainParams(int numEpochs, int numBatchSize, int numFrozenStages, Guid trainingRunId, Guid trainParamsId)
        {
            try
            {
                TrainingRunTrainParams trainingRunTrainParams = new()
                {
                    Id = trainParamsId,
                    BatchSize = numBatchSize,
                    NumEpochs = numEpochs,
                    NumFrozenStages = numFrozenStages,
                    TrainingRunId = trainingRunId
                };

                ResultDTO<TrainingRunTrainParams> createTrainingRunTrainParams = await _trainingRunTrainParamsRepository.CreateAndReturnEntity(trainingRunTrainParams);
                if (createTrainingRunTrainParams.IsSuccess == false && createTrainingRunTrainParams.HandleError())
                    return ResultDTO<TrainingRunTrainParamsDTO>.Fail(createTrainingRunTrainParams.ErrMsg!);
                if(createTrainingRunTrainParams.Data == null)                
                    return ResultDTO<TrainingRunTrainParamsDTO>.Fail("No params found");                

                TrainingRunTrainParamsDTO? trainingRunTrainParamsDTO = _mapper.Map<TrainingRunTrainParamsDTO>(createTrainingRunTrainParams.Data);
                if (trainingRunTrainParamsDTO == null)
                    return ResultDTO<TrainingRunTrainParamsDTO>.Fail("Mapping failed");

                return ResultDTO<TrainingRunTrainParamsDTO>.Ok(trainingRunTrainParamsDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<TrainingRunTrainParamsDTO>.ExceptionFail(ex.Message, ex);
            }
        }
    }
}

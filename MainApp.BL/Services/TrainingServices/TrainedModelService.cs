using AutoMapper;
using DAL.Interfaces.Repositories.TrainingRepositories;
using DTOs.MainApp.BL.TrainingDTOs;
using Entities.TrainingEntities;
using MainApp.BL.Interfaces.Services;
using MainApp.BL.Interfaces.Services.TrainingServices;
using Microsoft.Extensions.Logging;
using SD;

namespace MainApp.BL.Services.TrainingServices
{
    public class TrainedModelService : ITrainedModelService
    {
        private readonly ITrainedModelsRepository _trainedModelsRepository;
        
        private readonly IMapper _mapper;
        private readonly ILogger<TrainingRunService> _logger;

        protected readonly IMMDetectionConfigurationService _MMDetectionConfiguration;
        private readonly ITrainingRunService _trainingRunService;

        public TrainedModelService(ITrainedModelsRepository trainedModelsRepository, IMapper mapper, ILogger<TrainingRunService> logger, 
            IMMDetectionConfigurationService mMDetectionConfiguration, ITrainingRunService trainingRunService)
        {
            _trainedModelsRepository = trainedModelsRepository;
            _mapper = mapper;
            _logger = logger;
            _MMDetectionConfiguration = mMDetectionConfiguration;
            _trainingRunService = trainingRunService;
        }

        public async Task<ResultDTO<TrainedModelDTO>> GetTrainedModelById(Guid id, bool track = false)
        {
            try
            {
                ResultDTO<TrainedModel?> resultGetById =
                    await _trainedModelsRepository.GetById(id, track: track, includeProperties: "CreatedBy");

                if (resultGetById.IsSuccess == false && resultGetById.HandleError())
                    return ResultDTO<TrainedModelDTO>.Fail(resultGetById.ErrMsg!);

                TrainedModelDTO trainedModelDTO = _mapper.Map<TrainedModelDTO>(resultGetById.Data);
                if (trainedModelDTO is null)
                    return ResultDTO<TrainedModelDTO>.Fail("Failed mapping to DTO for Trained Model");

                return ResultDTO<TrainedModelDTO>.Ok(trainedModelDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<TrainedModelDTO>.ExceptionFail(ex.Message, ex);
            }
        }

        // TODO: Implement
        public async Task<ResultDTO<List<TrainedModelDTO>>> GetAllTrainedModels()
        {
            throw new NotImplementedException();
        }

        // TODO: Implement
        public Task<ResultDTO<List<TrainedModelDTO>>> GetAllTrainedModelsIncludingTrainRuns()
        {
            throw new NotImplementedException();
        }

        public async Task<ResultDTO<List<TrainedModelDTO>>> GetPublishedTrainedModelsIncludingTrainRuns()
        {
            // TODO: Return comment, for only published models
            ResultDTO<IEnumerable<TrainedModel>> getPublishedModelsResult = 
                await _trainedModelsRepository.GetAll(/*x => x.IsPublished, */track: false, includeProperties: "TrainingRun");
            if (getPublishedModelsResult.IsSuccess == false && getPublishedModelsResult.HandleError())
                return ResultDTO<List<TrainedModelDTO>>.Fail(getPublishedModelsResult.ErrMsg!);

            if(getPublishedModelsResult.Data is null || getPublishedModelsResult.Data.Count() == 0)
                return ResultDTO<List<TrainedModelDTO>>.Fail("No Published Trained Models found");

            List<TrainedModelDTO>? publishedModelsDtos = _mapper.Map<List<TrainedModelDTO>>(getPublishedModelsResult.Data);
            if(publishedModelsDtos is null)
                return ResultDTO<List<TrainedModelDTO>>.Fail("Mapping failed from List<TrainedModels> to DTO");

            return ResultDTO<List<TrainedModelDTO>>.Ok(publishedModelsDtos);
        }

        public async Task<ResultDTO<TrainingRunResultsDTO>> GetBestEpochForTrainedModelById(Guid trainedModelId)
        {
            try
            {
                ResultDTO<TrainedModelDTO> getTrainedModelResult = await GetTrainedModelById(trainedModelId, false);
                if(getTrainedModelResult.IsSuccess == false && getTrainedModelResult.HandleError())
                    return ResultDTO<TrainingRunResultsDTO>.Fail(getTrainedModelResult.ErrMsg!);

                TrainedModelDTO trainedModel = getTrainedModelResult.Data!;

                ResultDTO<TrainingRunResultsDTO> getBestEpochResult = 
                    _trainingRunService.GetBestEpochForTrainingRun(trainedModel.TrainingRunId!.Value);
                if(getBestEpochResult.IsSuccess == false && getBestEpochResult.HandleError())
                    return ResultDTO<TrainingRunResultsDTO>.Fail(getBestEpochResult.ErrMsg!);

                return ResultDTO<TrainingRunResultsDTO>.Ok(getBestEpochResult.Data!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<TrainingRunResultsDTO>.ExceptionFail(ex.Message, ex);
            }
        }

        // TODO: Implement 
        public async Task<ResultDTO<TrainedModel>> EditTrainedModelById(Guid trainedModelId)
        {
            // Allowed Update only for Name and IsPublished
            throw new NotImplementedException();
        }

        // TODO: Implement , Delete files as well
        public async Task<ResultDTO> DeleteTrainedModelById(Guid trainedModelId)
        {
            throw new NotImplementedException();
        }
    }
}

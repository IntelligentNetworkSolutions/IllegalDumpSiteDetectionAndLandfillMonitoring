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
                ResultDTO<TrainedModel?>? resultGetById = await _trainedModelsRepository.GetById(id, track: track, includeProperties: "CreatedBy");
                if (resultGetById.IsSuccess == false && resultGetById.HandleError())
                    return ResultDTO<TrainedModelDTO>.Fail(resultGetById.ErrMsg!);
                if (resultGetById.Data == null)
                    return ResultDTO<TrainedModelDTO>.Fail("Trained model not found");

                TrainedModelDTO? trainedModelDTO = _mapper.Map<TrainedModelDTO>(resultGetById.Data);
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

        public async Task<ResultDTO<List<TrainedModelDTO>>> GetAllTrainedModels()
        {
            try
            {
                ResultDTO<IEnumerable<TrainedModel>>? resultGetEntities = await _trainedModelsRepository.GetAll(includeProperties: "CreatedBy");
                if (resultGetEntities.IsSuccess == false && resultGetEntities.HandleError())
                    return ResultDTO<List<TrainedModelDTO>>.Fail(resultGetEntities.ErrMsg!);
                if (resultGetEntities.Data == null)                
                    return ResultDTO<List<TrainedModelDTO>>.Fail("Trained models not found");
                
                List<TrainedModelDTO>? dto = _mapper.Map<List<TrainedModelDTO>>(resultGetEntities.Data);
                if (dto == null)
                    return ResultDTO<List<TrainedModelDTO>>.Fail("Mapping trained model failed");

                return ResultDTO<List<TrainedModelDTO>>.Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<List<TrainedModelDTO>>.ExceptionFail(ex.Message, ex);
            }
        }
                
        public async Task<ResultDTO<List<TrainedModelDTO>>> GetPublishedTrainedModelsIncludingTrainRuns()
        {
            // TODO: Return comment, for only published models
            ResultDTO<IEnumerable<TrainedModel>> getPublishedModelsResult = await _trainedModelsRepository.GetAll(/*x => x.IsPublished, */track: false, includeProperties: "TrainingRun");
            if (getPublishedModelsResult.IsSuccess == false && getPublishedModelsResult.HandleError())
                return ResultDTO<List<TrainedModelDTO>>.Fail(getPublishedModelsResult.ErrMsg!);

            if (getPublishedModelsResult.Data is null || getPublishedModelsResult.Data.Count() == 0)
                return ResultDTO<List<TrainedModelDTO>>.Fail("No Published Trained Models found");

            List<TrainedModelDTO>? publishedModelsDtos = _mapper.Map<List<TrainedModelDTO>>(getPublishedModelsResult.Data);
            if (publishedModelsDtos is null)
                return ResultDTO<List<TrainedModelDTO>>.Fail("Mapping failed from List<TrainedModels> to DTO");

            return ResultDTO<List<TrainedModelDTO>>.Ok(publishedModelsDtos);
        }

        public async Task<ResultDTO<TrainingRunResultsDTO>> GetBestEpochForTrainedModelById(Guid trainedModelId)
        {
            try
            {
                ResultDTO<TrainedModelDTO>? getTrainedModelResult = await GetTrainedModelById(trainedModelId, false);
                if (getTrainedModelResult.IsSuccess == false && getTrainedModelResult.HandleError())
                    return ResultDTO<TrainingRunResultsDTO>.Fail(getTrainedModelResult.ErrMsg!);
                if (getTrainedModelResult.Data == null)
                    return ResultDTO<TrainingRunResultsDTO>.Fail("Training run results not found");

                TrainedModelDTO trainedModel = getTrainedModelResult.Data;

                ResultDTO<TrainingRunResultsDTO>? getBestEpochResult = _trainingRunService.GetBestEpochForTrainingRun(trainedModel.TrainingRunId!.Value);
                if (getBestEpochResult.IsSuccess == false && getBestEpochResult.HandleError())
                    return ResultDTO<TrainingRunResultsDTO>.Fail(getBestEpochResult.ErrMsg!);
                if (getBestEpochResult.Data == null)
                    return ResultDTO<TrainingRunResultsDTO>.Fail("Training run result not found");

                return ResultDTO<TrainingRunResultsDTO>.Ok(getBestEpochResult.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<TrainingRunResultsDTO>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO> EditTrainedModelById(Guid trainedModelId, string? name = null, bool? isPublished = null)
        {
            try
            {
                ResultDTO<TrainedModel?>? resultGetEntity = await _trainedModelsRepository.GetById(trainedModelId);
                if (resultGetEntity.IsSuccess == false && resultGetEntity.HandleError())
                    return ResultDTO.Fail(resultGetEntity.ErrMsg!);
                if (resultGetEntity.Data is null)
                    return ResultDTO.Fail($"No Trained Model found with ID: {trainedModelId}");

                if (name != null)
                    resultGetEntity.Data.Name = name;
                
                if (isPublished != null)
                    resultGetEntity.Data.IsPublished = (bool)isPublished;
                
                ResultDTO? updateRunResult = await _trainedModelsRepository.Update(resultGetEntity.Data);
                if (updateRunResult.IsSuccess == false && updateRunResult.HandleError())
                    return ResultDTO.Fail(updateRunResult.ErrMsg!);

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO> DeleteTrainedModelById(Guid trainedModelId)
        {
            try
            {
                ResultDTO<TrainedModel?>? resultGetEntity = await _trainedModelsRepository.GetById(trainedModelId);
                if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())
                    return ResultDTO.Fail(resultGetEntity.ErrMsg!);
                if (resultGetEntity.Data is null)
                    return ResultDTO.Fail($"No Trained Model found with ID: {trainedModelId}");

                TrainedModel? trainedModel = resultGetEntity.Data;
                if (trainedModel == null)
                    return ResultDTO.Fail("Trained model not found.");

                if (!string.IsNullOrEmpty(trainedModel.ModelConfigPath) && File.Exists(trainedModel.ModelConfigPath))
                    File.Delete(trainedModel.ModelConfigPath);

                if (!string.IsNullOrEmpty(trainedModel.ModelFilePath) && File.Exists(trainedModel.ModelFilePath))
                    File.Delete(trainedModel.ModelFilePath);

                ResultDTO? deleteResult = await _trainedModelsRepository.Delete(trainedModel);
                if (!deleteResult.IsSuccess && deleteResult.HandleError())
                    return ResultDTO.Fail(deleteResult.ErrMsg!);

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

    }
}

using AutoMapper;
using DAL.Interfaces.Repositories.TrainingRepositories;
using DTOs.MainApp.BL.TrainingDTOs;
using Entities.TrainingEntities;
using MainApp.BL.Interfaces.Services;
using MainApp.BL.Interfaces.Services.TrainingServices;
using MainApp.BL.Services.TrainingServices;
using Microsoft.Extensions.Logging;
using Moq;
using SD;

namespace Tests.MainAppBLTests.Services.TrainingServices
{
    public class TrainedModelServiceTests
    {
        private readonly Mock<ITrainedModelsRepository> _trainedModelsRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<TrainingRunService>> _loggerMock;
        private readonly Mock<IMMDetectionConfigurationService> _mMDetectionConfigurationMock;
        private readonly Mock<ITrainingRunService> _trainingRunServiceMock;
        private readonly TrainedModelService _service;

        public TrainedModelServiceTests()
        {
            _trainedModelsRepositoryMock = new Mock<ITrainedModelsRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<TrainingRunService>>();
            _mMDetectionConfigurationMock = new Mock<IMMDetectionConfigurationService>();
            _trainingRunServiceMock = new Mock<ITrainingRunService>();

            _service = new TrainedModelService(
                _trainedModelsRepositoryMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _mMDetectionConfigurationMock.Object,
                _trainingRunServiceMock.Object
            );
        }

        [Fact]
        public async Task GetTrainedModelById_Returns_Success_When_Model_Exists()
        {
            var modelId = Guid.NewGuid();
            var trainedModel = new TrainedModel { Id = modelId };
            var trainedModelDTO = new TrainedModelDTO { Id = modelId };

            _trainedModelsRepositoryMock.Setup(repo => repo.GetById(modelId, false, "CreatedBy"))
                .ReturnsAsync(ResultDTO<TrainedModel>.Ok(trainedModel));

            _mapperMock.Setup(mapper => mapper.Map<TrainedModelDTO>(trainedModel))
                .Returns(trainedModelDTO);

            var result = await _service.GetTrainedModelById(modelId);

            Assert.True(result.IsSuccess);
            Assert.Equal(modelId, result.Data.Id);
        }

        [Fact]
        public async Task GetTrainedModelById_Returns_Failure_When_Model_Not_Found()
        {
            var modelId = Guid.NewGuid();

            _trainedModelsRepositoryMock.Setup(repo => repo.GetById(modelId, false, "CreatedBy"))
                .ReturnsAsync(ResultDTO<TrainedModel>.Fail("Model not found"));

            var result = await _service.GetTrainedModelById(modelId);

            Assert.False(result.IsSuccess);
            Assert.Equal("Model not found", result.ErrMsg);
        }

        [Fact]
        public async Task GetTrainedModelById_Returns_Failure_When_Mapping_Fails()
        {
            var modelId = Guid.NewGuid();
            var trainedModel = new TrainedModel { Id = modelId };

            _trainedModelsRepositoryMock.Setup(repo => repo.GetById(modelId, false, "CreatedBy"))
                .ReturnsAsync(ResultDTO<TrainedModel>.Ok(trainedModel));

            _mapperMock.Setup(mapper => mapper.Map<TrainedModelDTO>(trainedModel))
                .Returns((TrainedModelDTO)null);

            var result = await _service.GetTrainedModelById(modelId);

            Assert.False(result.IsSuccess);
            Assert.Equal("Failed mapping to DTO for Trained Model", result.ErrMsg);
        }

        [Fact]
        public async Task GetTrainedModelById_Logs_Error_On_Exception()
        {
            var modelId = Guid.NewGuid();

            _trainedModelsRepositoryMock.Setup(repo => repo.GetById(modelId, false, "CreatedBy"))
                .ThrowsAsync(new Exception("Database error"));

            var result = await _service.GetTrainedModelById(modelId);

            Assert.False(result.IsSuccess);
            Assert.Equal("Database error", result.ErrMsg);
        }

        [Fact]
        public async Task GetAllTrainedModels_Returns_Success_When_Models_Exist()
        {
            var trainedModels = new List<TrainedModel> { new TrainedModel { Id = Guid.NewGuid() } };
            var trainedModelDTOs = new List<TrainedModelDTO> { new TrainedModelDTO { Id = trainedModels[0].Id } };

            _trainedModelsRepositoryMock.Setup(repo => repo.GetAll(null, null, false, "CreatedBy", null))
                .ReturnsAsync(ResultDTO<IEnumerable<TrainedModel>>.Ok(trainedModels));

            _mapperMock.Setup(mapper => mapper.Map<List<TrainedModelDTO>>(trainedModels))
                .Returns(trainedModelDTOs);

            var result = await _service.GetAllTrainedModels();

            Assert.True(result.IsSuccess);
            Assert.Equal(trainedModelDTOs.Count, result.Data.Count);
        }

        [Fact]
        public async Task GetAllTrainedModels_Returns_Failure_When_No_Models_Found()
        {
            _trainedModelsRepositoryMock.Setup(repo => repo.GetAll(null, null, false, "CreatedBy", null))
                .ReturnsAsync(ResultDTO<IEnumerable<TrainedModel>>.Ok(null));

            var result = await _service.GetAllTrainedModels();

            Assert.False(result.IsSuccess);
            Assert.Equal("Trained models not found", result.ErrMsg);
        }

        [Fact]
        public async Task GetAllTrainedModels_Returns_Failure_When_GetAll_Fails()
        {
            _trainedModelsRepositoryMock.Setup(repo => repo.GetAll(null, null, false, "CreatedBy", null))
                .ReturnsAsync(ResultDTO<IEnumerable<TrainedModel>>.Fail("Error retrieving models"));

            var result = await _service.GetAllTrainedModels();

            Assert.False(result.IsSuccess);
            Assert.Equal("Error retrieving models", result.ErrMsg);
        }

        [Fact]
        public async Task GetAllTrainedModels_Logs_Error_On_Exception()
        {
            _trainedModelsRepositoryMock.Setup(repo => repo.GetAll(null, null, false, "CreatedBy", null))
                .ThrowsAsync(new Exception("Database error"));

            var result = await _service.GetAllTrainedModels();

            Assert.False(result.IsSuccess);
            Assert.Equal("Database error", result.ErrMsg);
        }

        [Fact]
        public async Task GetPublishedTrainedModelsIncludingTrainRuns_Returns_Success_When_Published_Models_Exist()
        {
            var trainedModels = new List<TrainedModel> { new TrainedModel { Id = Guid.NewGuid(), IsPublished = true } };
            var trainedModelDTOs = new List<TrainedModelDTO> { new TrainedModelDTO { Id = trainedModels[0].Id } };

            _trainedModelsRepositoryMock.Setup(repo => repo.GetAll(null, null, false, "TrainingRun", null))
                .ReturnsAsync(ResultDTO<IEnumerable<TrainedModel>>.Ok(trainedModels));

            _mapperMock.Setup(mapper => mapper.Map<List<TrainedModelDTO>>(trainedModels))
                .Returns(trainedModelDTOs);

            var result = await _service.GetPublishedTrainedModelsIncludingTrainRuns();

            Assert.True(result.IsSuccess);
            Assert.Equal(trainedModelDTOs.Count, result.Data.Count);
        }

        [Fact]
        public async Task GetPublishedTrainedModelsIncludingTrainRuns_Returns_Failure_When_No_Published_Models_Found()
        {
            _trainedModelsRepositoryMock.Setup(repo => repo.GetAll(null, null, false, "TrainingRun", null))
                .ReturnsAsync(ResultDTO<IEnumerable<TrainedModel>>.Ok(new List<TrainedModel>()));

            var result = await _service.GetPublishedTrainedModelsIncludingTrainRuns();

            Assert.False(result.IsSuccess);
            Assert.Equal("No Published Trained Models found", result.ErrMsg);
        }

        [Fact]
        public async Task GetPublishedTrainedModelsIncludingTrainRuns_Returns_Failure_When_GetAll_Fails()
        {
            _trainedModelsRepositoryMock.Setup(repo => repo.GetAll(null, null, false, "TrainingRun", null))
                .ReturnsAsync(ResultDTO<IEnumerable<TrainedModel>>.Fail("Error retrieving models"));

            var result = await _service.GetPublishedTrainedModelsIncludingTrainRuns();

            Assert.False(result.IsSuccess);
            Assert.Equal("Error retrieving models", result.ErrMsg);
        }

        [Fact]
        public async Task GetPublishedTrainedModelsIncludingTrainRuns_Logs_Error_On_Exception()
        {
            _trainedModelsRepositoryMock.Setup(repo => repo.GetAll(null, null, false, "TrainingRun", null))
                .ThrowsAsync(new Exception("Database error"));

            var exception = await Assert.ThrowsAsync<Exception>(async () => await _service.GetPublishedTrainedModelsIncludingTrainRuns());
            Assert.Equal("Database error", exception.Message);

        }

        [Fact]
        public async Task GetPublishedTrainedModelsIncludingTrainRuns_Returns_Failure_When_Mapping_Fails()
        {
            var trainedModels = new List<TrainedModel> { new TrainedModel { Id = Guid.NewGuid(), IsPublished = true } };

            _trainedModelsRepositoryMock.Setup(repo => repo.GetAll(null, null, false, "TrainingRun", null))
                .ReturnsAsync(ResultDTO<IEnumerable<TrainedModel>>.Ok(trainedModels));

            _mapperMock.Setup(mapper => mapper.Map<List<TrainedModelDTO>>(trainedModels))
                .Returns((List<TrainedModelDTO>)null);

            var result = await _service.GetPublishedTrainedModelsIncludingTrainRuns();

            Assert.False(result.IsSuccess);
            Assert.Equal("Mapping failed from List<TrainedModels> to DTO", result.ErrMsg);
        }

        [Fact]
        public async Task GetBestEpochForTrainedModelById_Returns_Success_When_Valid_TrainedModelId_Provided()
        {
            var trainedModelId = Guid.NewGuid();
            var trainedModel = new TrainedModel { TrainingRunId = Guid.NewGuid() };
            var trainedModelDTO = new TrainedModelDTO { TrainingRunId = trainedModel.TrainingRunId };
            var trainingRunResultsDTO = new TrainingRunResultsDTO();

            _trainedModelsRepositoryMock.Setup(repo => repo.GetById(trainedModelId, false, "CreatedBy"))
                .ReturnsAsync(ResultDTO<TrainedModel>.Ok(trainedModel));
            _mapperMock.Setup(mapper => mapper.Map<TrainedModelDTO>(trainedModel))
                .Returns(trainedModelDTO);

            _trainingRunServiceMock.Setup(service => service.GetBestEpochForTrainingRun(trainedModel.TrainingRunId.Value))
                .Returns(ResultDTO<TrainingRunResultsDTO>.Ok(trainingRunResultsDTO));

            var result = await _service.GetBestEpochForTrainedModelById(trainedModelId);

            Assert.True(result.IsSuccess);
            Assert.Equal(trainingRunResultsDTO, result.Data);
        }

        [Fact]
        public async Task GetBestEpochForTrainedModelById_Returns_Failure_When_TrainedModel_Not_Found()
        {
            var trainedModelId = Guid.NewGuid();

            _trainedModelsRepositoryMock.Setup(repo => repo.GetById(trainedModelId, false, "CreatedBy"))
                .ReturnsAsync(ResultDTO<TrainedModel>.Fail("Trained Model not found"));

            var result = await _service.GetBestEpochForTrainedModelById(trainedModelId);

            Assert.False(result.IsSuccess);
            Assert.Equal("Trained Model not found", result.ErrMsg);
        }

        [Fact]
        public async Task GetBestEpochForTrainedModelById_Returns_Failure_When_GetBestEpoch_Fails()
        {
            var trainedModelId = Guid.NewGuid();
            var trainedModel = new TrainedModel { Id = trainedModelId, Name = "TrainedModelName", ModelConfigPath = "Model-config-path", ModelFilePath = "model-file-path", TrainingRunId = Guid.NewGuid(), BaseModelId = Guid.NewGuid(), TrainedModelStatisticsId = Guid.NewGuid(), };
            var dto = new TrainedModelDTO { Id = trainedModel.Id, Name = trainedModel.Name, ModelConfigPath = trainedModel.ModelConfigPath, ModelFilePath = trainedModel.ModelFilePath, TrainingRunId = trainedModel.TrainingRunId, BaseModelId = trainedModel.BaseModelId, TrainedModelStatisticsId = trainedModel.TrainedModelStatisticsId };
            _trainedModelsRepositoryMock.Setup(repo => repo.GetById(trainedModelId, false, "CreatedBy"))
                .ReturnsAsync(ResultDTO<TrainedModel>.Ok(trainedModel));
            _mapperMock.Setup(mapper => mapper.Map<TrainedModelDTO>(trainedModel))
                .Returns(dto);

            _trainingRunServiceMock.Setup(service => service.GetBestEpochForTrainingRun(trainedModel.TrainingRunId.Value))
                .Returns(ResultDTO<TrainingRunResultsDTO>.Fail("Error getting best epoch"));

            var result = await _service.GetBestEpochForTrainedModelById(trainedModelId);

            Assert.False(result.IsSuccess);
            Assert.Equal("Error getting best epoch", result.ErrMsg);
        }

        //[Fact]
        //public async Task GetBestEpochForTrainedModelById_Logs_Error_On_Exception()
        //{
        //    var trainedModelId = Guid.NewGuid();

        //    _trainedModelsRepositoryMock.Setup(repo => repo.GetById(trainedModelId, false, "CreatedBy"))
        //        .ThrowsAsync(new Exception("Database error"));


        //    var exception = await Assert.ThrowsAsync<Exception>(async () => await _service.GetPublishedTrainedModelsIncludingTrainRuns());
        //    Assert.Equal("Database error", exception.Message);
        //}


        [Fact]
        public async Task DeleteTrainedModelById_Returns_Success_When_Model_Exists_And_Deleted()
        {
            var trainedModelId = Guid.NewGuid();
            var trainedModel = new TrainedModel { ModelConfigPath = "configPath", ModelFilePath = "modelPath" };

            _trainedModelsRepositoryMock.Setup(repo => repo.GetById(trainedModelId, false, null))
                .ReturnsAsync(ResultDTO<TrainedModel>.Ok(trainedModel));

            _trainedModelsRepositoryMock.Setup(repo => repo.Delete(trainedModel, true, default))
                .ReturnsAsync(ResultDTO.Ok());

            var result = await _service.DeleteTrainedModelById(trainedModelId);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteTrainedModelById_Returns_Failure_When_TrainedModel_Not_Found()
        {
            var trainedModelId = Guid.NewGuid();

            _trainedModelsRepositoryMock.Setup(repo => repo.GetById(trainedModelId, false, null))
                .ReturnsAsync(ResultDTO<TrainedModel>.Fail("Trained model not found."));

            var result = await _service.DeleteTrainedModelById(trainedModelId);

            Assert.False(result.IsSuccess);
            Assert.Equal("Trained model not found.", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteTrainedModelById_Returns_Failure_When_Delete_Fails()
        {
            var trainedModelId = Guid.NewGuid();
            var trainedModel = new TrainedModel();

            _trainedModelsRepositoryMock.Setup(repo => repo.GetById(trainedModelId, false, null))
                .ReturnsAsync(ResultDTO<TrainedModel>.Ok(trainedModel));

            _trainedModelsRepositoryMock.Setup(repo => repo.Delete(trainedModel, true, default))
                .ReturnsAsync(ResultDTO.Fail("Error deleting trained model."));

            var result = await _service.DeleteTrainedModelById(trainedModelId);

            Assert.False(result.IsSuccess);
            Assert.Equal("Error deleting trained model.", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteTrainedModelById_Deletes_Files_When_They_Exist()
        {
            var trainedModelId = Guid.NewGuid();
            var trainedModel = new TrainedModel { ModelConfigPath = "configPath", ModelFilePath = "modelPath" };

            _trainedModelsRepositoryMock.Setup(repo => repo.GetById(trainedModelId, false, null))
                .ReturnsAsync(ResultDTO<TrainedModel>.Ok(trainedModel));

            _trainedModelsRepositoryMock.Setup(repo => repo.Delete(trainedModel, true, default))
                .ReturnsAsync(ResultDTO.Ok());

            var result = await _service.DeleteTrainedModelById(trainedModelId);

            Assert.True(result.IsSuccess);
            Assert.True(File.Exists("configPath") == false);
            Assert.True(File.Exists("modelPath") == false);
        }

        [Fact]
        public async Task DeleteTrainedModelById_Logs_Error_On_Exception()
        {
            var trainedModelId = Guid.NewGuid();

            _trainedModelsRepositoryMock.Setup(repo => repo.GetById(trainedModelId, false, null))
                .ThrowsAsync(new Exception("Database error"));

            var result = await _service.DeleteTrainedModelById(trainedModelId);

            Assert.False(result.IsSuccess);
            Assert.Equal("Database error", result.ErrMsg);
        }

        [Fact]
        public async Task EditTrainedModelById_Returns_Success_When_Model_Exists_And_Updated()
        {
            var trainedModelId = Guid.NewGuid();
            var trainedModel = new TrainedModel { Name = "Old Name" };

            _trainedModelsRepositoryMock.Setup(repo => repo.GetById(trainedModelId, false, null))
                .ReturnsAsync(ResultDTO<TrainedModel>.Ok(trainedModel));

            _trainedModelsRepositoryMock.Setup(repo => repo.Update(trainedModel, true, default))
                .ReturnsAsync(ResultDTO.Ok());

            var result = await _service.EditTrainedModelById(trainedModelId, "New Name", true);

            Assert.True(result.IsSuccess);
            Assert.Equal("New Name", trainedModel.Name);
            Assert.True(trainedModel.IsPublished);
        }

        [Fact]
        public async Task EditTrainedModelById_Returns_Failure_When_TrainedModel_Not_Found()
        {
            var trainedModelId = Guid.NewGuid();

            _trainedModelsRepositoryMock.Setup(repo => repo.GetById(trainedModelId, false, null))
                .ReturnsAsync(ResultDTO<TrainedModel>.Fail($"No Trained Model found with ID: {trainedModelId}"));

            var result = await _service.EditTrainedModelById(trainedModelId);

            Assert.False(result.IsSuccess);
            Assert.Equal($"No Trained Model found with ID: {trainedModelId}", result.ErrMsg);
        }

        [Fact]
        public async Task EditTrainedModelById_Returns_Failure_When_Update_Fails()
        {
            var trainedModelId = Guid.NewGuid();
            var trainedModel = new TrainedModel();

            _trainedModelsRepositoryMock.Setup(repo => repo.GetById(trainedModelId, false, null))
                .ReturnsAsync(ResultDTO<TrainedModel>.Ok(trainedModel));

            _trainedModelsRepositoryMock.Setup(repo => repo.Update(trainedModel, true, default))
                .ReturnsAsync(ResultDTO.Fail("Error updating trained model."));

            var result = await _service.EditTrainedModelById(trainedModelId);

            Assert.False(result.IsSuccess);
            Assert.Equal("Error updating trained model.", result.ErrMsg);
        }

        [Fact]
        public async Task EditTrainedModelById_Updates_Name_When_Provided()
        {
            var trainedModelId = Guid.NewGuid();
            var trainedModel = new TrainedModel { Name = "Old Name" };

            _trainedModelsRepositoryMock.Setup(repo => repo.GetById(trainedModelId, false, null))
                .ReturnsAsync(ResultDTO<TrainedModel>.Ok(trainedModel));
            _trainedModelsRepositoryMock.Setup(repo => repo.Update(trainedModel, true, default))
                .ReturnsAsync(ResultDTO.Ok());

            var result = await _service.EditTrainedModelById(trainedModelId, name: "New Name");

            Assert.True(result.IsSuccess);
            Assert.Equal("New Name", trainedModel.Name);
        }

        [Fact]
        public async Task EditTrainedModelById_Updates_IsPublished_When_Provided()
        {
            var trainedModelId = Guid.NewGuid();
            var isPublished = true;
            var trainedModel = new TrainedModel { Id = trainedModelId, IsPublished = false };

            _trainedModelsRepositoryMock.Setup(repo => repo.GetById(trainedModelId, false, null))
                .ReturnsAsync(ResultDTO<TrainedModel>.Ok(trainedModel));
            _trainedModelsRepositoryMock.Setup(repo => repo.Update(trainedModel, true, default))
                .ReturnsAsync(ResultDTO.Ok());

            var result = await _service.EditTrainedModelById(trainedModelId, null, isPublished);

            Assert.True(result.IsSuccess);
            Assert.True(trainedModel.IsPublished);
        }

        [Fact]
        public async Task EditTrainedModelById_Logs_Error_On_Exception()
        {
            var trainedModelId = Guid.NewGuid();

            _trainedModelsRepositoryMock.Setup(repo => repo.GetById(trainedModelId, false, null))
                .ThrowsAsync(new Exception("Database error"));

            var result = await _service.EditTrainedModelById(trainedModelId);

            Assert.False(result.IsSuccess);
            Assert.Equal("Database error", result.ErrMsg);
        }


    }
}

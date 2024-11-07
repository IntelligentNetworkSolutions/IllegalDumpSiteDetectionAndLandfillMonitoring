using AutoMapper;
using DAL.Interfaces.Repositories.TrainingRepositories;
using DTOs.MainApp.BL.TrainingDTOs;
using Entities.TrainingEntities;
using MainApp.BL.Services.TrainingServices;
using Microsoft.Extensions.Logging;
using Moq;
using SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppBLTests.Services.TrainingServices
{
    public class TrainingRunTrainParamsServiceTests
    {
        private readonly Mock<ITrainingRunTrainParamsRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<TrainingRunTrainParamsService>> _mockLogger;
        private readonly TrainingRunTrainParamsService _service;

        public TrainingRunTrainParamsServiceTests()
        {
            _mockRepository = new Mock<ITrainingRunTrainParamsRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<TrainingRunTrainParamsService>>();
            _service = new TrainingRunTrainParamsService(_mockRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateTrainingRunTrainParams_ReturnsSuccess_WhenParametersAreValid()
        {
            // Arrange
            var trainingRunId = Guid.NewGuid();
            var trainParamsId = Guid.NewGuid();
            var trainingRunTrainParams = new TrainingRunTrainParams
            {
                Id = trainParamsId,
                BatchSize = 32,
                NumEpochs = 10,
                NumFrozenStages = 2,
                TrainingRunId = trainingRunId
            };
            var trainingRunTrainParamsDTO = new TrainingRunTrainParamsDTO
            {
                Id = trainParamsId,
                BatchSize = 32,
                NumEpochs = 10,
                NumFrozenStages = 2,
                TrainingRunId = trainingRunId
            };

            _mockRepository
                .Setup(repo => repo.CreateAndReturnEntity(It.IsAny<TrainingRunTrainParams>(), true, default))
                .ReturnsAsync(ResultDTO<TrainingRunTrainParams>.Ok(trainingRunTrainParams));

            _mockMapper
                .Setup(mapper => mapper.Map<TrainingRunTrainParamsDTO>(trainingRunTrainParams))
                .Returns(trainingRunTrainParamsDTO);

            // Act
            var result = await _service.CreateTrainingRunTrainParams(10, 32, 2, trainingRunId, trainParamsId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(trainingRunTrainParamsDTO, result.Data);
        }

        [Fact]
        public async Task CreateTrainingRunTrainParams_ReturnsFail_WhenRepositoryFails()
        {
            // Arrange
            var trainingRunId = Guid.NewGuid();
            var trainParamsId = Guid.NewGuid();

            _mockRepository
                .Setup(repo => repo.CreateAndReturnEntity(It.IsAny<TrainingRunTrainParams>(), true, default))
                .ReturnsAsync(ResultDTO<TrainingRunTrainParams>.Fail("Repository error"));

            // Act
            var result = await _service.CreateTrainingRunTrainParams(10, 32, 2, trainingRunId, trainParamsId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Repository error", result.ErrMsg);
        }

        [Fact]
        public async Task CreateTrainingRunTrainParams_ReturnsFail_WhenMappingFails()
        {
            // Arrange
            var trainingRunId = Guid.NewGuid();
            var trainParamsId = Guid.NewGuid();
            var trainingRunTrainParams = new TrainingRunTrainParams
            {
                Id = trainParamsId,
                BatchSize = 32,
                NumEpochs = 10,
                NumFrozenStages = 2,
                TrainingRunId = trainingRunId
            };

            _mockRepository
                .Setup(repo => repo.CreateAndReturnEntity(It.IsAny<TrainingRunTrainParams>(), true, default))
                .ReturnsAsync(ResultDTO<TrainingRunTrainParams>.Ok(trainingRunTrainParams));

            _mockMapper
                .Setup(mapper => mapper.Map<TrainingRunTrainParamsDTO>(trainingRunTrainParams))
                .Returns((TrainingRunTrainParamsDTO)null);

            // Act
            var result = await _service.CreateTrainingRunTrainParams(10, 32, 2, trainingRunId, trainParamsId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Mapping failed", result.ErrMsg);
        }

        [Fact]
        public async Task CreateTrainingRunTrainParams_ReturnsExceptionFail_OnException()
        {
            // Arrange
            var trainingRunId = Guid.NewGuid();
            var trainParamsId = Guid.NewGuid();

            _mockRepository
                .Setup(repo => repo.CreateAndReturnEntity(It.IsAny<TrainingRunTrainParams>(), true, default))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _service.CreateTrainingRunTrainParams(10, 32, 2, trainingRunId, trainParamsId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Unexpected error", result.ErrMsg);
        }
    }
}

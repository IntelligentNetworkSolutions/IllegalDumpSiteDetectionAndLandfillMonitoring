using AutoMapper;
using DTOs.MainApp.BL.TrainingDTOs;
using Entities.TrainingEntities;
using MainApp.BL.Mappers;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppBLTests.Mappers
{
    public class TrainingProfileBLTests
    {
        private readonly IMapper _mapper;

        public TrainingProfileBLTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<TrainingProfileBL>());
            _mapper = config.CreateMapper();
        }

        [Fact]
        public void Should_Map_TrainingRunDTO_To_TrainingRun()
        {
            // Arrange
            var trainingRunDto = new TrainingRunDTO
            {
                Id = Guid.NewGuid(),
                Name = "Test Run",
                IsCompleted = true,
                Status = "Completed",
                DatasetId = Guid.NewGuid(),
                CreatedById = "user123",
                CreatedOn = DateTime.UtcNow
            };

            // Act
            var trainingRun = _mapper.Map<TrainingRun>(trainingRunDto);

            // Assert
            Assert.NotNull(trainingRun);
            Assert.Equal(trainingRunDto.Name, trainingRun.Name);
            Assert.Equal(trainingRunDto.IsCompleted, trainingRun.IsCompleted);
            Assert.Equal(trainingRunDto.Status, trainingRun.Status);
            Assert.Equal(trainingRunDto.DatasetId, trainingRun.DatasetId);
            Assert.Equal(trainingRunDto.CreatedById, trainingRun.CreatedById);
        }

        [Fact]
        public void Should_Map_TrainingRun_To_TrainingRunDTO()
        {
            // Arrange
            var trainingRun = new TrainingRun
            {
                Id = Guid.NewGuid(),
                Name = "Test Run",
                IsCompleted = true,
                Status = "Completed",
                DatasetId = Guid.NewGuid(),
                CreatedById = "user123",
                CreatedOn = DateTime.UtcNow
            };

            // Act
            var trainingRunDto = _mapper.Map<TrainingRunDTO>(trainingRun);

            // Assert
            Assert.NotNull(trainingRunDto);
            Assert.Equal(trainingRun.Name, trainingRunDto.Name);
            Assert.Equal(trainingRun.IsCompleted, trainingRunDto.IsCompleted);
            Assert.Equal(trainingRun.Status, trainingRunDto.Status);
            Assert.Equal(trainingRun.DatasetId, trainingRunDto.DatasetId);
            Assert.Equal(trainingRun.CreatedById, trainingRunDto.CreatedById);
        }

        [Fact]
        public void Should_Map_TrainedModelDTO_To_TrainedModel()
        {
            // Arrange
            var trainedModelDto = new TrainedModelDTO
            {
                Id = Guid.NewGuid(),
                Name = "Test Model",
                IsPublished = true,
                DatasetId = Guid.NewGuid(),
                CreatedById = "user123",
                CreatedOn = DateTime.UtcNow
            };

            // Act
            var trainedModel = _mapper.Map<TrainedModel>(trainedModelDto);

            // Assert
            Assert.NotNull(trainedModel);
            Assert.Equal(trainedModelDto.Name, trainedModel.Name);
            Assert.Equal(trainedModelDto.IsPublished, trainedModel.IsPublished);
        }

        [Fact]
        public void Should_Map_TrainedModel_To_TrainedModelDTO()
        {
            // Arrange
            var trainedModel = new TrainedModel
            {
                Id = Guid.NewGuid(),
                Name = "Test Model",
                IsPublished = true,
                DatasetId = Guid.NewGuid(),
                CreatedById = "user123",
                CreatedOn = DateTime.UtcNow
            };

            // Act
            var trainedModelDto = _mapper.Map<TrainedModelDTO>(trainedModel);

            // Assert
            Assert.NotNull(trainedModelDto);
            Assert.Equal(trainedModel.Name, trainedModelDto.Name);
        }

        [Fact]
        public void Should_Map_TrainedModelStatisticsDTO_To_TrainedModelStatistics()
        {
            // Arrange
            var statisticsDto = new TrainedModelStatisticsDTO
            {
                Id = Guid.NewGuid(),
                TrainedModelId = Guid.NewGuid(),
                TrainingDuration = TimeSpan.FromHours(1),
                TotalParameters = 1000,
                NumEpochs = 10,
                LearningRate = 0.001,
                AvgBoxLoss = 0.05,
                mApp = 0.9
            };

            // Act 
            var statisticsEntity = _mapper.Map<TrainedModelStatistics>(statisticsDto);

            // Assert 
            Assert.NotNull(statisticsEntity);
            Assert.Equal(statisticsDto.TrainedModelId, statisticsEntity.TrainedModelId);
        }
    }
}

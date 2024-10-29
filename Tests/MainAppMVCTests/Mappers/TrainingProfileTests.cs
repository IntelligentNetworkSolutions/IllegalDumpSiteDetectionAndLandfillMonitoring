using AutoMapper;
using DTOs.MainApp.BL.TrainingDTOs;
using DTOs.MainApp.BL;
using MainApp.MVC.Mappers;
using MainApp.MVC.ViewModels.IntranetPortal.Training;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppMVCTests.Mappers
{
    public class TrainingProfileTests
    {
        private readonly IMapper _mapper;

        public TrainingProfileTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<TrainingProfile>());
            _mapper = config.CreateMapper();
        }

        [Fact]
        public void Map_TrainingRunDTO_To_TrainingRunViewModel_Should_Map_Required_Properties()
        {
            // Arrange
            var dto = new TrainingRunDTO
            {
                Id = Guid.NewGuid(),
                Name = "Test Training Run",
                TrainedModelId = Guid.NewGuid(),
                DatasetId = Guid.NewGuid()
            };

            // Act
            var viewModel = _mapper.Map<TrainingRunViewModel>(dto);

            // Assert
            Assert.NotNull(viewModel);
            Assert.Equal(dto.Name, viewModel.Name);
            Assert.Equal(dto.TrainedModelId.Value, viewModel.TrainedModelId);
            Assert.Equal(dto.DatasetId.Value, viewModel.DatasetId);
        }

        [Fact]
        public void Map_TrainingRunDTO_To_TrainingRunIndexViewModel_Should_Map_All_Properties()
        {
            // Arrange
            var createdOn = DateTime.UtcNow;
            var dto = new TrainingRunDTO
            {
                Id = Guid.NewGuid(),
                Name = "Test Training Run",
                IsCompleted = true,
                Status = "Completed",
                CreatedById = "user1",
                CreatedOn = createdOn,
                CreatedBy = new UserDTO()
            };

            // Act
            var viewModel = _mapper.Map<TrainingRunIndexViewModel>(dto);

            // Assert
            Assert.NotNull(viewModel);
            Assert.Equal(dto.Id.Value, viewModel.Id);
            Assert.Equal(dto.Name, viewModel.Name);
            Assert.Equal(dto.IsCompleted, viewModel.IsCompleted);
            Assert.Equal(dto.Status, viewModel.Status);
            Assert.Equal(dto.CreatedById, viewModel.CreatedById);
            Assert.Equal(dto.CreatedOn, viewModel.CreatedOn);
            Assert.NotNull(viewModel.CreatedBy);
        }

        [Fact]
        public void Map_TrainedModelDTO_To_TrainedModelViewModel_Should_Map_All_Properties()
        {
            // Arrange
            var createdOn = DateTime.UtcNow;
            var dto = new TrainedModelDTO
            {
                Id = Guid.NewGuid(),
                Name = "Test Model",
                ModelFilePath = "/path/to/model",
                ModelConfigPath = "/path/to/config",
                IsPublished = true,
                CreatedById = "user1",
                CreatedOn = createdOn,
                CreatedBy = new UserDTO()
            };

            // Act
            var viewModel = _mapper.Map<TrainedModelViewModel>(dto);

            // Assert
            Assert.NotNull(viewModel);
            Assert.Equal(dto.Id.Value, viewModel.Id);
            Assert.Equal(dto.Name, viewModel.Name);
            Assert.Equal(dto.ModelFilePath, viewModel.ModelFilePath);
            Assert.Equal(dto.ModelConfigPath, viewModel.ModelConfigPath);
            Assert.Equal(dto.IsPublished, viewModel.IsPublished);
            Assert.Equal(dto.CreatedById, viewModel.CreatedById);
            Assert.Equal(dto.CreatedOn, viewModel.CreatedOn);
            Assert.NotNull(viewModel.CreatedBy);
        }

        [Fact]
        public void Map_TrainingRunViewModel_To_TrainingRunDTO_Should_Map_Required_Properties()
        {
            // Arrange
            var viewModel = new TrainingRunViewModel
            {
                Name = "Test Training Run",
                TrainedModelId = Guid.NewGuid(),
                DatasetId = Guid.NewGuid()
            };

            // Act
            var dto = _mapper.Map<TrainingRunDTO>(viewModel);

            // Assert
            Assert.NotNull(dto);
            Assert.Equal(viewModel.Name, dto.Name);
            Assert.Equal(viewModel.TrainedModelId, dto.TrainedModelId);
            Assert.Equal(viewModel.DatasetId, dto.DatasetId);
        }

        [Fact]
        public void Map_Null_DTOs_Should_Return_Null_ViewModels()
        {
            // Arrange
            TrainingRunDTO trainingRunDto = null;
            TrainedModelDTO trainedModelDto = null;

            // Act
            var trainingRunViewModel = _mapper.Map<TrainingRunViewModel>(trainingRunDto);
            var trainingRunIndexViewModel = _mapper.Map<TrainingRunIndexViewModel>(trainingRunDto);
            var trainedModelViewModel = _mapper.Map<TrainedModelViewModel>(trainedModelDto);

            // Assert
            Assert.Null(trainingRunViewModel);
            Assert.Null(trainingRunIndexViewModel);
            Assert.Null(trainedModelViewModel);
        }

        [Fact]
        public void Map_TrainingRun_With_Null_OptionalProperties_Should_Map_Successfully()
        {
            // Arrange
            var dto = new TrainingRunDTO
            {
                Id = Guid.NewGuid(),
                Name = "Test Training Run",
                Status = null,
                Dataset = null,
                TrainedModel = null,
                BaseModel = null,
                TrainParams = null,
                CreatedBy = null
            };

            // Act
            var indexViewModel = _mapper.Map<TrainingRunIndexViewModel>(dto);

            // Assert
            Assert.NotNull(indexViewModel);
            Assert.Equal(dto.Id.Value, indexViewModel.Id);
            Assert.Equal(dto.Name, indexViewModel.Name);
            Assert.Null(indexViewModel.Status);
            Assert.Null(indexViewModel.CreatedBy);
        }

        [Fact]
        public void Map_TrainedModel_With_Null_OptionalProperties_Should_Map_Successfully()
        {
            // Arrange
            var dto = new TrainedModelDTO
            {
                Id = Guid.NewGuid(),
                Name = "Test Model",
                ModelFilePath = null,
                ModelConfigPath = null,
                Dataset = null,
                TrainingRun = null,
                BaseModel = null,
                TrainedModelStatistics = null,
                CreatedBy = null
            };

            // Act
            var viewModel = _mapper.Map<TrainedModelViewModel>(dto);

            // Assert
            Assert.NotNull(viewModel);
            Assert.Equal(dto.Id.Value, viewModel.Id);
            Assert.Equal(dto.Name, viewModel.Name);
            Assert.Null(viewModel.ModelFilePath);
            Assert.Null(viewModel.ModelConfigPath);
            Assert.Null(viewModel.CreatedBy);
        }
    }
}

using AutoMapper;
using DTOs.MainApp.BL;
using DTOs.MainApp.BL.DetectionDTOs;
using DTOs.MainApp.BL.TrainingDTOs;
using MainApp.MVC.Mappers;
using MainApp.MVC.ViewModels.IntranetPortal.Detection;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppMVCTests.Mappers
{
    public class DetectionProfileTests
    {
        private readonly IMapper _mapper;

        public DetectionProfileTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<DetectionProfile>());
            _mapper = config.CreateMapper();
        }

        [Fact]
        public void Map_DetectionRunDTO_To_DetectionRunViewModel_Should_Map_All_Properties()
        {
            // Arrange
            var createdOn = DateTime.Now;
            var dto = new DetectionRunDTO
            {
                Id = Guid.NewGuid(),
                Name = "Test Detection Run",
                Description = "Test Description",
                InputImgPath = "path/to/image",
                IsCompleted = true,
                Status = "Completed",
                DetectionInputImageId = Guid.NewGuid(),
                TrainedModelId = Guid.NewGuid(),
                CreatedById = "user123",
                CreatedOn = createdOn,
                DetectionInputImage = new DetectionInputImageDTO(),
                TrainedModel = new TrainedModelDTO(),
                CreatedBy = new UserDTO(),
                DetectedDumpSites = new List<DetectedDumpSiteDTO>()
            };

            // Act
            var viewModel = _mapper.Map<DetectionRunViewModel>(dto);

            // Assert
            Assert.NotNull(viewModel);
            Assert.Equal(dto.Id, viewModel.Id);
            Assert.Equal(dto.Name, viewModel.Name);
            Assert.Equal(dto.Description, viewModel.Description);
            Assert.Equal(dto.IsCompleted, viewModel.IsCompleted);
            Assert.Equal(dto.Status, viewModel.Status);
            Assert.Equal(dto.DetectionInputImageId.ToString(), viewModel.DetectionInputImageId);
            Assert.Equal(dto.CreatedById, viewModel.CreatedById);
            Assert.Equal(dto.CreatedOn, viewModel.CreatedOn);
            Assert.NotNull(viewModel.DetectionInputImage);
            Assert.NotNull(viewModel.CreatedBy);
            Assert.NotNull(viewModel.DetectedDumpSites);
        }

        [Fact]
        public void Map_DetectionRunViewModel_To_DetectionRunDTO_Should_Map_All_Properties()
        {
            // Arrange
            var createdOn = DateTime.Now;
            var viewModel = new DetectionRunViewModel
            {
                Id = Guid.NewGuid(),
                Name = "Test Detection Run",
                Description = "Test Description",
                SelectedInputImageId = Guid.NewGuid(),
                SelectedTrainedModelId = Guid.NewGuid(),
                IsCompleted = true,
                Status = "Completed",
                DetectionInputImageId = Guid.NewGuid().ToString(),
                CreatedById = "user123",
                CreatedOn = createdOn,
                DetectionInputImage = new DetectionInputImageViewModel(),
                CreatedBy = new UserDTO(),
                DetectedDumpSites = new List<DetectedDumpSiteViewModel>()
            };

            // Act
            var dto = _mapper.Map<DetectionRunDTO>(viewModel);

            // Assert
            Assert.NotNull(dto);
            Assert.Equal(viewModel.Id, dto.Id);
            Assert.Equal(viewModel.Name, dto.Name);
            Assert.Equal(viewModel.Description, dto.Description);
            Assert.Equal(viewModel.IsCompleted, dto.IsCompleted);
            Assert.Equal(viewModel.Status, dto.Status);
            Assert.Equal(Guid.Parse(viewModel.DetectionInputImageId), dto.DetectionInputImageId);
            Assert.Equal(viewModel.CreatedById, dto.CreatedById);
            Assert.Equal(viewModel.CreatedOn, dto.CreatedOn);
            Assert.NotNull(dto.DetectionInputImage);
            Assert.NotNull(dto.CreatedBy);
            Assert.NotNull(dto.DetectedDumpSites);
        }

        [Fact]
        public void Map_Null_DetectionRunDTO_Should_Return_Null_ViewModel()
        {
            // Arrange
            DetectionRunDTO dto = null;

            // Act
            var viewModel = _mapper.Map<DetectionRunViewModel>(dto);

            // Assert
            Assert.Null(viewModel);
        }

        [Fact]
        public void Map_Null_DetectionRunViewModel_Should_Return_Null_DTO()
        {
            // Arrange
            DetectionRunViewModel viewModel = null;

            // Act
            var dto = _mapper.Map<DetectionRunDTO>(viewModel);

            // Assert
            Assert.Null(dto);
        }

        [Fact]
        public void Map_Empty_Collections_Should_Initialize_Empty_Collections()
        {
            // Arrange
            var dto = new DetectionRunDTO();
            var viewModel = new DetectionRunViewModel();

            // Act
            var mappedViewModel = _mapper.Map<DetectionRunViewModel>(dto);
            var mappedDto = _mapper.Map<DetectionRunDTO>(viewModel);

            // Assert
            Assert.NotNull(mappedViewModel.DetectedDumpSites);
            Assert.Empty(mappedViewModel.DetectedDumpSites);
            Assert.NotNull(mappedDto.DetectedDumpSites);
            Assert.Empty(mappedDto.DetectedDumpSites);
        }
        [Fact]
        public void DetectedDumpSiteDTO_To_DetectedDumpSiteViewModel_Should_Map_Properties()
        {
            var dto = new DetectedDumpSiteDTO
            {
                Id = Guid.NewGuid(),
                ConfidenceRate = 0.85,
                DetectionRunId = Guid.NewGuid(),
                DatasetClassId = Guid.NewGuid(),
                Geom = new Polygon(new LinearRing(new[] { new Coordinate(1, 2), new Coordinate(3, 4), new Coordinate(5, 6), new Coordinate(1, 2) }))
            };

            var viewModel = _mapper.Map<DetectedDumpSiteViewModel>(dto);

            Assert.Equal(dto.Id, viewModel.Id);
            Assert.Equal(dto.ConfidenceRate, viewModel.ConfidenceRate);
            Assert.Equal(dto.DetectionRunId, viewModel.DetectionRunId);
            Assert.Equal(dto.DatasetClassId, viewModel.DatasetClassId);
            Assert.Equal(dto.GeoJson, viewModel.GeoJson);
        }

        [Fact]
        public void DetectionInputImageDTO_To_DetectionInputImageViewModel_Should_Map_Properties()
        {
            var dto = new DetectionInputImageDTO
            {
                Id = Guid.NewGuid(),
                Name = "Sample Image",
                Description = "Sample Description",
                DateTaken = DateTime.UtcNow.AddDays(-10),
                ImagePath = "/images/sample.jpg",
                ImageFileName = "sample.jpg",
                CreatedById = "User1",
                CreatedOn = DateTime.UtcNow.AddDays(-5),
                UpdatedById = "User2",
                UpdatedOn = DateTime.UtcNow
            };

            var viewModel = _mapper.Map<DetectionInputImageViewModel>(dto);

            Assert.Equal(dto.Id, viewModel.Id);
            Assert.Equal(dto.Name, viewModel.Name);
            Assert.Equal(dto.Description, viewModel.Description);
            Assert.Equal(dto.DateTaken, viewModel.DateTaken);
            Assert.Equal(dto.ImagePath, viewModel.ImagePath);
            Assert.Equal(dto.ImageFileName, viewModel.ImageFileName);
            Assert.Equal(dto.CreatedById, viewModel.CreatedById);
            Assert.Equal(dto.CreatedOn, viewModel.CreatedOn);
            Assert.Equal(dto.UpdatedById, viewModel.UpdatedById);
            Assert.Equal(dto.UpdatedOn, viewModel.UpdatedOn);
        }
    }
}

using AutoMapper;
using DTOs.MainApp.BL.DatasetDTOs;
using DTOs.MainApp.BL.DetectionDTOs;
using Entities.DatasetEntities;
using Entities.DetectionEntities;
using MainApp.BL.Mappers;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppBLTests.Mappers
{
    public class DetectionProfileBLTests
    {
        private readonly IMapper _mapper;

        public DetectionProfileBLTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<DetectionProfileBL>());
            _mapper = config.CreateMapper();
        }

        [Fact]
        public void DetectedDumpSiteDTO_To_DetectedDumpSite_MapsCorrectly()
        {
            // Arrange
            var dto = new DetectedDumpSiteDTO
            {
                Id = Guid.NewGuid(),
                ConfidenceRate = 0.95,
                IsInsideIgnoreZone = true,
                DetectionRunId = Guid.NewGuid(),
                DatasetClassId = Guid.NewGuid(),
                Geom = new Polygon(new LinearRing(new Coordinate[]
                {
                new(0, 0),
                new(1, 0),
                new(1, 1),
                new(0, 1),
                new(0, 0)
                }))
            };

            // Act
            var entity = _mapper.Map<DetectedDumpSite>(dto);

            // Assert
            Assert.NotNull(entity);
            Assert.Equal(dto.Id.Value, entity.Id);
            Assert.Equal(dto.ConfidenceRate, entity.ConfidenceRate);
            Assert.Equal(dto.DetectionRunId.Value, entity.DetectionRunId);
            Assert.Equal(dto.DatasetClassId.Value, entity.DatasetClassId);
            Assert.NotNull(entity.Geom);
            Assert.Equal(5, entity.Geom.Coordinates.Length);
        }

        [Fact]
        public void DetectionRunDTO_To_DetectionRun_MapsCorrectly()
        {
            // Arrange
            var dto = new DetectionRunDTO
            {
                Id = Guid.NewGuid(),
                Name = "Test Run",
                Description = "Test Description",
                InputImgPath = "/path/to/image",
                IsCompleted = true,
                Status = "Completed",
                DetectionInputImageId = Guid.NewGuid(),
                TrainedModelId = Guid.NewGuid(),
                CreatedById = "user123",
                CreatedOn = DateTime.UtcNow,
                DetectedDumpSites = new List<DetectedDumpSiteDTO>()
            };

            // Act
            var entity = _mapper.Map<DetectionRun>(dto);

            // Assert
            Assert.NotNull(entity);
            Assert.Equal(dto.Id.Value, entity.Id);
            Assert.Equal(dto.Name, entity.Name);
            Assert.Equal(dto.Description, entity.Description);
            Assert.Equal(dto.IsCompleted, entity.IsCompleted);
            Assert.Equal(dto.Status, entity.Status);
            Assert.Equal(dto.DetectionInputImageId.Value, entity.DetectionInputImageId);
            Assert.Equal(dto.TrainedModelId.Value, entity.TrainedModelId);
            Assert.NotNull(entity.DetectedDumpSites);
        }

        [Fact]
        public void DetectionIgnoreZoneDTO_To_DetectionIgnoreZone_MapsCorrectly()
        {
            // Arrange
            var dto = new DetectionIgnoreZoneDTO
            {
                Id = Guid.NewGuid(),
                Name = "Ignore Zone 1",
                Description = "Test Ignore Zone",
                IsEnabled = true,
                Geom = new Polygon(new LinearRing(new Coordinate[]
                {
                new(0, 0),
                new(1, 0),
                new(1, 1),
                new(0, 1),
                new(0, 0)
                })),
                CreatedById = "user123",
                CreatedOn = DateTime.UtcNow
            };

            // Act
            var entity = _mapper.Map<DetectionIgnoreZone>(dto);

            // Assert
            Assert.NotNull(entity);
            Assert.Equal(dto.Id.Value, entity.Id);
            Assert.Equal(dto.Name, entity.Name);
            Assert.Equal(dto.Description, entity.Description);
            Assert.Equal(dto.IsEnabled, entity.IsEnabled);
            Assert.NotNull(entity.Geom);
            Assert.Equal(dto.CreatedById, entity.CreatedById);
            Assert.Equal(dto.CreatedOn.Value, entity.CreatedOn);
        }

        [Fact]
        public void DetectionInputImageDTO_To_DetectionInputImage_MapsCorrectly()
        {
            // Arrange
            var dto = new DetectionInputImageDTO
            {
                Id = Guid.NewGuid(),
                Name = "Input Image 1",
                Description = "Test Input Image",
                DateTaken = DateTime.UtcNow,
                ImagePath = "/path/to/image",
                ImageFileName = "image.jpg",
                CreatedById = "user123",
                CreatedOn = DateTime.UtcNow,
                UpdatedById = "user456",
                UpdatedOn = DateTime.UtcNow
            };

            // Act
            var entity = _mapper.Map<DetectionInputImage>(dto);

            // Assert
            Assert.NotNull(entity);
            Assert.Equal(dto.Id.Value, entity.Id);
            Assert.Equal(dto.Name, entity.Name);
            Assert.Equal(dto.Description, entity.Description);
            Assert.Equal(dto.DateTaken.Value, entity.DateTaken);
            Assert.Equal(dto.ImagePath, entity.ImagePath);
            Assert.Equal(dto.ImageFileName, entity.ImageFileName);
            Assert.Equal(dto.CreatedById, entity.CreatedById);
            Assert.Equal(dto.CreatedOn.Value, entity.CreatedOn);
            Assert.Equal(dto.UpdatedById, entity.UpdatedById);
            Assert.Equal(dto.UpdatedOn.Value, entity.UpdatedOn);
        }

        [Fact]
        public void ReverseMapping_DetectedDumpSite_To_DetectedDumpSiteDTO_MapsCorrectly()
        {
            // Arrange
            var entity = new DetectedDumpSite
            {
                Id = Guid.NewGuid(),
                ConfidenceRate = 0.95,
                DatasetClassId = Guid.NewGuid(),
                DetectionRunId = Guid.NewGuid(),
                Geom = new Polygon(new LinearRing(new Coordinate[]
                {
                new(0, 0),
                new(1, 0),
                new(1, 1),
                new(0, 1),
                new(0, 0)
                }))
            };

            // Act
            var dto = _mapper.Map<DetectedDumpSiteDTO>(entity);

            // Assert
            Assert.NotNull(dto);
            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.ConfidenceRate, dto.ConfidenceRate);
            Assert.Equal(entity.DatasetClassId, dto.DatasetClassId);
            Assert.Equal(entity.DetectionRunId, dto.DetectionRunId);
            Assert.NotNull(dto.Geom);
        }

    }
}

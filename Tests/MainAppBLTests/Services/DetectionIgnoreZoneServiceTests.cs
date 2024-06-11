using AutoMapper;
using DAL.Interfaces.Repositories.DetectionRepositories;
using DTOs.MainApp.BL.DetectionDTOs;
using Entities.DetectionEntities;
using MainApp.BL.Services.DetectionServices;
using Microsoft.Extensions.Logging;
using Moq;
using SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppBLTests.Services
{
    public class DetectionIgnoreZoneServiceTests
    {
        private readonly Mock<IDetectionIgnoreZonesRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<DetectionIgnoreZoneService>> _mockLogger;
        private readonly DetectionIgnoreZoneService _service;

        public DetectionIgnoreZoneServiceTests()
        {
            _mockRepository = new Mock<IDetectionIgnoreZonesRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<DetectionIgnoreZoneService>>();
            _service = new DetectionIgnoreZoneService(_mockRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllIgnoreZonesDTOs_ReturnsOkResult_WhenDataIsAvailable()
        {
            // Arrange
            var detectionIgnoreZones = new List<DetectionIgnoreZone>
            {
                new DetectionIgnoreZone { Id = Guid.NewGuid(), Name = "Zone 1", Description = "Description 1" },
                new DetectionIgnoreZone { Id = Guid.NewGuid(), Name = "Zone 2", Description = "Description 2" }
            };

            var detectionIgnoreZoneDTOs = new List<DetectionIgnoreZoneDTO>
            {
                new DetectionIgnoreZoneDTO { Id = Guid.NewGuid(), Name = "Zone 1", Description = "Description 1" },
                new DetectionIgnoreZoneDTO { Id = Guid.NewGuid(), Name = "Zone 2", Description = "Description 2" }
            };

            var resultDTO = ResultDTO<IEnumerable<DetectionIgnoreZone>>.Ok(detectionIgnoreZones);

            _mockRepository.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DetectionIgnoreZone, bool>>>(),
                                                      It.IsAny<Func<IQueryable<DetectionIgnoreZone>, IOrderedQueryable<DetectionIgnoreZone>>>(),
                                                      false,
                                                      "CreatedBy",
                                                      null))
                           .ReturnsAsync(resultDTO);
            _mockMapper.Setup(m => m.Map<List<DetectionIgnoreZoneDTO>>(It.IsAny<IEnumerable<DetectionIgnoreZone>>()))
                       .Returns(detectionIgnoreZoneDTOs);

            // Act
            var result = await _service.GetAllIgnoreZonesDTOs();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(detectionIgnoreZoneDTOs.Count, result.Data.Count);
            _mockRepository.Verify(repo => repo.GetAll(It.IsAny<Expression<Func<DetectionIgnoreZone, bool>>>(),
                                                       It.IsAny<Func<IQueryable<DetectionIgnoreZone>, IOrderedQueryable<DetectionIgnoreZone>>>(),
                                                       false,
                                                       "CreatedBy",
                                                       null),
                                  Times.Once);
            _mockMapper.Verify(m => m.Map<List<DetectionIgnoreZoneDTO>>(It.IsAny<IEnumerable<DetectionIgnoreZone>>()), Times.Once);
        }

        [Fact]
        public async Task GetAllIgnoreZonesDTOs_ReturnsFailResult_WhenRepositoryFails()
        {
            // Arrange
            var resultDTO = ResultDTO<IEnumerable<DetectionIgnoreZone>>.Fail("Repository failed");

            _mockRepository.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DetectionIgnoreZone, bool>>>(),
                                                      It.IsAny<Func<IQueryable<DetectionIgnoreZone>, IOrderedQueryable<DetectionIgnoreZone>>>(),
                                                      false,
                                                      "CreatedBy",
                                                      null))
                           .ReturnsAsync(resultDTO);

            // Act
            var result = await _service.GetAllIgnoreZonesDTOs();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Repository failed", result.ErrMsg);
            _mockRepository.Verify(repo => repo.GetAll(It.IsAny<Expression<Func<DetectionIgnoreZone, bool>>>(),
                                                       It.IsAny<Func<IQueryable<DetectionIgnoreZone>, IOrderedQueryable<DetectionIgnoreZone>>>(),
                                                       false,
                                                       "CreatedBy",
                                                       null),
                                  Times.Once);
        }

        [Fact]
        public async Task CreateDetectionIgnoreZoneFromDTO_ReturnsFail_WhenDTOIsNull()
        {
            // Arrange
            DetectionIgnoreZoneDTO dto = null;

            // Act
            var result = await _service.CreateDetectionIgnoreZoneFromDTO(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("DTO Object is null", result.ErrMsg);
        }

        [Fact]
        public async Task CreateDetectionIgnoreZoneFromDTO_ReturnsFail_WhenMappingFails()
        {
            // Arrange
            var dto = new DetectionIgnoreZoneDTO { Id = Guid.NewGuid(), Name = "Test Zone" };
            _mockMapper.Setup(m => m.Map<DetectionIgnoreZone>(dto)).Returns((DetectionIgnoreZone)null);

            // Act
            var result = await _service.CreateDetectionIgnoreZoneFromDTO(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("DTO not mapped", result.ErrMsg);
        }

        [Fact]
        public async Task CreateDetectionIgnoreZoneFromDTO_ReturnsFail_WhenRepositoryFails()
        {
            // Arrange
            var dto = new DetectionIgnoreZoneDTO { Id = Guid.NewGuid(), Name = "Test Zone" };
            var ignoreZone = new DetectionIgnoreZone { Id = dto.Id.Value, Name = dto.Name };

            _mockMapper.Setup(m => m.Map<DetectionIgnoreZone>(dto)).Returns(ignoreZone);
            _mockRepository.Setup(repo => repo.Create(It.IsAny<DetectionIgnoreZone>(), true, default)).ReturnsAsync(ResultDTO.Fail("Creation failed"));

            // Act
            var result = await _service.CreateDetectionIgnoreZoneFromDTO(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Creation failed", result.ErrMsg);
            _mockRepository.Verify(repo => repo.Create(ignoreZone, true, default), Times.Once);
        }

        [Fact]
        public async Task CreateDetectionIgnoreZoneFromDTO_ReturnsOk_WhenCreationSucceeds()
        {
            // Arrange
            var dto = new DetectionIgnoreZoneDTO { Id = Guid.NewGuid(), Name = "Test Zone" };
            var ignoreZone = new DetectionIgnoreZone { Id = dto.Id.Value, Name = dto.Name };

            _mockMapper.Setup(m => m.Map<DetectionIgnoreZone>(dto)).Returns(ignoreZone);
            _mockRepository.Setup(repo => repo.Create(ignoreZone, true, default)).ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _service.CreateDetectionIgnoreZoneFromDTO(dto);

            // Assert
            Assert.True(result.IsSuccess);
            _mockRepository.Verify(repo => repo.Create(ignoreZone, true, default), Times.Once);
        }

        [Fact]
        public async Task UpdateDetectionIgnoreZoneFromDTO_ReturnsFail_WhenDTOIsNull()
        {
            // Arrange
            DetectionIgnoreZoneDTO dto = null;

            // Act
            var result = await _service.UpdateDetectionIgnoreZoneFromDTO(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("DTO Object is null", result.ErrMsg);
        }

        [Fact]
        public async Task UpdateDetectionIgnoreZoneFromDTO_ReturnsFail_WhenDTOIdIsNull()
        {
            // Arrange
            var dto = new DetectionIgnoreZoneDTO { Id = null, Name = "Test Zone" };

            // Act
            var result = await _service.UpdateDetectionIgnoreZoneFromDTO(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("DTO Id is null", result.ErrMsg);
        }

        [Fact]
        public async Task UpdateDetectionIgnoreZoneFromDTO_ReturnsFail_WhenGetByIdFails()
        {
            // Arrange
            var dto = new DetectionIgnoreZoneDTO { Id = Guid.NewGuid(), Name = "Test Zone" };
            var ignoreZone = new DetectionIgnoreZone { Id = dto.Id.Value, Name = dto.Name };

            _mockMapper.Setup(m => m.Map<DetectionIgnoreZone>(dto)).Returns(ignoreZone);
            _mockRepository.Setup(repo => repo.GetById(ignoreZone.Id, true, null)).ReturnsAsync(ResultDTO<DetectionIgnoreZone>.Fail("Get by ID failed"));

            // Act
            var result = await _service.UpdateDetectionIgnoreZoneFromDTO(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Get by ID failed", result.ErrMsg);
        }

        [Fact]
        public async Task UpdateDetectionIgnoreZoneFromDTO_ReturnsOk_WhenUpdateSucceeds()
        {
            // Arrange
            var dto = new DetectionIgnoreZoneDTO { Id = Guid.NewGuid(), Name = "Test Zone" };
            var ignoreZone = new DetectionIgnoreZone { Id = dto.Id.Value, Name = dto.Name };

            _mockMapper.Setup(m => m.Map<DetectionIgnoreZone>(dto)).Returns(ignoreZone);
            _mockRepository.Setup(repo => repo.GetById(ignoreZone.Id, true, null)).ReturnsAsync(ResultDTO<DetectionIgnoreZone>.Ok(ignoreZone));
            _mockRepository.Setup(repo => repo.Update(ignoreZone, true, default)).ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _service.UpdateDetectionIgnoreZoneFromDTO(dto);

            // Assert
            Assert.True(result.IsSuccess);
            _mockRepository.Verify(repo => repo.Update(ignoreZone, true, default), Times.Once);
        }

        [Fact]
        public async Task UpdateDetectionIgnoreZoneFromDTO_ReturnsFail_WhenUpdateFails()
        {
            // Arrange
            var dto = new DetectionIgnoreZoneDTO { Id = Guid.NewGuid(), Name = "Test Zone" };
            var ignoreZone = new DetectionIgnoreZone { Id = dto.Id.Value, Name = dto.Name };

            _mockMapper.Setup(m => m.Map<DetectionIgnoreZone>(dto)).Returns(ignoreZone);
            _mockRepository.Setup(repo => repo.GetById(ignoreZone.Id, true, null)).ReturnsAsync(ResultDTO<DetectionIgnoreZone>.Ok(ignoreZone));
            _mockRepository.Setup(repo => repo.Update(ignoreZone, true, default)).ReturnsAsync(ResultDTO.Fail("Update failed"));

            // Act
            var result = await _service.UpdateDetectionIgnoreZoneFromDTO(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Update failed", result.ErrMsg);
            _mockRepository.Verify(repo => repo.Update(ignoreZone, true, default), Times.Once);
        }

        [Fact]
        public async Task DeleteDetectionIgnoreZoneFromDTO_ReturnsFail_WhenDTOIsNull()
        {
            // Arrange
            DetectionIgnoreZoneDTO dto = null;

            // Act
            var result = await _service.DeleteDetectionIgnoreZoneFromDTO(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("DTO Object is null", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDetectionIgnoreZoneFromDTO_ReturnsFail_WhenDTOIdIsNull()
        {
            // Arrange
            var dto = new DetectionIgnoreZoneDTO { Id = null, Name = "Test Zone" };

            // Act
            var result = await _service.DeleteDetectionIgnoreZoneFromDTO(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("DTO Id is null", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDetectionIgnoreZoneFromDTO_ReturnsFail_WhenMappingFails()
        {
            // Arrange
            var dto = new DetectionIgnoreZoneDTO { Id = Guid.NewGuid(), Name = "Test Zone" };
            _mockMapper.Setup(m => m.Map<DetectionIgnoreZone>(dto)).Returns((DetectionIgnoreZone)null);

            // Act
            var result = await _service.DeleteDetectionIgnoreZoneFromDTO(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("DTO not mapped", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDetectionIgnoreZoneFromDTO_ReturnsFail_WhenGetByIdFails()
        {
            // Arrange
            var dto = new DetectionIgnoreZoneDTO { Id = Guid.NewGuid(), Name = "Test Zone" };
            var ignoreZone = new DetectionIgnoreZone { Id = dto.Id.Value, Name = dto.Name };

            _mockMapper.Setup(m => m.Map<DetectionIgnoreZone>(dto)).Returns(ignoreZone);
            _mockRepository.Setup(repo => repo.GetById(ignoreZone.Id, true, null)).ReturnsAsync(ResultDTO<DetectionIgnoreZone>.Fail("Get by ID failed"));

            // Act
            var result = await _service.DeleteDetectionIgnoreZoneFromDTO(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Get by ID failed", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDetectionIgnoreZoneFromDTO_ReturnsOk_WhenDeleteSucceeds()
        {
            // Arrange
            var dto = new DetectionIgnoreZoneDTO { Id = Guid.NewGuid(), Name = "Test Zone" };
            var ignoreZone = new DetectionIgnoreZone { Id = dto.Id.Value, Name = dto.Name };

            _mockMapper.Setup(m => m.Map<DetectionIgnoreZone>(dto)).Returns(ignoreZone);
            _mockRepository.Setup(repo => repo.GetById(ignoreZone.Id, true, null)).ReturnsAsync(ResultDTO<DetectionIgnoreZone>.Ok(ignoreZone));
            _mockRepository.Setup(repo => repo.Delete(ignoreZone, true, default)).ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _service.DeleteDetectionIgnoreZoneFromDTO(dto);

            // Assert
            Assert.True(result.IsSuccess);
            _mockRepository.Verify(repo => repo.Delete(ignoreZone, true, default), Times.Once);
        }

        [Fact]
        public async Task DeleteDetectionIgnoreZoneFromDTO_ReturnsFail_WhenDeleteFails()
        {
            // Arrange
            var dto = new DetectionIgnoreZoneDTO { Id = Guid.NewGuid(), Name = "Test Zone" };
            var ignoreZone = new DetectionIgnoreZone { Id = dto.Id.Value, Name = dto.Name };

            _mockMapper.Setup(m => m.Map<DetectionIgnoreZone>(dto)).Returns(ignoreZone);
            _mockRepository.Setup(repo => repo.GetById(ignoreZone.Id, true, null)).ReturnsAsync(ResultDTO<DetectionIgnoreZone>.Ok(ignoreZone));
            _mockRepository.Setup(repo => repo.Delete(ignoreZone, true, default)).ReturnsAsync(ResultDTO.Fail("Delete failed"));

            // Act
            var result = await _service.DeleteDetectionIgnoreZoneFromDTO(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Delete failed", result.ErrMsg);
            _mockRepository.Verify(repo => repo.Delete(ignoreZone, true, default), Times.Once);
        }

      
    }
}

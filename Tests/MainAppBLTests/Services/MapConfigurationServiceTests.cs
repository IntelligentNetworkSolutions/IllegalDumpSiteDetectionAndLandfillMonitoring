using AutoMapper;
using DAL.Interfaces.Repositories.MapConfigurationRepositories;
using DTOs.MainApp.BL.MapConfigurationDTOs;
using Entities.MapConfigurationEntities;
using MainApp.BL.Services.MapConfigurationServices;
using Microsoft.Extensions.Logging;
using Moq;
using SD;

namespace Tests.MainAppBLTests.Services
{
    public class MapConfigurationServiceTests
    {
        private readonly Mock<IMapConfigurationRepository> _mockMapConfigRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<MapConfigurationService>> _mockLogger;
        private readonly MapConfigurationService _service;

        public MapConfigurationServiceTests()
        {
            _mockMapConfigRepository = new Mock<IMapConfigurationRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<MapConfigurationService>>();
            _service = new MapConfigurationService(
                _mockMapConfigRepository.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );
        }



        [Fact]
        public async Task GetMapConfigurationByName_ValidMapName_ReturnsMapConfigurationDTO()
        {
            // Arrange
            string mapName = "ValidMapName";
            var expectedMapConfigDTO = new MapConfigurationDTO
            {
                Id = Guid.NewGuid()
            };

            var mapConfigRepositoryMock = new Mock<IMapConfigurationRepository>();
            mapConfigRepositoryMock.Setup(r => r.GetMapConfigurationByName(mapName)).ReturnsAsync(new MapConfiguration());
            var mapConfigLoggerMock = new Mock<ILogger<MapConfigurationService>>();

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<MapConfigurationDTO>(It.IsAny<MapConfiguration>())).Returns(expectedMapConfigDTO);

            var mapConfigurationService = new MapConfigurationService(mapConfigRepositoryMock.Object, mapperMock.Object, mapConfigLoggerMock.Object);

            // Act
            var result = await mapConfigurationService.GetMapConfigurationByName(mapName);

            // Assert
            Assert.Equal(expectedMapConfigDTO, result);
        }

        [Fact]
        public async Task GetMapConfigurationByName_InvalidMapName_ThrowsException()
        {
            // Arrange
            string invalidMapName = "InvalidMapName";

            var mapConfigRepositoryMock = new Mock<IMapConfigurationRepository>();
            mapConfigRepositoryMock.Setup(r => r.GetMapConfigurationByName(invalidMapName)).ReturnsAsync((MapConfiguration)null);
            var mapConfigLoggerMock = new Mock<ILogger<MapConfigurationService>>();

            var mapperMock = new Mock<IMapper>();

            var mapConfigurationService = new MapConfigurationService(mapConfigRepositoryMock.Object, mapperMock.Object, mapConfigLoggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => mapConfigurationService.GetMapConfigurationByName(invalidMapName));
        }

        [Fact]
        public async Task GetMapConfigurationByName_ValidMapConfiguration_ReturnsMappedDTO()
        {
            // Arrange
            string mapName = "ValidMapName";
            var expectedMapConfig = new MapConfiguration
            {
                Id = Guid.NewGuid()
            };
            var expectedMapConfigDTO = new MapConfigurationDTO
            {
                Id = Guid.NewGuid()
            };

            var mapConfigRepositoryMock = new Mock<IMapConfigurationRepository>();
            mapConfigRepositoryMock.Setup(r => r.GetMapConfigurationByName(mapName)).ReturnsAsync(expectedMapConfig);
            var mapConfigLoggerMock = new Mock<ILogger<MapConfigurationService>>();

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<MapConfigurationDTO>(expectedMapConfig)).Returns(expectedMapConfigDTO);

            var mapConfigurationService = new MapConfigurationService(mapConfigRepositoryMock.Object, mapperMock.Object, mapConfigLoggerMock.Object);

            // Act
            var result = await mapConfigurationService.GetMapConfigurationByName(mapName);

            // Assert
            Assert.Equal(expectedMapConfigDTO, result);
        }

        [Fact]
        public async Task GetMapConfigurationByName_MapperReturnsNull_ThrowsException()
        {
            // Arrange
            string mapName = "ValidMapName";
            var mapConfigRepositoryMock = new Mock<IMapConfigurationRepository>();
            mapConfigRepositoryMock.Setup(r => r.GetMapConfigurationByName(mapName)).ReturnsAsync(new MapConfiguration());
            var mapConfigLoggerMock = new Mock<ILogger<MapConfigurationService>>();

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<MapConfigurationDTO>(It.IsAny<MapConfiguration>())).Returns((MapConfigurationDTO)null);

            var mapConfigurationService = new MapConfigurationService(mapConfigRepositoryMock.Object, mapperMock.Object, mapConfigLoggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => mapConfigurationService.GetMapConfigurationByName(mapName));
        }

        [Fact]
        public async Task GetMapConfigurationByName_EmptyMapName_ThrowsException()
        {
            // Arrange
            string emptyMapName = string.Empty;
            var mapConfigRepositoryMock = new Mock<IMapConfigurationRepository>();
            var mapperMock = new Mock<IMapper>();
            var mapConfigLoggerMock = new Mock<ILogger<MapConfigurationService>>();


            var mapConfigurationService = new MapConfigurationService(mapConfigRepositoryMock.Object, mapperMock.Object, mapConfigLoggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => mapConfigurationService.GetMapConfigurationByName(emptyMapName));
        }

        [Fact]
        public async Task GetMapConfigurationByName_RepositoryThrowsException_ThrowsException()
        {
            // Arrange
            string mapName = "ValidMapName";
            var mapConfigRepositoryMock = new Mock<IMapConfigurationRepository>();
            var mapConfigLoggerMock = new Mock<ILogger<MapConfigurationService>>();
            mapConfigRepositoryMock.Setup(r => r.GetMapConfigurationByName(mapName)).ThrowsAsync(new Exception("Repository exception"));

            var mapperMock = new Mock<IMapper>();

            var mapConfigurationService = new MapConfigurationService(mapConfigRepositoryMock.Object, mapperMock.Object, mapConfigLoggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => mapConfigurationService.GetMapConfigurationByName(mapName));
        }

        [Fact]
        public async Task GetMapConfigurationByName_RepositoryReturnsNull_ThrowsException()
        {
            // Arrange
            string mapName = "NonExistentMapName";
            var mapConfigRepositoryMock = new Mock<IMapConfigurationRepository>();
            mapConfigRepositoryMock.Setup(r => r.GetMapConfigurationByName(mapName)).ReturnsAsync((MapConfiguration)null);
            var mapConfigLoggerMock = new Mock<ILogger<MapConfigurationService>>();

            var mapperMock = new Mock<IMapper>();

            var mapConfigurationService = new MapConfigurationService(mapConfigRepositoryMock.Object, mapperMock.Object, mapConfigLoggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => mapConfigurationService.GetMapConfigurationByName(mapName));
        }

        //KJ

        [Fact]
        public async Task GetAllMapConfigurations_ShouldReturnSuccess_WhenDataIsFetched()
        {
            // Arrange
            var mapConfigurations = new List<MapConfiguration>
        {
            new MapConfiguration { },
            new MapConfiguration { }
        };

            var result = ResultDTO<IEnumerable<MapConfiguration>>.Ok(mapConfigurations);
            _mockMapConfigRepository.Setup(r => r.GetAll(null, null, false, null, null)).ReturnsAsync(result);
            _mockMapper.Setup(m => m.Map<List<MapConfigurationDTO>>(It.IsAny<IEnumerable<MapConfiguration>>()))
                       .Returns(new List<MapConfigurationDTO> { });

            // Act
            var response = await _service.GetAllMapConfigurations();

            // Assert
            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
        }

        [Fact]
        public async Task GetAllMapConfigurations_ShouldReturnFail_WhenRepositoryReturnsError()
        {
            // Arrange
            var result = ResultDTO<IEnumerable<MapConfiguration>>.Fail("Error fetching data");
            _mockMapConfigRepository.Setup(r => r.GetAll(null, null, false, null, null)).ReturnsAsync(result);

            // Act
            var response = await _service.GetAllMapConfigurations();

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Error fetching data", response.ErrMsg);
        }

        [Fact]
        public async Task GetAllMapConfigurations_ShouldReturnExceptionFail_WhenExceptionIsThrown()
        {
            // Arrange
            _mockMapConfigRepository.Setup(r => r.GetAll(null, null, false, null, null)).ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var response = await _service.GetAllMapConfigurations();

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Contains("Unexpected error", response.ErrMsg);
        }

        [Fact]
        public async Task GetMapConfigurationById_ShouldReturnSuccess_WhenDataIsFetched()
        {
            // Arrange
            var mapConfigurationId = Guid.NewGuid();
            var mapConfiguration = new MapConfiguration { };
            var dto = new MapConfigurationDTO { };

            var result = ResultDTO<MapConfiguration?>.Ok(mapConfiguration);
            _mockMapConfigRepository.Setup(r => r.GetById(mapConfigurationId, false, "MapLayerConfigurations, MapLayerGroupConfigurations"))
                                    .ReturnsAsync(result);
            _mockMapper.Setup(m => m.Map<MapConfigurationDTO>(It.IsAny<MapConfiguration>()))
                       .Returns(dto);

            // Act
            var response = await _service.GetMapConfigurationById(mapConfigurationId);

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal(dto, response.Data);
        }

        [Fact]
        public async Task GetMapConfigurationById_ShouldReturnFail_WhenRepositoryReturnsError()
        {
            // Arrange
            var mapConfigurationId = Guid.NewGuid();
            var result = ResultDTO<MapConfiguration?>.Fail("Error fetching data");
            _mockMapConfigRepository.Setup(r => r.GetById(mapConfigurationId, false, "MapLayerConfigurations, MapLayerGroupConfigurations"))
                                    .ReturnsAsync(result);

            // Act
            var response = await _service.GetMapConfigurationById(mapConfigurationId);

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Error fetching data", response.ErrMsg);
        }

        [Fact]
        public async Task GetMapConfigurationById_ShouldReturnExceptionFail_WhenExceptionIsThrown()
        {
            // Arrange
            var mapConfigurationId = Guid.NewGuid();
            _mockMapConfigRepository.Setup(r => r.GetById(mapConfigurationId, false, "MapLayerConfigurations, MapLayerGroupConfigurations"))
                                    .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var response = await _service.GetMapConfigurationById(mapConfigurationId);

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Contains("Unexpected error", response.ErrMsg);
        }

        [Fact]
        public async Task CreateMapConfiguration_ShouldReturnOk_WhenCreateIsSuccessful()
        {
            // Arrange
            var dto = new MapConfigurationDTO { };
            var entity = new MapConfiguration { };
            _mockMapper.Setup(m => m.Map<MapConfiguration>(dto)).Returns(entity);
            _mockMapConfigRepository.Setup(r => r.Create(entity, true, default)).ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _service.CreateMapConfiguration(dto);

            // Assert
            Assert.True(result.IsSuccess);
            _mockMapConfigRepository.Verify(r => r.Create(entity, true, default), Times.Once);
        }

        [Fact]
        public async Task CreateMapConfiguration_ShouldReturnFail_WhenCreateFails()
        {
            // Arrange
            var dto = new MapConfigurationDTO { };
            var entity = new MapConfiguration { };
            _mockMapper.Setup(m => m.Map<MapConfiguration>(dto)).Returns(entity);
            _mockMapConfigRepository.Setup(r => r.Create(entity, true, default)).ReturnsAsync(ResultDTO.Fail("Error message"));

            // Act
            var result = await _service.CreateMapConfiguration(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error message", result.ErrMsg);
        }

        [Fact]
        public async Task CreateMapConfiguration_ShouldLogError_WhenExceptionOccurs()
        {
            // Arrange
            var dto = new MapConfigurationDTO { };
            var entity = new MapConfiguration { };
            _mockMapper.Setup(m => m.Map<MapConfiguration>(dto)).Returns(entity);
            _mockMapConfigRepository.Setup(r => r.Create(entity, true, default)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _service.CreateMapConfiguration(dto);

            // Assert
            Assert.False(result.IsSuccess);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Test exception")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once
            );
        }

        [Fact]
        public async Task EditMapConfiguration_ShouldReturnOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var dto = new MapConfigurationDTO { };
            var entity = new MapConfiguration { };
            _mockMapper.Setup(m => m.Map<MapConfiguration>(dto)).Returns(entity);
            _mockMapConfigRepository.Setup(r => r.Update(entity, true, default)).ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _service.EditMapConfiguration(dto);

            // Assert
            Assert.True(result.IsSuccess);
            _mockMapConfigRepository.Verify(r => r.Update(entity, true, default), Times.Once);
        }

        [Fact]
        public async Task EditMapConfiguration_ShouldReturnFail_WhenUpdateFails()
        {
            // Arrange
            var dto = new MapConfigurationDTO { };
            var entity = new MapConfiguration { };
            _mockMapper.Setup(m => m.Map<MapConfiguration>(dto)).Returns(entity);
            _mockMapConfigRepository.Setup(r => r.Update(entity, true, default)).ReturnsAsync(ResultDTO.Fail("Error message"));

            // Act
            var result = await _service.EditMapConfiguration(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error message", result.ErrMsg);
        }

        [Fact]
        public async Task EditMapConfiguration_ShouldLogError_WhenExceptionOccurs()
        {
            // Arrange
            var dto = new MapConfigurationDTO { };
            var entity = new MapConfiguration { };
            _mockMapper.Setup(m => m.Map<MapConfiguration>(dto)).Returns(entity);
            _mockMapConfigRepository.Setup(r => r.Update(entity, true, default)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _service.EditMapConfiguration(dto);

            // Assert
            Assert.False(result.IsSuccess);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Test exception")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once
            );
        }

        [Fact]
        public async Task DeleteMapConfiguration_ShouldReturnFail_WhenDeleteFails()
        {
            // Arrange
            var dto = new MapConfigurationDTO { };
            var entity = new MapConfiguration { };
            _mockMapper.Setup(m => m.Map<MapConfiguration>(dto)).Returns(entity);
            _mockMapConfigRepository.Setup(r => r.Delete(entity, true, default)).ReturnsAsync(ResultDTO.Fail("Error message"));

            // Act
            var result = await _service.DeleteMapConfiguration(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error message", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteMapConfiguration_ShouldLogError_WhenExceptionOccurs()
        {
            // Arrange
            var dto = new MapConfigurationDTO { };
            var entity = new MapConfiguration { };
            _mockMapper.Setup(m => m.Map<MapConfiguration>(dto)).Returns(entity);
            _mockMapConfigRepository.Setup(r => r.Delete(entity, true, default)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _service.DeleteMapConfiguration(dto);

            // Assert
            Assert.False(result.IsSuccess);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Test exception")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once
            );
        }
        [Fact]
        public async Task DeleteMapConfiguration_ShouldReturnOk_WhenDeleteIsSuccessful()
        {
            // Arrange
            var dto = new MapConfigurationDTO { };
            var entity = new MapConfiguration { };
            _mockMapper.Setup(m => m.Map<MapConfiguration>(dto)).Returns(entity);
            _mockMapConfigRepository.Setup(r => r.Delete(entity, true, default))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _service.DeleteMapConfiguration(dto);

            // Assert
            Assert.True(result.IsSuccess);
            _mockMapConfigRepository.Verify(r => r.Delete(entity, true, default), Times.Once);
        }

    }
}

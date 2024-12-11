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
        public async Task GetMapConfigurationByName_ShouldReturnFail_WhenRepositoryFails()
        {
            // Arrange
            string mapName = "testMap";
            _mockMapConfigRepository.Setup(r => r.GetMapConfigurationByName(mapName))
                           .ReturnsAsync(ResultDTO<MapConfiguration>.Fail("Repository error"));

            // Act
            var result = await _service.GetMapConfigurationByName(mapName);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Repository error", result.ErrMsg);
        }

        [Fact]
        public async Task GetMapConfigurationByName_ShouldReturnFail_WhenMapConfigurationNotFound()
        {
            // Arrange
            string mapName = "testMap";
            _mockMapConfigRepository.Setup(r => r.GetMapConfigurationByName(mapName))
                           .ReturnsAsync(ResultDTO<MapConfiguration>.Ok(null));

            // Act
            var result = await _service.GetMapConfigurationByName(mapName);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Map Configuration not found", result.ErrMsg);
        }

        [Fact]
        public async Task GetMapConfigurationByName_ShouldReturnFail_WhenMappingFails()
        {
            // Arrange
            string mapName = "testMap";
            var mapConfiguration = new MapConfiguration();
            _mockMapConfigRepository.Setup(r => r.GetMapConfigurationByName(mapName))
                           .ReturnsAsync(ResultDTO<MapConfiguration>.Ok(mapConfiguration));
            _mockMapper.Setup(m => m.Map<MapConfigurationDTO>(mapConfiguration)).Returns((MapConfigurationDTO)null);

            // Act
            var result = await _service.GetMapConfigurationByName(mapName);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Mapping to map configuration dto failed", result.ErrMsg);
        }

        [Fact]
        public async Task GetMapConfigurationByName_ShouldReturnExceptionFail_WhenExceptionOccurs()
        {
            // Arrange
            string mapName = "testMap";
            _mockMapConfigRepository.Setup(r => r.GetMapConfigurationByName(mapName)).Throws(new Exception("Unexpected error"));

            // Act
            var result = await _service.GetMapConfigurationByName(mapName);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Unexpected error", result.ErrMsg);
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

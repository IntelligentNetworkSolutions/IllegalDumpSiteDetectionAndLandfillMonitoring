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
    public class MapLayerGroupsConfigurationServiceTests
    {
        private readonly Mock<IMapLayerGroupsConfigurationRepository> _mockMapLayerGroupsConfigRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<MapLayersConfigurationService>> _mockLogger;
        private readonly MapLayerGroupsConfigurationService _service;

        public MapLayerGroupsConfigurationServiceTests()
        {
            _mockMapLayerGroupsConfigRepository = new Mock<IMapLayerGroupsConfigurationRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<MapLayersConfigurationService>>();
            _service = new MapLayerGroupsConfigurationService(
                _mockMapLayerGroupsConfigRepository.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task GetAllMapLayerGroupConfigurations_ShouldReturnSuccess()
        {
            // Arrange
            var entities = new List<MapLayerGroupConfiguration> { new MapLayerGroupConfiguration() };
            var dtos = new List<MapLayerGroupConfigurationDTO> { new MapLayerGroupConfigurationDTO() };

            _mockMapLayerGroupsConfigRepository.Setup(r => r.GetAll(null, null, false, "MapLayerConfigurations", null))
                .ReturnsAsync(ResultDTO<IEnumerable<MapLayerGroupConfiguration>>.Ok(entities));

            _mockMapper.Setup(m => m.Map<List<MapLayerGroupConfigurationDTO>>(It.IsAny<IEnumerable<MapLayerGroupConfiguration>>()))
                .Returns(dtos);

            // Act
            var result = await _service.GetAllMapLayerGroupConfigurations();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(dtos, result.Data);
        }

        [Fact]
        public async Task GetAllMapLayerGroupConfigurations_WhenRepositoryFails_ShouldReturnFailure()
        {
            // Arrange
            _mockMapLayerGroupsConfigRepository.Setup(r => r.GetAll(null, null, false, "MapLayerConfigurations", null))
                .ReturnsAsync(ResultDTO<IEnumerable<MapLayerGroupConfiguration>>.Fail("Repository error"));
            // Act
            var result = await _service.GetAllMapLayerGroupConfigurations();
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Repository error", result.ErrMsg);
        }

        [Fact]
        public async Task GetAllMapLayerGroupConfigurations_ShouldReturnException_WhenExceptionOccurs()
        {
            // Arrange
            _mockMapLayerGroupsConfigRepository.Setup(r => r.GetAll(null, null, false, null, null))
                .ThrowsAsync(new Exception("Object reference not set to an instance of an object"));

            // Act
            var result = await _service.GetAllMapLayerGroupConfigurations();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Object reference not set", result.ErrMsg);
        }


        [Fact]
        public async Task GetAllGroupLayersById_ShouldReturnSuccess()
        {
            // Arrange
            var groupConfigurationId = Guid.NewGuid();
            var groupConfiguration = new MapLayerGroupConfiguration { Id = groupConfigurationId };
            var mappedDTO = new MapLayerGroupConfigurationDTO();

            _mockMapLayerGroupsConfigRepository.Setup(r => r.GetById(groupConfigurationId, false, "MapLayerConfigurations"))
                .ReturnsAsync(ResultDTO<MapLayerGroupConfiguration?>.Ok(groupConfiguration));
            _mockMapper.Setup(m => m.Map<MapLayerGroupConfigurationDTO>(groupConfiguration))
                .Returns(mappedDTO);

            // Act
            var result = await _service.GetAllGroupLayersById(groupConfigurationId);

            // Assert
            Assert.True(result.IsSuccess); // This should now pass
            Assert.Equal(mappedDTO, result.Data);
        }

        [Fact]
        public async Task GetAllGroupLayersById_WhenRepositoryFails_ShouldReturnFailure()
        {
            // Arrange
            var groupConfigurationId = Guid.NewGuid();

            _mockMapLayerGroupsConfigRepository.Setup(r => r.GetById(groupConfigurationId, false, "MapLayerConfigurations"))
                .ReturnsAsync(ResultDTO<MapLayerGroupConfiguration?>.Fail("Error"));

            // Act
            var result = await _service.GetAllGroupLayersById(groupConfigurationId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error", result.ErrMsg);
        }

        [Fact]
        public async Task GetAllGroupLayersById_ShouldReturnException_WhenExceptionOccurs()
        {
            // Arrange
            var groupConfigurationId = Guid.NewGuid();

            _mockMapLayerGroupsConfigRepository.Setup(r => r.GetById(groupConfigurationId, false, "MapLayerConfigurations"))
                .ThrowsAsync(new Exception("Error"));

            // Act
            var result = await _service.GetAllGroupLayersById(groupConfigurationId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Error", result.ErrMsg);

        }

        [Fact]
        public async Task GetMapLayerGroupConfigurationById_ShouldReturnSuccess()
        {
            // Arrange
            var groupConfigurationId = Guid.NewGuid();
            var groupConfigurationEntity = new MapLayerGroupConfiguration();
            var mappedDTO = new MapLayerGroupConfigurationDTO();

            _mockMapLayerGroupsConfigRepository.Setup(r => r.GetById(groupConfigurationId, false, null))
                .ReturnsAsync(ResultDTO<MapLayerGroupConfiguration?>.Ok(groupConfigurationEntity));
            _mockMapper.Setup(m => m.Map<MapLayerGroupConfigurationDTO>(groupConfigurationEntity)).Returns(mappedDTO);

            // Act
            var result = await _service.GetMapLayerGroupConfigurationById(groupConfigurationId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(mappedDTO, result.Data);
        }

        [Fact]
        public async Task GetMapLayerGroupConfigurationById_WhenRepositoryFails_ShouldReturnFailure()
        {
            // Arrange
            var groupConfigurationId = Guid.NewGuid();

            _mockMapLayerGroupsConfigRepository.Setup(r => r.GetById(groupConfigurationId, false, null))
                .ReturnsAsync(ResultDTO<MapLayerGroupConfiguration?>.Fail("Error"));

            // Act
            var result = await _service.GetMapLayerGroupConfigurationById(groupConfigurationId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error", result.ErrMsg);
        }

        [Fact]
        public async Task GetMapLayerGroupConfigurationById_ShouldReturnException_WhenExceptionOccurs()
        {
            // Arrange
            var groupConfigurationId = Guid.NewGuid();

            _mockMapLayerGroupsConfigRepository.Setup(r => r.GetById(groupConfigurationId, false, null))
                .ThrowsAsync(new Exception("Error"));

            // Act
            var result = await _service.GetMapLayerGroupConfigurationById(groupConfigurationId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Error", result.ErrMsg);
        }

        [Fact]
        public async Task CreateMapLayerGroupConfiguration_ShouldReturnOk_WhenCreateIsSuccessful()
        {
            // Arrange
            var dto = new MapLayerGroupConfigurationDTO { };
            var entity = new MapLayerGroupConfiguration { };
            var resultEntity = new MapLayerGroupConfiguration { };

            _mockMapper.Setup(m => m.Map<MapLayerGroupConfiguration>(dto)).Returns(entity);
            _mockMapLayerGroupsConfigRepository.Setup(r => r.CreateAndReturnEntity(entity, true, default))
                .ReturnsAsync(ResultDTO<MapLayerGroupConfiguration>.Ok(resultEntity));

            _mockMapper.Setup(m => m.Map<MapLayerGroupConfigurationDTO>(resultEntity)).Returns(dto);

            // Act
            var result = await _service.CreateMapLayerGroupConfiguration(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(dto, result.Data);
            _mockMapLayerGroupsConfigRepository.Verify(r => r.CreateAndReturnEntity(entity, true, default), Times.Once);
        }


        [Fact]
        public async Task CreateMapLayerGroupConfiguration_ShouldReturnFail_WhenCreateFails()
        {
            // Arrange
            var dto = new MapLayerGroupConfigurationDTO { };
            var entity = new MapLayerGroupConfiguration { };

            _mockMapper.Setup(m => m.Map<MapLayerGroupConfiguration>(dto)).Returns(entity);

            // Adjusted the setup to specify the type in the ResultDTO
            _mockMapLayerGroupsConfigRepository.Setup(r => r.CreateAndReturnEntity(entity, true, default))
                .ReturnsAsync(ResultDTO<MapLayerGroupConfiguration>.Fail("Error message"));

            // Act
            var result = await _service.CreateMapLayerGroupConfiguration(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error message", result.ErrMsg);
        }


        [Fact]
        public async Task CreateMapLayerGroupConfiguration_ShouldLogError_WhenExceptionOccurs()
        {
            // Arrange
            var dto = new MapLayerGroupConfigurationDTO { };
            var entity = new MapLayerGroupConfiguration { };
            _mockMapper.Setup(m => m.Map<MapLayerGroupConfiguration>(dto)).Returns(entity);
            _mockMapLayerGroupsConfigRepository.Setup(r => r.CreateAndReturnEntity(entity, true, default))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _service.CreateMapLayerGroupConfiguration(dto);

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
        public async Task EditMapLayerGroupConfiguration_ShouldReturnOk_WhenEditIsSuccessful()
        {
            // Arrange
            var dto = new MapLayerGroupConfigurationDTO { };
            var entity = new MapLayerGroupConfiguration { };
            _mockMapper.Setup(m => m.Map<MapLayerGroupConfiguration>(dto)).Returns(entity);
            _mockMapLayerGroupsConfigRepository.Setup(r => r.Update(entity, true, default)).ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _service.EditMapLayerGroupConfiguration(dto);

            // Assert
            Assert.True(result.IsSuccess);
            _mockMapLayerGroupsConfigRepository.Verify(r => r.Update(entity, true, default), Times.Once);
        }

        [Fact]
        public async Task EditMapLayerGroupConfiguration_ShouldReturnFail_WhenEditFails()
        {
            // Arrange
            var dto = new MapLayerGroupConfigurationDTO { };
            var entity = new MapLayerGroupConfiguration { };
            _mockMapper.Setup(m => m.Map<MapLayerGroupConfiguration>(dto)).Returns(entity);
            _mockMapLayerGroupsConfigRepository.Setup(r => r.Update(entity, true, default)).ReturnsAsync(ResultDTO.Fail("Error message"));

            // Act
            var result = await _service.EditMapLayerGroupConfiguration(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error message", result.ErrMsg);
        }

        [Fact]
        public async Task EditMapLayerGroupConfiguration_ShouldLogError_WhenExceptionOccurs()
        {
            // Arrange
            var dto = new MapLayerGroupConfigurationDTO { };
            var entity = new MapLayerGroupConfiguration { };
            _mockMapper.Setup(m => m.Map<MapLayerGroupConfiguration>(dto)).Returns(entity);
            _mockMapLayerGroupsConfigRepository.Setup(r => r.Update(entity, true, default)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _service.EditMapLayerGroupConfiguration(dto);

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
        public async Task DeleteMapLayerGroupConfiguration_ShouldReturnOk_WhenDeleteIsSuccessful()
        {
            // Arrange
            var dto = new MapLayerGroupConfigurationDTO { };
            var entity = new MapLayerGroupConfiguration { };
            _mockMapper.Setup(m => m.Map<MapLayerGroupConfiguration>(dto)).Returns(entity);
            _mockMapLayerGroupsConfigRepository.Setup(r => r.Delete(entity, true, default)).ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _service.DeleteMapLayerGroupConfiguration(dto);

            // Assert
            Assert.True(result.IsSuccess);
            _mockMapLayerGroupsConfigRepository.Verify(r => r.Delete(entity, true, default), Times.Once);
        }

        [Fact]
        public async Task DeleteMapLayerGroupConfiguration_ShouldReturnFail_WhenDeleteFails()
        {
            // Arrange
            var dto = new MapLayerGroupConfigurationDTO { };
            var entity = new MapLayerGroupConfiguration { };
            _mockMapper.Setup(m => m.Map<MapLayerGroupConfiguration>(dto)).Returns(entity);
            _mockMapLayerGroupsConfigRepository.Setup(r => r.Delete(entity, true, default)).ReturnsAsync(ResultDTO.Fail("Error message"));

            // Act
            var result = await _service.DeleteMapLayerGroupConfiguration(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error message", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteMapLayerGroupConfiguration_ShouldLogError_WhenExceptionOccurs()
        {
            // Arrange
            var dto = new MapLayerGroupConfigurationDTO { };
            var entity = new MapLayerGroupConfiguration { };
            _mockMapper.Setup(m => m.Map<MapLayerGroupConfiguration>(dto)).Returns(entity);
            _mockMapLayerGroupsConfigRepository.Setup(r => r.Delete(entity, true, default)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _service.DeleteMapLayerGroupConfiguration(dto);

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

    }

}

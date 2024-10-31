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
    public class MapLayerConfigurationServiceTests
    {
        private readonly Mock<IMapLayersConfigurationRepository> _mockMapLayerConfigRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<MapLayersConfigurationService>> _mockLogger;
        private readonly MapLayersConfigurationService _service;

        public MapLayerConfigurationServiceTests()
        {
            _mockMapLayerConfigRepository = new Mock<IMapLayersConfigurationRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<MapLayersConfigurationService>>();
            _service = new MapLayersConfigurationService(
                _mockMapLayerConfigRepository.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );
        }
        [Fact]
        public async Task GetAllMapLayerConfigurations_ShouldReturnSuccess()
        {
            // Arrange
            var entities = new List<MapLayerConfiguration> { new MapLayerConfiguration() };
            var dtos = new List<MapLayerConfigurationDTO> { new MapLayerConfigurationDTO() };
            //Func<IQueryable<MapLayerConfiguration>, IOrderedQueryable<MapLayerConfiguration>> orderBy = query => query.OrderBy(x => x.Order);

            _mockMapLayerConfigRepository.Setup(r => r.GetAll(null, null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<MapLayerConfiguration>>.Ok(entities));

            _mockMapper.Setup(m => m.Map<List<MapLayerConfigurationDTO>>(It.IsAny<IEnumerable<MapLayerConfiguration>>()))
                .Returns(dtos);


            // Act
            var result = await _service.GetAllMapLayerConfigurations();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task GetAllMapLayerConfigurations_ShouldReturnFailure_WhenRepositoryFails()
        {
            // Arrange
            _mockMapLayerConfigRepository.Setup(r => r.GetAll(null, null, false, null, null))
            .ReturnsAsync(ResultDTO<IEnumerable<MapLayerConfiguration>>.Fail("Repository error"));

            // Act
            var result = await _service.GetAllMapLayerConfigurations();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Repository error", result.ErrMsg);
        }

        [Fact]
        public async Task GetMapLayerConfigurationById_ShouldReturnSuccess()
        {
            // Arrange
            var id = Guid.NewGuid();
            var mockEntity = new MapLayerConfiguration();
            var resultDTO = ResultDTO<MapLayerConfiguration>.Ok(mockEntity);
            _mockMapLayerConfigRepository.Setup(r => r.GetById(id, false, null)).ReturnsAsync(resultDTO);
            _mockMapper.Setup(m => m.Map<MapLayerConfigurationDTO>(mockEntity)).Returns(new MapLayerConfigurationDTO());

            // Act
            var result = await _service.GetMapLayerConfigurationById(id);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task GetMapLayerConfigurationById_ShouldReturnFailure_WhenRepositoryFails()
        {
            // Arrange
            var id = Guid.NewGuid();
            var resultDTO = ResultDTO<MapLayerConfiguration>.Fail("Error");
            _mockMapLayerConfigRepository.Setup(r => r.GetById(id, false, null)).ReturnsAsync(resultDTO);

            // Act
            var result = await _service.GetMapLayerConfigurationById(id);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error", result.ErrMsg);
        }

        [Fact]
        public async Task CreateMapLayerConfiguration_ShouldReturnSuccess()
        {
            // Arrange
            var dto = new MapLayerConfigurationDTO();
            var entity = new MapLayerConfiguration();
            var resultEntity = new MapLayerConfiguration();
            var resultDTO = ResultDTO<MapLayerConfiguration>.Ok(resultEntity);

            _mockMapper.Setup(m => m.Map<MapLayerConfiguration>(dto)).Returns(entity);
            _mockMapLayerConfigRepository.Setup(r => r.CreateAndReturnEntity(entity, true, default)).ReturnsAsync(resultDTO);
            _mockMapper.Setup(m => m.Map<MapLayerConfigurationDTO>(resultEntity)).Returns(dto);

            // Act
            var result = await _service.CreateMapLayerConfiguration(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task CreateMapLayerConfiguration_ShouldReturnFailure_WhenRepositoryFails()
        {
            // Arrange
            var dto = new MapLayerConfigurationDTO();
            var entity = new MapLayerConfiguration();
            var resultDTO = ResultDTO<MapLayerConfiguration>.Fail("Error");

            _mockMapper.Setup(m => m.Map<MapLayerConfiguration>(dto)).Returns(entity);
            _mockMapLayerConfigRepository.Setup(r => r.CreateAndReturnEntity(entity, true, default)).ReturnsAsync(resultDTO);

            // Act
            var result = await _service.CreateMapLayerConfiguration(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error", result.ErrMsg);
        }

        [Fact]
        public async Task CreateMapLayerConfiguration_ShouldReturnException_WhenExceptionOccurs()
        {
            // Arrange
            var dto = new MapLayerConfigurationDTO();
            var entity = new MapLayerConfiguration();

            _mockMapper.Setup(m => m.Map<MapLayerConfiguration>(dto)).Returns(entity);
            _mockMapLayerConfigRepository.Setup(r => r.CreateAndReturnEntity(entity, true, default)).ThrowsAsync(new Exception("Error"));

            // Act
            var result = await _service.CreateMapLayerConfiguration(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Error", result.ErrMsg);
        }

        [Fact]
        public async Task EditMapLayerConfiguration_ShouldReturnSuccess()
        {
            // Arrange
            var dto = new MapLayerConfigurationDTO();
            var entity = new MapLayerConfiguration();
            var resultDTO = ResultDTO.Ok();

            _mockMapper.Setup(m => m.Map<MapLayerConfiguration>(dto)).Returns(entity);
            _mockMapLayerConfigRepository.Setup(r => r.Update(entity, true, default)).ReturnsAsync(resultDTO);

            // Act
            var result = await _service.EditMapLayerConfiguration(dto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task EditMapLayerConfiguration_ShouldReturnFailure_WhenRepositoryFails()
        {
            // Arrange
            var dto = new MapLayerConfigurationDTO();
            var entity = new MapLayerConfiguration();
            var resultDTO = ResultDTO.Fail("Error");

            _mockMapper.Setup(m => m.Map<MapLayerConfiguration>(dto)).Returns(entity);
            _mockMapLayerConfigRepository.Setup(r => r.Update(entity, true, default)).ReturnsAsync(resultDTO);

            // Act
            var result = await _service.EditMapLayerConfiguration(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error", result.ErrMsg);
        }

        [Fact]
        public async Task EditMapLayerConfiguration_ShouldReturnException_WhenExceptionOccurs()
        {
            // Arrange
            var dto = new MapLayerConfigurationDTO();
            var entity = new MapLayerConfiguration();

            _mockMapper.Setup(m => m.Map<MapLayerConfiguration>(dto)).Returns(entity);
            _mockMapLayerConfigRepository.Setup(r => r.Update(entity, true, default)).ThrowsAsync(new Exception("Error"));

            // Act
            var result = await _service.EditMapLayerConfiguration(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Error", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteMapLayerConfiguration_ShouldReturnSuccess()
        {
            // Arrange
            var dto = new MapLayerConfigurationDTO();
            var entity = new MapLayerConfiguration();
            var resultDTO = ResultDTO.Ok();

            _mockMapper.Setup(m => m.Map<MapLayerConfiguration>(dto)).Returns(entity);
            _mockMapLayerConfigRepository.Setup(r => r.Delete(entity, true, default)).ReturnsAsync(resultDTO);

            // Act
            var result = await _service.DeleteMapLayerConfiguration(dto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteMapLayerConfiguration_ShouldReturnFailure_WhenRepositoryFails()
        {
            // Arrange
            var dto = new MapLayerConfigurationDTO();
            var entity = new MapLayerConfiguration();
            var resultDTO = ResultDTO.Fail("Error");

            _mockMapper.Setup(m => m.Map<MapLayerConfiguration>(dto)).Returns(entity);
            _mockMapLayerConfigRepository.Setup(r => r.Delete(entity, true, default)).ReturnsAsync(resultDTO);

            // Act
            var result = await _service.DeleteMapLayerConfiguration(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteMapLayerConfiguration_ShouldReturnException_WhenExceptionOccurs()
        {
            // Arrange
            var dto = new MapLayerConfigurationDTO();
            var entity = new MapLayerConfiguration();

            _mockMapper.Setup(m => m.Map<MapLayerConfiguration>(dto)).Returns(entity);
            _mockMapLayerConfigRepository.Setup(r => r.Delete(entity, true, default)).ThrowsAsync(new Exception("Error"));

            // Act
            var result = await _service.DeleteMapLayerConfiguration(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Error", result.ErrMsg);
        }

        [Fact]
        public async Task GetAllMapLayerConfigurations_RepositoryThrowsException_LogsErrorAndReturnsExceptionFail()
        {
            // Arrange

            _mockMapLayerConfigRepository
                .Setup(r => r.GetAll(null, null, false, null, null))
            .ThrowsAsync(new Exception("Database error"));


            // Act
            var result = await _service.GetAllMapLayerConfigurations();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Database error", result.ErrMsg);
        }

        [Fact]
        public async Task GetMapLayerConfigurationById_RepositoryThrowsException_LogsErrorAndReturnsExceptionFail()
        {
            // Arrange
            var mapLayerConfigurationId = Guid.NewGuid();
            _mockMapLayerConfigRepository
                .Setup(r => r.GetById(mapLayerConfigurationId, false, null))
            .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _service.GetMapLayerConfigurationById(mapLayerConfigurationId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Database error", result.ErrMsg);
        }


    }
}

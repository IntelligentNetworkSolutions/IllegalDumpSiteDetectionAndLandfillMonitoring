using AutoMapper;
using DAL.Interfaces.Repositories.MapConfigurationRepositories;
using DTOs.MainApp.BL.MapConfigurationDTOs;
using Entities.MapConfigurationEntities;
using MainApp.BL.Services.MapConfigurationServices;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppBLTests.Services
{
    public class MapConfigurationServiceTests
    {
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
            mapConfigRepositoryMock.Setup(repo => repo.GetMapConfigurationByName(mapName)).ReturnsAsync(new MapConfiguration());

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<MapConfigurationDTO>(It.IsAny<MapConfiguration>())).Returns(expectedMapConfigDTO);

            var mapConfigurationService = new MapConfigurationService(mapConfigRepositoryMock.Object, mapperMock.Object);

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
            mapConfigRepositoryMock.Setup(repo => repo.GetMapConfigurationByName(invalidMapName)).ReturnsAsync((MapConfiguration)null);

            var mapperMock = new Mock<IMapper>();

            var mapConfigurationService = new MapConfigurationService(mapConfigRepositoryMock.Object, mapperMock.Object);

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
            mapConfigRepositoryMock.Setup(repo => repo.GetMapConfigurationByName(mapName)).ReturnsAsync(expectedMapConfig);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<MapConfigurationDTO>(expectedMapConfig)).Returns(expectedMapConfigDTO);

            var mapConfigurationService = new MapConfigurationService(mapConfigRepositoryMock.Object, mapperMock.Object);

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
            mapConfigRepositoryMock.Setup(repo => repo.GetMapConfigurationByName(mapName)).ReturnsAsync(new MapConfiguration());

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<MapConfigurationDTO>(It.IsAny<MapConfiguration>())).Returns((MapConfigurationDTO)null);

            var mapConfigurationService = new MapConfigurationService(mapConfigRepositoryMock.Object, mapperMock.Object);

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

            var mapConfigurationService = new MapConfigurationService(mapConfigRepositoryMock.Object, mapperMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => mapConfigurationService.GetMapConfigurationByName(emptyMapName));
        }

        [Fact]
        public async Task GetMapConfigurationByName_RepositoryThrowsException_ThrowsException()
        {
            // Arrange
            string mapName = "ValidMapName";
            var mapConfigRepositoryMock = new Mock<IMapConfigurationRepository>();
            mapConfigRepositoryMock.Setup(repo => repo.GetMapConfigurationByName(mapName)).ThrowsAsync(new Exception("Repository exception"));

            var mapperMock = new Mock<IMapper>();

            var mapConfigurationService = new MapConfigurationService(mapConfigRepositoryMock.Object, mapperMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => mapConfigurationService.GetMapConfigurationByName(mapName));
        }

        [Fact]
        public async Task GetMapConfigurationByName_RepositoryReturnsNull_ThrowsException()
        {
            // Arrange
            string mapName = "NonExistentMapName";
            var mapConfigRepositoryMock = new Mock<IMapConfigurationRepository>();
            mapConfigRepositoryMock.Setup(repo => repo.GetMapConfigurationByName(mapName)).ReturnsAsync((MapConfiguration)null);

            var mapperMock = new Mock<IMapper>();

            var mapConfigurationService = new MapConfigurationService(mapConfigRepositoryMock.Object, mapperMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => mapConfigurationService.GetMapConfigurationByName(mapName));
        }
    }
}

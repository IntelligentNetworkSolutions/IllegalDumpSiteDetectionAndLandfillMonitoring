using DTOs.MainApp.BL.DetectionDTOs;
using MainApp.BL.Interfaces.Services.DetectionServices;
using MainApp.MVC.Areas.Common.Controllers;
using MainApp.MVC.Areas.IntranetPortal.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppMVCTests.Areas.IntranetPortal.Controllers
{
    public class DetectionIgnoreZonesControllerTests
    {
        private readonly Mock<IDetectionIgnoreZoneService> _detectionIgnoreZoneServiceMock;
        private readonly DetectionIgnoreZonesController _controller;

        public DetectionIgnoreZonesControllerTests()
        {
            _detectionIgnoreZoneServiceMock = new Mock<IDetectionIgnoreZoneService>();
            _controller = new DetectionIgnoreZonesController(_detectionIgnoreZoneServiceMock.Object);
        }

        [Fact]
        public async Task GetAllIgnoreZones_ReturnsOk_WhenZonesExist()
        {
            // Arrange
            var ignoreZones = new List<DetectionIgnoreZoneDTO>
            {
                new DetectionIgnoreZoneDTO { Id = Guid.NewGuid(), Name = "Zone1" }
            };
            var resultDto = ResultDTO<List<DetectionIgnoreZoneDTO>>.Ok(ignoreZones);

            _detectionIgnoreZoneServiceMock.Setup(service => service.GetAllIgnoreZonesDTOs())
                .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.GetAlllIgnoreZones();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(ignoreZones, result.Data);
        }

        [Fact]
        public async Task GetAllIgnoreZones_ReturnsFail_WhenZonesNotFound()
        {
            // Arrange
            var resultDto = ResultDTO<List<DetectionIgnoreZoneDTO>>.Ok(null);

            _detectionIgnoreZoneServiceMock.Setup(service => service.GetAllIgnoreZonesDTOs())
                .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.GetAlllIgnoreZones();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Igonre zones are not found", result.ErrMsg);
        }

        [Fact]
        public async Task GetAllIgnoreZones_ReturnsFail_WhenServiceFails()
        {
            // Arrange
            var resultDto = ResultDTO<List<DetectionIgnoreZoneDTO>>.Fail("Service failure");

            _detectionIgnoreZoneServiceMock.Setup(service => service.GetAllIgnoreZonesDTOs())
                .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.GetAlllIgnoreZones();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Service failure", result.ErrMsg);
        }

        [Fact]
        public async Task GetIgnoreZoneById_ReturnsOk_WhenZoneExists()
        {
            // Arrange
            var zoneId = Guid.NewGuid();
            var ignoreZone = new DetectionIgnoreZoneDTO { Id = zoneId, Name = "Zone1" };
            var resultDto = ResultDTO<DetectionIgnoreZoneDTO?>.Ok(ignoreZone);

            _detectionIgnoreZoneServiceMock.Setup(service => service.GetIgnoreZoneById(zoneId))
                .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.GetIgnoreZoneById(zoneId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(ignoreZone, result.Data);
        }

        [Fact]
        public async Task GetIgnoreZoneById_ReturnsFail_WhenZoneIdIsEmpty()
        {
            // Act
            var result = await _controller.GetIgnoreZoneById(Guid.Empty);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid zone id", result.ErrMsg);
        }

        [Fact]
        public async Task GetIgnoreZoneById_ReturnsFail_WhenServiceFails()
        {
            // Arrange
            var zoneId = Guid.NewGuid();
            var resultDto = ResultDTO<DetectionIgnoreZoneDTO?>.Fail("Service failure");

            _detectionIgnoreZoneServiceMock.Setup(service => service.GetIgnoreZoneById(zoneId))
                .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.GetIgnoreZoneById(zoneId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Service failure", result.ErrMsg);
        }              

        [Fact]
        public async Task AddIgnoreZone_ReturnsFail_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("error", "Model validation error");

            var dto = new DetectionIgnoreZoneDTO();

            // Act
            var result = await _controller.AddIgnoreZone(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Model validation error", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteIgnoreZone_ReturnsOk_WhenZoneIsDeleted()
        {
            // Arrange
            var zoneId = Guid.NewGuid();
            var ignoreZone = new DetectionIgnoreZoneDTO { Id = zoneId, Name = "Zone1" };

            _detectionIgnoreZoneServiceMock.Setup(service => service.GetIgnoreZoneById(zoneId))
                .ReturnsAsync(ResultDTO<DetectionIgnoreZoneDTO?>.Ok(ignoreZone));

            _detectionIgnoreZoneServiceMock.Setup(service => service.DeleteDetectionIgnoreZoneFromDTO(ignoreZone))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _controller.DeleteIgnoreZone(zoneId);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteIgnoreZone_ReturnsFail_WhenZoneIdIsEmpty()
        {
            // Act
            var result = await _controller.DeleteIgnoreZone(Guid.Empty);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid zone id", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteIgnoreZone_ReturnsFail_WhenZoneDoesNotExist()
        {
            // Arrange
            var zoneId = Guid.NewGuid();

            _detectionIgnoreZoneServiceMock.Setup(service => service.GetIgnoreZoneById(zoneId))
                .ReturnsAsync(ResultDTO<DetectionIgnoreZoneDTO?>.Ok(null));

            // Act
            var result = await _controller.DeleteIgnoreZone(zoneId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Ignore zone does not exist", result.ErrMsg);
        }

        [Fact]
        public async Task UpdateIgnoreZone_ReturnsFail_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("error", "Model validation error");

            var dto = new DetectionIgnoreZoneDTO();

            // Act
            var result = await _controller.UpdateIgnoreZone(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Model validation error", result.ErrMsg);
        }

        [Fact]
        public async Task UpdateIgnoreZone_ReturnsFail_WhenServiceFails()
        {
            // Arrange
            var dto = new DetectionIgnoreZoneDTO { Id = Guid.NewGuid() };

            _detectionIgnoreZoneServiceMock.Setup(service => service.UpdateDetectionIgnoreZoneFromDTO(dto))
                .ReturnsAsync(ResultDTO.Fail("Service failure"));

            // Act
            var result = await _controller.UpdateIgnoreZone(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Service failure", result.ErrMsg);
        }

    }
}

using AutoMapper;
using DAL.Interfaces.Helpers;
using MainApp.BL.Interfaces.Services.DatasetServices;
using MainApp.BL.Interfaces.Services;
using MainApp.MVC.Areas.IntranetPortal.Controllers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using DTOs.MainApp.BL.DatasetDTOs;
using MainApp.MVC.ViewModels.IntranetPortal.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using SD;

namespace Tests.MainAppMVCTests.Areas.IntranetPortal.Controllers
{
    public class AnnotationsControllerTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IAppSettingsAccessor> _mockAppSettingsAccessor;
        private readonly Mock<IDatasetService> _mockDatasetService;
        private readonly Mock<IDatasetImagesService> _mockDatasetImagesService;
        private readonly Mock<IImageAnnotationsService> _mockImageAnnotationsService;
        private readonly Mock<IDatasetClassesService> _mockDatasetClassesService;
        private readonly AnnotationsController _controller;

        public AnnotationsControllerTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockMapper = new Mock<IMapper>();
            _mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();
            _mockDatasetService = new Mock<IDatasetService>();
            _mockDatasetImagesService = new Mock<IDatasetImagesService>();
            _mockImageAnnotationsService = new Mock<IImageAnnotationsService>();
            _mockDatasetClassesService = new Mock<IDatasetClassesService>();

            _controller = new AnnotationsController(
                _mockConfiguration.Object,
                _mockMapper.Object,
                _mockAppSettingsAccessor.Object,
                _mockDatasetService.Object,
                _mockDatasetImagesService.Object,
                _mockImageAnnotationsService.Object,
                _mockDatasetClassesService.Object);
        }

        [Fact]
        public async Task Annotate_InvalidDatasetImageId_ThrowsException()
        {
            // Arrange
            var datasetImageId = Guid.Empty;

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.Annotate(datasetImageId));
        }

        [Fact]
        public async Task Annotate_ValidDatasetImageId_ReturnsViewResult()
        {
            // Arrange
            var datasetImageId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();

            var datasetImage = new DatasetImageDTO { Id = datasetImageId, DatasetId = datasetId };
            var datasetAllImages = new List<DatasetImageDTO> { datasetImage };
            var dataset = new DatasetDTO { Id = datasetId };
            var datasetClasses = new List<DatasetClassDTO>();

            _mockDatasetImagesService.Setup(service => service.GetDatasetImageById(datasetImageId))
                .ReturnsAsync(datasetImage);

            _mockDatasetImagesService.Setup(service => service.GetImagesForDataset(datasetId))
                .ReturnsAsync(datasetAllImages);

            _mockDatasetService.Setup(service => service.GetDatasetById(datasetId))
                .ReturnsAsync(dataset);

            _mockDatasetClassesService.Setup(service => service.GetAllDatasetClassesByDatasetId(datasetId))
                .ReturnsAsync(datasetClasses);

            // Act
            var result = await _controller.Annotate(datasetImageId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AnnotateViewModel>(viewResult.ViewData.Model);
            Assert.Equal(datasetImage, model.DatasetImage);
            Assert.Equal(dataset, model.Dataset);
            Assert.Equal(datasetClasses, model.DatasetClasses);
            Assert.Equal(1, model.CurrentImagePositionInDataset);
            Assert.Null(model.NextImage);
            Assert.Null(model.PreviousImage);
            Assert.Equal(1, model.TotalImagesCount);
        }

        [Fact]
        public async Task Annotate_DatasetImageNotFound_ThrowsException()
        {
            // Arrange
            var datasetImageId = Guid.NewGuid();

            _mockDatasetImagesService.Setup(service => service.GetDatasetImageById(datasetImageId))
                .ReturnsAsync((DatasetImageDTO)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.Annotate(datasetImageId));
        }

        [Fact]
        public async Task GetImageAnnotations_InvalidDatasetImageId_ThrowsException()
        {
            // Arrange
            var datasetImageId = Guid.Empty;

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.GetImageAnnotations(datasetImageId));
        }

        [Fact]
        public async Task GetImageAnnotations_ValidDatasetImageId_ReturnsOkResult()
        {
            // Arrange
            var datasetImageId = Guid.NewGuid();
            var imageAnnotations = new List<ImageAnnotationDTO>
            {
                new ImageAnnotationDTO { Id = Guid.NewGuid(), DatasetImageId = datasetImageId }
            };

            _mockImageAnnotationsService.Setup(service => service.GetImageAnnotationsByImageId(datasetImageId))
                .ReturnsAsync(imageAnnotations);

            // Act
            var result = await _controller.GetImageAnnotations(datasetImageId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAnnotations = Assert.IsType<List<ImageAnnotationDTO>>(okResult.Value);
            Assert.Equal(imageAnnotations, returnedAnnotations);
        }

        [Fact]
        public async Task SaveImageAnnotations_NullAnnotations_ReturnsOkResult()
        {
            // Arrange
            var editImageAnnotations = new EditImageAnnotationsDTO
            {
                DatasetImageId = Guid.NewGuid(),
                ImageAnnotations = null
            };

            var resultDto = new ResultDTO<bool>(true, true, "", null);
            _mockImageAnnotationsService.Setup(service => service.BulkUpdateImageAnnotations(It.IsAny<EditImageAnnotationsDTO>()))
                .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.SaveImageAnnotations(editImageAnnotations);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResult = Assert.IsType<ResultDTO<bool>>(okResult.Value);
            Assert.True(returnedResult.Data);
        }

        [Fact]
        public async Task SaveImageAnnotations_ValidAnnotations_ReturnsOkResult()
        {
            // Arrange
            var datasetImageId = Guid.NewGuid();
            var userId = "test-user-id";
            var imageAnnotations = new List<ImageAnnotationDTO>
            {
                new ImageAnnotationDTO { Id = null, AnnotationJson = "{}", DatasetImageId = datasetImageId },
                new ImageAnnotationDTO { Id = Guid.NewGuid(), AnnotationJson = "{}", DatasetImageId = datasetImageId }
            };

            var editImageAnnotations = new EditImageAnnotationsDTO
            {
                DatasetImageId = datasetImageId,
                ImageAnnotations = imageAnnotations
            };

            var claims = new List<Claim> { new Claim("UserId", userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var resultDto = new ResultDTO<bool>(true, true, "", null);
            _mockImageAnnotationsService.Setup(service => service.BulkUpdateImageAnnotations(It.IsAny<EditImageAnnotationsDTO>()))
                .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.SaveImageAnnotations(editImageAnnotations);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResult = Assert.IsType<ResultDTO<bool>>(okResult.Value);
            Assert.True(returnedResult.Data);
            _mockImageAnnotationsService.Verify(service => service.BulkUpdateImageAnnotations(It.IsAny<EditImageAnnotationsDTO>()), Times.Once);
        }

    }
}

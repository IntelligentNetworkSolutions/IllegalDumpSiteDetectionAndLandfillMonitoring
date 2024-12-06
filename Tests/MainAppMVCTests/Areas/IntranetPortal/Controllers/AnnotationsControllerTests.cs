using AutoMapper;
using DAL.Interfaces.Helpers;
using DTOs.MainApp.BL.DatasetDTOs;
using MainApp.BL.Interfaces.Services;
using MainApp.BL.Interfaces.Services.DatasetServices;
using MainApp.MVC.Areas.IntranetPortal.Controllers;
using MainApp.MVC.ViewModels.IntranetPortal.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json.Linq;
using SD;
using System.Security.Claims;

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
        public async Task Annotate_ValidDatasetImageId_ReturnsCorrectResult()
        {
            // Arrange
            var datasetImageId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();
            var datasetImage = new DatasetImageDTO { Id = datasetImageId, DatasetId = datasetId };
            var datasetAllImages = new List<DatasetImageDTO> { datasetImage };
            var dataset = new DatasetDTO { Id = datasetId };
            var datasetClasses = new List<DatasetClassDTO>();

            _mockDatasetImagesService.Setup(service => service.GetDatasetImageById(datasetImageId))
                .ReturnsAsync(ResultDTO<DatasetImageDTO>.Ok(datasetImage));

            _mockDatasetImagesService.Setup(service => service.GetImagesForDataset(datasetId))
                .ReturnsAsync(ResultDTO<List<DatasetImageDTO>>.Ok(datasetAllImages));

            _mockDatasetService.Setup(service => service.GetDatasetById(datasetId))
                .ReturnsAsync(ResultDTO<DatasetDTO>.Ok(dataset));

            _mockDatasetClassesService.Setup(service => service.GetAllDatasetClassesByDatasetId(datasetId))
                .ReturnsAsync(ResultDTO<List<DatasetClassDTO>>.Ok(datasetClasses));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = await _controller.Annotate(datasetImageId);

            // Assert
            if (_controller.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var jsonResult = Assert.IsType<JsonResult>(result);
                var jsonResponse = Assert.IsType<Dictionary<string, object>>(jsonResult.Value);

                Assert.True((bool)jsonResponse["responseSuccess"]);
                var model = Assert.IsType<AnnotateViewModel>(jsonResponse["model"]);

                Assert.Equal(datasetImage, model.DatasetImage);
                Assert.Equal(dataset, model.Dataset);
                Assert.Equal(datasetClasses, model.DatasetClasses);
                Assert.Equal(1, model.CurrentImagePositionInDataset);
                Assert.Equal(datasetImage, model.NextImage);
                Assert.Equal(datasetImage, model.PreviousImage);
                Assert.Equal(1, model.TotalImagesCount);
            }
            else
            {
                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsType<AnnotateViewModel>(viewResult.ViewData.Model);

                Assert.Equal(datasetImage, model.DatasetImage);
                Assert.Equal(dataset, model.Dataset);
                Assert.Equal(datasetClasses, model.DatasetClasses);
                Assert.Equal(1, model.CurrentImagePositionInDataset);
                Assert.Equal(datasetImage, model.NextImage);
                Assert.Equal(datasetImage, model.PreviousImage);
                Assert.Equal(1, model.TotalImagesCount);
            }
        }

        [Fact]
        public async Task GetImageAnnotations_InvalidDatasetImageId_ReturnsJsonError()
        {
            // Arrange
            var datasetImageId = Guid.Empty;

            // Act
            var result = await _controller.GetImageAnnotations(datasetImageId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var json = JObject.FromObject(jsonResult.Value);
            Assert.False(json["responseSuccess"]?.Value<bool>());
            Assert.Equal("Such image does not exist.", json["responseError"]?.ToString());

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
                .ReturnsAsync(ResultDTO<List<ImageAnnotationDTO>>.Ok(imageAnnotations));

            // Act
            var result = await _controller.GetImageAnnotations(datasetImageId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var json = JObject.FromObject(okResult.Value);
            Assert.True(json["responseSuccess"]?.Value<bool>());
            var returnedAnnotations = json["data"]?.ToObject<List<ImageAnnotationDTO>>();
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
            var responseObject = okResult.Value;

            // Verify the structure of the anonymous object
            var json = JObject.FromObject(responseObject);
            Assert.True(json["isSuccess"]?.Value<bool>());
            Assert.NotNull(json["data"]);
        }

        [Fact]
        public async Task SaveImageAnnotations_ValidAnnotations_ReturnsJsonResult()
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
            var responseObject = okResult.Value;

            var json = JObject.FromObject(responseObject);
            Assert.True(json["isSuccess"]?.Value<bool>());
            Assert.NotNull(json["data"]);
        }


        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task Annotate_NoDatasetImageFound_ReturnsJsonError()
        {
            // Arrange
            var datasetImageId = Guid.NewGuid();

            _mockDatasetImagesService.Setup(service => service.GetDatasetImageById(datasetImageId))
                .ReturnsAsync(ResultDTO<DatasetImageDTO>.Fail("An error occurred. Could not retrieve dataset image."));

            // Act
            var result = await _controller.Annotate(datasetImageId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var json = JObject.FromObject(jsonResult.Value);

            Assert.False(json["responseSuccess"]?.Value<bool>(), "Expected responseSuccess to be false.");
            Assert.Equal("An error occurred. Could not retrieve dataset image.", json["responseError"]?.Value<string>());
        }


    }
}

﻿using AutoMapper;
using DAL.Interfaces.Helpers;
using DTOs.MainApp.BL;
using DTOs.MainApp.BL.DatasetDTOs;
using MainApp.BL.Interfaces.Services.DatasetServices;
using MainApp.MVC.Areas.IntranetPortal.Controllers;
using MainApp.MVC.ViewModels.IntranetPortal.Dataset;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json.Linq;
using SD;
using System.Security.Claims;

namespace Tests.MainAppMVCTests.Areas.IntranetPortal.Controllers
{
    public class DatasetsControllerTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IDatasetService> _mockDatasetService;
        private readonly Mock<IDatasetImagesService> _mockDatasetImagesService;
        private readonly Mock<IWebHostEnvironment> _mockWebHostEnvironment;
        private readonly Mock<IAppSettingsAccessor> _mockAppSettingsAccessor;
        private readonly DatasetsController _controller;

        public DatasetsControllerTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockMapper = new Mock<IMapper>();
            _mockDatasetService = new Mock<IDatasetService>();
            _mockDatasetImagesService = new Mock<IDatasetImagesService>();
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();

            _controller = new DatasetsController(
                _mockConfiguration.Object,
                _mockMapper.Object,
                _mockDatasetService.Object,
                _mockDatasetImagesService.Object,
                _mockWebHostEnvironment.Object,
                _mockAppSettingsAccessor.Object
            );

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, "userId")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public async Task GetParentAndChildrenDatasets_ReturnsJsonResult_ForValidDatasetId()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var parentDataset = new DatasetDTO { Id = datasetId, Name = "Parent Dataset" };
            var childDataset1 = new DatasetDTO { Id = Guid.NewGuid(), Name = "Child Dataset 1", ParentDatasetId = datasetId };
            var childDataset2 = new DatasetDTO { Id = Guid.NewGuid(), Name = "Child Dataset 2", ParentDatasetId = datasetId };

            _mockDatasetService.Setup(s => s.GetAllDatasets()).ReturnsAsync(new List<DatasetDTO> { childDataset1, childDataset2 });
            _mockDatasetService.Setup(s => s.GetDatasetById(datasetId)).ReturnsAsync(parentDataset);

            // Act
            var result = await _controller.GetParentAndChildrenDatasets(datasetId) as JsonResult;
            var data = JObject.FromObject(result.Value);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetParentAndChildrenDatasets_ReturnsErrorJsonResult_ForInvalidDatasetId()
        {
            // Arrange
            var invalidDatasetId = Guid.Empty;

            // Act
            var result = await _controller.GetParentAndChildrenDatasets(invalidDatasetId) as JsonResult;
            var data = JObject.FromObject(result.Value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Invalid dataset id", data["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task Edit_ReturnsNotFound_WhenDatasetIdIsEmpty()
        {
            // Arrange
            Guid emptyId = Guid.Empty;

            // Act
            var result = await _controller.Edit(emptyId, null, null, null, null, null, null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsViewWithModel_WhenDatasetIsNotPublished()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var datasetDto = new DatasetDTO { IsPublished = false };
            var editDto = new EditDatasetDTO
            {
                ListOfDatasetImages = new List<DatasetImageDTO> { },
                NumberOfDatasetImages = 100
            };
            _mockDatasetService.Setup(service => service.GetDatasetById(datasetId)).ReturnsAsync(datasetDto);
            _mockDatasetService.Setup(service => service.GetObjectForEditDataset(It.IsAny<Guid>(), null, null, null, null, 1, 20))
                .ReturnsAsync(editDto);

            var model = new EditDatasetViewModel { /* initialize with properties */ };
            _mockMapper.Setup(m => m.Map<EditDatasetViewModel>(editDto)).Returns(model);

            // Act
            var result = await _controller.Edit(datasetId, null, null, null, null, null, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var modelResult = Assert.IsAssignableFrom<EditDatasetViewModel>(viewResult.ViewData.Model);
            Assert.Equal(model, modelResult);
        }

        [Fact]
        public async Task PublishDataset_ReturnsJsonError_WhenUserIdIsNull()
        {
            // Arrange
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { }))
                }
            };
            var datasetId = Guid.NewGuid();

            // Act
            var result = await _controller.PublishDataset(datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("User id is not valid", jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task PublishDataset_ReturnsJsonError_WhenDatasetIdIsInvalid()
        {
            // Arrange
            var userId = "test-user-id";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };
            var datasetId = Guid.Empty;

            // Act
            var result = await _controller.PublishDataset(datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Incorect dataset id", jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task PublishDataset_SuccessfullyPublishesDataset()
        {
            // Arrange
            var userId = "test-user-id";
            var datasetId = Guid.NewGuid();
            var datasetDto = new DatasetDTO { IsPublished = false };
            var resultDto = new ResultDTO<int>(true, 1, null, null);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            // Mocking dependencies
            _mockDatasetService.Setup(service => service.GetDatasetById(datasetId)).ReturnsAsync(datasetDto);
            _mockDatasetService.Setup(service => service.PublishDataset(datasetId, userId)).ReturnsAsync(resultDto);
            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<int>("NumberOfImagesNeededToPublishDataset", 100))
                                    .ReturnsAsync(ResultDTO<int>.Ok(100));
            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<int>("NumberOfClassesNeededToPublishDataset", 1))
                                    .ReturnsAsync(ResultDTO<int>.Ok(1));

            // Act
            var result = await _controller.PublishDataset(datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Successfully published dataset", jsonData["responseSuccess"]["Value"].ToString());
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfDatasets()
        {
            // Arrange
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(mapper => mapper.Map<List<DatasetViewModel>>(It.IsAny<List<DatasetDTO>>()))
                .Returns(new List<DatasetViewModel>());

            var mockDatasetService = new Mock<IDatasetService>();
            mockDatasetService.Setup(service => service.GetAllDatasets())
                .ReturnsAsync(new List<DatasetDTO>());

            var controller = new DatasetsController(null, mockMapper.Object, mockDatasetService.Object, null, null, null);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<DatasetViewModel>>(viewResult.Model);
            Assert.Empty(model);
        }

        [Fact]
        public async Task GetAllDatasets_ReturnsListOfDatasets()
        {
            // Arrange
            var expectedDatasets = new List<DatasetDTO>
            {
                new DatasetDTO { Id = Guid.NewGuid(), Name = "Dataset 1" },
                new DatasetDTO { Id = Guid.NewGuid(), Name = "Dataset 2" }
            };

            var mockDatasetService = new Mock<IDatasetService>();
            mockDatasetService.Setup(service => service.GetAllDatasets()).ReturnsAsync(expectedDatasets);
            var controller = new DatasetsController(null, null, mockDatasetService.Object, null, null, null);

            // Act
            var result = await controller.GetAllDatasets();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<DatasetDTO>>(result);
            Assert.Equal(expectedDatasets.Count, result.Count);
        }

        [Fact]
        public async Task CreateConfirmed_ReturnsJsonError_WhenParentDatasetIsNotPublished()
        {
            // Arrange
            var parentDatasetId = Guid.NewGuid();
            var datasetViewModel = new CreateDatasetViewModel { ParentDatasetId = parentDatasetId };
            _mockDatasetService.Setup(s => s.GetDatasetById(parentDatasetId))
                .ReturnsAsync(new DatasetDTO { IsPublished = false });

            // Act
            var result = await _controller.CreateConfirmed(datasetViewModel) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var jsonData = JObject.FromObject(result.Value);
            Assert.Equal("User id is not valid", jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task DeleteDatasetConfirmed_ReturnsJsonSuccess_WhenSuccessful()
        {
            // Arrange
            var datasetId = Guid.NewGuid();

            // Mocking the successful deletion result from the dataset service
            _mockDatasetService.Setup(s => s.DeleteDatasetCompletelyIncluded(datasetId))
                .ReturnsAsync(ResultDTO.Ok());

            // Mocking the application settings for dataset images and thumbnails
            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>("DatasetImagesFolder", "DatasetImages"))
                .ReturnsAsync(new ResultDTO<string>(true, "DatasetImages", null, null));
            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>("DatasetThumbnailsFolder", "DatasetThumbnails"))
                .ReturnsAsync(new ResultDTO<string>(true, "DatasetThumbnails", null, null));

            // Mocking the web host environment
            _mockWebHostEnvironment.Setup(e => e.WebRootPath).Returns("wwwroot");

            // Ensure the directories exist for the test
            var datasetImagesFolderPath = Path.Combine("wwwroot", "DatasetImages", datasetId.ToString());
            var datasetThumbnailsFolderPath = Path.Combine("wwwroot", "DatasetThumbnails", datasetId.ToString());

            Directory.CreateDirectory(datasetImagesFolderPath);
            Directory.CreateDirectory(datasetThumbnailsFolderPath);

            // Act
            var result = await _controller.DeleteDatasetConfirmed(datasetId) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var jsonData = JObject.FromObject(result.Value);
            Assert.Equal("Successfully deleted dataset", jsonData["responseSuccess"]["Value"].ToString());

            // Verify that the folders were deleted
            Assert.False(Directory.Exists(datasetImagesFolderPath));
            Assert.False(Directory.Exists(datasetThumbnailsFolderPath));
        }

        [Fact]
        public async Task DeleteDatasetConfirmed_InvalidId_ReturnsJsonError()
        {
            // Arrange
            var datasetId = Guid.Empty;

            // Act
            var result = await _controller.DeleteDatasetConfirmed(datasetId) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var jsonData = JObject.FromObject(result.Value);
            Assert.Equal("Invalid dataset id", jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task DeleteDatasetConfirmed_DeletionFails_ReturnsJsonError()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            _mockDatasetService.Setup(s => s.DeleteDatasetCompletelyIncluded(datasetId))
                .ReturnsAsync(new ResultDTO(false, "Failed to delete dataset", null));

            // Act
            var result = await _controller.DeleteDatasetConfirmed(datasetId) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var jsonData = JObject.FromObject(result.Value);
            Assert.Equal("Failed to delete dataset", jsonData["responseError"]["Value"].ToString());
        }



        //[Fact]
        //public async Task ImportDataset_ValidCocoFormattedDirectoryAtPath_ReturnsDataset()
        //{
        //    // Arrange - Arrange your objects, create and set ImportDataset_ValidCocoFormattedDirectoryAtPath them up as necessary.
        //    string pathToCocoFormattedDirectory = "";

        //    _mockDatasetService.Setup(s => s.ImportDatasetCocoFormatedAtDirectoryPath(pathToCocoFormattedDirectory))
        //        .ReturnsAsync(pathToCocoFormattedDirectory);

        //    // Act - Act on an object ValidCocoFormattedDirectoryAtPath.


        //    // Assert - Assert that something is as expected ReturnsDataset.

        //}

    }
}

using AutoMapper;
using DAL.Interfaces.Helpers;
using DTOs.MainApp.BL.DatasetDTOs;
using MainApp.BL.Interfaces.Services.DatasetServices;
using MainApp.MVC.Areas.IntranetPortal.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json.Linq;
using SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppMVCTests.Areas.IntranetPortal.Controllers
{
    public class DatasetImagesControllerTests
    {
        private readonly DatasetImagesController _controller;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IDatasetService> _mockDatasetService;
        private readonly Mock<IDatasetImagesService> _mockDatasetImagesService;
        private readonly Mock<IWebHostEnvironment> _mockWebHostEnvironment;
        private readonly Mock<IAppSettingsAccessor> _mockAppSettingsAccessor;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor;

        public DatasetImagesControllerTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockMapper = new Mock<IMapper>();
            _mockDatasetService = new Mock<IDatasetService>();
            _mockDatasetImagesService = new Mock<IDatasetImagesService>();
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();

            _controller = new DatasetImagesController(
                _mockConfiguration.Object,
                _mockMapper.Object,
                _mockDatasetService.Object,
                _mockDatasetImagesService.Object,
                _mockWebHostEnvironment.Object,
                _mockAppSettingsAccessor.Object);

            // Setup User
            var identity = new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, "1")
            }, "TestAuthType");

            var user = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task UploadDatasetImage_ReturnsError_WhenDatasetNotFound()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var imageCropped = "data:image/jpeg;base64,/9j/4AAQSkZJRgABA...";
            var imageName = "testimage";

            _mockDatasetService.Setup(ds => ds.GetDatasetById(datasetId)).ReturnsAsync((DatasetDTO)null);

            // Act
            var result = await _controller.UploadDatasetImage(datasetId, imageCropped, imageName);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = JObject.FromObject(jsonResult.Value);
            Assert.Equal("User id is not valid", data["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task EditDatasetImage_ReturnsError_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("key", "error");
            var model = new EditDatasetImageDTO { DatasetId = Guid.NewGuid() };

            // Act
            var result = await _controller.EditDatasetImage(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = JObject.FromObject(jsonResult.Value);
            Assert.Equal("User id is not valid", data["responseError"]["Value"].ToString());
        }



        [Fact]
        public async Task EditDatasetImage_ReturnsError_WhenDatasetNotFound()
        {
            // Arrange
            var model = new EditDatasetImageDTO { DatasetId = Guid.NewGuid(), Name = "TestImage" };
            _mockDatasetService.Setup(s => s.GetDatasetById(model.DatasetId)).ReturnsAsync((DatasetDTO)null); // Simulate dataset not found

            // Act
            var result = await _controller.EditDatasetImage(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = JObject.FromObject(jsonResult.Value);
            Assert.Equal("User id is not valid", data["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task DeleteDatasetImage_ReturnsError_WhenDatasetIdIsEmpty()
        {
            // Arrange
            var datasetImageId = Guid.NewGuid();
            var datasetId = Guid.Empty;

            // Act
            var result = await _controller.DeleteDatasetImage(datasetImageId, datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Invalid dataset id", data["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task DeleteDatasetImage_ReturnsError_WhenDatasetImageIdIsEmpty()
        {
            // Arrange
            var datasetImageId = Guid.Empty;
            var datasetId = Guid.NewGuid();

            // Act
            var result = await _controller.DeleteDatasetImage(datasetImageId, datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Invalid dataset image id", data["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task DeleteDatasetImage_ReturnsError_WhenDatasetIsPublished()
        {
            // Arrange
            var datasetImageId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();
            var dataset = new DatasetDTO { IsPublished = true };
            _mockDatasetService.Setup(s => s.GetDatasetById(datasetId)).ReturnsAsync(dataset);

            // Act
            var result = await _controller.DeleteDatasetImage(datasetImageId, datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Dataset is already published. No changes allowed", data["responseErrorAlreadyPublished"]["Value"].ToString());
        }

       
    }
}

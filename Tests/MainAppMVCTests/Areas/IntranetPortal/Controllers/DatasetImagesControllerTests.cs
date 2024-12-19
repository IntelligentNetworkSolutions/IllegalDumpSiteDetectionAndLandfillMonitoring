using AutoMapper;
using DAL.Interfaces.Helpers;
using DTOs.MainApp.BL.DatasetDTOs;
using MainApp.BL.Interfaces.Services;
using MainApp.BL.Interfaces.Services.DatasetServices;
using MainApp.MVC.Areas.IntranetPortal.Controllers;
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
    public class DatasetImagesControllerTests
    {
        private readonly DatasetImagesController _controller;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IDatasetService> _mockDatasetService;
        private readonly Mock<IDatasetImagesService> _mockDatasetImagesService;
        private readonly Mock<IImageAnnotationsService> _mockImageAnnotationsService;
        private readonly Mock<IWebHostEnvironment> _mockWebHostEnvironment;
        private readonly Mock<IAppSettingsAccessor> _mockAppSettingsAccessor;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor;

        public DatasetImagesControllerTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockMapper = new Mock<IMapper>();
            _mockDatasetService = new Mock<IDatasetService>();
            _mockDatasetImagesService = new Mock<IDatasetImagesService>();
            _mockImageAnnotationsService = new Mock<IImageAnnotationsService>();
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();


            _controller = new DatasetImagesController(
                _mockConfiguration.Object,
                _mockMapper.Object,
                _mockDatasetService.Object,
                _mockDatasetImagesService.Object,
                _mockImageAnnotationsService.Object,
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

            _mockDatasetService.Setup(ds => ds.GetDatasetById(datasetId)).ReturnsAsync((ResultDTO<DatasetDTO>)null);

            // Act
            var result = await _controller.UploadDatasetImage(datasetId, imageCropped, imageName);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = JObject.FromObject(jsonResult.Value);
            Assert.Equal("User id is not valid", data["responseError"]["Value"].ToString());
        }
        [Fact]
        public async Task UploadDatasetImage_ReturnsError_WhenUserIdIsNotValid()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var imageCropped = "data:image/jpeg;base64,/9j/4AAQSkZJRgABA...";
            var imageName = "testimage";

            var claims = new List<Claim>(); // No UserId
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await _controller.UploadDatasetImage(datasetId, imageCropped, imageName);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = JObject.FromObject(jsonResult.Value);
            Assert.Equal("User id is not valid", data["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task UploadDatasetImage_ReturnsError_WhenDatasetIdIsEmpty()
        {
            // Arrange
            var datasetId = Guid.Empty;
            var imageCropped = "data:image/jpeg;base64,/9j/4AAQSkZJRgABA...";
            var imageName = "testimage";
            var userId = "test-user-id";

            var claims = new List<Claim> { new Claim("UserId", userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await _controller.UploadDatasetImage(datasetId, imageCropped, imageName);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Invalid dataset id", data["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task UploadDatasetImage_ReturnsError_WhenImageNameIsInvalid()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var imageCropped = "data:image/jpeg;base64,/9j/4AAQSkZJRgABA...";
            string imageName = null;
            var userId = "test-user-id";

            var claims = new List<Claim> { new Claim("UserId", userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await _controller.UploadDatasetImage(datasetId, imageCropped, imageName);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Invalid image name", data["responseError"]["Value"].ToString());
        }


        [Fact]
        public async Task UploadDatasetImage_ReturnsError_WhenImageCroppedIsInvalid()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            string imageCropped = null;
            var imageName = "testimage";
            var userId = "test-user-id";

            var claims = new List<Claim> { new Claim("UserId", userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await _controller.UploadDatasetImage(datasetId, imageCropped, imageName);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Invalid image", data["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task UploadDatasetImage_ReturnsError_WhenDatasetIsPublished()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var imageCropped = "data:image/jpeg;base64,/9j/4AAQSkZJRgABA...";
            var imageName = "testimage";

            var datasetDto = new DatasetDTO { IsPublished = true };
            _mockDatasetService.Setup(ds => ds.GetDatasetById(datasetId)).ReturnsAsync(ResultDTO<DatasetDTO>.Ok(datasetDto));

            var userId = "test-user-id";

            var claims = new List<Claim> { new Claim("UserId", userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };


            // Act
            var result = await _controller.UploadDatasetImage(datasetId, imageCropped, imageName);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Dataset is already published. No changes allowed", data["responseErrorAlreadyPublished"]["Value"].ToString());
        }

        [Fact]
        public async Task UploadDatasetImage_Proceeds_WhenDatasetIsNotPublished()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var imageCropped = "data:image/jpeg;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNk+A8AAQUBAScY42YAAAAASUVORK5CYII=";
            var imageName = "testimage";

            var datasetDto = new DatasetDTO { IsPublished = false };
            _mockDatasetService.Setup(ds => ds.GetDatasetById(datasetId)).ReturnsAsync(ResultDTO<DatasetDTO>.Ok(datasetDto));

            var tempImagesFolder = Path.Combine(Path.GetTempPath(), "DatasetImages");
            var tempThumbnailsFolder = Path.Combine(Path.GetTempPath(), "DatasetThumbnails");

            // Ensure directories exist
            Directory.CreateDirectory(tempImagesFolder);
            Directory.CreateDirectory(tempThumbnailsFolder);

            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>("DatasetImagesFolder", "DatasetImages"))
                .ReturnsAsync(ResultDTO<string?>.Ok(tempImagesFolder));
            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>("DatasetThumbnailsFolder", "DatasetThumbnails"))
                .ReturnsAsync(ResultDTO<string?>.Ok(tempThumbnailsFolder));

            _mockWebHostEnvironment.Setup(wh => wh.WebRootPath).Returns(Path.GetTempPath());

            var resultGuid = Guid.NewGuid();
            _mockDatasetImagesService
                .Setup(s => s.AddDatasetImage(It.IsAny<DatasetImageDTO>()))
                .ReturnsAsync(ResultDTO<Guid>.Ok(resultGuid));

            var claims = new List<Claim> { new Claim("UserId", "test-user-id") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await _controller.UploadDatasetImage(datasetId, imageCropped, imageName);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Successfully added dataset image", data["responseSuccess"]["Value"].ToString());
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
            Assert.Equal("Entered model is not valid", data["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task EditDatasetImage_ReturnsError_WhenUserIdIsMissing()
        {
            // Arrange
            var model = new EditDatasetImageDTO { DatasetId = Guid.NewGuid() };

            // Act
            var result = await _controller.EditDatasetImage(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = JObject.FromObject(jsonResult.Value);
            Assert.Equal("User id is not valid", data["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task EditDatasetImage_ShouldReturnFailure_WhenGetDatasetByIdFails()
        {
            // Arrange
            var editDto = new EditDatasetImageDTO
            {
                DatasetId = Guid.NewGuid(),
                Id = Guid.NewGuid(),
                IsEnabled = false,
                UpdatedById = Guid.NewGuid().ToString()
            };

            var userId = "test-user-id";

            var claims = new List<Claim> { new Claim("UserId", userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockDatasetService.Setup(x => x.GetDatasetById(editDto.DatasetId))
                .ReturnsAsync(ResultDTO<DatasetDTO>.Fail("Error retrieving dataset"));

            // Act
            var result = await _controller.EditDatasetImage(editDto);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Error retrieving dataset", data["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task EditDatasetImage_ShouldReturnFailure_WhenUnexpectedExceptionOccurs()
        {
            // Arrange
            var editDto = new EditDatasetImageDTO
            {
                DatasetId = Guid.NewGuid(),
                Id = Guid.NewGuid(),
                IsEnabled = false,
                UpdatedById = Guid.NewGuid().ToString()
            };

            var userId = "test-user-id";

            var claims = new List<Claim> { new Claim("UserId", userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockDatasetService.Setup(x => x.GetDatasetById(editDto.DatasetId))
                .ThrowsAsync(new Exception("An unexpected error occurred: Unexpected error"));

            // Act
            var result = await _controller.EditDatasetImage(editDto);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = JObject.FromObject(jsonResult.Value);
            Assert.Equal("An unexpected error occurred: An unexpected error occurred: Unexpected error", data["responseError"]["Value"].ToString());
        }


        [Fact]
        public async Task EditDatasetImage_ReturnsError_WhenDatasetNotFound()
        {
            // Arrange
            var model = new EditDatasetImageDTO { DatasetId = Guid.NewGuid(), Name = "TestImage" };
            _mockDatasetService.Setup(s => s.GetDatasetById(model.DatasetId)).ReturnsAsync((ResultDTO<DatasetDTO>)null);

            // Act
            var result = await _controller.EditDatasetImage(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = JObject.FromObject(jsonResult.Value);
            Assert.Equal("User id is not valid", data["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task EditDatasetImage_ReturnsError_WhenDatasetIsPublished()
        {
            // Arrange
            var model = new EditDatasetImageDTO { DatasetId = Guid.NewGuid(), Name = "TestImage" };
            var datasetDto = new DatasetDTO { IsPublished = true };
            _mockDatasetService.Setup(s => s.GetDatasetById(model.DatasetId)).ReturnsAsync(ResultDTO<DatasetDTO>.Ok(datasetDto));
            var userId = "test-user-id";

            var claims = new List<Claim> { new Claim("UserId", userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
            // Act
            var result = await _controller.EditDatasetImage(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Dataset is already published. No changes allowed", data["responseErrorAlreadyPublished"]["Value"].ToString());
        }

        [Fact]
        public async Task EditDatasetImage_ReturnsSuccess_WhenImageIsUpdatedSuccessfully()
        {
            // Arrange
            var model = new EditDatasetImageDTO { DatasetId = Guid.NewGuid(), Id = Guid.NewGuid(), Name = "TestImage" };
            var datasetDto = new DatasetDTO { IsPublished = false };
            var datasetImageDto = new DatasetImageDTO { ImageAnnotations = new List<ImageAnnotationDTO> { new ImageAnnotationDTO() } }; // With annotations
            _mockDatasetService.Setup(s => s.GetDatasetById(model.DatasetId)).ReturnsAsync(ResultDTO<DatasetDTO>.Ok(datasetDto));
            _mockDatasetImagesService.Setup(s => s.GetDatasetImageById(model.Id)).ReturnsAsync(ResultDTO<DatasetImageDTO>.Ok(datasetImageDto));
            _mockDatasetImagesService.Setup(s => s.EditDatasetImage(model)).ReturnsAsync(ResultDTO<int>.Ok(1));
            var userId = "test-user-id";

            var claims = new List<Claim> { new Claim("UserId", userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
            // Act
            var result = await _controller.EditDatasetImage(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Successfully updated dataset image", data["responseSuccess"]["Value"].ToString());
        }

        [Fact]
        public async Task EditDatasetImage_ReturnsError_WhenImageUpdateFailsWithErrorMessage()
        {
            // Arrange
            var model = new EditDatasetImageDTO { DatasetId = Guid.NewGuid(), Id = Guid.NewGuid(), Name = "TestImage" };
            var datasetDto = new DatasetDTO { IsPublished = false };
            var datasetImageDto = new DatasetImageDTO { ImageAnnotations = new List<ImageAnnotationDTO> { new ImageAnnotationDTO() } };
            _mockDatasetService.Setup(s => s.GetDatasetById(model.DatasetId)).ReturnsAsync(ResultDTO<DatasetDTO>.Ok(datasetDto));
            _mockDatasetImagesService.Setup(s => s.GetDatasetImageById(model.Id)).ReturnsAsync(ResultDTO<DatasetImageDTO>.Ok(datasetImageDto));
            _mockDatasetImagesService.Setup(s => s.EditDatasetImage(model)).ReturnsAsync(ResultDTO<int>.Fail("Some error occurred"));
            var userId = "test-user-id";

            var claims = new List<Claim> { new Claim("UserId", userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
            // Act
            var result = await _controller.EditDatasetImage(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Some error occurred", data["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task EditDatasetImage_ReturnsError_WhenImageUpdateFailsWithoutErrorMessage()
        {
            // Arrange
            var model = new EditDatasetImageDTO { DatasetId = Guid.NewGuid(), Id = Guid.NewGuid(), Name = "TestImage" };
            var datasetDto = new DatasetDTO { IsPublished = false };
            var datasetImageDto = new DatasetImageDTO { ImageAnnotations = new List<ImageAnnotationDTO> { new ImageAnnotationDTO() } };
            _mockDatasetService.Setup(s => s.GetDatasetById(model.DatasetId)).ReturnsAsync(ResultDTO<DatasetDTO>.Ok(datasetDto));
            _mockDatasetImagesService.Setup(s => s.GetDatasetImageById(model.Id)).ReturnsAsync(ResultDTO<DatasetImageDTO>.Ok(datasetImageDto));
            _mockDatasetImagesService.Setup(s => s.EditDatasetImage(model)).ReturnsAsync(ResultDTO<int>.Fail("Dataset image was not updated"));
            var userId = "test-user-id";

            var claims = new List<Claim> { new Claim("UserId", userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
            // Act
            var result = await _controller.EditDatasetImage(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Dataset image was not updated", data["responseError"]["Value"].ToString());
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
            _mockDatasetService.Setup(s => s.GetDatasetById(datasetId)).ReturnsAsync(ResultDTO<DatasetDTO>.Ok(dataset));

            // Act
            var result = await _controller.DeleteDatasetImage(datasetImageId, datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Dataset is already published. No changes allowed", data["responseErrorAlreadyPublished"]["Value"].ToString());
        }

        [Fact]
        public async Task DeleteDatasetImage_ReturnsError_WhenDatasetPublished()
        {
            // Arrange
            var datasetImageId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();
            var datasetDto = new DatasetDTO { IsPublished = true };

            _mockDatasetService.Setup(s => s.GetDatasetById(datasetId)).ReturnsAsync(ResultDTO<DatasetDTO>.Ok(datasetDto));

            // Act
            var result = await _controller.DeleteDatasetImage(datasetImageId, datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Dataset is already published. No changes allowed", data["responseErrorAlreadyPublished"]["Value"].ToString());
        }

        [Fact]
        public async Task DeleteDatasetImage_ReturnsError_WhenDatasetImageDoesNotExist()
        {
            // Arrange
            var datasetImageId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();
            var datasetDto = new DatasetDTO { IsPublished = false };

            _mockDatasetService.Setup(s => s.GetDatasetById(datasetId)).ReturnsAsync(ResultDTO<DatasetDTO>.Ok(datasetDto));
            _mockDatasetImagesService.Setup(s => s.GetDatasetImageById(datasetImageId)).ReturnsAsync(ResultDTO<DatasetImageDTO>.Fail("Dataset image not found."));

            // Act
            var result = await _controller.DeleteDatasetImage(datasetImageId, datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = JObject.FromObject(jsonResult.Value);
            Assert.Equal($"Dataset image not found.", data["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task DeleteDatasetImage_ReturnsSuccess_WhenImageIsDeleted()
        {
            // Arrange
            var datasetImageId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();
            var datasetDto = new DatasetDTO { IsPublished = false };
            var datasetImageDto = new DatasetImageDTO { Id = datasetImageId };
            var annotations = new List<ImageAnnotationDTO>();
            _mockDatasetService.Setup(s => s.GetDatasetById(datasetId)).ReturnsAsync(ResultDTO<DatasetDTO>.Ok(datasetDto));
            _mockDatasetImagesService.Setup(s => s.GetDatasetImageById(datasetImageId)).ReturnsAsync(ResultDTO<DatasetImageDTO>.Ok(datasetImageDto));
            _mockImageAnnotationsService.Setup(s => s.GetImageAnnotationsByImageId(datasetImageId))
                .ReturnsAsync(ResultDTO<List<ImageAnnotationDTO>>.Ok(annotations));
            _mockDatasetImagesService.Setup(s => s.DeleteDatasetImage(datasetImageId, false))
                .ReturnsAsync(ResultDTO<int>.Ok(1));

            // Act
            var result = await _controller.DeleteDatasetImage(datasetImageId, datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Successfully deleted dataset image", data["responseSuccess"]["Value"].ToString());
        }

        [Fact]
        public async Task DeleteDatasetImage_ReturnsError_WhenGetDatasetByIdFails()
        {
            // Arrange
            var datasetImageId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();
            _mockDatasetService.Setup(s => s.GetDatasetById(datasetId))
                .ReturnsAsync(ResultDTO<DatasetDTO>.Fail("Error retrieving dataset"));

            // Act
            var result = await _controller.DeleteDatasetImage(datasetImageId, datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Error retrieving dataset", data["responseError"]["Value"].ToString());
        }



        [Fact]
        public async Task DeleteDatasetImage_ReturnsError_WhenImageHasActiveAnnotations()
        {
            // Arrange
            var datasetImageId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();
            var datasetDto = new DatasetDTO { IsPublished = false };
            var datasetImageDto = new DatasetImageDTO { Id = datasetImageId };
            var activeAnnotations = new List<ImageAnnotationDTO> { new ImageAnnotationDTO() }; // Mock active annotations

            _mockDatasetService.Setup(s => s.GetDatasetById(datasetId)).ReturnsAsync(ResultDTO<DatasetDTO>.Ok(datasetDto));
            _mockDatasetImagesService.Setup(s => s.GetDatasetImageById(datasetImageId)).ReturnsAsync(ResultDTO<DatasetImageDTO>.Ok(datasetImageDto));
            _mockImageAnnotationsService.Setup(s => s.GetImageAnnotationsByImageId(datasetImageId)).ReturnsAsync(ResultDTO<List<ImageAnnotationDTO>>.Ok(activeAnnotations));

            // Act
            var result = await _controller.DeleteDatasetImage(datasetImageId, datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = JObject.FromObject(jsonResult.Value);
            Assert.Equal("This image has active annotations. Do you want to continue anyway?", data["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task DeleteDatasetImage_ReturnsError_WhenGetImageAnnotationsFails()
        {
            // Arrange
            var datasetImageId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();
            var datasetDto = new DatasetDTO { IsPublished = false };
            var datasetImageDto = new DatasetImageDTO { Id = datasetImageId };
            _mockDatasetService.Setup(s => s.GetDatasetById(datasetId)).ReturnsAsync(ResultDTO<DatasetDTO>.Ok(datasetDto));
            _mockDatasetImagesService.Setup(s => s.GetDatasetImageById(datasetImageId)).ReturnsAsync(ResultDTO<DatasetImageDTO>.Ok(datasetImageDto));
            _mockImageAnnotationsService.Setup(s => s.GetImageAnnotationsByImageId(datasetImageId))
                .ReturnsAsync(ResultDTO<List<ImageAnnotationDTO>>.Fail("Error retrieving annotations"));

            // Act
            var result = await _controller.DeleteDatasetImage(datasetImageId, datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Failed to retrive annotations.Error retrieving annotations", data["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task DeleteDatasetImage_ReturnsError_WhenExceptionOccurs()
        {
            // Arrange
            var datasetImageId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();
            _mockDatasetService.Setup(s => s.GetDatasetById(datasetId))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.DeleteDatasetImage(datasetImageId, datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Error occurred while deleting the image. Unexpected error", data["responseError"]["Value"].ToString());
        }


        [Fact]
        public async Task DeleteDatasetImage_DeletesWithAnnotations_WhenFlagSet()
        {
            // Arrange
            var datasetImageId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();
            var datasetDto = new DatasetDTO { IsPublished = false };
            var datasetImageDto = new DatasetImageDTO { Id = datasetImageId };
            var activeAnnotations = new List<ImageAnnotationDTO> { new ImageAnnotationDTO() }; // Mock active annotations

            _mockDatasetService.Setup(s => s.GetDatasetById(datasetId)).ReturnsAsync(ResultDTO<DatasetDTO>.Ok(datasetDto));
            _mockDatasetImagesService.Setup(s => s.GetDatasetImageById(datasetImageId)).ReturnsAsync(ResultDTO<DatasetImageDTO>.Ok(datasetImageDto));
            _mockImageAnnotationsService.Setup(s => s.GetImageAnnotationsByImageId(datasetImageId)).ReturnsAsync(ResultDTO<List<ImageAnnotationDTO>>.Ok(activeAnnotations));
            _mockDatasetImagesService.Setup(s => s.DeleteDatasetImage(datasetImageId, true))
                .ReturnsAsync(ResultDTO<int>.Ok(1));

            // Act
            var result = await _controller.DeleteDatasetImage(datasetImageId, datasetId, true);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Successfully deleted dataset image", data["responseSuccess"]["Value"].ToString());
        }

        [Fact]
        public async Task DeleteDatasetImage_ReturnsError_WhenDeletionFails()
        {
            // Arrange
            var datasetImageId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();
            var datasetDto = new DatasetDTO { IsPublished = false };
            var datasetImageDto = new DatasetImageDTO { Id = datasetImageId };
            var activeAnnotations = new List<ImageAnnotationDTO>(); // Mock active annotations

            // Mock the dataset service to return a valid dataset
            _mockDatasetService.Setup(s => s.GetDatasetById(datasetId)).ReturnsAsync(ResultDTO<DatasetDTO>.Ok(datasetDto));
            _mockDatasetImagesService.Setup(s => s.GetDatasetImageById(datasetImageId)).ReturnsAsync(ResultDTO<DatasetImageDTO>.Ok(datasetImageDto));
            _mockImageAnnotationsService.Setup(s => s.GetImageAnnotationsByImageId(datasetImageId)).ReturnsAsync(ResultDTO<List<ImageAnnotationDTO>>.Ok(activeAnnotations));

            // Mock the deletion to return a failure result
            _mockDatasetImagesService.Setup(s => s.DeleteDatasetImage(datasetImageId, false))
                .ReturnsAsync(ResultDTO<int>.Fail("Error occurred while deleting the image"));

            // Act
            var result = await _controller.DeleteDatasetImage(datasetImageId, datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Error occurred while deleting the image", data["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task EditDatasetImage_ReturnsError_WhenUpdateFails()
        {
            // Arrange
            var model = new EditDatasetImageDTO
            {
                DatasetId = Guid.NewGuid(),
                Name = "SampleImage"
            };

            var userId = "test-user-id";

            var claims = new List<Claim> { new Claim("UserId", userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            }; var datasetDto = new DatasetDTO { IsPublished = false };

            // Mock the dataset service to return a valid dataset
            _mockDatasetService.Setup(s => s.GetDatasetById(model.DatasetId)).ReturnsAsync(ResultDTO<DatasetDTO>.Ok(datasetDto));

            // Mock the user ID

            // Mock the image update to fail
            _mockDatasetImagesService.Setup(s => s.EditDatasetImage(model))
                .ReturnsAsync(ResultDTO<int>.Fail("Failed to update image"));

            // Act
            var result = await _controller.EditDatasetImage(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Failed to update image", data["responseError"]["Value"].ToString());
        }


    }
}

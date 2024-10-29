using AutoMapper;
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
        public async Task GetParentAndChildrenDatasets_ThrowsException_WhenGetAllDatasetsReturnsNull()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            _mockDatasetService.Setup(s => s.GetAllDatasets()).ReturnsAsync((List<DatasetDTO>)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _controller.GetParentAndChildrenDatasets(datasetId));
        }

        [Fact]
        public async Task GetParentAndChildrenDatasets_ThrowsException_WhenGetDatasetByIdReturnsNull()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            _mockDatasetService.Setup(s => s.GetAllDatasets()).ReturnsAsync(new List<DatasetDTO>());
            _mockDatasetService.Setup(s => s.GetDatasetById(datasetId)).ReturnsAsync((DatasetDTO)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _controller.GetParentAndChildrenDatasets(datasetId));
        }

        [Fact]
        public async Task GetParentAndChildrenDatasets_ThrowsException_WhenChildrenDatasetsAreNotFound()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var parentDataset = new DatasetDTO { Id = datasetId, Name = "Parent Dataset" };

            _mockDatasetService.Setup(s => s.GetAllDatasets()).ReturnsAsync((List<DatasetDTO>)null);
            _mockDatasetService.Setup(s => s.GetDatasetById(datasetId)).ReturnsAsync(parentDataset);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _controller.GetParentAndChildrenDatasets(datasetId));
        }

        [Fact]
        public async Task GetParentAndChildrenDatasets_ThrowsException_WhenCurrentDatasetIsNull()
        {
            var datasetId = Guid.NewGuid();
            var allDatasets = new List<DatasetDTO> { new DatasetDTO { ParentDatasetId = datasetId } };
            _mockDatasetService.Setup(s => s.GetAllDatasets()).ReturnsAsync(allDatasets);
            _mockDatasetService.Setup(s => s.GetDatasetById(datasetId)).ReturnsAsync((DatasetDTO)null);

            await Assert.ThrowsAsync<Exception>(async () => await _controller.GetParentAndChildrenDatasets(datasetId));
        }

        [Fact]
        public async Task GetParentAndChildrenDatasets_ReturnsJsonResult_WhenCurrentDatasetIsNotNull()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var grandparentDataset = new DatasetDTO { Id = Guid.NewGuid(), Name = "Grandparent Dataset" };
            var parentDataset = new DatasetDTO
            {
                Id = datasetId,
                Name = "Parent Dataset",
                ParentDataset = grandparentDataset // Correctly set the parent dataset
            };
            var childDataset1 = new DatasetDTO { Id = Guid.NewGuid(), Name = "Child Dataset 1", ParentDatasetId = datasetId };
            var childDataset2 = new DatasetDTO { Id = Guid.NewGuid(), Name = "Child Dataset 2", ParentDatasetId = datasetId };

            _mockDatasetService.Setup(s => s.GetAllDatasets())
                .ReturnsAsync(new List<DatasetDTO> { childDataset1, childDataset2 });
            _mockDatasetService.Setup(s => s.GetDatasetById(datasetId))
                .ReturnsAsync(parentDataset);

            // Act
            var result = await _controller.GetParentAndChildrenDatasets(datasetId) as JsonResult;
            var data = JObject.FromObject(result.Value);

            // Assert
            Assert.NotNull(result);

            // Check the parent dataset
            var returnedParentDataset = data["parent"].ToObject<DatasetDTO>();
            Assert.NotNull(returnedParentDataset);
            Assert.Equal(grandparentDataset.Id, returnedParentDataset.Id);
            Assert.Equal(grandparentDataset.Name, returnedParentDataset.Name);

            // Check children list
            var childrenList = data["childrenList"].ToObject<List<DatasetDTO>>();
            Assert.Equal(2, childrenList.Count());

            // Check the current dataset
            var returnedCurrentDataset = data["currentDataset"].ToObject<DatasetDTO>();
            Assert.Equal(parentDataset.Id, returnedCurrentDataset.Id);
            Assert.Equal(parentDataset.Name, returnedCurrentDataset.Name);
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
        public async Task Edit_ThrowsException_WhenDatasetIsNotFound()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            _mockDatasetService.Setup(s => s.GetDatasetById(datasetId)).ReturnsAsync((DatasetDTO)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _controller.Edit(datasetId, null, null, null, null, null, null));
        }

        [Fact]
        public async Task EnableAllImages_ReturnsNotFound_WhenDatasetIdIsEmpty()
        {
            // Act
            var result = await _controller.EnableAllImages(Guid.Empty);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task EnableAllImages_ReturnsJsonError_WhenEnableAllImagesFails()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            _mockDatasetService.Setup(s => s.EnableAllImagesInDataset(datasetId)).ReturnsAsync(ResultDTO.Fail("Error"));

            // Act
            var result = await _controller.EnableAllImages(datasetId) as JsonResult;
            var jsonData = JObject.FromObject(result.Value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("An error occured while enabeling images.", jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task EnableAllImages_ReturnsJsonSuccess_WhenEnableAllImagesSucceeds()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            _mockDatasetService.Setup(s => s.EnableAllImagesInDataset(datasetId)).ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _controller.EnableAllImages(datasetId) as JsonResult;
            var jsonData = JObject.FromObject(result.Value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("All dataset images have been enabled", jsonData["responseSuccess"]["Value"].ToString());
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
        public async Task PublishDataset_ThrowsException_WhenDatasetNotFound()
        {
            // Arrange
            var userId = "test-user-id";
            var datasetId = Guid.NewGuid();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            // Mocking dependencies
            _mockDatasetService.Setup(service => service.GetDatasetById(datasetId)).ReturnsAsync((DatasetDTO)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _controller.PublishDataset(datasetId));
        }

        [Fact]
        public async Task PublishDataset_ReturnsJsonError_WhenDatasetIsAlreadyPublished()
        {
            // Arrange
            var userId = "test-user-id";
            var datasetId = Guid.NewGuid();
            var datasetDto = new DatasetDTO { IsPublished = true };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            // Mocking dependencies
            _mockDatasetService.Setup(service => service.GetDatasetById(datasetId)).ReturnsAsync(datasetDto);

            // Act
            var result = await _controller.PublishDataset(datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Dataset is already published. No changes allowed", jsonData["responseErrorAlreadyPublished"]["Value"].ToString());
        }

        [Fact]
        public async Task PublishDataset_ReturnsJsonError_WhenInsufficientImagesOrClasses()
        {
            // Arrange
            var userId = "test-user-id";
            var datasetId = Guid.NewGuid();
            var datasetDto = new DatasetDTO { IsPublished = false };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            // Mocking dependencies
            _mockDatasetService.Setup(service => service.GetDatasetById(datasetId)).ReturnsAsync(datasetDto);
            _mockDatasetService.Setup(service => service.PublishDataset(datasetId, userId)).ReturnsAsync(new ResultDTO<int>(IsSuccess: false, 2, null, null));
            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<int>("NumberOfImagesNeededToPublishDataset", 100))
                                    .ReturnsAsync(ResultDTO<int>.Ok(100));
            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<int>("NumberOfClassesNeededToPublishDataset", 1))
                                    .ReturnsAsync(ResultDTO<int>.Ok(1));

            // Act
            var result = await _controller.PublishDataset(datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Insert at least 100 images, 1 class/es and annotate all enabled images to publish the dataset", jsonData["responseError"]["Value"].ToString());
        }


        [Fact]
        public async Task PublishDataset_ReturnsJsonError_WhenPublishingFails()
        {
            // Arrange
            var userId = "test-user-id";
            var datasetId = Guid.NewGuid();
            var datasetDto = new DatasetDTO { IsPublished = false };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            // Mocking dependencies
            _mockDatasetService.Setup(service => service.GetDatasetById(datasetId)).ReturnsAsync(datasetDto);
            _mockDatasetService.Setup(service => service.PublishDataset(datasetId, userId)).ReturnsAsync(ResultDTO<int>.Fail("fail"));

            // Act
            var result = await _controller.PublishDataset(datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Dataset was not published", jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task ExportDataset_ReturnsJsonError_WhenDatasetIdIsInvalid()
        {
            // Arrange
            var datasetId = Guid.Empty;
            var exportOption = "option";
            var downloadLocation = "location";
            var asSplit = false;

            // Act
            var result = await _controller.ExportDataset(datasetId, exportOption, downloadLocation, asSplit);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Invalid dataset id", jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task ExportDataset_ReturnsJsonError_WhenExportFails()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var exportOption = "option";
            var downloadLocation = "location";
            var asSplit = false;
            var errorMessage = "Export failed";

            _mockDatasetService.Setup(service => service.ExportDatasetAsCOCOFormat(datasetId, exportOption, downloadLocation, asSplit))
                               .ReturnsAsync(new ResultDTO<string>(false, null, errorMessage, null));

            // Act
            var result = await _controller.ExportDataset(datasetId, exportOption, downloadLocation, asSplit);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal(errorMessage, jsonData["responseError"]["Value"].ToString());
        }

        // Check after
        //[Fact]
        //public async Task ExportDataset_ReturnsFile_WhenExportSucceeds()
        //{
        //    // Arrange
        //    var datasetId = Guid.NewGuid();
        //    var exportOption = "option";
        //    var downloadLocation = "location";
        //    var asSplit = false;

        //    // Create a mock path to return from the service
        //    var zipFilePath = "path/to/dataset.zip";

        //    // Simulate the zip file data in a MemoryStream
        //    var memoryStream = new MemoryStream();
        //    using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        //    {
        //        var zipEntry = archive.CreateEntry("dummy.txt");
        //        using (var entryStream = zipEntry.Open())
        //        using (var writer = new StreamWriter(entryStream))
        //        {
        //            await writer.WriteAsync("This is a dummy file inside the zip.");
        //        }
        //    }
        //    memoryStream.Position = 0; // Reset the position of the stream

        //    // Mocking the service to return the MemoryStream
        //    _mockDatasetService.Setup(service => service.ExportDatasetAsCOCOFormat(datasetId, exportOption, downloadLocation, asSplit))
        //                       .ReturnsAsync(new ResultDTO<string>(true, zipFilePath, null, null));

        //    // Act
        //    var result = await _controller.ExportDataset(datasetId, exportOption, downloadLocation, asSplit);

        //    // Since the actual controller uses FileStream, simulate opening a file stream
        //    var fileResult = Assert.IsType<FileStreamResult>(result);

        //    // Assert the response headers
        //    Assert.Equal("application/zip", fileResult.ContentType);
        //    Assert.Equal($"{datasetId}.zip", fileResult.FileDownloadName);

        //    // Note: You can't directly assert the MemoryStream here since it's not part of the controller's 
        //    // output in this test setup. This part assumes your controller correctly handles 
        //    // the MemoryStream returned from your service.
        //}


        //TODO
        [Fact]
        public async Task CleanupTempFiles_ReturnsOk_WhenFileExists()
        {
            // Arrange
            var fileGuid = "testfile.tmp";
            var filePath = Path.Combine(Path.GetTempPath(), fileGuid);
            System.IO.File.WriteAllText(filePath, "test content");

            // Act
            var result = await _controller.CleanupTempFilesFromExportDataset(fileGuid);

            // Assert
            Assert.IsType<OkResult>(result);
            Assert.False(System.IO.File.Exists(filePath));
        }

        [Fact]
        public async Task CleanupTempFiles_ReturnsOk_WhenFileDoesNotExist()
        {
            // Arrange
            var fileGuid = "nonexistentfile.tmp";

            // Act
            var result = await _controller.CleanupTempFilesFromExportDataset(fileGuid);

            // Assert
            Assert.IsType<OkResult>(result);
        }


        [Fact]
        public async Task AddDatasetClass_ReturnsJsonError_WhenUserIdIsNull()
        {
            // Arrange
            var selectedClassId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { }))
                }
            };

            // Act
            var result = await _controller.AddDatasetClass(selectedClassId, datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("User id is not valid", jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task AddDatasetClass_ReturnsJsonError_WhenDatasetIdIsInvalid()
        {
            // Arrange
            var selectedClassId = Guid.NewGuid();
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
            var result = await _controller.AddDatasetClass(selectedClassId, datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Invalid dataset id", jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task AddDatasetClass_ReturnsJsonError_WhenSelectedClassIdIsInvalid()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var userId = "test-user-id";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };
            var selectedClassId = Guid.Empty;

            // Act
            var result = await _controller.AddDatasetClass(selectedClassId, datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Invalid dataset class id", jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task AddDatasetClass_ReturnsJsonError_WhenDatasetIsPublished()
        {
            // Arrange
            var selectedClassId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();
            var datasetDto = new DatasetDTO { IsPublished = true };
            var userId = "test-user-id";
            _mockDatasetService.Setup(service => service.GetDatasetById(datasetId)).ReturnsAsync(datasetDto);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            // Act
            var result = await _controller.AddDatasetClass(selectedClassId, datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Dataset is already published. No changes allowed", jsonData["responseErrorAlreadyPublished"]["Value"].ToString());
        }

        [Fact]
        public async Task AddDatasetClass_SuccessfullyAddsClass()
        {
            // Arrange
            var selectedClassId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();
            var datasetDto = new DatasetDTO { IsPublished = false };
            var userId = "test-user-id";
            var resultDto = new ResultDTO<int>(true, 1, null, null);
            _mockDatasetService.Setup(service => service.GetDatasetById(datasetId)).ReturnsAsync(datasetDto);
            _mockDatasetService.Setup(service => service.AddDatasetClassForDataset(selectedClassId, datasetId, userId)).ReturnsAsync(resultDto);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            // Act
            var result = await _controller.AddDatasetClass(selectedClassId, datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Successfully added dataset class", jsonData["responseSuccess"]["Value"].ToString());
        }

        [Fact]
        public async Task AddDatasetClass_ReturnsJsonError_WhenErrorOccursWhileAddingClass()
        {
            // Arrange
            var selectedClassId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();
            var datasetDto = new DatasetDTO { IsPublished = false };
            var userId = "test-user-id";
            var errorMessage = "An error occurred";
            var resultDto = new ResultDTO<int>(false, 0, errorMessage, null);
            _mockDatasetService.Setup(service => service.GetDatasetById(datasetId)).ReturnsAsync(datasetDto);
            _mockDatasetService.Setup(service => service.AddDatasetClassForDataset(selectedClassId, datasetId, userId)).ReturnsAsync(resultDto);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            // Act
            var result = await _controller.AddDatasetClass(selectedClassId, datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal(errorMessage, jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task AddDatasetClass_ReturnsJsonError_WhenClassNotAdded()
        {
            // Arrange
            var selectedClassId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();
            var datasetDto = new DatasetDTO { IsPublished = false };
            var userId = "test-user-id";
            var resultDto = new ResultDTO<int>(false, 0, null, null);
            _mockDatasetService.Setup(service => service.GetDatasetById(datasetId)).ReturnsAsync(datasetDto);
            _mockDatasetService.Setup(service => service.AddDatasetClassForDataset(selectedClassId, datasetId, userId)).ReturnsAsync(resultDto);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            // Act
            var result = await _controller.AddDatasetClass(selectedClassId, datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Dataset class was not added", jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task AddDatasetClass_ThrowsException_WhenDatasetNotFound()
        {
            // Arrange
            string userId = "test-user-id";
            var datasetId = Guid.NewGuid();
            var selectedClassId = Guid.NewGuid();

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            _mockDatasetService.Setup(s => s.GetDatasetById(datasetId)).ReturnsAsync((DatasetDTO)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _controller.AddDatasetClass(selectedClassId, datasetId));
        }


        [Fact]
        public async Task ExportDataset_ReturnsJsonError_WhenExceptionOccurs()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var exportOption = "option";
            var downloadLocation = "location";
            var asSplit = false;

            _mockDatasetService.Setup(service => service.ExportDatasetAsCOCOFormat(datasetId, exportOption, downloadLocation, asSplit))
                               .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.ExportDataset(datasetId, exportOption, downloadLocation, asSplit);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Test exception", jsonData["responseError"]["Value"].ToString());
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
        public async Task DeleteDatasetClass_ReturnsJsonError_WhenUserIdIsNull()
        {
            // Arrange
            var datasetClassId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { }))
                }
            };

            // Act
            var result = await _controller.DeleteDatasetClass(datasetClassId, datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("User id is not valid", jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task DeleteDatasetClass_ThrowsException_WhenDatasetNotFound()
        {
            // Arrange
            string userId = "test-user-id";
            var datasetId = Guid.NewGuid();
            var datasetClassId = Guid.NewGuid();

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            _mockDatasetService.Setup(s => s.GetDatasetById(datasetId)).ReturnsAsync((DatasetDTO)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _controller.DeleteDatasetClass(datasetClassId, datasetId));
        }

        [Fact]
        public async Task DeleteDatasetClass_ReturnsJsonError_WhenDatasetIdIsInvalid()
        {
            // Arrange
            var datasetClassId = Guid.NewGuid();
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
            var result = await _controller.DeleteDatasetClass(datasetClassId, datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Invalid dataset id", jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task DeleteDatasetClass_ReturnsJsonError_WhenDatasetClassIdIsInvalid()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var userId = "test-user-id";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };
            var datasetClassId = Guid.Empty;

            // Act
            var result = await _controller.DeleteDatasetClass(datasetClassId, datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Invalid dataset class id", jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task DeleteDatasetClass_ReturnsJsonError_WhenDatasetIsPublished()
        {
            // Arrange
            var datasetClassId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();
            var datasetDto = new DatasetDTO { IsPublished = true };
            var userId = "test-user-id";
            _mockDatasetService.Setup(service => service.GetDatasetById(datasetId)).ReturnsAsync(datasetDto);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            // Act
            var result = await _controller.DeleteDatasetClass(datasetClassId, datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Dataset is already published. No changes allowed", jsonData["responseErrorAlreadyPublished"]["Value"].ToString());
        }

        [Fact]
        public async Task DeleteDatasetClass_SuccessfullyDeletesClass()
        {
            // Arrange
            var datasetClassId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();
            var datasetDto = new DatasetDTO { IsPublished = false };
            var userId = "test-user-id";
            var resultDto = new ResultDTO<int>(true, 1, null, null);
            _mockDatasetService.Setup(service => service.GetDatasetById(datasetId)).ReturnsAsync(datasetDto);
            _mockDatasetService.Setup(service => service.DeleteDatasetClassForDataset(datasetClassId, datasetId, userId)).ReturnsAsync(resultDto);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            // Act
            var result = await _controller.DeleteDatasetClass(datasetClassId, datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Successfully deleted dataset class", jsonData["responseSuccess"]["Value"].ToString());
        }

        [Fact]
        public async Task DeleteDatasetClass_ReturnsJsonError_WhenErrorOccursWhileDeletingClass()
        {
            // Arrange
            var datasetClassId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();
            var datasetDto = new DatasetDTO { IsPublished = false };
            var userId = "test-user-id";
            var errorMessage = "An error occurred";
            var resultDto = new ResultDTO<int>(false, 0, errorMessage, null);
            _mockDatasetService.Setup(service => service.GetDatasetById(datasetId)).ReturnsAsync(datasetDto);
            _mockDatasetService.Setup(service => service.DeleteDatasetClassForDataset(datasetClassId, datasetId, userId)).ReturnsAsync(resultDto);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            // Act
            var result = await _controller.DeleteDatasetClass(datasetClassId, datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal(errorMessage, jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task DeleteDatasetClass_ReturnsJsonError_WhenClassNotDeleted()
        {
            // Arrange
            var datasetClassId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();
            var datasetDto = new DatasetDTO { IsPublished = false };
            var userId = "test-user-id";
            var resultDto = new ResultDTO<int>(false, 0, null, null);
            _mockDatasetService.Setup(service => service.GetDatasetById(datasetId)).ReturnsAsync(datasetDto);
            _mockDatasetService.Setup(service => service.DeleteDatasetClassForDataset(datasetClassId, datasetId, userId)).ReturnsAsync(resultDto);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            // Act
            var result = await _controller.DeleteDatasetClass(datasetClassId, datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Some error occured", jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task ChooseDatasetClassType_ReturnsJsonError_WhenUserIdIsNull()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var annotationsPerSubclass = true;
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { }))
                }
            };

            // Act
            var result = await _controller.ChooseDatasetClassType(datasetId, annotationsPerSubclass);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("User id is not valid", jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task ChooseDatasetClassType_ReturnsJsonError_WhenDatasetIdIsInvalid()
        {
            // Arrange
            var annotationsPerSubclass = true;
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
            var result = await _controller.ChooseDatasetClassType(datasetId, annotationsPerSubclass);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Invalid dataset id", jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task ChooseDatasetClassType_SuccessfullySetsAnnotationsPerSubclass()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var annotationsPerSubclass = true;
            var userId = "test-user-id";
            var resultDto = new ResultDTO<int>(true, 1, null, null);
            _mockDatasetService.Setup(service => service.SetAnnotationsPerSubclass(datasetId, annotationsPerSubclass, userId)).ReturnsAsync(resultDto);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            // Act
            var result = await _controller.ChooseDatasetClassType(datasetId, annotationsPerSubclass);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Now you can add classes for this dataset", jsonData["responseSuccess"]["Value"].ToString());
        }

        [Fact]
        public async Task ChooseDatasetClassType_ReturnsJsonError_WhenErrorOccursWhileSettingAnnotations()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var annotationsPerSubclass = true;
            var userId = "test-user-id";
            var errorMessage = "An error occurred";
            var resultDto = new ResultDTO<int>(false, 0, errorMessage, null);
            _mockDatasetService.Setup(service => service.SetAnnotationsPerSubclass(datasetId, annotationsPerSubclass, userId)).ReturnsAsync(resultDto);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            // Act
            var result = await _controller.ChooseDatasetClassType(datasetId, annotationsPerSubclass);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal(errorMessage, jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task ChooseDatasetClassType_ReturnsJsonError_WhenOptionNotSaved()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var annotationsPerSubclass = true;
            var userId = "test-user-id";
            var resultDto = new ResultDTO<int>(false, 0, null, null);
            _mockDatasetService.Setup(service => service.SetAnnotationsPerSubclass(datasetId, annotationsPerSubclass, userId)).ReturnsAsync(resultDto);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            // Act
            var result = await _controller.ChooseDatasetClassType(datasetId, annotationsPerSubclass);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Choosed option was not saved", jsonData["responseError"]["Value"].ToString());
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
        public async Task GetAllDatasets_ThrowsException_WhenDatasetServiceReturnsNull()
        {
            // Arrange
            _mockDatasetService.Setup(s => s.GetAllDatasets()).ReturnsAsync((List<DatasetDTO>)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _controller.GetAllDatasets());
        }


        [Fact]
        public async Task Index_ThrowsException_WhenModelMappingFails()
        {
            // Arrange
            _mockDatasetService.Setup(s => s.GetAllDatasets()).ReturnsAsync(new List<DatasetDTO>());
            _mockMapper.Setup(m => m.Map<List<DatasetViewModel>>(It.IsAny<List<DatasetDTO>>())).Returns((List<DatasetViewModel>)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _controller.Index());
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
        public async Task CreateConfirmed_ThrowsException_WhenDatasetCreationFails()
        {
            // Arrange
            var datasetViewModel = new CreateDatasetViewModel { ParentDatasetId = null };
            string userId = "test-user-id";

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            _mockDatasetService.Setup(s => s.GetDatasetById(It.IsAny<Guid>())).ReturnsAsync(new DatasetDTO { IsPublished = true });
            _mockMapper.Setup(m => m.Map<DatasetDTO>(It.IsAny<CreateDatasetViewModel>())).Returns(new DatasetDTO());
            _mockDatasetService.Setup(s => s.CreateDataset(It.IsAny<DatasetDTO>())).ReturnsAsync((DatasetDTO)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _controller.CreateConfirmed(datasetViewModel));
        }


        // Check why model state error is not initiated
        //[Fact]
        //public async Task CreateConfirmed_ReturnsJsonError_WhenModelStateIsInvalid()
        //{
        //    // Arrange
        //    var datasetViewModel = new CreateDatasetViewModel();

        //    _controller.ModelState.AddModelError("Name", "Required");

        //    var userId = "test-user-id";
        //    var claims = new List<Claim> { new Claim("UserId", userId) };
        //    var identity = new ClaimsIdentity(claims, "TestAuthType");
        //    var claimsPrincipal = new ClaimsPrincipal(identity);

        //    _controller.ControllerContext = new ControllerContext
        //    {
        //        HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        //    };

        //    // Act
        //    var result = await _controller.CreateConfirmed(datasetViewModel) as JsonResult;

        //    // Assert
        //    Assert.NotNull(result);
        //    var jsonData = JObject.FromObject(result.Value);
        //    Assert.Equal("Dataset model is not valid", jsonData["responseError"]["Value"].ToString());
        //}

        [Fact]
        public async Task CreateConfirmed_ReturnsJsonError_WhenSelectedParentDatasetIsNotPublished()
        {
            // Arrange
            var userId = "test-user-id";
            var parentDatasetId = Guid.NewGuid();
            var claims = new List<Claim> { new Claim("UserId", userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var datasetViewModel = new CreateDatasetViewModel { ParentDatasetId = parentDatasetId };
            _mockDatasetService.Setup(s => s.GetDatasetById(parentDatasetId))
                .ReturnsAsync(new DatasetDTO { IsPublished = false });

            // Act
            var result = await _controller.CreateConfirmed(datasetViewModel) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var jsonData = JObject.FromObject(result.Value);
            Assert.Equal("Selected parent dataset is not published. You can not add subdataset for unpublished datasets!", jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task CreateConfirmed_ThrowsException_WhenDatasetMappingFails()
        {
            // Arrange
            var userId = "test-user-id";
            var claims = new List<Claim> { new Claim("UserId", userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var datasetViewModel = new CreateDatasetViewModel();
            _mockMapper.Setup(m => m.Map<DatasetDTO>(datasetViewModel)).Returns((DatasetDTO)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.CreateConfirmed(datasetViewModel));
        }

        [Fact]
        public async Task CreateConfirmed_CreatesDatasetAndReturnsJsonId_WhenValidInput()
        {
            // Arrange
            var userId = "test-user-id";
            var parentDatasetId = Guid.NewGuid();
            var claims = new List<Claim> { new Claim("UserId", userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var datasetViewModel = new CreateDatasetViewModel { ParentDatasetId = parentDatasetId };
            var datasetDTO = new DatasetDTO { Id = Guid.NewGuid() };
            var parentDatasetDTO = new DatasetDTO { Id = parentDatasetId, IsPublished = true };

            _mockMapper.Setup(m => m.Map<DatasetDTO>(datasetViewModel)).Returns(datasetDTO);
            _mockDatasetService.Setup(s => s.GetDatasetById(parentDatasetId)).ReturnsAsync(parentDatasetDTO);
            _mockDatasetService.Setup(s => s.CreateDataset(datasetDTO)).ReturnsAsync(datasetDTO);

            // Act
            var result = await _controller.CreateConfirmed(datasetViewModel) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var jsonData = JObject.FromObject(result.Value);
            Assert.Equal(datasetDTO.Id.ToString(), jsonData["id"].ToString());
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

        [Fact]
        public async Task ImportDataset_ReturnsJsonError_WhenDatasetFileIsNull()
        {
            // Arrange
            IFormFile datasetFile = null;
            var datasetName = "Test Dataset";
            var allowUnannotatedImages = false;
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", "test-user-id") }))
                }
            };

            // Act
            var result = await _controller.ImportDataset(datasetFile, datasetName, allowUnannotatedImages);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Invalid dataset file", jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task ImportDataset_ReturnsJsonError_WhenUserIdIsNull()
        {
            // Arrange
            var datasetFile = new FormFile(new MemoryStream(new byte[1]), 0, 1, "file", "test.zip");
            var datasetName = "Test Dataset";
            var allowUnannotatedImages = false;
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { }))
                }
            };

            // Act
            var result = await _controller.ImportDataset(datasetFile, datasetName, allowUnannotatedImages);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("User id is not valid", jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task ImportDataset_ReturnsJsonError_WhenErrorOccursWhileProcessingFile()
        {
            // Arrange
            var datasetFile = new FormFile(new MemoryStream(new byte[1]), 0, 1, "file", "test.zip");
            var datasetName = "Test Dataset";
            var allowUnannotatedImages = false;
            var userId = "test-user-id";

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            _mockWebHostEnvironment.Setup(wh => wh.WebRootPath).Returns(Path.GetTempPath());

            // Simulate an error when extracting the zip file
            _mockDatasetService.Setup(service => service.ImportDatasetCocoFormatedAtDirectoryPath(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ThrowsAsync(new Exception("Central Directory corrupt."));

            // Act
            var result = await _controller.ImportDataset(datasetFile, datasetName, allowUnannotatedImages);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Central Directory corrupt.", jsonData["responseError"]["Value"].ToString());
        }

        // Check process zip file method

        //[Fact]
        //public async Task ImportDataset_ReturnsJsonError_WhenImportingDatasetFails()
        //{
        //    // Arrange
        //    var datasetFile = new FormFile(new MemoryStream(new byte[1]), 0, 1, "file", "test.zip");
        //    var datasetName = "Test Dataset";
        //    var allowUnannotatedImages = false;
        //    var userId = "test-user-id";

        //    _controller.ControllerContext = new ControllerContext
        //    {
        //        HttpContext = new DefaultHttpContext
        //        {
        //            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
        //        }
        //    };

        //    var resultDto = new ResultDTO<DatasetDTO>(false, null, "Error importing dataset", null);
        //    _mockDatasetService.Setup(service => service.ImportDatasetCocoFormatedAtDirectoryPath(datasetName, It.IsAny<string>(), userId, It.IsAny<string>(), allowUnannotatedImages))
        //        .ReturnsAsync(resultDto);

        //    // Act
        //    var result = await _controller.ImportDataset(datasetFile, datasetName, allowUnannotatedImages);

        //    // Assert
        //    var jsonResult = Assert.IsType<JsonResult>(result);
        //    var jsonData = JObject.FromObject(jsonResult.Value);
        //    Assert.Equal("Error importing dataset file", jsonData["responseError"]["Value"].ToString());
        //}


        //[Fact]
        //public async Task ImportDataset_SuccessfullyImportsDataset()
        //{
        //    // Arrange
        //    var datasetFile = new FormFile(new MemoryStream(new byte[1]), 0, 1, "file", "test.zip");
        //    var datasetName = "Test Dataset";
        //    var allowUnannotatedImages = false;
        //    var userId = "test-user-id";
        //    var tempFilePath = Path.Combine(Path.GetTempPath(), datasetFile.FileName);
        //    var datasetDto = new DatasetDTO { Id = Guid.NewGuid() };

        //    _controller.ControllerContext = new ControllerContext
        //    {
        //        HttpContext = new DefaultHttpContext
        //        {
        //            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
        //        }
        //    };

        //    var resultDto = new ResultDTO<DatasetDTO>(true, datasetDto, null, null);
        //    _mockDatasetService.Setup(service => service.ImportDatasetCocoFormatedAtDirectoryPath(datasetName, It.IsAny<string>(), userId, It.IsAny<string>(), allowUnannotatedImages))
        //        .ReturnsAsync(resultDto);
        //    _mockDatasetService.Setup(service => service.GenerateThumbnailsForDatasetWithErrors(datasetDto.Id))
        //        .ReturnsAsync(new ResultDTO<string>(true, "Thumbnails generated", null, null));

        //    // Act
        //    var result = await _controller.ImportDataset(datasetFile, datasetName, allowUnannotatedImages);

        //    // Assert
        //    var jsonResult = Assert.IsType<JsonResult>(result);
        //    var jsonData = JObject.FromObject(jsonResult.Value);
        //    Assert.Equal("Dataset imported and Thumbnails generated", jsonData["responseSuccess"]["Value"].ToString());
        //    Assert.Equal(datasetDto, jsonData["dataset"].ToObject<DatasetDTO>());
        //}


        [Fact]
        public async Task GetAllPublishedDatasets_ReturnsOkResult_WhenDatasetsAreFound()
        {
            // Arrange
            var datasets = new List<DatasetDTO>
        {
            new DatasetDTO { },
            new DatasetDTO { }
        };

            var result = ResultDTO<List<DatasetDTO>>.Ok(datasets);
            _mockDatasetService.Setup(s => s.GetAllPublishedDatasets()).ReturnsAsync(result);

            // Act
            var response = await _controller.GetAllPublishedDatasets();

            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsSuccess);
            Assert.Equal(datasets, response.Data);
        }

        [Fact]
        public async Task GetAllPublishedDatasets_ReturnsFailResult_WhenNoDatasetsFound()
        {
            // Arrange
            var result = ResultDTO<List<DatasetDTO>>.Ok(null);
            _mockDatasetService.Setup(s => s.GetAllPublishedDatasets()).ReturnsAsync(result);

            // Act
            var response = await _controller.GetAllPublishedDatasets();

            // Assert
            Assert.NotNull(response);
            Assert.False(response.IsSuccess);
            Assert.Equal("Published datasets not found", response.ErrMsg);
        }

        [Fact]
        public async Task GetAllPublishedDatasets_ReturnsFailResult_WhenServiceFails()
        {
            // Arrange
            var result = ResultDTO<List<DatasetDTO>>.Fail("Service failure");
            _mockDatasetService.Setup(s => s.GetAllPublishedDatasets()).ReturnsAsync(result);

            // Act
            var response = await _controller.GetAllPublishedDatasets();

            // Assert
            Assert.NotNull(response);
            Assert.False(response.IsSuccess);
            Assert.Equal("Service failure", response.ErrMsg);
        }

        [Fact]
        public async Task GetAllPublishedDatasets_ReturnsFailResult_WhenDataIsNull()
        {
            // Arrange
            var result = ResultDTO<List<DatasetDTO>>.Ok(null);
            _mockDatasetService.Setup(s => s.GetAllPublishedDatasets()).ReturnsAsync(result);

            // Act
            var response = await _controller.GetAllPublishedDatasets();

            // Assert
            Assert.NotNull(response);
            Assert.False(response.IsSuccess);
            Assert.Equal("Published datasets not found", response.ErrMsg);
        }


        [Fact]
        public async Task GenerateThumbnailsForDataset_ReturnsJsonError_WhenUserIdIsInvalid()
        {
            // Arrange
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity() // no claims
                    )
                }
            };

            // Act
            var result = await _controller.GenerateThumbnailsForDataset(Guid.NewGuid());

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("User id is not valid", jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task GenerateThumbnailsForDataset_ReturnsJsonError_WhenDatasetIdIsInvalid()
        {
            // Arrange
            string userId = "test-user-id";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            // Act
            var result = await _controller.GenerateThumbnailsForDataset(Guid.Empty);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Invalid dataset id", jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task GenerateThumbnailsForDataset_ReturnsJsonError_WhenDatasetNotFound()
        {
            // Arrange
            string userId = "test-user-id";
            var datasetId = Guid.NewGuid();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            _mockDatasetService.Setup(s => s.GetDatasetById(datasetId)).ReturnsAsync((DatasetDTO)null);

            // Act
            var result = await _controller.GenerateThumbnailsForDataset(datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Dataset not found", jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task GenerateThumbnailsForDataset_ReturnsJsonError_WhenDatasetIsPublished()
        {
            // Arrange
            string userId = "test-user-id";
            var datasetId = Guid.NewGuid();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            var datasetDb = new DatasetDTO { IsPublished = true };
            _mockDatasetService.Setup(s => s.GetDatasetById(datasetId)).ReturnsAsync(datasetDb);

            // Act
            var result = await _controller.GenerateThumbnailsForDataset(datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Dataset is already published. No changes allowed", jsonData["responseErrorAlreadyPublished"]["Value"].ToString());
        }

        [Fact]
        public async Task GenerateThumbnailsForDataset_ReturnsJsonError_WhenThumbnailFolderCannotBeRetrieved()
        {
            // Arrange
            string userId = "test-user-id";
            var datasetId = Guid.NewGuid();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            var datasetDb = new DatasetDTO { IsPublished = false };
            _mockDatasetService.Setup(s => s.GetDatasetById(datasetId)).ReturnsAsync(datasetDb);
            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>("DatasetThumbnailsFolder", "DatasetThumbnails"))
                .ReturnsAsync(new ResultDTO<string>(false, null, "Error retrieving folder", null));

            // Act
            var result = await _controller.GenerateThumbnailsForDataset(datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Could not retrieve thumbnail folder", jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task GenerateThumbnailsForDataset_ReturnsJsonSuccess_WhenThumbnailsGeneratedSuccessfully()
        {
            // Arrange
            string userId = "test-user-id";
            var datasetId = Guid.NewGuid();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            var datasetDb = new DatasetDTO { IsPublished = false };
            _mockDatasetService.Setup(s => s.GetDatasetById(datasetId)).ReturnsAsync(datasetDb);
            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>("DatasetThumbnailsFolder", "DatasetThumbnails"))
                .ReturnsAsync(new ResultDTO<string>(true, "Thumbnails", null, null));
            _mockDatasetImagesService.Setup(s => s.GetImagesForDataset(datasetId))
                .ReturnsAsync(new List<DatasetImageDTO>
                {
                new DatasetImageDTO { Id = Guid.NewGuid(), ImagePath = "/images/", FileName = "image.jpg" }
                });
            _mockWebHostEnvironment.Setup(env => env.WebRootPath).Returns(Path.Combine("..", "..", "wwwroot"));

            // Act
            var result = await _controller.GenerateThumbnailsForDataset(datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Thumbnails generated successfully", jsonData["responseSuccess"]["Value"].ToString());
        }

        [Fact]
        public async Task GenerateThumbnailsForDataset_ReturnsJsonError_WhenExceptionOccurs()
        {
            // Arrange
            string userId = "test-user-id";
            var datasetId = Guid.NewGuid();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            var datasetDb = new DatasetDTO { IsPublished = false };
            _mockDatasetService.Setup(s => s.GetDatasetById(datasetId)).ReturnsAsync(datasetDb);
            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>("DatasetThumbnailsFolder", "DatasetThumbnails"))
                .ReturnsAsync(new ResultDTO<string>(true, "Thumbnails", null, null));

            _mockWebHostEnvironment.Setup(env => env.WebRootPath).Returns(Path.Combine("..", "..", "wwwroot"));

            _mockDatasetImagesService.Setup(s => s.GetImagesForDataset(datasetId))
                .ThrowsAsync(new Exception("An error occurred while getting images"));

            // Act
            var result = await _controller.GenerateThumbnailsForDataset(datasetId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("An error occurred while getting images", jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task GenerateThumbnailsForDatasetWithErrors_ReturnsFail_WhenUserIdIsInvalid()
        {
            // Arrange
            var datasetId = Guid.NewGuid(); // Example dataset ID
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity()) // No UserId claim
                }
            };

            // Act
            var result = await _controller.GenerateThumbnailsForDatasetWithErrors(datasetId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User id is not valid", result.ErrMsg);
        }

        [Fact]
        public async Task GenerateThumbnailsForDatasetWithErrors_ReturnsFail_WhenDatasetIdIsEmpty()
        {
            // Arrange
            string userId = "test-user-id";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            // Act
            var result = await _controller.GenerateThumbnailsForDatasetWithErrors(Guid.Empty);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid dataset id", result.ErrMsg);
        }

        [Fact]
        public async Task GenerateThumbnailsForDatasetWithErrors_ReturnsFail_WhenDatasetIsPublished()
        {
            // Arrange
            string userId = "test-user-id";
            var datasetId = Guid.NewGuid();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            var datasetDb = new DatasetDTO { IsPublished = true };
            _mockDatasetService.Setup(s => s.GetDatasetDTOFullyIncluded(datasetId, false))
                               .ReturnsAsync(ResultDTO<DatasetDTO>.Ok(datasetDb));

            // Act
            var result = await _controller.GenerateThumbnailsForDatasetWithErrors(datasetId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Dataset is already published. No changes allowed", result.ErrMsg);
        }

        [Fact]
        public async Task GenerateThumbnailsForDatasetWithErrors_ReturnsFail_WhenExceptionOccurs()
        {
            // Arrange
            string userId = "test-user-id";
            var datasetId = Guid.NewGuid();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            _mockDatasetService.Setup(s => s.GetDatasetDTOFullyIncluded(datasetId, false)).ThrowsAsync(new Exception("An error occurred"));

            // Act
            var result = await _controller.GenerateThumbnailsForDatasetWithErrors(datasetId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("An error occurred", result.ErrMsg);
        }

        [Fact]
        public async Task GenerateThumbnailsForDatasetWithErrors_ReturnsFail_WhenThumbnailFolderCannotBeRetrieved()
        {
            // Arrange
            string userId = "test-user-id";
            var datasetId = Guid.NewGuid();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            var datasetDb = new DatasetDTO { IsPublished = false };
            _mockDatasetService.Setup(s => s.GetDatasetDTOFullyIncluded(datasetId, false))
                               .ReturnsAsync(ResultDTO<DatasetDTO>.Ok(datasetDb));

            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>("DatasetThumbnailsFolder", "DatasetThumbnails"))
                                    .ReturnsAsync(new ResultDTO<string>(false, null, "Error retrieving folder", null));

            // Act
            var result = await _controller.GenerateThumbnailsForDatasetWithErrors(datasetId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Could not retrieve thumbnail folder", result.ErrMsg);
        }

        [Fact]
        public async Task GenerateThumbnailsForDatasetWithErrors_AppendsError_WhenImageDoesNotExist()
        {
            // Arrange
            string userId = "test-user-id";
            var datasetId = Guid.NewGuid();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            var datasetDb = new DatasetDTO { IsPublished = false };
            _mockDatasetService.Setup(s => s.GetDatasetDTOFullyIncluded(datasetId, false))
                               .ReturnsAsync(ResultDTO<DatasetDTO>.Ok(datasetDb));

            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>("DatasetThumbnailsFolder", "DatasetThumbnails"))
                                    .ReturnsAsync(new ResultDTO<string>(true, "Thumbnails", null, null));

            var imageId = Guid.NewGuid();
            _mockDatasetImagesService.Setup(s => s.GetImagesForDataset(datasetId))
                                     .ReturnsAsync(new List<DatasetImageDTO>
                                     {
                                 new DatasetImageDTO { Id = imageId, ImagePath = "images", FileName = "missing-image.jpg" }
                                     });

            _mockWebHostEnvironment.Setup(env => env.WebRootPath).Returns(Path.Combine("..", "..", "wwwroot"));

            // Act
            // Act
            var result = await _controller.GenerateThumbnailsForDatasetWithErrors(datasetId);
            var resultMessage = result.Data.ToString();

            // Assert
            Assert.True(result.IsSuccess);

        }

    }
}
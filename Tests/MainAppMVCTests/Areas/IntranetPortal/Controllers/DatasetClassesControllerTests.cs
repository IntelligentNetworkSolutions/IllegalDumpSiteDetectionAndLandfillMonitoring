using AutoMapper;
using DTOs.MainApp.BL.DatasetDTOs;
using MainApp.BL.Interfaces.Services.DatasetServices;
using MainApp.MVC.Areas.IntranetPortal.Controllers;
using MainApp.MVC.ViewModels.IntranetPortal.Dataset;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json.Linq;
using SD;
using System.Security.Claims;

namespace Tests.MainAppMVCTests.Areas.IntranetPortal.Controllers
{
    public class DatasetClassesControllerTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IDatasetClassesService> _mockDatasetClassesService;
        private readonly Mock<IDataset_DatasetClassService> _mockDatasetDatasetClassService;
        private readonly Mock<IDatasetService> _mockDatasetService;
        private readonly DatasetClassesController _controller;

        public DatasetClassesControllerTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockMapper = new Mock<IMapper>();
            _mockDatasetClassesService = new Mock<IDatasetClassesService>();
            _mockDatasetDatasetClassService = new Mock<IDataset_DatasetClassService>();
            _mockDatasetService = new Mock<IDatasetService>();

            _controller = new DatasetClassesController(
                _mockConfiguration.Object,
                _mockMapper.Object,
                _mockDatasetClassesService.Object,
                _mockDatasetDatasetClassService.Object,
                _mockDatasetService.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewWithCorrectModel()
        {
            // Arrange
            var datasetClassDtoList = new List<DatasetClassDTO>
            {
                new DatasetClassDTO { Id = Guid.NewGuid(), ClassName = "Class 1" },
                new DatasetClassDTO { Id = Guid.NewGuid(), ClassName = "Class 2" }
            };
            _mockDatasetClassesService.Setup(service => service.GetAllDatasetClasses())
                                      .ReturnsAsync(ResultDTO<List<DatasetClassDTO>>.Ok(datasetClassDtoList));
            var datasetClassViewModelList = new List<DatasetClassViewModel>
            {
                new DatasetClassViewModel { Id = datasetClassDtoList[0].Id, ClassName = "Class 1" },
                new DatasetClassViewModel { Id = datasetClassDtoList[1].Id, ClassName = "Class 2" }
            };
            _mockMapper.Setup(mapper => mapper.Map<List<DatasetClassViewModel>>(datasetClassDtoList))
                       .Returns(datasetClassViewModelList);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<DatasetClassViewModel>>(viewResult.Model);
            Assert.Equal(2, model.Count);
            Assert.Equal("Class 1", model[0].ClassName);
            Assert.Equal("Class 2", model[1].ClassName);
        }

        [Fact]
        public async Task CreateClass_ModelStateIsInvalid_ReturnsErrorJson()
        {
            // Arrange
            SetupUser("validUserId");
            _controller.ModelState.AddModelError("error", "some error");

            var model = new CreateDatasetClassDTO();

            // Act
            var result = await _controller.CreateClass(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Model is not valid", jsonData["responseError"]["Value"].ToString());
        }


        [Fact]
        public async Task CreateClass_UserIdIsNull_ReturnsErrorJson()
        {
            // Arrange
            SetupUser(null);

            var model = new CreateDatasetClassDTO();

            // Act
            var result = await _controller.CreateClass(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("User ID is not valid", jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task CreateClass_AddDatasetClassSuccess_ReturnsSuccessJson()
        {
            // Arrange
            SetupUser("validUserId");

            var model = new CreateDatasetClassDTO();
            var resultDto = new ResultDTO<int>(IsSuccess: true, 1, null, null);

            _mockDatasetClassesService.Setup(service => service.AddDatasetClass(It.IsAny<CreateDatasetClassDTO>()))
                                      .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.CreateClass(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Successfully added dataset class", jsonData["responseSuccess"]["Value"].ToString());
        }

        [Fact]
        public async Task CreateClass_AddDatasetClassFailureWithErrMsg_ReturnsErrorJson()
        {
            // Arrange
            SetupUser("validUserId");

            var model = new CreateDatasetClassDTO();
            var resultDto = new ResultDTO<int>(IsSuccess: false, 3, "Some error occurred", null);

            _mockDatasetClassesService.Setup(service => service.AddDatasetClass(It.IsAny<CreateDatasetClassDTO>()))
                                      .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.CreateClass(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("An error occurred. Dataset class was not added.", jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task CreateClass_AddDatasetClassFailureWithoutErrMsg_ReturnsGenericErrorJson()
        {
            // Arrange
            SetupUser("validUserId");

            var model = new CreateDatasetClassDTO();
            var resultDto = new ResultDTO<int>(IsSuccess: false, 0, null, null);

            _mockDatasetClassesService.Setup(service => service.AddDatasetClass(It.IsAny<CreateDatasetClassDTO>()))
                                      .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.CreateClass(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("An error occurred. Dataset class was not added.", jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task GetClassById_ClassIdIsEmpty_ReturnsNewDatasetClassDTO()
        {
            // Arrange
            var classId = Guid.Empty;

            // Act
            var result = await _controller.GetClassById(classId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<DatasetClassDTO>(result);
            Assert.Equal(Guid.Empty, result.Id);
        }

        [Fact]
        public async Task GetClassById_ClassIdIsNotEmpty_ReturnsDatasetClassDTO()
        {
            // Arrange
            var classId = Guid.NewGuid();
            var datasetClassDto = new DatasetClassDTO { Id = classId };

            _mockDatasetClassesService.Setup(service => service.GetDatasetClassById(classId))
                                      .ReturnsAsync(ResultDTO<DatasetClassDTO>.Ok(datasetClassDto));

            // Act
            var result = await _controller.GetClassById(classId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<DatasetClassDTO>(result);
            Assert.Equal(classId, result.Id);
        }

        [Fact]
        public async Task GetClassById_ClassIdIsNotEmpty_ServiceReturnsNull_ReturnsEmptyDatasetClassDTO()
        {
            // Arrange
            var classId = Guid.NewGuid();

            _mockDatasetClassesService.Setup(service => service.GetDatasetClassById(classId))
                .ReturnsAsync(ResultDTO<DatasetClassDTO>.Fail("Class not found"));

            // Act
            var result = await _controller.GetClassById(classId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<DatasetClassDTO>(result);
            Assert.Equal(Guid.Empty, result.Id);
        }


        [Fact]
        public async Task EditClass_ModelStateIsInvalid_ReturnsErrorJson()
        {
            // Arrange
            _controller.ModelState.AddModelError("error", "some error");

            var model = new EditDatasetClassDTO();

            // Act
            var result = await _controller.EditClass(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Model is not valid", jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task EditClass_UpdateSuccess_ReturnsSuccessJson()
        {
            // Arrange
            var model = new EditDatasetClassDTO { Id = Guid.NewGuid() };
            var datasets = new List<DatasetDTO>();
            var datasetDatasetClassDto = new List<Dataset_DatasetClassDTO>();
            var allClasses = new List<DatasetClassDTO> { new DatasetClassDTO { ParentClassId = model.Id } };

            _mockDatasetClassesService.Setup(service => service.EditDatasetClass(model))
                                      .ReturnsAsync(new ResultDTO<int>(true, 1, null, null));

            _mockDatasetClassesService.Setup(service => service.GetAllDatasetClasses())
                .ReturnsAsync(ResultDTO<List<DatasetClassDTO>>.Ok(allClasses));
            _mockDatasetDatasetClassService.Setup(service => service.GetDataset_DatasetClassByClassId(model.Id))
                   .ReturnsAsync(ResultDTO<List<Dataset_DatasetClassDTO>>.Ok(datasetDatasetClassDto));

            _mockDatasetService.Setup(service => service.GetAllDatasets())
                               .ReturnsAsync(ResultDTO<List<DatasetDTO>>.Ok(datasets));

            // Act
            var result = await _controller.EditClass(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Successfully updated dataset class", jsonData["responseSuccess"]["Value"].ToString());
        }

        [Fact]
        public async Task EditClass_UpdateFailsWithData2_ReturnsErrorJsonWithChildrenClasses()
        {
            // Arrange
            var model = new EditDatasetClassDTO { Id = Guid.NewGuid() };
            var errorMessage = "Error specific to Data = 2";
            var datasets = new List<DatasetDTO>();
            var datasetDatasetClassDto = new List<Dataset_DatasetClassDTO>();

            _mockDatasetClassesService.Setup(service => service.EditDatasetClass(model))
                .ReturnsAsync(new ResultDTO<int>(false, 2, errorMessage, null));

            var allClasses = new List<DatasetClassDTO> { new DatasetClassDTO { ParentClassId = model.Id } };
            _mockDatasetClassesService.Setup(service => service.GetAllDatasetClasses())
                .ReturnsAsync(ResultDTO<List<DatasetClassDTO>>.Ok(allClasses));
            _mockDatasetDatasetClassService.Setup(service => service.GetDataset_DatasetClassByClassId(model.Id))
                   .ReturnsAsync(ResultDTO<List<Dataset_DatasetClassDTO>>.Ok(datasetDatasetClassDto));

            _mockDatasetService.Setup(service => service.GetAllDatasets())
                               .ReturnsAsync(ResultDTO<List<DatasetDTO>>.Ok(datasets));

            // Act
            var result = await _controller.EditClass(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal(errorMessage, jsonData["responseError"]["Value"].ToString());
            Assert.NotNull(jsonData["childrenClassesList"]);
        }

        [Fact]
        public async Task EditClass_UpdateFailsWithData4_ReturnsErrorJson()
        {
            // Arrange
            var model = new EditDatasetClassDTO { Id = Guid.NewGuid() };
            var errorMessage = "Error specific to Data = 4";
            var datasets = new List<DatasetDTO>();
            var datasetDatasetClassDto = new List<Dataset_DatasetClassDTO>();

            _mockDatasetClassesService.Setup(service => service.EditDatasetClass(model))
                .ReturnsAsync(new ResultDTO<int>(false, 4, errorMessage, null));
            var allClasses = new List<DatasetClassDTO> { new DatasetClassDTO { ParentClassId = model.Id } };
            _mockDatasetClassesService.Setup(service => service.GetAllDatasetClasses())
                .ReturnsAsync(ResultDTO<List<DatasetClassDTO>>.Ok(allClasses));
            _mockDatasetDatasetClassService.Setup(service => service.GetDataset_DatasetClassByClassId(model.Id))
                   .ReturnsAsync(ResultDTO<List<Dataset_DatasetClassDTO>>.Ok(datasetDatasetClassDto));

            _mockDatasetService.Setup(service => service.GetAllDatasets())
                               .ReturnsAsync(ResultDTO<List<DatasetDTO>>.Ok(datasets));

            // Act
            var result = await _controller.EditClass(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal(errorMessage, jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task EditClass_UpdateFailsWithData5_ReturnsErrorJson()
        {
            // Arrange
            var model = new EditDatasetClassDTO { Id = Guid.NewGuid() };
            var errorMessage = "Error specific to Data = 5";
            var datasets = new List<DatasetDTO>();
            var datasetDatasetClassDto = new List<Dataset_DatasetClassDTO>();

            _mockDatasetClassesService.Setup(service => service.EditDatasetClass(model))
                .ReturnsAsync(new ResultDTO<int>(false, 5, errorMessage, null));
            var allClasses = new List<DatasetClassDTO> { new DatasetClassDTO { ParentClassId = model.Id } };
            _mockDatasetClassesService.Setup(service => service.GetAllDatasetClasses())
                .ReturnsAsync(ResultDTO<List<DatasetClassDTO>>.Ok(allClasses));
            _mockDatasetDatasetClassService.Setup(service => service.GetDataset_DatasetClassByClassId(model.Id))
                   .ReturnsAsync(ResultDTO<List<Dataset_DatasetClassDTO>>.Ok(datasetDatasetClassDto));

            _mockDatasetService.Setup(service => service.GetAllDatasets())
                               .ReturnsAsync(ResultDTO<List<DatasetDTO>>.Ok(datasets));

            // Act
            var result = await _controller.EditClass(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal(errorMessage, jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task EditClass_UpdateFailsWithNoSpecificData_ReturnsGeneralErrorJson()
        {
            // Arrange
            var model = new EditDatasetClassDTO { Id = Guid.NewGuid() };

            _mockDatasetClassesService.Setup(service => service.EditDatasetClass(model))
                .ReturnsAsync(new ResultDTO<int>(false, 0, "General update failure", null));
            var allClasses = new List<DatasetClassDTO> { new DatasetClassDTO { ParentClassId = model.Id } };
            var datasets = new List<DatasetDTO>();
            var datasetDatasetClassDto = new List<Dataset_DatasetClassDTO>();
            _mockDatasetClassesService.Setup(service => service.GetAllDatasetClasses())
                .ReturnsAsync(ResultDTO<List<DatasetClassDTO>>.Ok(allClasses));
            _mockDatasetDatasetClassService.Setup(service => service.GetDataset_DatasetClassByClassId(model.Id))
                   .ReturnsAsync(ResultDTO<List<Dataset_DatasetClassDTO>>.Ok(datasetDatasetClassDto));

            _mockDatasetService.Setup(service => service.GetAllDatasets())
                               .ReturnsAsync(ResultDTO<List<DatasetDTO>>.Ok(datasets));

            // Act
            var result = await _controller.EditClass(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Dataset class was not updated", jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task EditClass_ReturnsError_WhenChildrenClassesListIsEmpty()
        {
            // Arrange
            var model = new EditDatasetClassDTO { Id = Guid.NewGuid() };
            var resultDTO = new ResultDTO<int>(IsSuccess: false, 2, "Dataset class update failed", null);
            var datasets = new List<DatasetDTO>();
            _mockDatasetClassesService.Setup(service => service.GetAllDatasetClasses())
                .ReturnsAsync(ResultDTO<List<DatasetClassDTO>>.Ok(new List<DatasetClassDTO>()));

            _mockDatasetClassesService.Setup(service => service.EditDatasetClass(model))
                .ReturnsAsync(resultDTO);

            _mockDatasetService.Setup(service => service.GetAllDatasets())
                .ReturnsAsync(ResultDTO<List<DatasetDTO>>.Ok(datasets));

            // Act
            var result = await _controller.EditClass(model) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var jsonData = JObject.FromObject(result.Value);
            Assert.NotNull(jsonData["responseError"]);
            Assert.Equal("Dataset class update failed", jsonData["responseError"]["Value"].ToString());
            Assert.True(jsonData.ContainsKey("childrenClassesList"));
            Assert.Empty(jsonData["childrenClassesList"]);

        }

        [Fact]
        public async Task DeleteClass_Success_ReturnsSuccessJson()
        {
            // Arrange
            var classId = Guid.NewGuid();
            var datasetClass = new List<DatasetClassDTO>();
            var datasetDatasetClass = new List<Dataset_DatasetClassDTO>();
            var datasets = new List<DatasetDTO>();

            _mockDatasetClassesService.Setup(service => service.DeleteDatasetClass(classId))
                                      .ReturnsAsync(new ResultDTO<int>(true, 1, null, null));

            _mockDatasetClassesService.Setup(service => service.GetAllDatasetClasses())
                                      .ReturnsAsync(ResultDTO<List<DatasetClassDTO>>.Ok(datasetClass));

            _mockDatasetDatasetClassService.Setup(service => service.GetDataset_DatasetClassByClassId(classId))
                                           .ReturnsAsync(ResultDTO<List<Dataset_DatasetClassDTO>>.Ok(datasetDatasetClass));

            _mockDatasetService.Setup(service => service.GetAllDatasets())
                               .ReturnsAsync(ResultDTO<List<DatasetDTO>>.Ok(datasets));

            // Act
            var result = await _controller.DeleteClass(classId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Successfully deleted dataset class", jsonData["responseSuccess"]["Value"].ToString());
        }

        [Fact]
        public async Task DeleteClass_HasSubclasses_ReturnsErrorJsonWithChildrenClassesList()
        {
            // Arrange
            var classId = Guid.NewGuid();
            var errorMessage = "Subclasses for this class exist. Please delete them first.";
            var childrenClassesList = new List<DatasetClassDTO> { new DatasetClassDTO { Id = Guid.NewGuid(), ParentClassId = classId } };
            var datasetDatasetClass = new List<Dataset_DatasetClassDTO>();
            var datasets = new List<DatasetDTO>();

            _mockDatasetClassesService.Setup(service => service.DeleteDatasetClass(classId))
                                      .ReturnsAsync(new ResultDTO<int>(false, 2, errorMessage, null));

            _mockDatasetClassesService.Setup(service => service.GetAllDatasetClasses())
                                      .ReturnsAsync(ResultDTO<List<DatasetClassDTO>>.Ok(childrenClassesList));

            _mockDatasetDatasetClassService.Setup(service => service.GetDataset_DatasetClassByClassId(classId))
                                           .ReturnsAsync(ResultDTO<List<Dataset_DatasetClassDTO>>.Ok(datasetDatasetClass));

            _mockDatasetService.Setup(service => service.GetAllDatasets())
                               .ReturnsAsync(ResultDTO<List<DatasetDTO>>.Ok(datasets));

            // Act
            var result = await _controller.DeleteClass(classId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal(errorMessage, jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task DeleteClass_UnsuccessfulDeletion_ReturnsErrorJson()
        {
            // Arrange
            var classId = Guid.NewGuid();
            var errorMessage = "An unexpected error occurred";
            var datasetClass = new List<DatasetClassDTO>();
            var datasetDatasetClass = new List<Dataset_DatasetClassDTO>();
            var datasets = new List<DatasetDTO>();
            _mockDatasetClassesService.Setup(service => service.DeleteDatasetClass(classId))
                                      .ReturnsAsync(new ResultDTO<int>(false, 4, errorMessage, null));

            _mockDatasetClassesService.Setup(service => service.GetAllDatasetClasses())
                                      .ReturnsAsync(ResultDTO<List<DatasetClassDTO>>.Ok(datasetClass));

            _mockDatasetDatasetClassService.Setup(service => service.GetDataset_DatasetClassByClassId(classId))
                                           .ReturnsAsync(ResultDTO<List<Dataset_DatasetClassDTO>>.Ok(datasetDatasetClass));

            _mockDatasetService.Setup(service => service.GetAllDatasets())
                               .ReturnsAsync(ResultDTO<List<DatasetDTO>>.Ok(datasets));

            // Act
            var result = await _controller.DeleteClass(classId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal(errorMessage, jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task EditClass_DatasetsUsingClass_ReturnsErrorJsonWithDatasets()
        {
            // Arrange
            var classId = Guid.NewGuid();
            var errorMessage = "Datasets are using this class. Cannot update!";
            var model = new EditDatasetClassDTO { Id = classId };

            _mockDatasetClassesService.Setup(service => service.EditDatasetClass(model))
                                      .ReturnsAsync(new ResultDTO<int>(false, 3, errorMessage, null));

            var allClasses = new List<DatasetClassDTO>();
            _mockDatasetClassesService.Setup(service => service.GetAllDatasetClasses())
                                      .ReturnsAsync(ResultDTO<List<DatasetClassDTO>>.Ok(allClasses));

            var datasetDatasetClasses = new List<Dataset_DatasetClassDTO>();
            _mockDatasetDatasetClassService.Setup(service => service.GetDataset_DatasetClassByClassId(classId))
                                           .ReturnsAsync(ResultDTO<List<Dataset_DatasetClassDTO>>.Ok(datasetDatasetClasses));

            var datasets = new List<DatasetDTO>();
            _mockDatasetService.Setup(service => service.GetAllDatasets())
                               .ReturnsAsync(ResultDTO<List<DatasetDTO>>.Ok(datasets));

            // Act
            var result = await _controller.EditClass(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal(errorMessage, jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task DeleteClass_HasSubclasses_ReturnsErrorJsonWithChildrenClasses()
        {
            // Arrange
            var classId = Guid.NewGuid();
            var errorMessage = "Subclasses for this class exist. Please delete them first.";
            var childrenClassesList = new List<DatasetClassDTO>
            {
                new DatasetClassDTO { Id = Guid.NewGuid(), ParentClassId = classId }
            };

            _mockDatasetClassesService.Setup(service => service.DeleteDatasetClass(classId))
                                      .ReturnsAsync(new ResultDTO<int>(false, 2, errorMessage, null));

            _mockDatasetClassesService.Setup(service => service.GetAllDatasetClasses())
                                      .ReturnsAsync(ResultDTO<List<DatasetClassDTO>>.Ok(childrenClassesList));

            var datasetDatasetClasses = new List<Dataset_DatasetClassDTO>();
            _mockDatasetDatasetClassService.Setup(service => service.GetDataset_DatasetClassByClassId(classId))
                                           .ReturnsAsync(ResultDTO<List<Dataset_DatasetClassDTO>>.Ok(datasetDatasetClasses));

            var datasets = new List<DatasetDTO>();
            _mockDatasetService.Setup(service => service.GetAllDatasets())
                               .ReturnsAsync(ResultDTO<List<DatasetDTO>>.Ok(datasets));

            // Act
            var result = await _controller.DeleteClass(classId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal(errorMessage, jsonData["responseError"]["Value"].ToString());
        }

        [Fact]
        public async Task DeleteClass_ClassInUse_ReturnsErrorJsonWithDatasetsWhereClassIsUsed()
        {
            // Arrange
            var classId = Guid.NewGuid();
            var errorMessage = "Class is currently in use in datasets.";
            var datasetList = new List<DatasetDTO>
    {
        new DatasetDTO { Id = Guid.NewGuid(), Name = "Dataset1" },
        new DatasetDTO { Id = Guid.NewGuid(), Name = "Dataset2" }
    };
            var datasetClassesForClass = new List<Dataset_DatasetClassDTO>
    {
        new Dataset_DatasetClassDTO { DatasetId = datasetList[0].Id },
        new Dataset_DatasetClassDTO { DatasetId = datasetList[1].Id }
    };
            var datasetClassDTO = new List<DatasetClassDTO>();

            _mockDatasetClassesService.Setup(service => service.DeleteDatasetClass(classId))
                                      .ReturnsAsync(new ResultDTO<int>(false, 3, errorMessage, null));

            _mockDatasetClassesService.Setup(service => service.GetAllDatasetClasses())
                                      .ReturnsAsync(ResultDTO<List<DatasetClassDTO>>.Ok(datasetClassDTO));

            _mockDatasetDatasetClassService.Setup(service => service.GetDataset_DatasetClassByClassId(classId))
                                           .ReturnsAsync(ResultDTO<List<Dataset_DatasetClassDTO>>.Ok(datasetClassesForClass));

            _mockDatasetService.Setup(service => service.GetAllDatasets())
                               .ReturnsAsync(ResultDTO<List<DatasetDTO>>.Ok(datasetList));

            // Act
            var result = await _controller.DeleteClass(classId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal(errorMessage, jsonData["responseError"]["Value"].ToString());
            Assert.NotNull(jsonData["datasetsWhereClassIsUsed"]);
        }

        [Fact]
        public async Task DeleteClass_UnsuccessfulDeletion_DefaultErrorResponse()
        {
            // Arrange
            var classId = Guid.NewGuid();
            var datasetClassesDTO = new List<DatasetClassDTO>();
            var datasets = new List<DatasetDTO>();
            var datasetDatasetClassesDTO = new List<Dataset_DatasetClassDTO>();
            _mockDatasetClassesService.Setup(service => service.DeleteDatasetClass(classId))
                                      .ReturnsAsync(new ResultDTO<int>(false, 0, null, null));

            _mockDatasetClassesService.Setup(service => service.GetAllDatasetClasses())
                                      .ReturnsAsync(ResultDTO<List<DatasetClassDTO>>.Ok(datasetClassesDTO));

            _mockDatasetDatasetClassService.Setup(service => service.GetDataset_DatasetClassByClassId(classId))
                                           .ReturnsAsync(ResultDTO<List<Dataset_DatasetClassDTO>>.Ok(datasetDatasetClassesDTO));

            _mockDatasetService.Setup(service => service.GetAllDatasets())
                               .ReturnsAsync(ResultDTO<List<DatasetDTO>>.Ok(datasets));

            // Act
            var result = await _controller.DeleteClass(classId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonData = JObject.FromObject(jsonResult.Value);
            Assert.Equal("Dataset class was not deleted due to another error.", jsonData["responseError"]["Value"].ToString());
        }


        private void SetupUser(string userId)
        {
            var claims = new List<Claim>();
            if (!string.IsNullOrEmpty(userId))
            {
                claims.Add(new Claim("UserId", userId));
            }

            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }
    }
}

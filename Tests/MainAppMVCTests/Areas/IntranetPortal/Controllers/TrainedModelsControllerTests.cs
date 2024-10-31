using AutoMapper;
using DTOs.MainApp.BL.TrainingDTOs;
using MainApp.BL.Interfaces.Services.TrainingServices;
using MainApp.MVC.Areas.IntranetPortal.Controllers;
using MainApp.MVC.ViewModels.IntranetPortal.Training;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using SD;

namespace Tests.MainAppMVCTests.Areas.IntranetPortal.Controllers
{
    public class TrainedModelsControllerTests
    {
        private readonly Mock<ITrainedModelService> _trainedModelServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly TrainedModelsController _controller;

        public TrainedModelsControllerTests()
        {
            _trainedModelServiceMock = new Mock<ITrainedModelService>();
            _mapperMock = new Mock<IMapper>();
            _configurationMock = new Mock<IConfiguration>();

            _controller = new TrainedModelsController(
                _trainedModelServiceMock.Object,
                _mapperMock.Object,
                _configurationMock.Object
            );
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithTrainedModelViewModelList()
        {
            // Arrange
            var trainedModelDtos = new List<TrainedModelDTO>
            {
                new TrainedModelDTO { Id = Guid.NewGuid(), Name = "Model1" },
                new TrainedModelDTO { Id = Guid.NewGuid(), Name = "Model2" }
            };

            var viewModels = new List<TrainedModelViewModel>
            {
                new TrainedModelViewModel { Id = Guid.NewGuid(), Name = "Model1" },
                new TrainedModelViewModel { Id = Guid.NewGuid(), Name = "Model2" }
            };

            var resultDto = ResultDTO<List<TrainedModelDTO>>.Ok(trainedModelDtos);
            _trainedModelServiceMock.Setup(service => service.GetAllTrainedModels())
                .ReturnsAsync(resultDto);

            _mapperMock.Setup(mapper => mapper.Map<List<TrainedModelViewModel>>(trainedModelDtos))
                .Returns(viewModels);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<TrainedModelViewModel>>(viewResult.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public async Task Index_ReturnsRedirectToErrorView_WhenServiceCallFails()
        {
            // Arrange
            var resultDto = ResultDTO<List<TrainedModelDTO>>.Fail("Failed");
            _trainedModelServiceMock.Setup(service => service.GetAllTrainedModels())
                .ReturnsAsync(resultDto);

            _configurationMock.Setup(config => config["ErrorViewsPath:Error"])
                .Returns("/Error");

            // Act
            var result = await _controller.Index();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/Error", redirectResult.Url);
        }

        [Fact]
        public async Task Index_ReturnsRedirectTo404View_WhenDataIsNull()
        {
            // Arrange
            var resultDto = ResultDTO<List<TrainedModelDTO>>.Ok(null);
            _trainedModelServiceMock.Setup(service => service.GetAllTrainedModels())
                .ReturnsAsync(resultDto);

            _configurationMock.Setup(config => config["ErrorViewsPath:Error404"])
                .Returns("/Error404");

            // Act
            var result = await _controller.Index();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/Error404", redirectResult.Url);
        }

        [Fact]
        public async Task Index_ReturnsNotFound_WhenError404PathIsNullAndDataIsNull()
        {
            // Arrange
            var resultDto = ResultDTO<List<TrainedModelDTO>>.Ok(null);
            _trainedModelServiceMock.Setup(service => service.GetAllTrainedModels())
                .ReturnsAsync(resultDto);

            _configurationMock.Setup(config => config["ErrorViewsPath:Error404"])
                .Returns((string)null);

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Index_ReturnsBadRequest_WhenErrorPathIsNullAndServiceFails()
        {
            // Arrange
            var resultDto = ResultDTO<List<TrainedModelDTO>>.Fail("Failed");
            _trainedModelServiceMock.Setup(service => service.GetAllTrainedModels())
                .ReturnsAsync(resultDto);

            _configurationMock.Setup(config => config["ErrorViewsPath:Error"])
                .Returns((string)null);

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithNoTrainedModels()
        {
            // Arrange
            var trainedModelDtos = new List<TrainedModelDTO>();

            var resultDto = ResultDTO<List<TrainedModelDTO>>.Ok(trainedModelDtos);
            _trainedModelServiceMock.Setup(service => service.GetAllTrainedModels())
                .ReturnsAsync(resultDto);

            _mapperMock.Setup(mapper => mapper.Map<List<TrainedModelViewModel>>(trainedModelDtos))
                .Returns(new List<TrainedModelViewModel>());

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<TrainedModelViewModel>>(viewResult.Model);
            Assert.Empty(model);
        }
        [Fact]
        public async Task Index_ReturnsRedirectTo404View_WhenVmListIsNull()
        {
            // Arrange
            var trainedModelDtos = new List<TrainedModelDTO>
    {
        new TrainedModelDTO { Id = Guid.NewGuid(), Name = "Model1" },
        new TrainedModelDTO { Id = Guid.NewGuid(), Name = "Model2" }
    };

            var resultDto = ResultDTO<List<TrainedModelDTO>>.Ok(trainedModelDtos);
            _trainedModelServiceMock.Setup(service => service.GetAllTrainedModels())
                .ReturnsAsync(resultDto);

            _mapperMock.Setup(mapper => mapper.Map<List<TrainedModelViewModel>>(trainedModelDtos))
                .Returns((List<TrainedModelViewModel>)null);

            _configurationMock.Setup(config => config["ErrorViewsPath:Error404"])
                .Returns("/Error404");

            // Act
            var result = await _controller.Index();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/Error404", redirectResult.Url);
        }



        [Fact]
        public async Task GetTrainedModelById_ReturnsSuccessResult_WithTrainedModelData()
        {
            // Arrange
            var trainedModelId = Guid.NewGuid();
            var trainedModelDto = new TrainedModelDTO { Id = trainedModelId, Name = "TestModel" };

            var resultDto = ResultDTO<TrainedModelDTO>.Ok(trainedModelDto);
            _trainedModelServiceMock.Setup(service => service.GetTrainedModelById(trainedModelId, false))
                .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.GetTrainedModelById(trainedModelId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(trainedModelId, result.Data!.Id);
        }

        [Fact]
        public async Task GetTrainedModelById_ReturnsFailResult_WhenServiceCallFails()
        {
            // Arrange
            var trainedModelId = Guid.NewGuid();
            var errorMessage = "Service error";

            var resultDto = ResultDTO<TrainedModelDTO>.Fail(errorMessage);
            _trainedModelServiceMock.Setup(service => service.GetTrainedModelById(trainedModelId, false))
                .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.GetTrainedModelById(trainedModelId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(errorMessage, result.ErrMsg);
        }

        [Fact]
        public async Task GetTrainedModelById_ReturnsFailResult_WhenTrainedModelNotFound()
        {
            // Arrange
            var trainedModelId = Guid.NewGuid();

            var resultDto = ResultDTO<TrainedModelDTO>.Ok(null);
            _trainedModelServiceMock.Setup(service => service.GetTrainedModelById(trainedModelId, false))
                .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.GetTrainedModelById(trainedModelId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Trained model not found", result.ErrMsg);
        }

        [Fact]
        public async Task GetTrainedModelById_ThrowsException_WhenServiceThrowsException()
        {
            // Arrange
            var trainedModelId = Guid.NewGuid();
            _trainedModelServiceMock.Setup(service => service.GetTrainedModelById(trainedModelId, false))
                .ThrowsAsync(new Exception("Service exception"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.GetTrainedModelById(trainedModelId));
        }

        [Fact]
        public async Task GetTrainedModelById_HandlesNullErrorMessage()
        {
            // Arrange
            var trainedModelId = Guid.NewGuid();
            var resultDto = ResultDTO<TrainedModelDTO>.Fail(null);
            _trainedModelServiceMock.Setup(service => service.GetTrainedModelById(trainedModelId, false))
                .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.GetTrainedModelById(trainedModelId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.ErrMsg);
        }



        [Fact]
        public async Task EditTrainedModelById_ReturnsFailResult_WhenModelStateIsInvalid()
        {
            // Arrange
            var trainedModelViewModel = new TrainedModelViewModel { Id = Guid.NewGuid(), Name = "TestModel", IsPublished = true };
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.EditTrainedModelById(trainedModelViewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Name is required", result.ErrMsg);
        }

        [Fact]
        public async Task EditTrainedModelById_ReturnsOkResult_WhenEditIsSuccessful()
        {
            // Arrange
            var trainedModelViewModel = new TrainedModelViewModel { Id = Guid.NewGuid(), Name = "UpdatedModel", IsPublished = true };

            var resultDto = ResultDTO.Ok();
            _trainedModelServiceMock.Setup(service => service.EditTrainedModelById(trainedModelViewModel.Id, trainedModelViewModel.Name, trainedModelViewModel.IsPublished))
                .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.EditTrainedModelById(trainedModelViewModel);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.ErrMsg);
        }

        [Fact]
        public async Task EditTrainedModelById_ReturnsFailResult_WhenServiceFails()
        {
            // Arrange
            var trainedModelViewModel = new TrainedModelViewModel { Id = Guid.NewGuid(), Name = "UpdatedModel", IsPublished = true };

            var resultDto = ResultDTO.Fail("Service failed");
            _trainedModelServiceMock.Setup(service => service.EditTrainedModelById(trainedModelViewModel.Id, trainedModelViewModel.Name, trainedModelViewModel.IsPublished))
                .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.EditTrainedModelById(trainedModelViewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Service failed", result.ErrMsg);
        }

        [Fact]
        public async Task EditTrainedModelById_ReturnsFailResult_WhenHandleErrorIsTrue()
        {
            // Arrange
            var trainedModelViewModel = new TrainedModelViewModel { Id = Guid.NewGuid(), Name = "UpdatedModel", IsPublished = true };

            var resultDto = ResultDTO.Fail("Handled error");
            _trainedModelServiceMock.Setup(service => service.EditTrainedModelById(trainedModelViewModel.Id, trainedModelViewModel.Name, trainedModelViewModel.IsPublished))
                .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.EditTrainedModelById(trainedModelViewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Handled error", result.ErrMsg);
        }

        [Fact]
        public async Task EditTrainedModelById_ReturnsFailResult_WhenModelStateIsInvalid_EmptyName()
        {
            // Arrange
            var trainedModelViewModel = new TrainedModelViewModel { Id = Guid.NewGuid(), Name = "", IsPublished = true };
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.EditTrainedModelById(trainedModelViewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Name is required", result.ErrMsg);
        }

        [Fact]
        public async Task EditTrainedModelById_ReturnsFailResult_WhenNameIsTooLong()
        {
            // Arrange
            var longName = new string('a', 256);

            var trainedModelViewModel = new TrainedModelViewModel { Id = Guid.NewGuid(), Name = longName, IsPublished = true };

            _controller.ModelState.AddModelError("Name", "The field Name must be a string or array type with a maximum length of '255'.");

            // Act
            var result = await _controller.EditTrainedModelById(trainedModelViewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("The field Name must be a string or array type with a maximum length of '255'.", result.ErrMsg);
        }



        [Fact]
        public async Task DeleteTrainedModelById_ReturnsFailResult_WhenTrainedModelIdIsEmpty()
        {
            // Arrange
            var invalidTrainedModelId = Guid.Empty;

            // Act
            var result = await _controller.DeleteTrainedModelById(invalidTrainedModelId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid training run id", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteTrainedModelById_ReturnsOkResult_WhenDeletionIsSuccessful()
        {
            // Arrange
            var validTrainedModelId = Guid.NewGuid();

            var resultDto = ResultDTO.Ok();
            _trainedModelServiceMock.Setup(service => service.DeleteTrainedModelById(validTrainedModelId))
                .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.DeleteTrainedModelById(validTrainedModelId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.ErrMsg);
        }

        [Fact]
        public async Task DeleteTrainedModelById_ReturnsFailResult_WhenServiceFails()
        {
            // Arrange
            var validTrainedModelId = Guid.NewGuid();

            var resultDto = ResultDTO.Fail("Service failed");
            _trainedModelServiceMock.Setup(service => service.DeleteTrainedModelById(validTrainedModelId))
                .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.DeleteTrainedModelById(validTrainedModelId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Service failed", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteTrainedModelById_ReturnsFailResult_WhenHandleErrorIsTrue()
        {
            // Arrange
            var validTrainedModelId = Guid.NewGuid();

            var resultDto = ResultDTO.Fail("Handled error");
            _trainedModelServiceMock.Setup(service => service.DeleteTrainedModelById(validTrainedModelId))
                .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.DeleteTrainedModelById(validTrainedModelId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Handled error", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteTrainedModelById_ReturnsFailResult_WhenServiceReturnsNull()
        {
            // Arrange
            var validTrainedModelId = Guid.NewGuid();
            var resultDto = ResultDTO.Fail("Service returned null");
            _trainedModelServiceMock.Setup(service => service.DeleteTrainedModelById(validTrainedModelId))
                .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.DeleteTrainedModelById(validTrainedModelId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Service returned null", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteTrainedModelById_ReturnsBadRequest_WhenGuidIsEmpty()
        {
            // Arrange
            var invalidGuid = Guid.Empty;

            // Act
            var result = await _controller.DeleteTrainedModelById(invalidGuid);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid training run id", result.ErrMsg);
        }

        [Fact]
        public async Task GetAllTrainedModels_Returns_Success_With_Data()
        {
            // Arrange
            var trainedModels = new List<TrainedModelDTO>
                {
                    new TrainedModelDTO { Id = Guid.NewGuid(), Name = "Model1" },
                    new TrainedModelDTO { Id = Guid.NewGuid(), Name = "Model2" }
                };

            _trainedModelServiceMock.Setup(service => service.GetAllTrainedModels())
                .ReturnsAsync(ResultDTO<List<TrainedModelDTO>>.Ok(trainedModels));

            // Act
            var result = await _controller.GetAllTrainedModels();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count);
        }

        [Fact]
        public async Task GetAllTrainedModels_Returns_Failure_When_Service_Fails()
        {
            // Arrange
            _trainedModelServiceMock.Setup(service => service.GetAllTrainedModels())
                .ReturnsAsync(ResultDTO<List<TrainedModelDTO>>.Fail("Service error"));

            // Act
            var result = await _controller.GetAllTrainedModels();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Service error", result.ErrMsg);
        }

        [Fact]
        public async Task GetAllTrainedModels_Returns_Failure_When_Data_Is_Null()
        {
            // Arrange
            _trainedModelServiceMock.Setup(service => service.GetAllTrainedModels())
                .ReturnsAsync(ResultDTO<List<TrainedModelDTO>>.Ok(null));

            // Act
            var result = await _controller.GetAllTrainedModels();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Trained models are not found", result.ErrMsg);
        }

        [Fact]
        public async Task GetAllTrainedModels_Returns_ExceptionFail_When_Exception_Is_Thrown()
        {
            // Arrange
            var exceptionMessage = "An unexpected error occurred.";
            _trainedModelServiceMock.Setup(service => service.GetAllTrainedModels())
                .ThrowsAsync(new Exception("Exception"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.GetAllTrainedModels());

        }

        [Fact]
        public async Task Index_ReturnsNotFound_WhenError404PathIsNullAndVmListIsNull()
        {
            // Arrange
            var trainedModelDtos = new List<TrainedModelDTO>
    {
        new TrainedModelDTO { Id = Guid.NewGuid(), Name = "Model1" },
        new TrainedModelDTO { Id = Guid.NewGuid(), Name = "Model2" }
    };

            var resultDto = ResultDTO<List<TrainedModelDTO>>.Ok(trainedModelDtos);
            _trainedModelServiceMock.Setup(service => service.GetAllTrainedModels())
                .ReturnsAsync(resultDto);

            _mapperMock.Setup(mapper => mapper.Map<List<TrainedModelViewModel>>(trainedModelDtos))
                .Returns((List<TrainedModelViewModel>)null);

            _configurationMock.Setup(config => config["ErrorViewsPath:Error404"])
                .Returns((string)null);

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

    }
}

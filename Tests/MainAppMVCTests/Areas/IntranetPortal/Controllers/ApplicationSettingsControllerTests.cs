using DAL.Interfaces.Helpers;
using DTOs.MainApp.BL;
using MainApp.BL.Interfaces.Services;
using MainApp.MVC.Areas.IntranetPortal.Controllers;
using MainApp.MVC.ViewModels.IntranetPortal.ApplicationSettings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using SD;
using SD.Enums;

namespace Tests.MainAppMVCTests.Areas.IntranetPortal.Controllers
{
    public class ApplicationSettingsControllerTests
    {
        private readonly Mock<IApplicationSettingsService> _mockApplicationSettingsService;
        private readonly Mock<IAppSettingsAccessor> _mockAppSettingsAccessor;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IWebHostEnvironment> _mockWebHostEnviroment;
        private readonly Mock<IMMDetectionConfigurationService> _mockIMMDetectionConfigurationService;
        private readonly ApplicationSettingsController _controller;

        public ApplicationSettingsControllerTests()
        {
            _mockApplicationSettingsService = new Mock<IApplicationSettingsService>();
            _mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockWebHostEnviroment = new Mock<IWebHostEnvironment>();
            _mockIMMDetectionConfigurationService = new Mock<IMMDetectionConfigurationService>();

            _controller = new ApplicationSettingsController(
                _mockApplicationSettingsService.Object,
                _mockConfiguration.Object,
                _mockAppSettingsAccessor.Object,
                _mockWebHostEnviroment.Object,
                _mockIMMDetectionConfigurationService.Object
                );
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithApplicationSettingsViewModel()
        {
            // Arrange
            var genSet = "testValue";
            var genSetBool = (bool?)true;
            var genSetBoolche = (bool?)null;
            var genSetDateTime = (DateTime?)DateTime.Now;

            _mockAppSettingsAccessor.Setup(accessor => accessor.GetApplicationSettingValueByKey<string>("Generic", "hehe"))
                .ReturnsAsync(new ResultDTO<string>(true, genSet, null, null));
            _mockAppSettingsAccessor.Setup(accessor => accessor.GetApplicationSettingValueByKey<bool?>("GenericBool", false))
                .ReturnsAsync(new ResultDTO<bool?>(true, genSetBool, null, null));
            _mockAppSettingsAccessor.Setup(accessor => accessor.GetApplicationSettingValueByKey<bool?>("GenericBool4e", "2"))
                .ReturnsAsync(new ResultDTO<bool?>(true, genSetBoolche, null, null));
            _mockAppSettingsAccessor.Setup(accessor => accessor.GetApplicationSettingValueByKey<DateTime?>("GenericDateTime", "2"))
                .ReturnsAsync(new ResultDTO<DateTime?>(true, genSetDateTime, null, null));

            var apps = new List<AppSettingDTO>
        {
            new AppSettingDTO { Key = "Key1", Value = "Value1", DataType = ApplicationSettingsDataType.String, Description = "Description1", Module = "Module1" },
            new AppSettingDTO { Key = "Key2", Value = "Value2", DataType = ApplicationSettingsDataType.Boolean, Description = "Description2", Module = "Module2" }
        };

            _mockApplicationSettingsService.Setup(service => service.GetAllApplicationSettingsAsList())
                .ReturnsAsync(apps);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ApplicationSettingsViewModel>>(viewResult.Model);
            Assert.Equal(2, model.Count());

            var firstSetting = model.First();
            Assert.Equal("Key1", firstSetting.Key);
            Assert.Equal("Value1", firstSetting.Value);
            Assert.Equal("String", firstSetting.DataType.ToString());
            Assert.Equal("Description1", firstSetting.Description);
            Assert.Equal("Module1", firstSetting.Module);

            var secondSetting = model.ElementAt(1);
            Assert.Equal("Key2", secondSetting.Key);
            Assert.Equal("Value2", secondSetting.Value);
            Assert.Equal("Boolean", secondSetting.DataType.ToString());
            Assert.Equal("Description2", secondSetting.Description);
            Assert.Equal("Module2", secondSetting.Module);
        }

        [Fact]
        public async Task Index_ReturnsEmptyViewModel_WhenNoApplicationSettings()
        {
            // Arrange
            _mockAppSettingsAccessor.Setup(accessor => accessor.GetApplicationSettingValueByKey<string>("Generic", "hehe"))
                .ReturnsAsync(new ResultDTO<string>(true, "testValue", null, null));
            _mockAppSettingsAccessor.Setup(accessor => accessor.GetApplicationSettingValueByKey<bool?>("GenericBool", false))
                .ReturnsAsync(new ResultDTO<bool?>(true, true, null, null));
            _mockAppSettingsAccessor.Setup(accessor => accessor.GetApplicationSettingValueByKey<bool?>("GenericBool4e", "2"))
                .ReturnsAsync(new ResultDTO<bool?>(true, (bool?)null, null, null));
            _mockAppSettingsAccessor.Setup(accessor => accessor.GetApplicationSettingValueByKey<DateTime?>("GenericDateTime", null))
                .ReturnsAsync(new ResultDTO<DateTime?>(true, (DateTime?)null, null, null));

            _mockApplicationSettingsService.Setup(service => service.GetAllApplicationSettingsAsList())
                .ReturnsAsync(new List<AppSettingDTO>());

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ApplicationSettingsViewModel>>(viewResult.Model);
            Assert.Empty(model);
        }

        [Fact]
        public async Task Create_ReturnsViewResult_WithApplicationSettingsCreateViewModel()
        {
            // Arrange
            var keys = new List<string> { "Key1", "Key2" };
            _mockApplicationSettingsService.Setup(service => service.GetAllApplicationSettingsKeysAsList())
                .ReturnsAsync(keys);

            // Act
            var result = await _controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ApplicationSettingsCreateViewModel>(viewResult.Model);
            Assert.Equal(keys, model.AllApplicationSettingsKeys);
            Assert.NotEmpty(model.Modules);
        }

        [Fact]
        public async Task Create_ReturnsViewResult_WithEmptyKeys()
        {
            // Arrange
            _mockApplicationSettingsService.Setup(service => service.GetAllApplicationSettingsKeysAsList())
                .ReturnsAsync(new List<string>());

            // Act
            var result = await _controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ApplicationSettingsCreateViewModel>(viewResult.Model);
            Assert.Empty(model.AllApplicationSettingsKeys);
            Assert.NotEmpty(model.Modules);
        }

        [Fact]
        public async Task Create_Post_InvalidModelState_ReturnsViewWithModel()
        {
            // Arrange
            var model = new ApplicationSettingsCreateViewModel
            {
                Key = "Key1",
                Value = "Value1",
                DataType = ApplicationSettingsDataType.String,
                Description = "Description1",
                Modules = new List<Module>(),
                AllApplicationSettingsKeys = new List<string> { "Key1", "Key2" }
            };
            _controller.ModelState.AddModelError("Key", "Required");

            _mockApplicationSettingsService.Setup(service => service.GetAllApplicationSettingsKeysAsList())
                .ReturnsAsync(new List<string> { "Key1", "Key2" });

            // Act
            var result = await _controller.Create(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<ApplicationSettingsCreateViewModel>(viewResult.Model);
            Assert.True(viewModel.Key == "Key1");
            Assert.Equal(2, viewModel.AllApplicationSettingsKeys.Count());
            Assert.NotEmpty(viewModel.Modules);
        }

        [Fact]
        public async Task Create_Post_ValidModelState_CreatesApplicationSetting_AndRedirects()
        {
            // Arrange
            var model = new ApplicationSettingsCreateViewModel
            {
                Key = "Key1",
                Value = "Value1",
                DataType = ApplicationSettingsDataType.String,
                Description = "Description1",
                Modules = new List<Module> { SD.Modules.Admin },
                AllApplicationSettingsKeys = new List<string> { "Key1", "Key2" }
            };

            _mockApplicationSettingsService.Setup(service => service.GetAllApplicationSettingsKeysAsList())
                .ReturnsAsync(new List<string> { "Key1", "Key2" });

            _mockApplicationSettingsService.Setup(service => service.CreateApplicationSetting(It.IsAny<AppSettingDTO>()))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _controller.Create(model);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(ApplicationSettingsController.Index), redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Create_Post_CreateFails_ReturnsViewWithModel()
        {
            // Arrange
            var model = new ApplicationSettingsCreateViewModel
            {
                Key = "Key1",
                Value = "Value1",
                DataType = ApplicationSettingsDataType.String,
                Description = "Description1",
                Modules = new List<Module> { SD.Modules.Admin },
                AllApplicationSettingsKeys = new List<string> { "Key1", "Key2" }
            };

            _mockApplicationSettingsService.Setup(service => service.GetAllApplicationSettingsKeysAsList())
                .ReturnsAsync(new List<string> { "Key1", "Key2" });

            _mockApplicationSettingsService.Setup(service => service.CreateApplicationSetting(It.IsAny<AppSettingDTO>()))
                .ReturnsAsync(ResultDTO.Fail("Failed to create setting"));

            // Act
            var result = await _controller.Create(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<ApplicationSettingsCreateViewModel>(viewResult.Model);
            Assert.NotEmpty(viewModel.Modules);
        }

        [Fact]
        public async Task Edit_SettingExists_ReturnsViewWithModel()
        {
            // Arrange
            var settingKey = "Key1";
            var expectedSetting = new AppSettingDTO
            {
                Key = "Key1",
                Value = "Value1",
                Description = "Description1",
                DataType = ApplicationSettingsDataType.String,
                Module = "Admin"
            };

            _mockApplicationSettingsService.Setup(service => service.GetApplicationSettingByKey(settingKey))
                .ReturnsAsync(expectedSetting);

            // Act
            var result = await _controller.Edit(settingKey);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ApplicationSettingsEditViewModel>(viewResult.Model);
            Assert.Equal(expectedSetting.Key, model.Key);
            Assert.Equal(expectedSetting.Value, model.Value);
            Assert.Equal(expectedSetting.Description, model.Description);
            Assert.Equal(expectedSetting.DataType, model.DataType);
            Assert.Equal(expectedSetting.Module, model.InsertedModule);
            Assert.NotEmpty(model.Modules);
        }

        [Fact]
        public async Task Edit_SettingNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            var settingKey = "NonExistentKey";
            _mockApplicationSettingsService.Setup(service => service.GetApplicationSettingByKey(settingKey))
                .ReturnsAsync((AppSettingDTO)null);

            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error404"]).Returns((string)null);

            // Act
            var result = await _controller.Edit(settingKey);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_SettingNotFound_RedirectsToError404View()
        {
            // Arrange
            var settingKey = "NonExistentKey";
            var errorPath = "/Error404";
            _mockApplicationSettingsService.Setup(service => service.GetApplicationSettingByKey(settingKey))
                .ReturnsAsync((AppSettingDTO)null);

            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error404"]).Returns(errorPath);

            // Act
            var result = await _controller.Edit(settingKey);

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal(errorPath, redirectResult.Url);
        }

        [Fact]
        public async Task Edit_Post_InvalidModelState_ReturnsViewWithModel()
        {
            // Arrange
            var model = new ApplicationSettingsEditViewModel
            {
                Key = "Key1",
                Value = "Value1",
                Description = "Description1",
                DataType = ApplicationSettingsDataType.String
            };
            _controller.ModelState.AddModelError("Key", "Required");

            // Act
            var result = await _controller.Edit(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<ApplicationSettingsEditViewModel>(viewResult.Model);
            Assert.Equal(model.Key, viewModel.Key);
            Assert.Equal(model.Value, viewModel.Value);
            Assert.Equal(model.Description, viewModel.Description);
            Assert.NotEmpty(viewModel.Modules);
        }

        [Fact]
        public async Task Edit_Post_ValidModelStateAndUpdateSuccess_RedirectsToIndex()
        {
            // Arrange
            var model = new ApplicationSettingsEditViewModel
            {
                Key = "Key1",
                Value = "Value1",
                Description = "Description1",
                DataType = ApplicationSettingsDataType.String
            };

            _mockApplicationSettingsService.Setup(service => service.UpdateApplicationSetting(It.IsAny<AppSettingDTO>()))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _controller.Edit(model);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Edit_Post_ValidModelStateAndUpdateFails_ReturnsViewWithModel()
        {
            // Arrange
            var model = new ApplicationSettingsEditViewModel
            {
                Key = "Key1",
                Value = "Value1",
                Description = "Description1",
                DataType = ApplicationSettingsDataType.String
            };

            _mockApplicationSettingsService.Setup(service => service.UpdateApplicationSetting(It.IsAny<AppSettingDTO>()))
                .ReturnsAsync(ResultDTO.Fail("Failed to update setting"));

            // Act
            var result = await _controller.Edit(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<ApplicationSettingsEditViewModel>(viewResult.Model);
            Assert.Equal(model.Key, viewModel.Key);
            Assert.Equal(model.Value, viewModel.Value);
            Assert.Equal(model.Description, viewModel.Description);
            Assert.NotEmpty(viewModel.Modules);

        }

        [Fact]
        public async Task Delete_ExistingKey_ReturnsDeleteViewModel()
        {
            // Arrange
            string key = "ExistingKey";
            var appSettingDTO = new AppSettingDTO
            {
                Key = key,
                Value = "Some Value",
                Description = "Description",
                DataType = ApplicationSettingsDataType.String,
                Module = "Module1"
            };
            _mockApplicationSettingsService.Setup(s => s.GetApplicationSettingByKey(key)).ReturnsAsync(appSettingDTO);
            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error404"]).Returns("/Error/NotFound");

            // Act
            var result = await _controller.Delete(key);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ApplicationSettingsDeleteViewModel>(viewResult.Model);
            Assert.Equal(appSettingDTO.Key, model.Key);
            Assert.Equal(appSettingDTO.Value, model.Value);
            Assert.Equal(appSettingDTO.Description, model.Description);
            Assert.Equal(appSettingDTO.DataType, model.DataType);
            Assert.Equal(appSettingDTO.Module, model.InsertedModule);
        }

        [Fact]
        public async Task Delete_NullKey_ReturnsRedirectToErrorPage()
        {
            // Arrange
            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error404"]).Returns("/Error/NotFound");

            // Act
            var result = await _controller.Delete(null);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/Error/NotFound", redirectToActionResult.Url);
        }

        [Fact]
        public async Task Delete_KeyDoesNotExist_ReturnsRedirectToErrorPage()
        {
            // Arrange
            string key = "NonExistentKey";
            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error404"]).Returns("/Error/NotFound");
            _mockApplicationSettingsService.Setup(s => s.GetApplicationSettingByKey(key)).ReturnsAsync((AppSettingDTO)null);

            // Act
            var result = await _controller.Delete(key);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/Error/NotFound", redirectToActionResult.Url);
        }
        [Fact]
        public async Task Delete_KeyDoesNotExist_ReturnsNotFound_WhenErrorPathNotConfigured()
        {
            // Arrange
            string key = "NonExistentKey";
            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error404"]).Returns((string)null);
            _mockApplicationSettingsService.Setup(s => s.GetApplicationSettingByKey(key)).ReturnsAsync((AppSettingDTO)null);

            // Act
            var result = await _controller.Delete(key);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_AppSettingIsNull_ReturnsRedirectToErrorPage()
        {
            // Arrange
            string key = "NonExistentKey"; // Key that does not exist
            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error404"]).Returns("/Error/NotFound");
            _mockApplicationSettingsService.Setup(s => s.GetApplicationSettingByKey(key)).ReturnsAsync((AppSettingDTO)null);

            // Act
            var result = await _controller.Delete(key);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/Error/NotFound", redirectToActionResult.Url);
        }

        [Fact]
        public async Task Delete_AppSettingIsNull_ReturnsNotFound_WhenErrorPathNotConfigured()
        {
            // Arrange
            string key = "NonExistentKey"; // Key that does not exist
            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error404"]).Returns((string)null);
            _mockApplicationSettingsService.Setup(s => s.GetApplicationSettingByKey(key)).ReturnsAsync((AppSettingDTO)null);

            // Act
            var result = await _controller.Delete(key);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public async Task DeleteConfirmed_SuccessfulDeletion_RedirectsToIndex()
        {
            // Arrange
            string key = "ValidKey";
            _mockApplicationSettingsService.Setup(s => s.DeleteApplicationSetting(key)).ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _controller.DeleteConfirmed(key);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(ApplicationSettingsController.Index), redirectResult.ActionName);
        }

        [Fact]
        public async Task DeleteConfirmed_FailedDeletion_ReturnsView()
        {
            // Arrange
            string key = "InvalidKey";
            _mockApplicationSettingsService.Setup(s => s.DeleteApplicationSetting(key)).ReturnsAsync(ResultDTO.Fail("Failed to delete setting"));

            // Act
            var result = await _controller.DeleteConfirmed(key);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
        }
    }
}

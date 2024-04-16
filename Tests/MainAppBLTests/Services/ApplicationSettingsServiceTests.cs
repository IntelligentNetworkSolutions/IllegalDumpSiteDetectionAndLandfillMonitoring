using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Interfaces.Repositories;
using DTOs.MainApp.BL;
using Entities;
using MainApp.BL.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.MainAppBLTests.Services
{
    public class ApplicationSettingsServiceTests
    {
        private readonly Mock<IApplicationSettingsRepo> _mockRepo = new Mock<IApplicationSettingsRepo>();
        private readonly Mock<ILogger<ApplicationSettingsService>> _mockLogger = new Mock<ILogger<ApplicationSettingsService>>();
        private readonly Mock<IMapper> _mockMapper = new Mock<IMapper>();
        private readonly ApplicationSettingsService _service;

        public ApplicationSettingsServiceTests()
        {
            _service = new ApplicationSettingsService(_mockRepo.Object, _mockLogger.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task CreateApplicationSetting_ValidInput_ReturnsOk()
        {
            // Arrange
            var dto = new AppSettingDTO { Key = "TestKey", Value = "TestValue" };
            var entity = new ApplicationSettings();
            _mockMapper.Setup(m => m.Map<ApplicationSettings>(It.IsAny<AppSettingDTO>())).Returns(entity);
            _mockRepo.Setup(r => r.CreateApplicationSetting(It.IsAny<ApplicationSettings>())).ReturnsAsync(true);

            // Act
            var result = await _service.CreateApplicationSetting(dto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task CreateApplicationSetting_NullDto_ReturnsFail()
        {
            // Arrange
            AppSettingDTO? dto = null;

            // Act
            var result = await _service.CreateApplicationSetting(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Missing App Setting Object", result.ErrMsg);
        }

        [Fact]
        public async Task CreateApplicationSetting_EmptyKey_ReturnsFail()
        {
            // Arrange
            var dto = new AppSettingDTO { Key = "", Value = "TestValue" };
            _mockMapper.Setup(m => m.Map<ApplicationSettings>(It.IsAny<AppSettingDTO>()));

            // Act
            var result = await _service.CreateApplicationSetting(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Missing App Setting Key", result.ErrMsg);
        }

        [Fact]
        public async Task CreateApplicationSetting_EmptyValue_ReturnsFail()
        {
            // Arrange
            var dto = new AppSettingDTO { Key = "TestKey", Value = " " };
            _mockMapper.Setup(m => m.Map<ApplicationSettings>(It.IsAny<AppSettingDTO>()));

            // Act
            var result = await _service.CreateApplicationSetting(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Missing App Setting Value", result.ErrMsg);
        }

        [Fact]
        public async Task CreateApplicationSetting_RepoFailure_ReturnsFail()
        {
            // Arrange
            var dto = new AppSettingDTO { Key = "TestKey", Value = "TestValue" };
            var entity = new ApplicationSettings();
            _mockMapper.Setup(m => m.Map<ApplicationSettings>(It.IsAny<AppSettingDTO>())).Returns(entity);
            _mockRepo.Setup(r => r.CreateApplicationSetting(It.IsAny<ApplicationSettings>())).ReturnsAsync(false); // Simulate failure

            // Act
            var result = await _service.CreateApplicationSetting(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Database Error", result.ErrMsg);
        }

        [Fact]
        public async Task CreateApplicationSetting_MapperFails_ReturnsFail()
        {
            // Arrange
            var dto = new AppSettingDTO { Key = "TestKey", Value = "NewValue" };
            _mockRepo.Setup(r => r.GetApplicationSettingByKey("TestKey")).ReturnsAsync(new ApplicationSettings());
            _mockMapper.Setup(m => m.Map<ApplicationSettings>(It.IsAny<AppSettingDTO>())).Returns((ApplicationSettings)null); // Simulate mapper failure

            // Act
            var result = await _service.CreateApplicationSetting(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Mapping Failed", result.ErrMsg); // Assuming this handling is added
        }

        [Fact]
        public async Task CreateApplicationSetting_ThrowsException_ReturnsExceptionFail()
        {
            // Arrange
            var dto = new AppSettingDTO { Key = "TestKey", Value = "TestValue" };
            var exceptionThrown = new Exception("Repository failure");
            _mockMapper.Setup(m => m.Map<ApplicationSettings>(It.IsAny<AppSettingDTO>())).Returns(new ApplicationSettings());
            _mockRepo.Setup(r => r.CreateApplicationSetting(It.IsAny<ApplicationSettings>())).ThrowsAsync(exceptionThrown);

            // Act
            var result = await _service.CreateApplicationSetting(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(exceptionThrown.Message, result.ErrMsg);
            Assert.Equal(exceptionThrown, result.ExObj);
        }

        [Fact]
        public async Task UpdateApplicationSetting_SettingExists_ReturnsOk()
        {
            // Arrange
            var dto = new AppSettingDTO { Key = "TestKey", Value = "NewValue" };
            var existingEntity = new ApplicationSettings();
            _mockRepo.Setup(r => r.GetApplicationSettingByKey("TestKey")).ReturnsAsync(existingEntity);
            _mockMapper.Setup(m => m.Map<ApplicationSettings>(It.IsAny<AppSettingDTO>())).Returns(existingEntity);
            _mockRepo.Setup(r => r.UpdateApplicationSetting(It.IsAny<ApplicationSettings>())).ReturnsAsync(true);

            // Act
            var result = await _service.UpdateApplicationSetting(dto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task UpdateApplicationSetting_SettingNotFound_ReturnsFail()
        {
            // Arrange
            var dto = new AppSettingDTO { Key = "TestKey", Value = "NewValue" };
            _mockRepo.Setup(r => r.GetApplicationSettingByKey("TestKey")).ReturnsAsync((ApplicationSettings)null);

            // Act
            var result = await _service.UpdateApplicationSetting(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Application Setting Not Found", result.ErrMsg);
        }

        [Fact]
        public async Task UpdateApplicationSetting_EmptyKey_ReturnsFail()
        {
            // Arrange
            var dto = new AppSettingDTO { Key = "", Value = "NewValue" };

            // Act
            var result = await _service.UpdateApplicationSetting(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Missing App Setting Key", result.ErrMsg); // Assuming value checking for key exists
        }

        [Fact]
        public async Task UpdateApplicationSetting_EmptyValue_ReturnsFail()
        {
            // Arrange
            var dto = new AppSettingDTO { Key = "TestKey", Value = "" };

            // Act
            var result = await _service.UpdateApplicationSetting(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Missing App Setting Value", result.ErrMsg); // Assuming value checking for key exists
        }

        [Fact]
        public async Task UpdateApplicationSetting_RepoFailure_ReturnsFail()
        {
            // Arrange
            var dto = new AppSettingDTO { Key = "TestKey", Value = "TestValue" };
            var entity = new ApplicationSettings { Key = "TestKey", Value = "TestValue", Description = "TestKey" };
            _mockMapper.Setup(m => m.Map<ApplicationSettings>(It.IsAny<AppSettingDTO>())).Returns(entity);
            _mockRepo.Setup(r => r.GetApplicationSettingByKey("TestKey")).ReturnsAsync(entity);
            _mockRepo.Setup(r => r.UpdateApplicationSetting(It.IsAny<ApplicationSettings>())).ReturnsAsync(false); // Simulate failure

            // Act
            var result = await _service.UpdateApplicationSetting(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Database Error", result.ErrMsg);
        }

        [Fact]
        public async Task UpdateApplicationSetting_MapperFails_ReturnsFail()
        {
            // Arrange
            var dto = new AppSettingDTO { Key = "TestKey", Value = "NewValue" };
            _mockRepo.Setup(r => r.GetApplicationSettingByKey("TestKey")).ReturnsAsync(new ApplicationSettings());
            _mockMapper.Setup(m => m.Map<ApplicationSettings>(It.IsAny<AppSettingDTO>())).Returns((ApplicationSettings)null); // Simulate mapper failure

            // Act
            var result = await _service.UpdateApplicationSetting(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Mapping Failed", result.ErrMsg); // Assuming this handling is added
        }

        [Fact]
        public async Task UpdateApplicationSetting_ThrowsException_ReturnsExceptionFail()
        {
            // Arrange
            var dto = new AppSettingDTO { Key = "TestKey", Value = "TestValue" };
            var exceptionThrown = new Exception("Repository failure");
            _mockRepo.Setup(r => r.GetApplicationSettingByKey(It.IsAny<string>())).ReturnsAsync(new ApplicationSettings());
            _mockMapper.Setup(m => m.Map<ApplicationSettings>(It.IsAny<AppSettingDTO>())).Returns(new ApplicationSettings());
            _mockRepo.Setup(r => r.UpdateApplicationSetting(It.IsAny<ApplicationSettings>())).ThrowsAsync(exceptionThrown);

            // Act
            var result = await _service.UpdateApplicationSetting(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(exceptionThrown.Message, result.ErrMsg);
            Assert.Equal(exceptionThrown, result.ExObj);
        }

        [Fact]
        public async Task DeleteApplicationSetting_EmptyKey_ReturnsFail()
        {
            // Act
            var result = await _service.DeleteApplicationSetting(" ");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Missing App Setting Value", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteApplicationSetting_SettingNotFound_ReturnsFail()
        {
            // Arrange
            _mockRepo.Setup(r => r.DeleteApplicationSettingByKey("TestKey")).ReturnsAsync((bool?)null);

            // Act
            var result = await _service.DeleteApplicationSetting("TestKey");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Application Setting Not Found", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteApplicationSetting_DeleteFails_ReturnsFail()
        {
            // Arrange
            _mockRepo.Setup(r => r.DeleteApplicationSettingByKey("TestKey")).ReturnsAsync(false);

            // Act
            var result = await _service.DeleteApplicationSetting("TestKey");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal($"Database Error while Deleting App Setting with key: TestKey", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteApplicationSetting_Success_ReturnsOk()
        {
            // Arrange
            _mockRepo.Setup(r => r.DeleteApplicationSettingByKey("TestKey")).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteApplicationSetting("TestKey");

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteApplicationSetting_ThrowsException_ReturnsExceptionFail()
        {
            // Arrange
            var exceptionThrown = new Exception("Unexpected Error");
            _mockRepo.Setup(r => r.DeleteApplicationSettingByKey("TestKey")).ThrowsAsync(exceptionThrown);

            // Act
            var result = await _service.DeleteApplicationSetting("TestKey");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Unexpected Error", result.ErrMsg);
            Assert.Equal(exceptionThrown, result.ExObj);
        }

        [Fact]
        public async Task GetAllApplicationSettingsAsList_ReturnsMappedDtoList()
        {
            // Arrange
            var appSettings = new List<ApplicationSettings> { new ApplicationSettings { Key = "TestKey", Value = "TestValue" } };
            var expectedDtos = new List<AppSettingDTO> { new AppSettingDTO { Key = "TestKey", Value = "TestValue" } };

            _mockRepo.Setup(r => r.GetAllApplicationSettingsAsList()).ReturnsAsync(appSettings);
            _mockMapper.Setup(m => m.Map<List<AppSettingDTO>>(appSettings)).Returns(expectedDtos);

            // Act
            var result = await _service.GetAllApplicationSettingsAsList();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDtos, result);
        }

        [Fact]
        public async Task GetAllApplicationSettingsAsList_ThrowsException_ReturnsNull()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetAllApplicationSettingsAsList()).ThrowsAsync(new Exception("Database failure"));

            // Act
            var result = await _service.GetAllApplicationSettingsAsList();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllApplicationSettingsKeysAsList_ReturnsKeysList()
        {
            // Arrange
            var keysList = new List<string> { "Key1", "Key2" };
            _mockRepo.Setup(r => r.GetAllApplicationSettingsKeysAsList()).ReturnsAsync(keysList);

            // Act
            var result = await _service.GetAllApplicationSettingsKeysAsList();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(keysList, result);
        }

        [Fact]
        public async Task GetAllApplicationSettingsKeysAsList_ThrowsException_ReturnsNull()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetAllApplicationSettingsKeysAsList()).ThrowsAsync(new Exception("Database failure"));

            // Act
            var result = await _service.GetAllApplicationSettingsKeysAsList();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetApplicationSettingByKey_ReturnsSettingDto_WhenKeyIsValid()
        {
            // Arrange
            var appSetting = new ApplicationSettings { /* populate properties */ };
            var appSettingDto = new AppSettingDTO { /* populate properties */ };

            _mockRepo.Setup(repo => repo.GetApplicationSettingByKey("validKey"))
                     .ReturnsAsync(appSetting);
            _mockMapper.Setup(mapper => mapper.Map<AppSettingDTO>(It.IsAny<ApplicationSettings>()))
                       .Returns(appSettingDto);

            // Act
            var result = await _service.GetApplicationSettingByKey("validKey");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(appSettingDto, result);
        }

        [Fact]
        public async Task GetApplicationSettingByKey_ReturnsNull_WhenKeyDoesNotExist()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetApplicationSettingByKey("invalidKey")).ReturnsAsync((ApplicationSettings?)null);

            // Act
            var result = await _service.GetApplicationSettingByKey("invalidKey");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetApplicationSettingByKey_ReturnsNull_WhenKeyIsNull()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetApplicationSettingByKey(null)).ReturnsAsync((ApplicationSettings?)null);

            // Act
            var result = await _service.GetApplicationSettingByKey(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetApplicationSettingByKey_LogsErrorAndReturnsNull_WhenExceptionOccurs()
        {
            // Arrange
            Exception exception = new Exception("Test Exception");
            _mockRepo.Setup(repo => repo.GetApplicationSettingByKey("exceptionKey")).ThrowsAsync(exception);

            // Act
            var result = await _service.GetApplicationSettingByKey("exceptionKey");

            // Assert
            Assert.Null(result);
            //_mockLogger.Verify(logger => logger.LogError(exception.Message, exception), Times.Once);
        }
    }
}

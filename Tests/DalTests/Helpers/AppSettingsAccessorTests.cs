using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Helpers;
using DAL.Interfaces.Repositories;
using Entities;
using Moq;
using SD;

namespace Tests.DalTests.Helpers
{
    public class AppSettingsAccessorTests
    {
        [Fact]
        public async Task GetApplicationSettingValueByKey_ExistingValueNoDefaultAsync()
        {
            // Arrange
            string expectedValue = "Test Value";

            Mock<IApplicationSettingsRepo> mockRepository = new Mock<IApplicationSettingsRepo>();
            mockRepository.Setup(x => x.GetApplicationSettingByKey("test"))
                            .ReturnsAsync(new ApplicationSettings { Key = "test", Value = expectedValue });

            AppSettingsAccessor settingsAccessor = new AppSettingsAccessor(mockRepository.Object);

            // Act
            ResultDTO<string?> actualResult = await settingsAccessor.GetApplicationSettingValueByKey<string>("test");

            // Assert
            Assert.True(actualResult.IsSuccess);
            Assert.Equal(expectedValue, actualResult.Data.ToString());
        }

        [Fact]
        public async Task GetApplicationSettingValueByKey_NotFoundAsync()
        {
            // Arrange
            Mock<IApplicationSettingsRepo> mockRepository = new Mock<IApplicationSettingsRepo>();
            mockRepository.Setup(x => x.GetApplicationSettingByKey("notfound")).ReturnsAsync(() => null as ApplicationSettings);

            AppSettingsAccessor settingsAccessor = new AppSettingsAccessor(mockRepository.Object);

            // Act
            ResultDTO<int> actualResult = await settingsAccessor.GetApplicationSettingValueByKey<int>("notfound");

            // Assert
            Assert.False(actualResult.IsSuccess);
            Assert.Contains("missing", actualResult.ErrMsg, StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData(-5)]
        [InlineData(0)]
        [InlineData(42)]
        public async void GetApplicationSettingValueByKey_UseDefault(int defaultVal)
        {
            // Arrange
            Mock<IApplicationSettingsRepo> mockRepository = new Mock<IApplicationSettingsRepo>();
            mockRepository.Setup(x => x.GetApplicationSettingByKey("default")).ReturnsAsync(() => null as ApplicationSettings);

            AppSettingsAccessor settingsAccessor = new AppSettingsAccessor(mockRepository.Object);

            // Act
            ResultDTO<int> actualResult = await settingsAccessor.GetApplicationSettingValueByKey<int>("default", defaultVal);

            // Assert
            Assert.True(actualResult.IsSuccess);
            Assert.Equal(defaultVal, actualResult.Data);
        }
    }
}

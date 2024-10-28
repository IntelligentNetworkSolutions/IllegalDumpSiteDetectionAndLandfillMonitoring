using MainApp.MVC.Helpers;
using Microsoft.Extensions.Configuration;
using Moq;
using SD;

namespace Tests.MainAppMVCTests.Helpers
{
    public class ModulesAndAuthClaimsHelperTests
    {
        // Some of the tests are in UserManagementControllerTest
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly ModulesAndAuthClaimsHelper _helper;

        public ModulesAndAuthClaimsHelperTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _helper = new ModulesAndAuthClaimsHelper(_mockConfiguration.Object);
        }

        private Mock<IConfigurationSection> SetupMockModuleSection(string[] activeModules)
        {
            var mockModuleSection = new Mock<IConfigurationSection>();
            mockModuleSection.Setup(s => s.Value).Returns(string.Join(",", activeModules)); // For cases where you want to retrieve as a string
            _mockConfiguration.Setup(c => c.GetSection("AppSettings:Modules")).Returns(mockModuleSection.Object);
            return mockModuleSection;
        }

        [Fact]
        public void HasModule_ShouldReturnFalse_WhenModuleIsNotActive()
        {
            // Arrange
            var activeModules = new[] { "Module1", "Module2" };
            SetupMockModuleSection(activeModules);

            var moduleToCheck = new Module { Value = "Module3" };

            // Act
            var result = _helper.HasModule(moduleToCheck);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HasModule_ShouldReturnFalse_WhenNoActiveModules()
        {
            // Arrange
            var activeModules = new string[0]; // No active modules
            SetupMockModuleSection(activeModules);

            var moduleToCheck = new Module { Value = "Module1" };

            // Act
            var result = _helper.HasModule(moduleToCheck);

            // Assert
            Assert.False(result);
        }
    }
}

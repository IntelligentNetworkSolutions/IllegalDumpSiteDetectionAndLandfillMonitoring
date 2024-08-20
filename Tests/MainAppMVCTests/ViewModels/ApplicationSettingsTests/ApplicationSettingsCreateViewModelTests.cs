using MainApp.MVC.ViewModels.IntranetPortal.ApplicationSettings;
using SD;
using SD.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppMVCTests.ViewModels.ApplicationSettingsTests
{
    public class ApplicationSettingsCreateViewModelTests
    {
        [Fact]
        public void Constructor_ShouldInitializeModules()
        {
            // Arrange & Act
            var viewModel = new ApplicationSettingsCreateViewModel();

            // Assert
            Assert.NotNull(viewModel.Modules);
            Assert.Empty(viewModel.Modules);
        }

        [Fact]
        public void Constructor_ShouldInitializeAllApplicationSettingsKeys()
        {
            // Arrange & Act
            var viewModel = new ApplicationSettingsCreateViewModel();

            // Assert
            Assert.NotNull(viewModel.AllApplicationSettingsKeys);
            Assert.Empty(viewModel.AllApplicationSettingsKeys);
        }

        [Fact]
        public void KeyProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var viewModel = new ApplicationSettingsCreateViewModel();
            var expectedKey = "TestKey";

            // Act
            viewModel.Key = expectedKey;

            // Assert
            Assert.Equal(expectedKey, viewModel.Key);
        }

        [Fact]
        public void ValueProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var viewModel = new ApplicationSettingsCreateViewModel();
            var expectedValue = "TestValue";

            // Act
            viewModel.Value = expectedValue;

            // Assert
            Assert.Equal(expectedValue, viewModel.Value);
        }

        [Fact]
        public void DescriptionProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var viewModel = new ApplicationSettingsCreateViewModel();
            var expectedDescription = "TestDescription";

            // Act
            viewModel.Description = expectedDescription;

            // Assert
            Assert.Equal(expectedDescription, viewModel.Description);
        }

        [Fact]
        public void InsertedModuleProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var viewModel = new ApplicationSettingsCreateViewModel();
            var expectedInsertedModule = "TestModule";

            // Act
            viewModel.InsertedModule = expectedInsertedModule;

            // Assert
            Assert.Equal(expectedInsertedModule, viewModel.InsertedModule);
        }

        [Fact]
        public void DataTypeProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var viewModel = new ApplicationSettingsCreateViewModel();
            var expectedDataType = ApplicationSettingsDataType.String;

            // Act
            viewModel.DataType = expectedDataType;

            // Assert
            Assert.Equal(expectedDataType, viewModel.DataType);
        }

        [Fact]
        public void ModulesProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var viewModel = new ApplicationSettingsCreateViewModel();
            var expectedModules = new List<Module> { new Module() };

            // Act
            viewModel.Modules = expectedModules;

            // Assert
            Assert.Equal(expectedModules, viewModel.Modules);
        }

        [Fact]
        public void AllApplicationSettingsKeysProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var viewModel = new ApplicationSettingsCreateViewModel();
            var expectedKeys = new List<string> { "Key1", "Key2" };

            // Act
            viewModel.AllApplicationSettingsKeys = expectedKeys;

            // Assert
            Assert.Equal(expectedKeys, viewModel.AllApplicationSettingsKeys);
        }
    }
}

using MainApp.MVC.ViewModels.IntranetPortal.ApplicationSettings;
using SD;
using SD.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppMVCTests.ViewModels.ApplicationSettingsTests
{
    public class ApplicationSettingsEditViewModelTests
    {
        [Fact]
        public void Constructor_ShouldInitializeModules()
        {
            // Arrange & Act
            var viewModel = new ApplicationSettingsEditViewModel();

            // Assert
            Assert.NotNull(viewModel.Modules);
            Assert.Empty(viewModel.Modules);
        }

        [Fact]
        public void KeyProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var viewModel = new ApplicationSettingsEditViewModel();
            var expectedKey = "TestKey";

            // Act
            viewModel.Key = expectedKey;

            // Assert
            Assert.Equal(expectedKey, viewModel.Key);
        }

        [Fact]
        public void KeyProperty_ShouldBeRequired()
        {
            // Arrange
            var viewModel = new ApplicationSettingsEditViewModel();

            // Act
            var validationContext = new ValidationContext(viewModel, null, null);
            var results = new List<ValidationResult>();

            // Assert
            Assert.False(Validator.TryValidateObject(viewModel, validationContext, results, true));
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(viewModel.Key)) && r.ErrorMessage.Contains("required"));
        }

        [Fact]
        public void ValueProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var viewModel = new ApplicationSettingsEditViewModel();
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
            var viewModel = new ApplicationSettingsEditViewModel();
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
            var viewModel = new ApplicationSettingsEditViewModel();
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
            var viewModel = new ApplicationSettingsEditViewModel();
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
            var viewModel = new ApplicationSettingsEditViewModel();
            var expectedModules = new List<Module> { new Module() };

            // Act
            viewModel.Modules = expectedModules;

            // Assert
            Assert.Equal(expectedModules, viewModel.Modules);
        }
    }
}

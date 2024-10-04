using MainApp.MVC.ViewModels.IntranetPortal.LegalLandfillManagement;

namespace Tests.MainAppMVCTests.ViewModels.LegalLandfillManagementTests
{
    public class LegalLandfillWasteTypeViewModelTests
    {
        [Fact]
        public void IdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var id = Guid.NewGuid();
            var viewModel = new LegalLandfillWasteTypeViewModel
            {
                Id = id
            };

            // Act & Assert
            Assert.Equal(id, viewModel.Id);
        }

        [Fact]
        public void NameProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var name = "Hazardous Waste";
            var viewModel = new LegalLandfillWasteTypeViewModel
            {
                Name = name
            };

            // Act & Assert
            Assert.Equal(name, viewModel.Name);
        }

        [Fact]
        public void DescriptionProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var description = "This is a type of hazardous waste.";
            var viewModel = new LegalLandfillWasteTypeViewModel
            {
                Description = description
            };

            // Act & Assert
            Assert.Equal(description, viewModel.Description);
        }

        [Fact]
        public void DescriptionProperty_ShouldBeNullByDefault()
        {
            // Arrange & Act
            var viewModel = new LegalLandfillWasteTypeViewModel();

            // Assert
            Assert.Null(viewModel.Description);
        }
    }

}

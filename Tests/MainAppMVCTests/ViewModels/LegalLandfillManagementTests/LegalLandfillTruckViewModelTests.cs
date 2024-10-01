using MainApp.MVC.ViewModels.IntranetPortal.LegalLandfillManagement;

namespace Tests.MainAppMVCTests.ViewModels.LegalLandfillManagementTests
{
    public class LegalLandfillTruckViewModelTests
    {
        [Fact]
        public void IdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var id = Guid.NewGuid();
            var viewModel = new LegalLandfillTruckViewModel
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
            var name = "Truck A";
            var viewModel = new LegalLandfillTruckViewModel
            {
                Name = name
            };

            // Act & Assert
            Assert.Equal(name, viewModel.Name);
        }

        [Fact]
        public void RegistrationProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var registration = "ABC123";
            var viewModel = new LegalLandfillTruckViewModel
            {
                Registration = registration
            };

            // Act & Assert
            Assert.Equal(registration, viewModel.Registration);
        }

        [Fact]
        public void UnladenWeightProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var unladenWeight = 1200.50;
            var viewModel = new LegalLandfillTruckViewModel
            {
                UnladenWeight = unladenWeight
            };

            // Act & Assert
            Assert.Equal(unladenWeight, viewModel.UnladenWeight);
        }

        [Fact]
        public void PayloadWeightProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var payloadWeight = 1200.50;
            var viewModel = new LegalLandfillTruckViewModel
            {
                PayloadWeight = payloadWeight
            };

            // Act & Assert
            Assert.Equal(payloadWeight, viewModel.PayloadWeight);
        }

        [Fact]
        public void CapacityProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var capacity = 1500.75;
            var viewModel = new LegalLandfillTruckViewModel
            {
                Capacity = capacity
            };

            // Act & Assert
            Assert.Equal(capacity, viewModel.Capacity);
        }

        [Fact]
        public void IsEnabledProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var isEnabled = true;
            var viewModel = new LegalLandfillTruckViewModel
            {
                IsEnabled = isEnabled
            };

            // Act & Assert
            Assert.Equal(isEnabled, viewModel.IsEnabled);
        }

        [Fact]
        public void DescriptionProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var description = "This is a test description for the truck.";
            var viewModel = new LegalLandfillTruckViewModel
            {
                Description = description
            };

            // Act & Assert
            Assert.Equal(description, viewModel.Description);
        }

        [Fact]
        public void IsEnabled_ShouldDefaultToFalse()
        {
            // Arrange & Act
            var viewModel = new LegalLandfillTruckViewModel();

            // Assert
            Assert.False(viewModel.IsEnabled);
        }
    }

}

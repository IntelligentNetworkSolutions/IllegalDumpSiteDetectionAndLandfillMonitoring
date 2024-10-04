using DTOs.MainApp.BL.LegalLandfillManagementDTOs;

namespace Tests.DTOsTests.MainAppBL.LegalLandfillManagementDTOsTests
{
    public class LegalLandfillTruckDTOTests
    {
        [Fact]
        public void DefaultConstructor_InitializesProperties()
        {
            // Arrange
            var truckDTO = new LegalLandfillTruckDTO();

            // Act
            // No action required

            // Assert
            Assert.Equal(Guid.Empty, truckDTO.Id);
            Assert.Null(truckDTO.Name);
            Assert.Null(truckDTO.Registration);
            Assert.Null(truckDTO.UnladenWeight);
            Assert.Null(truckDTO.PayloadWeight);
            Assert.Null(truckDTO.Capacity);
            Assert.True(truckDTO.IsEnabled);
            Assert.Null(truckDTO.Description);
        }

        [Fact]
        public void Properties_CanBeAssignedAndRetrieved()
        {
            // Arrange
            var truckDTO = new LegalLandfillTruckDTO();
            var id = Guid.NewGuid();
            var name = "Test Truck";
            var registration = "ABC123";
            var unladenWeight = 15000.5;
            var payloadWeight = 15000.5;
            var capacity = 20000.0;
            var isEnabled = true;
            var description = "Test Description";

            // Act
            truckDTO.Id = id;
            truckDTO.Name = name;
            truckDTO.Registration = registration;
            truckDTO.UnladenWeight = unladenWeight;
            truckDTO.PayloadWeight = payloadWeight;
            truckDTO.Capacity = capacity;
            truckDTO.IsEnabled = isEnabled;
            truckDTO.Description = description;

            // Assert
            Assert.Equal(id, truckDTO.Id);
            Assert.Equal(name, truckDTO.Name);
            Assert.Equal(registration, truckDTO.Registration);
            Assert.Equal(unladenWeight, truckDTO.UnladenWeight);
            Assert.Equal(payloadWeight, truckDTO.PayloadWeight);
            Assert.Equal(capacity, truckDTO.Capacity);
            Assert.Equal(isEnabled, truckDTO.IsEnabled);
            Assert.Equal(description, truckDTO.Description);
        }

        [Fact]
        public void IsEnabled_DefaultValue_IsEnabled()
        {
            // Arrange
            var truckDTO = new LegalLandfillTruckDTO();

            // Act
            // No action required

            // Assert
            Assert.True(truckDTO.IsEnabled);
        }

        [Fact]
        public void CanUpdate_IsEnabled_Property()
        {
            // Arrange
            var truckDTO = new LegalLandfillTruckDTO();
            var initialIsEnabled = !truckDTO.IsEnabled;

            // Act
            truckDTO.IsEnabled = !initialIsEnabled;

            // Assert
            Assert.NotEqual(initialIsEnabled, truckDTO.IsEnabled);
        }
    }
}

using Entities;
using Entities.LegalLandfillsManagementEntites;

namespace Tests.EntitiesTests.LegalLandfillManagementTests
{
    public class LegalLandfillTruckTests
    {
        [Fact]
        public void LegalLandfillTruck_ShouldInheritFromBaseEntity()
        {
            // Arrange & Act
            var legalLandfillTruck = new LegalLandfillTruck();

            // Assert
            Assert.True(legalLandfillTruck is BaseEntity<Guid>);
        }

        [Fact]
        public void Name_ShouldBeSettableAndGettable()
        {
            // Arrange
            var legalLandfillTruck = new LegalLandfillTruck();
            var expectedName = "Test Legallandfill Truck";

            // Act
            legalLandfillTruck.Name = expectedName;

            // Assert
            Assert.Equal(expectedName, legalLandfillTruck.Name);
        }

        [Fact]
        public void Description_ShouldBeSettableAndGettable()
        {
            // Arrange
            var legalLandfillTruck = new LegalLandfillTruck();
            var expectedDescription = "This is a test landfill truck description.";

            // Act
            legalLandfillTruck.Description = expectedDescription;

            // Assert
            Assert.Equal(expectedDescription, legalLandfillTruck.Description);
        }

        [Fact]
        public void UnladenWeight_ShouldBeSettableAndGettable()
        {
            // Arrange
            var legalLandfillTruck = new LegalLandfillTruck();
            var expectedWeight = 102.2;

            // Act
            legalLandfillTruck.UnladenWeight = expectedWeight;

            // Assert
            Assert.Equal(expectedWeight, legalLandfillTruck.UnladenWeight);
        }

        [Fact]
        public void PayloadWeight_ShouldBeSettableAndGettable()
        {
            // Arrange
            var legalLandfillTruck = new LegalLandfillTruck();
            var expectedWeight = 102.2;

            // Act
            legalLandfillTruck.PayloadWeight = expectedWeight;

            // Assert
            Assert.Equal(expectedWeight, legalLandfillTruck.PayloadWeight);
        }

        [Fact]
        public void Capacity_ShouldBeSettableAndGettable()
        {
            // Arrange
            var legalLandfillTruck = new LegalLandfillTruck();
            var expectedCapacity = 102.2;

            // Act
            legalLandfillTruck.Capacity = expectedCapacity;

            // Assert
            Assert.Equal(expectedCapacity, legalLandfillTruck.Capacity);
        }

        [Fact]
        public void IsEnabled_ShouldBeSettableAndGettable()
        {
            // Arrange
            var legalLandfillTruck = new LegalLandfillTruck();
            var isEnabled = true;

            // Act
            legalLandfillTruck.IsEnabled = isEnabled;

            // Assert
            Assert.Equal(isEnabled, legalLandfillTruck.IsEnabled);
        }

        [Fact]
        public void Registration_ShouldBeSettableAndGettable()
        {
            // Aranage
            var legalLandfillTruck = new LegalLandfillTruck();
            var registration = "Test Registration";

            // Act
            legalLandfillTruck.Registration = registration;

            // Assert
            Assert.Equal(registration, legalLandfillTruck.Registration);

        }
    }
}

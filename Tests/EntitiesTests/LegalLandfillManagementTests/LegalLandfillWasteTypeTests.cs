using Entities.LegalLandfillsManagementEntites;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.EntitiesTests.LegalLandfillManagementTests
{
    public class LegalLandfillWasteTypeTests
    {
        [Fact]
        public void LegalLandfillWasteType_ShouldInheritFromBaseEntity()
        {
            // Arrange & Act
            var legalLandfillWasteType = new LegalLandfillWasteType();

            // Assert
            Assert.True(legalLandfillWasteType is BaseEntity<Guid>);
        }

        [Fact]
        public void Name_ShouldBeSettableAndGettable()
        {
            // Arrange
            var legalLandfillWasteType = new LegalLandfillWasteType();
            var expectedName = "Test WasteType";

            // Act
            legalLandfillWasteType.Name = expectedName;

            // Assert
            Assert.Equal(expectedName, legalLandfillWasteType.Name);
        }

        [Fact]
        public void Description_ShouldBeSettableAndGettable()
        {
            // Arrange
            var legalLandfillWasteType = new LegalLandfillWasteType();
            var expectedDescription = "This is a test landfill waste type description.";

            // Act
            legalLandfillWasteType.Description = expectedDescription;

            // Assert
            Assert.Equal(expectedDescription, legalLandfillWasteType.Description);
        }

    }
}

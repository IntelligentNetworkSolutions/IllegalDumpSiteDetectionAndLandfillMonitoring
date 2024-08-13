using Entities.LegalLandfillsManagementEntites;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.EntitiesTests.LegalLandfillManagementTests
{
    public class LegalLandfillTests
    {
        [Fact]
        public void LegalLandfill_ShouldInheritFromBaseEntity()
        {
            // Arrange & Act
            var legalLandfill = new LegalLandfill();

            // Assert
            Assert.True(legalLandfill is BaseEntity<Guid>);
        }

        [Fact]
        public void Name_ShouldBeSettableAndGettable()
        {
            // Arrange
            var legalLandfill = new LegalLandfill();
            var expectedName = "Test Landfill";

            // Act
            legalLandfill.Name = expectedName;

            // Assert
            Assert.Equal(expectedName, legalLandfill.Name);
        }

        [Fact]
        public void Description_ShouldBeSettableAndGettable()
        {
            // Arrange
            var legalLandfill = new LegalLandfill();
            var expectedDescription = "This is a test landfill description.";

            // Act
            legalLandfill.Description = expectedDescription;

            // Assert
            Assert.Equal(expectedDescription, legalLandfill.Description);
        }

        [Fact]
        public void LegalLandfillPointCloudFiles_ShouldBeNullByDefault()
        {
            // Arrange & Act
            var legalLandfill = new LegalLandfill();

            // Assert
            Assert.Null(legalLandfill.LegalLandfillPointCloudFiles);
        }

        [Fact]
        public void LegalLandfillPointCloudFiles_ShouldBeSettableAndGettable()
        {
            // Arrange
            var legalLandfill = new LegalLandfill();
            var pointCloudFiles = new List<LegalLandfillPointCloudFile>();

            // Act
            legalLandfill.LegalLandfillPointCloudFiles = pointCloudFiles;

            // Assert
            Assert.Equal(pointCloudFiles, legalLandfill.LegalLandfillPointCloudFiles);
        }
    }
}

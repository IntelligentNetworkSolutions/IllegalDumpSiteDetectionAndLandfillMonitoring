using DTOs.MainApp.BL.LegalLandfillManagementDTOs;

namespace Tests.DTOsTests.MainAppBL.LegalLandfillManagementDTOsTests
{
    public class LegalLandfillWasteTypeDTOTests
    {
        [Fact]
        public void DefaultConstructor_InitializesProperties()
        {
            // Arrange
            var wasteTypeDTO = new LegalLandfillWasteTypeDTO();

            // Act
            var id = wasteTypeDTO.Id;
            var name = wasteTypeDTO.Name;
            var description = wasteTypeDTO.Description;

            // Assert
            Assert.Equal(default, id);
            Assert.Null(name);
            Assert.Null(description);
        }

        [Fact]
        public void Properties_CanBeAssignedAndRetrieved()
        {
            // Arrange
            var wasteTypeDTO = new LegalLandfillWasteTypeDTO();
            var id = Guid.NewGuid();
            var name = "Test Waste Type";
            var description = "Test Description";

            // Act
            wasteTypeDTO.Id = id;
            wasteTypeDTO.Name = name;
            wasteTypeDTO.Description = description;

            // Assert
            Assert.Equal(id, wasteTypeDTO.Id);
            Assert.Equal(name, wasteTypeDTO.Name);
            Assert.Equal(description, wasteTypeDTO.Description);
        }
    }
}

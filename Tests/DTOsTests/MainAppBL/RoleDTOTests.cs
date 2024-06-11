using DTOs.MainApp.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DTOsTests.MainAppBL
{
    public class RoleDTOTests
    {
        [Fact]
        public void RoleDTO_Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var id = "role123";
            var name = "Role Name";
            var normalizedName = "ROLE NAME";

            // Act
            var dto = new RoleDTO
            {
                Id = id,
                Name = name,
                NormalizedName = normalizedName
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(name, dto.Name);
            Assert.Equal(normalizedName, dto.NormalizedName);
        }

        [Fact]
        public void RoleDTO_DefaultConstructor_ShouldInitializePropertiesToDefaultValues()
        {
            // Act
            var dto = new RoleDTO();

            // Assert
            Assert.Null(dto.Id);
            Assert.Null(dto.Name);
            Assert.Null(dto.NormalizedName);
        }
               
        [Fact]
        public void RoleDTO_Properties_ShouldBeSettable()
        {
            // Arrange
            var dto = new RoleDTO();

            // Act
            dto.Id = "role456";
            dto.Name = "New Role Name";
            dto.NormalizedName = "NEW ROLE NAME";

            // Assert
            Assert.Equal("role456", dto.Id);
            Assert.Equal("New Role Name", dto.Name);
            Assert.Equal("NEW ROLE NAME", dto.NormalizedName);
        }

        [Fact]
        public void RoleDTO_NullableProperties_ShouldAcceptNullValues()
        {
            // Arrange
            var dto = new RoleDTO
            {
                Name = null,
                NormalizedName = null
            };

            // Assert
            Assert.Null(dto.Name);
            Assert.Null(dto.NormalizedName);
        }
    }
}

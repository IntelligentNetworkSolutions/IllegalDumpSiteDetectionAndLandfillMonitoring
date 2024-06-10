using DTOs.MainApp.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DTOsTests.MainAppBL
{
    public class UserRoleDTOTests
    {
        [Fact]
        public void UserRoleDTO_Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var userId = "user123";
            var roleId = "role123";

            // Act
            var dto = new UserRoleDTO
            {
                UserId = userId,
                RoleId = roleId
            };

            // Assert
            Assert.Equal(userId, dto.UserId);
            Assert.Equal(roleId, dto.RoleId);
        }

        [Fact]
        public void UserRoleDTO_DefaultConstructor_ShouldInitializePropertiesToDefaultValues()
        {
            // Act
            var dto = new UserRoleDTO();

            // Assert
            Assert.Null(dto.UserId);
            Assert.Null(dto.RoleId);
        }

        [Fact]
        public void UserRoleDTO_Properties_ShouldBeSettable()
        {
            // Arrange
            var dto = new UserRoleDTO();

            // Act
            dto.UserId = "user123";
            dto.RoleId = "role123";

            // Assert
            Assert.Equal("user123", dto.UserId);
            Assert.Equal("role123", dto.RoleId);
        }
    }
}

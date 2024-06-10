using DTOs.MainApp.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DTOsTests.MainAppBL
{
    public class UserClaimDTOTests
    {
        [Fact]
        public void UserClaimDTO_Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var id = 1;
            var userId = "user123";
            var claimType = "Role";
            var claimValue = "Admin";

            // Act
            var dto = new UserClaimDTO
            {
                Id = id,
                UserId = userId,
                ClaimType = claimType,
                ClaimValue = claimValue
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(userId, dto.UserId);
            Assert.Equal(claimType, dto.ClaimType);
            Assert.Equal(claimValue, dto.ClaimValue);
        }

        [Fact]
        public void UserClaimDTO_DefaultConstructor_ShouldInitializePropertiesToDefaultValues()
        {
            // Act
            var dto = new UserClaimDTO();

            // Assert
            Assert.Equal(0, dto.Id);
            Assert.Null(dto.UserId);
            Assert.Null(dto.ClaimType);
            Assert.Null(dto.ClaimValue);
        }
               
        [Fact]
        public void UserClaimDTO_Properties_ShouldBeSettable()
        {
            // Arrange
            var dto = new UserClaimDTO();

            // Act
            dto.Id = 1;
            dto.UserId = "user123";
            dto.ClaimType = "Role";
            dto.ClaimValue = "Admin";

            // Assert
            Assert.Equal(1, dto.Id);
            Assert.Equal("user123", dto.UserId);
            Assert.Equal("Role", dto.ClaimType);
            Assert.Equal("Admin", dto.ClaimValue);
        }
       
    }
}

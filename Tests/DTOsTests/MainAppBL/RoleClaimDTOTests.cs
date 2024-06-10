using DTOs.MainApp.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DTOsTests.MainAppBL
{
    public class RoleClaimDTOTests
    {
        [Fact]
        public void RoleClaimDTO_Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var id = 1;
            var roleId = "role123";
            var claimType = "type1";
            var claimValue = "value1";

            // Act
            var dto = new RoleClaimDTO
            {
                Id = id,
                RoleId = roleId,
                ClaimType = claimType,
                ClaimValue = claimValue
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(roleId, dto.RoleId);
            Assert.Equal(claimType, dto.ClaimType);
            Assert.Equal(claimValue, dto.ClaimValue);
        }

        [Fact]
        public void RoleClaimDTO_DefaultConstructor_ShouldInitializePropertiesToDefaultValues()
        {
            // Act
            var dto = new RoleClaimDTO();

            // Assert
            Assert.Equal(0, dto.Id);
            Assert.Null(dto.RoleId);
            Assert.Null(dto.ClaimType);
            Assert.Null(dto.ClaimValue);
        }

        [Fact]
        public void RoleClaimDTO_Properties_ShouldBeSettable()
        {
            // Arrange
            var dto = new RoleClaimDTO();

            // Act
            dto.Id = 2;
            dto.RoleId = "role456";
            dto.ClaimType = "type2";
            dto.ClaimValue = "value2";

            // Assert
            Assert.Equal(2, dto.Id);
            Assert.Equal("role456", dto.RoleId);
            Assert.Equal("type2", dto.ClaimType);
            Assert.Equal("value2", dto.ClaimValue);
        }

        [Fact]
        public void RoleClaimDTO_NullableProperties_ShouldAcceptNullValues()
        {
            // Arrange
            var dto = new RoleClaimDTO
            {
                ClaimType = null,
                ClaimValue = null
            };

            // Assert
            Assert.Null(dto.ClaimType);
            Assert.Null(dto.ClaimValue);
        }
    }
}

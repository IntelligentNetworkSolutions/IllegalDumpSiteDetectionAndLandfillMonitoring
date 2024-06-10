using DTOs.MainApp.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DTOsTests.MainAppBL
{
    public class ClaimDTOTests
    {
        [Fact]
        public void ClaimDTO_Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var claimType = "TestClaimType";
            var claimValue = "TestClaimValue";

            // Act
            var dto = new ClaimDTO
            {
                ClaimType = claimType,
                ClaimValue = claimValue
            };

            // Assert
            Assert.Equal(claimType, dto.ClaimType);
            Assert.Equal(claimValue, dto.ClaimValue);
        }

        [Fact]
        public void ClaimDTO_DefaultConstructor_ShouldInitializePropertiesToDefaultValues()
        {
            // Act
            var dto = new ClaimDTO();

            // Assert
            Assert.Null(dto.ClaimType);
            Assert.Null(dto.ClaimValue);
        }
      
        [Fact]
        public void ClaimDTO_Properties_ShouldBeInitializedWithProvidedValues()
        {
            // Arrange
            var dto = new ClaimDTO
            {
                ClaimType = "AnotherClaimType",
                ClaimValue = "AnotherClaimValue"
            };

            // Assert
            Assert.Equal("AnotherClaimType", dto.ClaimType);
            Assert.Equal("AnotherClaimValue", dto.ClaimValue);
        }
    }
}

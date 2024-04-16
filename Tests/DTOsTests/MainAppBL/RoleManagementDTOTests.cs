using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.MainApp.BL;
using SD;

namespace Tests.DTOsTests.MainAppBL
{
    public class RoleManagementDTOTests
    {
        [Fact]
        public void Constructor_Initialization()
        {
            // Arrange & Act
            var roleManagementDto = new RoleManagementDTO();

            // Assert
            Assert.NotNull(roleManagementDto.Claims);
            Assert.NotNull(roleManagementDto.ClaimsInsert);

            Assert.IsType<List<AuthClaim>>(roleManagementDto.Claims);
            Assert.IsType<List<string?>>(roleManagementDto.ClaimsInsert);

            Assert.Empty(roleManagementDto.Claims);
            Assert.Empty(roleManagementDto.ClaimsInsert);
        }
    }
}

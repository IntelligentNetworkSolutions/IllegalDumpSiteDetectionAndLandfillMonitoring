using AutoMapper;
using DTOs.MainApp.BL;
using MainApp.MVC.Mappers;
using MainApp.MVC.ViewModels.IntranetPortal.UserManagement;
using SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppMVCTests.Mappers
{
    public class UserManagementProfileTests
    {
        private readonly IMapper _mapper;

        public UserManagementProfileTests()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<UserManagementProfile>();
            });
            _mapper = configuration.CreateMapper();
        }

        [Fact]
        public void CreateMap_UserManagementDTO_To_UserManagementCreateUserViewModel_ShouldBeValid()
        {
            // Arrange
            var userManagementDto = new UserManagementDTO();

            // Act
            var createUserViewModel = _mapper.Map<UserManagementCreateUserViewModel>(userManagementDto);

            // Assert
            Assert.NotNull(createUserViewModel);
        }

        [Fact]
        public void CreateMap_UserManagementDTO_To_UserManagementEditUserViewModel_ShouldBeValid()
        {
            // Arrange
            var userManagementDto = new UserManagementDTO();

            // Act
            var editUserViewModel = _mapper.Map<UserManagementEditUserViewModel>(userManagementDto);

            // Assert
            Assert.NotNull(editUserViewModel);
        }

        [Fact]
        public void CreateMap_AuthClaim_To_RoleClaimDTO_ShouldBeValid()
        {
            // Arrange
            var authClaim = new AuthClaim { Value = "claimType", Description = "claimValue" };

            // Act
            var roleClaimDto = _mapper.Map<RoleClaimDTO>(authClaim);

            // Assert
            Assert.Equal(authClaim.Value, roleClaimDto.ClaimType);
            Assert.Equal(authClaim.Description, roleClaimDto.ClaimValue);
        }

        [Fact]
        public void CreateMap_AuthClaim_To_UserClaimDTO_ShouldBeValid()
        {
            // Arrange
            var authClaim = new AuthClaim { Value = "claimType", Description = "claimValue" };

            // Act
            var userClaimDto = _mapper.Map<UserClaimDTO>(authClaim);

            // Assert
            Assert.Equal(authClaim.Value, userClaimDto.ClaimType);
            Assert.Equal(authClaim.Description, userClaimDto.ClaimValue);
        }
    }
}

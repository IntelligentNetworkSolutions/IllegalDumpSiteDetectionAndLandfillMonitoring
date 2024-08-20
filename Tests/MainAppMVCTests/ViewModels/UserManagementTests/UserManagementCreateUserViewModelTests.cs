using MainApp.MVC.ViewModels.IntranetPortal.UserManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppMVCTests.ViewModels.UserManagementTests
{
    public class UserManagementCreateUserViewModelTests
    {
        [Fact]
        public void RolesProperty_ShouldInitializeAsEmptyList()
        {
            // Arrange & Act
            var viewModel = new UserManagementCreateUserViewModel();

            // Act & Assert
            Assert.NotNull(viewModel.Roles);
            Assert.Empty(viewModel.Roles);
        }

        [Fact]
        public void RolesInsertProperty_ShouldInitializeAsEmptyList()
        {
            // Arrange & Act
            var viewModel = new UserManagementCreateUserViewModel();

            // Act & Assert
            Assert.NotNull(viewModel.RolesInsert);
            Assert.Empty(viewModel.RolesInsert);
        }

        [Fact]
        public void ClaimsProperty_ShouldInitializeAsEmptyList()
        {
            // Arrange & Act
            var viewModel = new UserManagementCreateUserViewModel();

            // Act & Assert
            Assert.NotNull(viewModel.Claims);
            Assert.Empty(viewModel.Claims);
        }

        [Fact]
        public void RoleClaimsProperty_ShouldInitializeAsEmptyList()
        {
            // Arrange & Act
            var viewModel = new UserManagementCreateUserViewModel();

            // Act & Assert
            Assert.NotNull(viewModel.RoleClaims);
            Assert.Empty(viewModel.RoleClaims);
        }

        [Fact]
        public void ClaimsInsertProperty_ShouldInitializeAsEmptyList()
        {
            // Arrange & Act
            var viewModel = new UserManagementCreateUserViewModel();

            // Act & Assert
            Assert.NotNull(viewModel.ClaimsInsert);
            Assert.Empty(viewModel.ClaimsInsert);
        }

        [Fact]
        public void PasswordProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var password = "TestPassword123!";
            var viewModel = new UserManagementCreateUserViewModel
            {
                Password = password
            };

            // Act & Assert
            Assert.Equal(password, viewModel.Password);
        }

        [Fact]
        public void ConfirmPasswordProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var confirmPassword = "TestPassword123!";
            var viewModel = new UserManagementCreateUserViewModel
            {
                ConfirmPassword = confirmPassword
            };

            // Act & Assert
            Assert.Equal(confirmPassword, viewModel.ConfirmPassword);
        }

        [Fact]
        public void EmailProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var email = "test@example.com";
            var viewModel = new UserManagementCreateUserViewModel
            {
                Email = email
            };

            // Act & Assert
            Assert.Equal(email, viewModel.Email);
        }

        [Fact]
        public void FirstNameProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var firstName = "John";
            var viewModel = new UserManagementCreateUserViewModel
            {
                FirstName = firstName
            };

            // Act & Assert
            Assert.Equal(firstName, viewModel.FirstName);
        }

        [Fact]
        public void LastNameProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var lastName = "Doe";
            var viewModel = new UserManagementCreateUserViewModel
            {
                LastName = lastName
            };

            // Act & Assert
            Assert.Equal(lastName, viewModel.LastName);
        }

        [Fact]
        public void PhoneNumberProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var phoneNumber = "123456789";
            var viewModel = new UserManagementCreateUserViewModel
            {
                PhoneNumber = phoneNumber
            };

            // Act & Assert
            Assert.Equal(phoneNumber, viewModel.PhoneNumber);
        }

        [Fact]
        public void UserNameProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var userName = "testuser";
            var viewModel = new UserManagementCreateUserViewModel
            {
                UserName = userName
            };

            // Act & Assert
            Assert.Equal(userName, viewModel.UserName);
        }
      
        
    }
}

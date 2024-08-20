using MainApp.MVC.ViewModels.IntranetPortal.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppMVCTests.ViewModels.UserManagementTests
{
    public class UserManagementUserViewModelTests
    {
        [Fact]
        public void RolesProperty_ShouldInitializeAsEmptyList()
        {
            // Arrange & Act
            var viewModel = new UserManagementUserViewModel();

            // Act & Assert
            Assert.NotNull(viewModel.Roles);
            Assert.Empty(viewModel.Roles);
        }

        [Fact]
        public void IdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var id = "12345";
            var viewModel = new UserManagementUserViewModel
            {
                Id = id
            };

            // Act & Assert
            Assert.Equal(id, viewModel.Id);
        }

        [Fact]
        public void UserNameProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var userName = "testuser";
            var viewModel = new UserManagementUserViewModel
            {
                UserName = userName
            };

            // Act & Assert
            Assert.Equal(userName, viewModel.UserName);
        }

        [Fact]
        public void FirstNameProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var firstName = "John";
            var viewModel = new UserManagementUserViewModel
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
            var viewModel = new UserManagementUserViewModel
            {
                LastName = lastName
            };

            // Act & Assert
            Assert.Equal(lastName, viewModel.LastName);
        }

        [Fact]
        public void IsActiveProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var isActive = true;
            var viewModel = new UserManagementUserViewModel
            {
                IsActive = isActive
            };

            // Act & Assert
            Assert.Equal(isActive, viewModel.IsActive);
        }

        [Fact]
        public void EmailProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var email = "test@example.com";
            var viewModel = new UserManagementUserViewModel
            {
                Email = email
            };

            // Act & Assert
            Assert.Equal(email, viewModel.Email);
        }

        [Fact]
        public void PhoneNumberProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var phoneNumber = "123-456-7890";
            var viewModel = new UserManagementUserViewModel
            {
                PhoneNumber = phoneNumber
            };

            // Act & Assert
            Assert.Equal(phoneNumber, viewModel.PhoneNumber);
        }
    }
}

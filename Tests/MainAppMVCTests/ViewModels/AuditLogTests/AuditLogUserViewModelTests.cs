using MainApp.MVC.ViewModels.IntranetPortal.AuditLog;

namespace Tests.MainAppMVCTests.ViewModels.AuditLogTests
{
    public class AuditLogUserViewModelTests
    {
        [Fact]
        public void UsernameProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var username = "user1";
            var viewModel = new AuditLogUserViewModel { Username = username };

            // Act & Assert
            Assert.Equal(username, viewModel.Username);
        }

        [Fact]
        public void FullNameProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var fullName = "John Doe";
            var viewModel = new AuditLogUserViewModel { FullName = fullName };

            // Act & Assert
            Assert.Equal(fullName, viewModel.FullName);
        }
    }
}

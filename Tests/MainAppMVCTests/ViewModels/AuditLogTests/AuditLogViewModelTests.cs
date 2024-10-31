using MainApp.MVC.ViewModels.IntranetPortal.AuditLog;

namespace Tests.MainAppMVCTests.ViewModels.AuditLogTests
{
    public class AuditLogViewModelTests
    {
        [Fact]
        public void InternalUsersListProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var internalUsersList = new List<AuditLogUserViewModel> { new AuditLogUserViewModel { Username = "user1", FullName = "John Doe" } };
            var viewModel = new AuditLogViewModel { InternalUsersList = internalUsersList };

            // Act & Assert
            Assert.Equal(internalUsersList, viewModel.InternalUsersList);
        }

        [Fact]
        public void AuditActionsListProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var auditActionsList = new List<string> { "Create", "Update" };
            var viewModel = new AuditLogViewModel { AuditActionsList = auditActionsList };

            // Act & Assert
            Assert.Equal(auditActionsList, viewModel.AuditActionsList);
        }
    }
}

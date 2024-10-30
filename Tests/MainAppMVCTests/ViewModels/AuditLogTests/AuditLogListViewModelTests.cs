using MainApp.MVC.ViewModels.IntranetPortal.AuditLog;

namespace Tests.MainAppMVCTests.ViewModels.AuditLogTests
{
    public class AuditLogListViewModelTests
    {
        [Fact]
        public void AuditLogIdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var auditLogId = 12345L;
            var viewModel = new AuditLogListViewModel { AuditLogId = auditLogId };

            // Act & Assert
            Assert.Equal(auditLogId, viewModel.AuditLogId);
        }

        [Fact]
        public void AuditDataProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var auditData = "Sample audit data";
            var viewModel = new AuditLogListViewModel { AuditData = auditData };

            // Act & Assert
            Assert.Equal(auditData, viewModel.AuditData);
        }

        [Fact]
        public void AuditActionProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var auditAction = "Create";
            var viewModel = new AuditLogListViewModel { AuditAction = auditAction };

            // Act & Assert
            Assert.Equal(auditAction, viewModel.AuditAction);
        }

        [Fact]
        public void EntityTypeProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var entityType = "User";
            var viewModel = new AuditLogListViewModel { EntityType = entityType };

            // Act & Assert
            Assert.Equal(entityType, viewModel.EntityType);
        }

        [Fact]
        public void AuditDateProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var auditDate = DateTime.UtcNow;
            var viewModel = new AuditLogListViewModel { AuditDate = auditDate };

            // Act & Assert
            Assert.Equal(auditDate, viewModel.AuditDate);
        }

        [Fact]
        public void AuditInternalUserProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var auditInternalUser = "internal_user";
            var viewModel = new AuditLogListViewModel { AuditInternalUser = auditInternalUser };

            // Act & Assert
            Assert.Equal(auditInternalUser, viewModel.AuditInternalUser);
        }

        [Fact]
        public void TablePkProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var tablePk = "12345";
            var viewModel = new AuditLogListViewModel { TablePk = tablePk };

            // Act & Assert
            Assert.Equal(tablePk, viewModel.TablePk);
        }
    }
}

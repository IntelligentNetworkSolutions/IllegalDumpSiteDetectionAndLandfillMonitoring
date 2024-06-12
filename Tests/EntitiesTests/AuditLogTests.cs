using Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.EntitiesTests
{
    public class AuditLogTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var auditData = "Sample audit data";
            var auditAction = "INSERT";
            var entityType = "User";
            var auditDate = DateTime.UtcNow;
            var auditInternalUser = "admin";
            var tablePk = "1";

            // Act
            var auditLog = new AuditLog
            {
                AuditData = auditData,
                AuditAction = auditAction,
                EntityType = entityType,
                AuditDate = auditDate,
                AuditInternalUser = auditInternalUser,
                TablePk = tablePk
            };

            // Assert
            Assert.Equal(auditData, auditLog.AuditData);
            Assert.Equal(auditAction, auditLog.AuditAction);
            Assert.Equal(entityType, auditLog.EntityType);
            Assert.Equal(auditDate, auditLog.AuditDate);
            Assert.Equal(auditInternalUser, auditLog.AuditInternalUser);
            Assert.Equal(tablePk, auditLog.TablePk);
        }

        [Fact]
        public void AuditLogId_ShouldHaveDatabaseGeneratedOptionIdentity()
        {
            // Arrange & Act
            var auditLogIdProperty = typeof(AuditLog).GetProperty("AuditLogId");

            // Assert
            var databaseGeneratedAttribute = (DatabaseGeneratedAttribute)Attribute.GetCustomAttribute(auditLogIdProperty, typeof(DatabaseGeneratedAttribute));
            Assert.NotNull(databaseGeneratedAttribute);
            Assert.Equal(DatabaseGeneratedOption.Identity, databaseGeneratedAttribute.DatabaseGeneratedOption);
        }

        [Fact]
        public void Properties_ShouldInitializeCorrectly()
        {
            // Arrange & Act
            var auditLog = new AuditLog
            {
                AuditData = "Sample audit data",
                AuditAction = "UPDATE",
                EntityType = "Order",
                AuditDate = DateTime.UtcNow,
                AuditInternalUser = "user123",
                TablePk = "42"
            };

            // Assert
            Assert.Equal("Sample audit data", auditLog.AuditData);
            Assert.Equal("UPDATE", auditLog.AuditAction);
            Assert.Equal("Order", auditLog.EntityType);
            Assert.NotEqual(default(DateTime), auditLog.AuditDate);
            Assert.Equal("user123", auditLog.AuditInternalUser);
            Assert.Equal("42", auditLog.TablePk);
        }

    }
}

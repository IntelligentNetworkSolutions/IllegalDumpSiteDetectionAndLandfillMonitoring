using DAL.Interfaces.Repositories;
using Entities;
using MainApp.MVC.Areas.IntranetPortal.Controllers;
using MainApp.MVC.ViewModels.IntranetPortal.AuditLog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Services;

namespace Tests.MainAppMVCTests.Areas.IntranetPortal.Controllers
{
    public class AuditLogsControllerTests
    {
        private readonly Mock<IAuditLogsDa> _mockAuditLogsDa;
        private readonly Mock<IUserManagementDa> _mockUserManagementDa;
        private readonly AuditLogBl _auditLogBl;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly AuditLogsController _controller;

        public AuditLogsControllerTests()
        {
            _mockAuditLogsDa = new Mock<IAuditLogsDa>();
            _mockUserManagementDa = new Mock<IUserManagementDa>();
            _mockConfiguration = new Mock<IConfiguration>();
            _auditLogBl = new AuditLogBl(); // Assuming this has a parameterless constructor or adjust accordingly
            _controller = new AuditLogsController(_mockAuditLogsDa.Object, _auditLogBl, _mockUserManagementDa.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task _AuditLogs_ReturnsPartialView_WithCorrectModel_WhenLogsFound()
        {
            // Arrange
            var internalUsername = "testUser";
            var dateFrom = DateTime.Now.AddDays(-1);
            var dateTo = DateTime.Now;
            var actionType = "Create";
            var type = "Entity";

            var auditLogs = new List<AuditLog>
        {
            new AuditLog
            {
                AuditInternalUser = internalUsername,
                AuditAction = actionType,
                EntityType = type,
                AuditData = "Some data",
                AuditDate = DateTime.Now,
                AuditLogId = 1
            }
        };

            _mockAuditLogsDa.Setup(x => x.GetAllByUsernameFromToDate(internalUsername, dateFrom, dateTo, actionType, type))
                .ReturnsAsync(auditLogs);

            // Act
            var result = await _controller._AuditLogs(internalUsername, dateFrom, dateTo, actionType, type) as PartialViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<PartialViewResult>(result);
            var model = Assert.IsAssignableFrom<List<AuditLogListViewModel>>(result.ViewData.Model);
            Assert.Single(model);
            Assert.Equal(internalUsername, model[0].AuditInternalUser);
        }

        [Fact]
        public async Task _AuditLogs_ReturnsPartialView_WithEmptyModel_WhenNoLogsFound()
        {
            // Arrange
            var internalUsername = "nonExistentUser";
            var dateFrom = DateTime.Now.AddDays(-1);
            var dateTo = DateTime.Now;
            var actionType = "Delete";
            var type = "Entity";

            _mockAuditLogsDa.Setup(x => x.GetAllByUsernameFromToDate(internalUsername, dateFrom, dateTo, actionType, type))
                .ReturnsAsync(new List<AuditLog>());

            // Act
            var result = await _controller._AuditLogs(internalUsername, dateFrom, dateTo, actionType, type) as PartialViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<PartialViewResult>(result);
            var model = Assert.IsAssignableFrom<List<AuditLogListViewModel>>(result.ViewData.Model);
            Assert.Empty(model);
        }

        [Fact]
        public void GetAuditActions_ReturnsListOfActions_WhenActionsExist()
        {
            // Arrange
            var expectedActions = new List<string> { "Create", "Update", "Delete" };
            _mockAuditLogsDa.Setup(x => x.GetAuditActions()).ReturnsAsync(expectedActions);

            // Act
            var result = _controller.GetAuditActions();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedActions, result);
        }

        [Fact]
        public void GetAuditActions_ReturnsEmptyList_WhenNoActionsExist()
        {
            // Arrange
            var expectedActions = new List<string>();
            _mockAuditLogsDa.Setup(x => x.GetAuditActions()).ReturnsAsync(expectedActions);

            // Act
            var result = _controller.GetAuditActions();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task RedirectToUserManagement_ReturnsRedirectToActionResult_WhenUserIsFound()
        {
            // Arrange
            var username = "testUser";
            var user = new ApplicationUser { LastName = "test", FirstName = username }; // Assuming a User class with Id and Username properties

            _mockUserManagementDa.Setup(x => x.GetUserByUsername(username)).ReturnsAsync(user);

            // Act
            var result = await _controller.RedirectToUserManagement(username) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("EditUser", result.ActionName);
            Assert.Equal("UserManagement", result.ControllerName);
            Assert.Equal("IntranetPortal", result.RouteValues["area"]);
            Assert.Equal(user.Id, result.RouteValues["id"]);
        }

        [Fact]
        public async Task GetAuditData_ReturnsAuditLog_WhenLogIsFound()
        {
            // Arrange
            var auditLogId = 1;
            var expectedAuditLog = new AuditLog
            {
                AuditLogId = auditLogId,
                AuditInternalUser = "testUser",
                AuditAction = "Create",
                EntityType = "Entity",
                AuditData = "Some data",
                AuditDate = DateTime.Now
            };

            _mockAuditLogsDa.Setup(x => x.GetAuditData(auditLogId)).ReturnsAsync(expectedAuditLog);

            // Act
            var result = await _controller.GetAuditData(auditLogId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedAuditLog.AuditLogId, result.AuditLogId);
            Assert.Equal(expectedAuditLog.AuditInternalUser, result.AuditInternalUser);
        }

        [Fact]
        public async Task GetAuditData_ReturnsNull_WhenLogIsNotFound()
        {
            // Arrange
            var auditLogId = 999; // Assuming this ID does not exist
            _mockAuditLogsDa.Setup(x => x.GetAuditData(auditLogId)).ReturnsAsync((AuditLog)null);

            // Act
            var result = await _controller.GetAuditData(auditLogId);

            // Assert
            Assert.Null(result);
        }
        [Fact]
        public async Task Index_ReturnsViewWithCorrectModel()
        {
            // Arrange
            var users = new List<ApplicationUser>
        {
            new ApplicationUser { UserName = "testUser1", FirstName = "John", LastName = "Doe" },
            new ApplicationUser { UserName = "testUser2", FirstName = "Jane", LastName = "Smith" }
        };

            _mockUserManagementDa.Setup(x => x.GetAllIntanetPortalUsers()).ReturnsAsync(users);

            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<AuditLogViewModel>(result.ViewData.Model);
            Assert.Equal(2, model.InternalUsersList.Count);
            Assert.Contains(model.InternalUsersList, u => u.Username == "testUser1" && u.FullName == "testUser1 (Doe John)");
            Assert.Contains(model.InternalUsersList, u => u.Username == "testUser2" && u.FullName == "testUser2 (Smith Jane)");
            Assert.Equal(3, model.AuditActionsList.Count); // Assuming "Insert", "Update", "Delete" are added
        }
    }
}

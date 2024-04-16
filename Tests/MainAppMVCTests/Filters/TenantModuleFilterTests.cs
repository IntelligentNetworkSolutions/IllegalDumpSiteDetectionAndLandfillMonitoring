using MainApp.MVC.Filters;
using MainApp.MVC.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Tests.MainAppMVCTests.Filters
{
    public class TenantModuleFilterTests
    {
        [Fact]
        public void OnAuthorization_ConfigurationHasModule_AccessGranted()
        {
            // Arrange
            string moduleName = SD.Modules.UserManagement.Value;

            var configDict = new Dictionary<string, string>
            {
                {"AppSettings:Modules:0", "UserManagement"},
                {"AppSettings:Modules:1", "AuditLog"},
                {"AppSettings:Modules:2", "Admin"},
                {"AppSettings:Modules:3", "SpecialActions"}
            };

            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(configDict);
            var config = configBuilder.Build();

            var modulesAndClaimsHelperMock = new Mock<ModulesAndAuthClaimsHelper>(config);
            //modulesAndClaimsHelperMock.Setup(m => m.HasModule(moduleName));

            var httpContext = new DefaultHttpContext();

            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());

            var filter = new TenantModuleAuthorizationFilter(modulesAndClaimsHelperMock.Object, moduleName);

            // Act
            filter.OnAuthorization(context);

            // Assert
            Assert.Null(context.Result); // No result set means authorization passed
        }

        [Fact]
        public void OnAuthorization_ConfigurationDoeNotHaveModule_AccessDenied()
        {
            // Arrange
            string moduleName = SD.Modules.UserManagement.Value;

            var configDict = new Dictionary<string, string>
            {
                //{"AppSettings:Modules:0", "UserManagement"},
                {"AppSettings:Modules:1", "AuditLog"},
                {"AppSettings:Modules:2", "Admin"},
                {"AppSettings:Modules:3", "SpecialActions"}
            };

            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(configDict);
            var config = configBuilder.Build();

            var modulesAndClaimsHelperMock = new Mock<ModulesAndAuthClaimsHelper>(config);

            var httpContext = new DefaultHttpContext();

            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());

            var filter = new TenantModuleAuthorizationFilter(modulesAndClaimsHelperMock.Object, moduleName);

            // Act
            filter.OnAuthorization(context);

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(context.Result);
            Assert.Equal(403, statusCodeResult.StatusCode); // 403 result set means authorization failed
        }

        [Fact]
        public void OnAuthorization_ConfigurationDoeNotHaveAnyModulesThrowsException_AccessDenied()
        {
            // Arrange
            string moduleName = SD.Modules.UserManagement.Value;

            var configDict = new Dictionary<string, string>
            {
                //{"AppSettings:Modules:0", "UserManagement"},
                //{"AppSettings:Modules:1", "AuditLog"},
                //{"AppSettings:Modules:2", "Admin"},
                //{"AppSettings:Modules:3", "SpecialActions"}
            };

            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(configDict);
            var config = configBuilder.Build();

            var modulesAndClaimsHelperMock = new Mock<ModulesAndAuthClaimsHelper>(config);

            var httpContext = new DefaultHttpContext();

            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());

            var filter = new TenantModuleAuthorizationFilter(modulesAndClaimsHelperMock.Object, moduleName);

            // Act
            filter.OnAuthorization(context);

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(context.Result);
            Assert.Equal(403, statusCodeResult.StatusCode); // 403 result set means authorization failed
        }
    }
}

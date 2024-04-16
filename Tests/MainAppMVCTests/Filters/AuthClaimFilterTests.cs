using System.Security.Claims;
using MainApp.MVC.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace Tests.MainAppMVCTests.Filters
{
    public class AuthClaimFilterTests
    {
        [Fact]
        public void OnAuthorization_UserHasClaim_AccessGranted()
        {
            // Arrange
            string authClaimName = nameof(SD.AuthClaims.UserManagement);
            string authClaimValue = SD.AuthClaims.UserManagement.Value;

            var user = new ClaimsPrincipal();
            user.AddIdentity(new ClaimsIdentity(new[] { new Claim("AuthorizationClaim", authClaimValue) }));

            var httpContext = new DefaultHttpContext() { User = user };
            
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
            
            var filter = new AuthorizeActionFilter(authClaimName);

            // Act
            filter.OnAuthorization(context);

            // Assert
            Assert.Null(context.Result); // No result set means authorization passed
        }

        [Fact]
        public void OnAuthorization_UserDoesNotHaveClaim_AccessDenied()
        {
            // Arrange
            string authClaimName = nameof(SD.AuthClaims.UserManagement);
            var user = new ClaimsPrincipal();
            var httpContext = new DefaultHttpContext() { User = user };

            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());

            var filter = new AuthorizeActionFilter(authClaimName);

            // Act
            filter.OnAuthorization(context);

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(context.Result);
            Assert.Equal(403, statusCodeResult.StatusCode); // 403 result set means authorization failed
        }

        [Fact]
        public void OnAuthorization_CheckForNonListedAllowedUserClaimThrowsException_AccessDenied()
        {
            // Arrange
            string authClaimName = "Just Throw It";
            var user = new ClaimsPrincipal();
            var httpContext = new DefaultHttpContext() { User = user };

            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());

            var filter = new AuthorizeActionFilter(authClaimName);

            // Act
            filter.OnAuthorization(context);

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(context.Result);
            Assert.Equal(403, statusCodeResult.StatusCode); // 403 result set means authorization failed
        }
    }
}

using System.Security.Claims;
using SD;
using SD.Helpers;

namespace Tests.SDTests.Helpers
{
    public class ClaimsPrincipalExtensionsTests
    {
        [Fact]
        public void HasAuthClaim_ReturnsTrueIfUserHasClaim()
        {
            // Arrange
            var user = new ClaimsPrincipal();
            user.AddIdentity(new ClaimsIdentity(new[] { new Claim("SpecialAuthClaim", "superadmin") }));

            // Act
            var result = user.HasAuthClaim(new AuthClaim() { Value = "superadmin" });

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void HasAuthClaim_ReturnsFalseIfUserDoesNotHaveClaim()
        {
            // Arrange
            var user = new ClaimsPrincipal();

            // Act
            var result = user.HasAuthClaim(new AuthClaim() { Value = "superadmin" });

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HasAnyAuthClaim_ReturnsTrueIfUserHasAnyClaim()
        {
            // Arrange
            var user = new ClaimsPrincipal();
            user.AddIdentity(new ClaimsIdentity(new[] { new Claim("SpecialAuthClaim", "superadmin") }));
            user.AddIdentity(new ClaimsIdentity(new[] { new Claim("AuthorizationClaim", "admin") }));

            // Act
            bool result = user.HasAnyAuthClaim(new[] { new AuthClaim() { Value = "superadmin" }, new AuthClaim() { Value = "admin" }});

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void HasAnyAuthClaim_ReturnsFalseIfUserDoesNotHaveAnyClaim()
        {
            // Arrange
            var user = new ClaimsPrincipal();

            // Act
            var result = user.HasAnyAuthClaim(new[] { new AuthClaim() { Value = "superadmin" }, new AuthClaim() { Value = "admin" }});

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HasCustomClaim_ReturnsTrueIfUserHasClaimWithMatchingTypeAndValue()
        {
            // Arrange
            var user = new ClaimsPrincipal();
            user.AddIdentity(new ClaimsIdentity(new[] { new Claim("SpecialAuthClaim", "superadmin") }));

            // Act
            var result = user.HasCustomClaim("SpecialAuthClaim", "superadmin");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void HasCustomClaim_ReturnsFalseIfUserDoesNotHaveClaimWithMatchingTypeAndValue()
        {
            // Arrange
            var user = new ClaimsPrincipal();

            // Act
            var result = user.HasCustomClaim("SpecialAuthClaim", "superadmin");

            // Assert
            Assert.False(result);
        }
    }
}

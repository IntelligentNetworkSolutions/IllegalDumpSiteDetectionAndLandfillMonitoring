using SD;

namespace Tests.SDTests.Classes
{
    public class AuthClaimsTests
    {
        [Fact]
        public void TestAuthClaims_GetAll_NoDuplicates()
        {
            // Arrange
            var claims = AuthClaims.GetAll();

            // Act
            var duplicateValues = claims.GroupBy(c => c.Value).Where(g => g.Count() > 1).Select(g => g.Key);

            // Assert
            Assert.Empty(duplicateValues);
        }

        [Fact]
        public void TestAuthClaims_GetAll_NoInvalidCharacters()
        {
            // Arrange
            var claims = AuthClaims.GetAll();

            // Act
            var claimsWithInvalidCharacters = claims.Where(c => c.Value.Contains(','));

            // Assert
            Assert.Empty(claimsWithInvalidCharacters);
        }

        [Fact]
        public void TestAuthClaims_CheckAuthClaimsValuesForDuplicates_NoDuplicates()
        {
            // Arrange
            var claims = AuthClaims.GetAll();

            // Act
            AuthClaims.CheckAuthClaimsValuesForDuplicates();

            // Assert
            Assert.True(true); // No exception should be thrown
        }

        [Fact]
        public void TestAuthClaims_CheckAuthClaimsForInvalidCharacters_NoInvalidCharacters()
        {
            // Arrange
            var claims = AuthClaims.GetAll();

            // Act
            AuthClaims.CheckAuthClaimsForInvalidCharacters();

            // Assert
            Assert.True(true); // No exception should be thrown
        }

        [Fact]
        public void TestAuthClaim_Value_Unique()
        {
            // Arrange
            var claims = AuthClaims.GetAll();

            // Act
            var duplicateValues = claims.GroupBy(c => c.Value).Where(g => g.Count() > 1).Select(g => g.Key);

            // Assert
            Assert.Empty(duplicateValues);
        }

        [Fact]
        public void TestAuthClaim_Description_NotNull()
        {
            // Arrange
            var claims = AuthClaims.GetAll();

            // Act
            var claimsWithoutDescription = claims.Where(c => c.Description == null);

            // Assert
            Assert.Empty(claimsWithoutDescription);
        }

        [Fact]
        public void TestAuthClaim_FromModule_NotNull()
        {
            // Arrange
            var claims = AuthClaims.GetAll();

            // Act
            var claimsWithoutModule = claims.Where(c => c.FromModule == null);

            // Assert
            Assert.Empty(claimsWithoutModule);
        }
    }
}

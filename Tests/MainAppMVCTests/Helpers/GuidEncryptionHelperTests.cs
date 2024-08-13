using MainApp.MVC.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppMVCTests.Helpers
{
    public class GuidEncryptionHelperTests
    {
        [Fact]
        public void EncryptGuid_ValidGuid_ShouldReturnEncryptedString()
        {
            // Arrange
            Guid guid = Guid.NewGuid();

            // Act
            string encryptedGuid = GuidEncryptionHelper.EncryptGuid(guid);

            // Assert
            Assert.False(string.IsNullOrEmpty(encryptedGuid));
        }

        [Fact]
        public void DecryptGuid_ValidEncryptedGuid_ShouldReturnOriginalGuid()
        {
            // Arrange
            Guid originalGuid = Guid.NewGuid();
            string encryptedGuid = GuidEncryptionHelper.EncryptGuid(originalGuid);

            // Act
            Guid decryptedGuid = GuidEncryptionHelper.DecryptGuid(encryptedGuid);

            // Assert
            Assert.Equal(originalGuid, decryptedGuid);
        }

        [Fact]
        public void EncryptDecryptGuid_RoundTrip_ShouldReturnOriginalGuid()
        {
            // Arrange
            Guid originalGuid = Guid.NewGuid();

            // Act
            string encryptedGuid = GuidEncryptionHelper.EncryptGuid(originalGuid);
            Guid decryptedGuid = GuidEncryptionHelper.DecryptGuid(encryptedGuid);

            // Assert
            Assert.Equal(originalGuid, decryptedGuid);
        }

        [Fact]
        public void DecryptGuid_InvalidEncryptedString_ShouldThrowFormatException()
        {
            // Arrange
            string invalidEncryptedString = "InvalidString";

            // Act & Assert
            Assert.Throws<FormatException>(() => GuidEncryptionHelper.DecryptGuid(invalidEncryptedString));
        }               
       
    }
}

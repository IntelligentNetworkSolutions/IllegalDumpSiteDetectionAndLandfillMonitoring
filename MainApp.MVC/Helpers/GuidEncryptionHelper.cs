using System.Security.Cryptography;
using System.Text;

namespace MainApp.MVC.Helpers
{
    public class GuidEncryptionHelper
    {
        private static readonly string Key = GenerateSecureKey();

        private static string GenerateSecureKey(int keyBytes = 32)
        {
            byte[] tokenData = new byte[keyBytes];
            RandomNumberGenerator.Fill(tokenData);
            return Convert.ToBase64String(tokenData);
        }

        public static string EncryptGuid(Guid guid)
        {
            byte[] guidBytes = guid.ToByteArray();
            using (var aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(Key);
                aes.IV = new byte[16];

                using (var encryptor = aes.CreateEncryptor())
                {
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(guidBytes, 0, guidBytes.Length);
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }

        public static Guid DecryptGuid(string encryptedGuid)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedGuid);
            using (var aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(Key);
                aes.IV = new byte[16];

                using (var decryptor = aes.CreateDecryptor())
                {
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                    return new Guid(decryptedBytes);
                }
            }
        }
    }
}

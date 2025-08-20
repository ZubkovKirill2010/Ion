using System.Security.Cryptography;
using System.Text;

namespace Ion
{
    public static class SecurePasswordStorage
    {
        public static string Encrypt(string plainText)
        {
            byte[] Data = ProtectedData.Protect
            (
                Encoding.UTF8.GetBytes(plainText),
                null,
                DataProtectionScope.CurrentUser
            );
            return Convert.ToBase64String(Data);
        }

        public static string Decrypt(string encryptedText)
        {
            byte[] Data = ProtectedData.Unprotect
            (
                Convert.FromBase64String(encryptedText),
                null,
                DataProtectionScope.CurrentUser
            );
            return Encoding.UTF8.GetString(Data);
        }
    }
}
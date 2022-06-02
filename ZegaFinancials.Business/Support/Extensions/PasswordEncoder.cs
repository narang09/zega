using JetBrains.Annotations;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ZegaFinancials.Business.Support.Extensions
{
    public static class PasswordEncoder
    {
        [NotNull]
        public static string EncryptPassword([NotNull] string password)
        {
            if (password == null) throw new ArgumentNullException("password");

            using (SHA256 algorithm = SHA256.Create())
            {
                Byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                Byte[] hashPasswordPlusPasswordByte = Encoding.UTF8.GetBytes(algorithm.ComputeHash(passwordBytes).ToString() + password);
                return BitConverter.ToString(algorithm.ComputeHash(hashPasswordPlusPasswordByte));
            }
        }
        public static bool ComparePasswords([NotNull] string currentPassword, [NotNull] string newPassword)
        {
            if (currentPassword == null) throw new ArgumentNullException("currentPassword");
            if (newPassword == null) throw new ArgumentNullException("newPassword");

            return currentPassword.Equals(EncryptPassword(newPassword));
        }
    }
    public static class StringEncoderDecoder
    {
        [NotNull]
        public static string Encrypt(string plainText,string encryptionKey)
        {
            if (plainText == null)
                return null;

            // Get the bytes of the string
            var bytesToBeEncrypted = Encoding.UTF8.GetBytes(plainText);
            var passwordBytes = Encoding.UTF8.GetBytes(encryptionKey);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
            var bytesEncrypted = Encrypt(bytesToBeEncrypted, passwordBytes);

            return Convert.ToBase64String(bytesEncrypted).Replace("+","-");
        }
        public static string Decrypt(string encryptedText, string encryptionKey)
        {
            if (encryptedText == null)
                return null;
            //Replacing invalid character
            encryptedText = encryptedText.Replace("-","+");
            // Get the bytes of the string
            var bytesToBeDecrypted = Convert.FromBase64String(encryptedText);
            var passwordBytes = Encoding.UTF8.GetBytes(encryptionKey);

            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            var bytesDecrypted = Decrypt(bytesToBeDecrypted, passwordBytes);

            return Encoding.UTF8.GetString(bytesDecrypted);
        }
        private static byte[] Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using var ms = new MemoryStream();
            using var AES = new RijndaelManaged();
            var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);

            AES.KeySize = 256;
            AES.BlockSize = 128;
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Mode = CipherMode.CBC;
            using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                cs.Close();
            }
            encryptedBytes = ms.ToArray();
            return encryptedBytes;
        }
        private static  byte[] Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using var ms = new MemoryStream();
            using var AES = new RijndaelManaged();
            var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);

            AES.KeySize = 256;
            AES.BlockSize = 128;
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Mode = CipherMode.CBC;

            using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                cs.Close();
            }
            decryptedBytes = ms.ToArray();

            return decryptedBytes;
        }
    }
}

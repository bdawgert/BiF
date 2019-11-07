using System;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace BiF.DAL.Extensions
{
    public static class CryptoTools
    {

        public static string HashValue(this SecureString secureValue) {
            return HashValue(secureValue.Unsceure());
        }

        public static string HashValue(this string value)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(value);
            byte[] hash = new SHA512CryptoServiceProvider().ComputeHash(passwordBytes);
            return Convert.ToBase64String(hash);
        }

        public static string HashValue(this SecureString secureValue, byte[] salt) {
            return HashValue(secureValue.Unsceure(), salt);
        }

        public static string HashValue(this string value, byte[] salt) {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(value);
            byte[] hash = new SHA512CryptoServiceProvider().ComputeHash(salt.Union(passwordBytes).ToArray());
            return Convert.ToBase64String(hash);
        }

        public static byte[] CreateSalt()
        {
            byte[] salt = new byte[32];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider()) {
                rng.GetBytes(salt);
            }

            return salt;

        }

    }
}

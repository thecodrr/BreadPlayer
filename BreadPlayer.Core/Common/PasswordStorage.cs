using System;
using System.Security.Cryptography;
using System.Text;

namespace BreadPlayer.Core.Common
{
    public class PasswordStorage
    {
        // These constants may be changed without breaking existing hashes.
        private const int SaltBytes = 64;

        /// <summary>
        /// Returns the salt and the hash
        /// </summary>
        /// <param name="password">The password that needs to be hashed</param>
        /// <returns>Returns the salt and the hash</returns>
        public static (string Salt, string Hash) CreateHash(string password)
        {
            // Generate a random salt
            byte[] salt = new byte[SaltBytes];

            using (RandomNumberGenerator csprng = RandomNumberGenerator.Create())
            {
                csprng.GetBytes(salt);
            }

            return (Convert.ToBase64String(salt),
                Convert.ToBase64String(ComputeSha512(password, Convert.ToBase64String(salt))));
        }

        /// <summary>
        /// Checks if the password matches the hash
        /// </summary>
        /// <param name="password">The password</param>
        /// <param name="salt">The salt</param>
        /// <param name="hash">The hash</param>
        /// <returns>Returns true is the password matches the hash</returns>
        public static bool VerifyPassword(string password, string salt, string hash)
        {
            return SlowEquals(ComputeSha512(password, salt), Convert.FromBase64String(hash));
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }

        private static byte[] ComputeSha512(string password, string salt)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                return sha512.ComputeHash(Encoding.Unicode.GetBytes(salt + password));
            }
        }
    }
}
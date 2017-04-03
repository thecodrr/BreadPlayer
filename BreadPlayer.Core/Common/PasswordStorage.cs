using System;
using System.Text;
using System.Security.Cryptography;

namespace BreadPlayer.Core.Common
{
    public class PasswordStorage
    {
        // These constants may be changed without breaking existing hashes.
        private const int SALT_BYTES = 24;

        /// <summary>
        /// Returns a Tuple, item1 is the salt and item2 is the hash
        /// </summary>
        /// <param name="password">The password that needs to be hashed</param>
        /// <returns>A Tuple, item1 is the salt and item2 is the hash</returns>
        public static Tuple<string, string> CreateHash(string password)
        {
            // Generate a random salt
            byte[] salt = new byte[SALT_BYTES];

            using (RandomNumberGenerator csprng = RandomNumberGenerator.Create())
            {
                csprng.GetBytes(salt);
            }

            return new Tuple<string, string>(
                Convert.ToBase64String(salt),
                Convert.ToBase64String(ComputeSHA512(password, Convert.ToBase64String(salt))));
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
            return SlowEquals(ComputeSHA512(password, salt), Convert.FromBase64String(hash));
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

        private static byte[] ComputeSHA512(string password, string salt)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                return sha512.ComputeHash(Encoding.Unicode.GetBytes(salt + password));
            }
        }
    }
}

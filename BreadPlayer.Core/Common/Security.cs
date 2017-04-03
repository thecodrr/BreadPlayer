using System.Text;
using System.Security.Cryptography;

namespace BreadPlayer.Common
{
    public static class Security
    {
        public static string ComputeSHA512(string input)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] hash = sha512.ComputeHash(Encoding.Unicode.GetBytes(input));

                StringBuilder sb = new StringBuilder();

                foreach (byte b in hash) sb.AppendFormat("{0:x2}", b);

                return sb.ToString();
            }
        }
    }
}

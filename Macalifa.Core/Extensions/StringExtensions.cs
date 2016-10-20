using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Extensions
{
    public static class StringExtensions
    {
        public static string ToSha1(this string text) => SHA1.Create().ComputeHash(text.ToBytes()).ToHex();
        public static byte[] ToBytes(this string text) => Encoding.UTF8.GetBytes(text);
        public static string ToHex(this byte[] bytes) => BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }
}

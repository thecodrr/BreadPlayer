using System;
using System.Security.Cryptography;
using System.Text;

namespace BreadPlayer.Extensions
{
	public static class StringExtensions
    {
        public static string ToSha1(this string text) => SHA1.Create().ComputeHash(text.ToBytes()).ToHex();
        public static byte[] ToBytes(this string text) => Encoding.UTF8.GetBytes(text);
        public static string ToHex(this byte[] bytes) => BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }
}

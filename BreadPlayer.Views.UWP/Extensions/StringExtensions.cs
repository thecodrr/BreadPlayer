using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace BreadPlayer.Extensions
{
	public static class StringExtensions
    {
        public static string ToSha1(this string text) => SHA1.Create().ComputeHash(text.ToBytes()).ToHex();
        public static byte[] ToBytes(this string text) => Encoding.UTF8.GetBytes(text);
        public static string ToHex(this byte[] bytes) => BitConverter.ToString(bytes).Replace("-", "").ToLower();

        public static bool StartsWithLetter(this string input)
        {
            return Regex.Match(input.Remove(1), "[a-zA-Z]").Success;
        }
        public static bool StartsWithNumber(this string input)
        {
            return Regex.Match(input.Remove(1), "\\d").Success;
        }
        public static bool StartsWithSymbol(this string input)
        {
            return Regex.Match(input.Remove(1), "[^a-zA-Z0-9]").Success;
        }

    }
}

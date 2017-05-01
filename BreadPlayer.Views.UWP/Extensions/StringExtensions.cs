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
            return !string.IsNullOrEmpty(input) && char.IsLetter(input[0]);
        }

        public static bool StartsWithNumber(this string input)
        {
            return !string.IsNullOrEmpty(input) && Regex.IsMatch(input, @"^\d+");
        }

        public static bool StartsWithSymbol(this string input)
        {
            return !string.IsNullOrEmpty(input) && Regex.IsMatch(input, "[^a-zA-Z0-9]");
        }

        public static bool ContainsOnlyNumbers(this string input)
        {
            return !string.IsNullOrEmpty(input) && Regex.IsMatch(input, "\\d");
        }
        public static string ScrubHtml(this string value)
        {
            var step1 = Regex.Replace(value, @"<(.|\n)*?>", "").Trim();
            //var step2 = Regex.Replace(step1, @"\s{2,}", " ");
            return step1;
        }
    }
}

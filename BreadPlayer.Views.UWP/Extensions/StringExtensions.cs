using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BreadPlayer.Extensions
{
    public static class StringExtensions
    {
        public static string GetStringForNullOrEmptyProperty(this string data, string setInstead)
        {
            return string.IsNullOrEmpty(data) ? setInstead : data;
        }

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

        public static string ScrubGarbage(this string value)
        {
            var step1 = Regex.Replace(value, @"(?i)((http|https)\:\/\/)?[a-zA-Z0-9\.\/\?\:@\-_=#]+\.([a-zA-Z0-9\&\.\/\?\:@\-_=#])*(?i)", "").Trim();
            var step2 = Regex.Replace(step1, @"\(\s\)|\(\)|\[\s\]|\[\]|\[\d+\]", "");
            return step2;
        }

        public static string GetTag(this string value)
        {
            var parts = value.Split('-');
            var tag = parts[parts.Length - 1];
            return tag;
        }

        public static string ScrubHtml(this string value)
        {
            var step1 = Regex.Replace(value, @"<(.|\n)*?>", "").Trim();
            //var step2 = Regex.Replace(step1, @"\s{2,}", " ");
            return step1;
        }

        public static async Task<string> ZipAsync(this string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    //msi.CopyTo(gs);
                    await msi.CopyToAsync(gs);
                }

                return Convert.ToBase64String(mso.ToArray());
            }
        }

        public static async Task<string> UnzipAsync(this string base64String)
        {
            if (base64String == null)
                return "";
            using (var msi = new MemoryStream(Convert.FromBase64String(base64String)))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    //gs.CopyTo(mso);
                    await gs.CopyToAsync(mso);
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }
    }
}
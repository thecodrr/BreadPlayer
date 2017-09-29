using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BreadPlayer.Web.NeteaseLyricsAPI
{
    public class Crypto
    {
        // for aes , 第一个 key
        private const string nonce = "0CoJUm6Qyw8W8jud";

        // for aes , 密码向量
        private const string ivString = "0102030405060708";

        // for rsa
        private const string pubKey = "010001";    // e-> ‭65537‬

                                                   // 公钥
        private const string modulus = "00e0b509f6259df8642dbc35662901477df22" +
                "677ec152b5ff68ace615bb7b725152b3ab17a876aea8a5aa76d2e4" +
                "17629ec4ee341f56135fccf695280104e0312ecbda92557c938701" +
                "14af6c9d05c4f7f0c3685b7a46bee255932575cce10b424d813cfe" +
                "4875d3e82047b97ddef52741d546b8e289dc6935b3ece0462db0a22b8e7";

        // for 随机数 [a-zA-Z0-9]
        private const string randomStr = "abcdefghijklmnopqrstuvwxyzABCDEFGHI" +
                "JKLMNOPQRSTUVWXYZ0123456789";

        public static string AddPadding(string encText, string modulus)
        {
            var ml = modulus.Length;
            for (int i = 0; ml > 0 && modulus[i] == '0'; i++) ml--;
            var num = ml - encText.Length;
            var prefix = "";
            for (var i = 0; i < num; i++)
            {
                prefix += '0';
            }
            return prefix + encText;
        }

        public static byte[] ConvertToByteArray(string str, Encoding encoding)
        {
            return encoding.GetBytes(str);
        }

        public static String ToBinary(Byte[] data)
        {
            return string.Join(" ", data.Select(byt => Convert.ToString(byt, 2).PadLeft(8, '0')));
        }

        // Use any sort of encoding you like.
        public static string RsaEncrypt(string text, string exponent, string modulus)
        {
            var rText = "";
            var radix = 16;
            for (var i = text.Length - 1; i >= 0; i--)
                rText += text[i];//reverse text
            var biText = new BigInteger(ToBinary(ConvertToByteArray(rText, Encoding.UTF8)), radix);
            var biEx = new BigInteger(exponent, radix);
            var biMod = new BigInteger(modulus, radix);
            var biRet = biText.modPow(biEx, biMod);
            return AddPadding(biRet.ToString(radix), modulus);
        }

        public static string AESEncrypt(string text, string secKey)
        {
            byte[] iv = Encoding.UTF8.GetBytes(ivString);
            byte[] key = Encoding.UTF8.GetBytes(secKey);
            byte[] src = Encoding.UTF8.GetBytes(text);
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor())
                {
                    byte[] dest = encryptor.TransformFinalBlock(src, 0, src.Length);
                    var com = Convert.ToBase64String(dest);
                    return Convert.ToBase64String(dest);
                }
            }
        }

        public static string CreateSecretKey(int size)
        {
            Random random = new Random();
            string buf = "";
            for (int i = 0; i < size; i++)
            {
                int num = random.Next(randomStr.Length);
                buf += randomStr[num];
            }

            return buf;
        }

        public static object[] AESRSAEncrypt(string text)
        {
            string secKey = CreateSecretKey(16);
            var param = AESEncrypt(AESEncrypt(text, nonce), secKey);
            string enc = RsaEncrypt(secKey, pubKey, modulus);
            return new object[]
            {
                param,
                enc
            };
        }
    }
}
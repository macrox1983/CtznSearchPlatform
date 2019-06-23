using System;
using System.Web;
using System.Text;
using System.Security.Cryptography;

namespace BreakingStorm.Common.Helpers
{
    public class HelperCrypt
    {
        public static string UrlEncode(string str) { return HttpUtility.UrlEncode(str); }
        public static string UrlDecode(string str) { return HttpUtility.UrlDecode(str); }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public static string Base64EncodeFromBytes(byte[] plain)
        {            
            return System.Convert.ToBase64String(plain);
        }
        public static byte[] Base64DecodeToBytes(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return base64EncodedBytes;
        }

        public static string GetMD5(string str)
        {            
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(str));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                //указывает, что нужно преобразовать элемент в шестнадцатиричную строку длиной в два символа
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
                
        public static string GetSHA256(string str)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(str);            
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            string hashString = string.Empty;
            foreach (byte x in hash)
            {
                hashString += String.Format("{0:x2}", x);
            }
            return hashString;            
        }
        public static string GetSHA512(string str)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(str);
            SHA512Managed hashstring = new SHA512Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            string hashString = string.Empty;
            foreach (byte x in hash)
            {
                hashString += String.Format("{0:x2}", x);
            }
            return hashString;
        }


        public static string Win1251ToUTF8(string source)
        {

            Encoding utf8 = Encoding.GetEncoding("utf-8");
            Encoding win1251 = Encoding.GetEncoding("windows-1251");

            byte[] utf8Bytes = win1251.GetBytes(source);
            byte[] win1251Bytes = Encoding.Convert(win1251, utf8, utf8Bytes);
            source = win1251.GetString(win1251Bytes);
            return source;

        }

        public static string DefaultToUTF8(string source)
        {

            Encoding utf8 = Encoding.GetEncoding("utf-8");
            Encoding win1251 = Encoding.UTF8;// .GetEncoding("windows-1251");

            byte[] utf8Bytes = win1251.GetBytes(source);
            byte[] win1251Bytes = Encoding.Convert(win1251, utf8, utf8Bytes);
            source = win1251.GetString(win1251Bytes);
            return source;

        }

        public static byte[] DefaultToUTF8Bytes(string source)
        {

            Encoding utf8 = Encoding.GetEncoding("utf-8");
            Encoding win1251 = Encoding.UTF8;// .GetEncoding("windows-1251");

            byte[] utf8Bytes = win1251.GetBytes(source);
            byte[] win1251Bytes = Encoding.Convert(win1251, utf8, utf8Bytes);
            return win1251Bytes;
        }

        public static string UTF8ToWin1251(string source)
        {
            Encoding utf8 = Encoding.UTF8;
            Encoding win1251 = Encoding.GetEncoding("Windows-1251");

            byte[] utf8Bytes = utf8.GetBytes(source);
            byte[] win1251Bytes = Encoding.Convert(utf8, win1251, utf8Bytes);
            return win1251.GetString(win1251Bytes);
        }
    }
}

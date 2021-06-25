using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace YFPos.Utils
{
    public class EncryptDecryptHelper
    {
        /// <summary>
        /// MD5-32
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetMd5_32byte(string str)
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            var buffers = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            var sb = new StringBuilder("");
            foreach (var b in buffers)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString().ToUpper();
        }

        /// <summary>
        /// 加密 AES-128-CBC
        /// </summary>
        /// <param name="text"></param>
        /// <param name="password"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string AESEncrypt(string text, string password, string iv)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;
            byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] keyBytes = new byte[16];
            int len = pwdBytes.Length;
            if (len > keyBytes.Length) len = keyBytes.Length;
            System.Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            byte[] ivBytes = System.Text.Encoding.UTF8.GetBytes(iv);
            rijndaelCipher.IV = ivBytes;
            ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
            byte[] plainText = Encoding.UTF8.GetBytes(text);
            byte[] cipherBytes = transform.TransformFinalBlock(plainText, 0, plainText.Length);
            return Convert.ToBase64String(cipherBytes);
        }

        /// <summary>
        /// 解密 AES-128-CBC
        /// </summary>
        /// <param name="text"></param>
        /// <param name="password"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string AESDecrypt(string text, string password, string iv)
        {
            try
            {
                //16进制数据转换成byte  
                byte[] encryptedData = Convert.FromBase64String(text);  // strToToHexByte(text);
                RijndaelManaged rijndaelCipher = new RijndaelManaged();
                rijndaelCipher.Key = Encoding.UTF8.GetBytes(password); // Encoding.UTF8.GetBytes(AesKey);
                rijndaelCipher.IV = Encoding.UTF8.GetBytes(iv);// Encoding.UTF8.GetBytes(AesIV); Convert.FromBase64String(iv)
                rijndaelCipher.Mode = CipherMode.CBC;
                rijndaelCipher.Padding = PaddingMode.PKCS7;
                ICryptoTransform transform = rijndaelCipher.CreateDecryptor();
                byte[] plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                string result = Encoding.Default.GetString(plainText);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        ///  AES 加密
        /// </summary>
        /// <param name="str">明文（待加密）</param>
        /// <param name="key">密文</param>
        /// <returns></returns>
        public static string AesEncrypt(string str, string key,string iv)
        {
            if (string.IsNullOrEmpty(str)) return null;
            Byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);
            byte[] ivBytes = System.Text.Encoding.UTF8.GetBytes(iv);
            RijndaelManaged rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                IV = ivBytes
            };

            ICryptoTransform cTransform = rm.CreateEncryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        /// <summary>
        ///  AES 解密
        /// </summary>
        /// <param name="str">明文（待解密）</param>
        /// <param name="key">密文</param>
        /// <returns></returns>
        public static string AesDecrypt(string str, string key,string iv)
        {
            if (string.IsNullOrEmpty(str)) return null;
            Byte[] toEncryptArray = Convert.FromBase64String(str);
            byte[] ivBytes = System.Text.Encoding.UTF8.GetBytes(iv);
            RijndaelManaged rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                IV = ivBytes
            };

            ICryptoTransform cTransform = rm.CreateDecryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }
    }
}

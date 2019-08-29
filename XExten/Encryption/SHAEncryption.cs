using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace XExten.Encryption
{
    /// <summary>
    /// SHA
    /// </summary>
    sealed class SHAEncryption
    {
        /// <summary>
        /// SHA1
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string SHA1Encrypt(string source)
        {
            var bytes = Encoding.Default.GetBytes(source);
            var encryptbytes = SHA1.Create().ComputeHash(bytes);
            return Base64To16(encryptbytes);
        }
        /// <summary>
        /// SHA256
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string SHA256Encrypt(string source)
        {
            var bytes = Encoding.Default.GetBytes(source);
            var encryptbytes = SHA256.Create().ComputeHash(bytes);
            return Base64To16(encryptbytes);
        }
        /// <summary>
        /// SHA384
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string SHA384Encrypt(string source)
        {
            var bytes = Encoding.Default.GetBytes(source);
            var encryptbytes = SHA384.Create().ComputeHash(bytes);
            return Base64To16(encryptbytes);
        }
        /// <summary>
        /// SHA512
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string SHA512Encrypt(string source)
        {
            var bytes = Encoding.Default.GetBytes(source);
            var encryptbytes = SHA512.Create().ComputeHash(bytes);
            return Base64To16(encryptbytes);
        }
        private static string Base64To16(byte[] buffer)
        {
            string md_str = string.Empty;
            for (int i = 0; i < buffer.Length; i++)
            {
                md_str += buffer[i].ToString("x2");
            }
            return md_str;
        }
    }
}

using System;
using System.Security.Cryptography;
using System.Text;

namespace XExten.Encryption
{
    /// <summary>
    /// MD5
    /// </summary>
    internal sealed class MD5Encryption
    {
        /// <summary>
        /// 32位MD5
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static string MD5_32(string Source)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(Source));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                string hash = sBuilder.ToString();
                return hash.ToUpper();
            }
        }

        /// <summary>
        /// 16位MD5
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static string MD5_16(string Source)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(Source));
                //转换成字符串，并取9到25位
                string sBuilder = BitConverter.ToString(data, 4, 8);
                //BitConverter转换出来的字符串会在每个字符中间产生一个分隔符，需要去除掉
                sBuilder = sBuilder.Replace("-", "");
                return sBuilder.ToString().ToUpper();
            }
        }
    }
}
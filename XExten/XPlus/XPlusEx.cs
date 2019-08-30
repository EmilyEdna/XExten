using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using XExten.Encryption;

namespace XExten.XPlus
{
    /// <summary>
    /// Common Extension Class
    /// </summary>
    public class XPlusEx
    {
        #region Func
        /// <summary>
        /// 取一个随机手机号(Return a random phone number)
        /// </summary>
        /// <returns></returns>
        public static String XTel()
        {
            String[] PhonesHost = "134,135,136,137,138,139,150,151,152,157,158,159,130,131,132,155,156,133,153,180,181,182,183,185,186,176,187,188,189,177,178".Split(',');
            Random random = new Random();
            int index = random.Next(0, PhonesHost.Length - 1);
            return PhonesHost[index] + (random.Next(100, 888) + 10000).ToString().Substring(1) + (random.Next(1, 9100) + 10000).ToString().Substring(1);
        }
        /// <summary>
        /// 创建一个验证吗(Create a verification code)
        /// </summary>
        /// <returns></returns>
        public static String XVerifyCode()
        {
            char[] CharArray ={
                '1','2','3','4','5','6','7','8','9',
                'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
                'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
            };
            string randomNum = "";
            int flag = -1;//记录上次随机数的数值，尽量避免产生几个相同的随机数 
            Random rand = new Random();
            for (int i = 0; i < 4; i++)
            {
                if (flag != -1)
                {
                    rand = new Random(i * flag * ((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(60);
                if (flag == t)
                {
                    return XVerifyCode();
                }
                flag = t;
                randomNum += CharArray[t];
            }
            return randomNum;
        }
        /// <summary>
        ///  返回条形码(Return barcode)
        /// </summary>
        /// <param name="Param"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <returns></returns>
        public static String XBarHtml(String Param, int Width, int Height)
        {
            Hashtable Has = new Hashtable();
            #region 39码 12位
            Has.Add('A', "110101001011");
            Has.Add('B', "101101001011");
            Has.Add('C', "110110100101");
            Has.Add('D', "101011001011");
            Has.Add('E', "110101100101");
            Has.Add('F', "101101100101");
            Has.Add('G', "101010011011");
            Has.Add('H', "110101001101");
            Has.Add('I', "101101001101");
            Has.Add('J', "101011001101");
            Has.Add('K', "110101010011");
            Has.Add('L', "101101010011");
            Has.Add('M', "110110101001");
            Has.Add('N', "101011010011");
            Has.Add('O', "110101101001");
            Has.Add('P', "101101101001");
            Has.Add('Q', "101010110011");
            Has.Add('R', "110101011001");
            Has.Add('S', "101101011001");
            Has.Add('T', "101011011001");
            Has.Add('U', "110010101011");
            Has.Add('V', "100110101011");
            Has.Add('W', "110011010101");
            Has.Add('X', "100101101011");
            Has.Add('Y', "110010110101");
            Has.Add('Z', "100110110101");
            Has.Add('0', "101001101101");
            Has.Add('1', "110100101011");
            Has.Add('2', "101100101011");
            Has.Add('3', "110110010101");
            Has.Add('4', "101001101011");
            Has.Add('5', "110100110101");
            Has.Add('6', "101100110101");
            Has.Add('7', "101001011011");
            Has.Add('8', "110100101101");
            Has.Add('9', "101100101101");
            Has.Add('+', "100101001001");
            Has.Add('-', "100101011011");
            Has.Add('*', "100101101101");
            Has.Add('/', "100100101001");
            Has.Add('%', "101001001001");
            Has.Add('$', "100100100101");
            Has.Add('.', "110010101101");
            Has.Add(' ', "100110101101");
            #endregion
            Param = "*" + Param.ToUpper() + "*";
            string Result = "";
            try
            {
                foreach (char ch in Param)
                {
                    Result += Has[ch].ToString();
                    Result += "0";
                }
            }
            catch { return "not supported char!"; }
            string Html = "";
            string Color;
            foreach (char res in Result)
            {
                Color = res == '0' ? "#FFFFFF" : "#000000";
                Html += $"<div style=\"width:{Width}px;height:{Height}px;float:left;background:{Color}\"></div>";
            }
            Html += @"<div style='clear:both'></div>";
            int Len = Has['*'].ToString().Length;
            foreach (char item in Param)
            {
                Html += $"<div style=\"width:{(Width * (Len + 1))}px;float:left;color:#000000;text-align:center;\">{item}</div>";
            }
            Html += @"<div style=clear:both></div>";
            return $"<div style=\"background:#FFFFFF;padding:5px;font-size:{(Width * 5)}px;font-family:楷体;\">{Html}</div>";
        }
        /// <summary>
        /// 将小写的金钱转换成大写的金钱(Convert money into Chinese characters)
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static string XConvertCHN(decimal Param)
        {
            string[] numList = { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
            string[] unitList = { "分", "角", "元", "拾", "佰", "仟", "万", "拾", "佰", "仟", "亿", "拾", "佰", "仟" };
            if (Param == 0) return "零元整";
            StringBuilder strMoney = new StringBuilder();
            string strNum = decimal.Truncate(Param * 100).ToString();
            int len = strNum.Length;
            int zero = 0;
            for (int i = 0; i < len; i++)
            {
                int num = int.Parse(strNum.Substring(i, 1));
                int unitNum = len - i - 1;
                if (num == 0)
                {
                    zero++;
                    if (unitNum == 2 || unitNum == 6 || unitNum == 10)
                    {
                        if (unitNum == 2 || zero < 4)
                            strMoney.Append(unitList[unitNum]);
                        zero = 0;
                    }
                }
                else
                {
                    if (zero > 0)
                    {
                        strMoney.Append(numList[0]);
                        zero = 0;
                    }
                    strMoney.Append(numList[num]);
                    strMoney.Append(unitList[unitNum]);
                }
            }
            if (zero > 0)
                strMoney.Append("整");
            return strMoney.ToString();
        }
        /// <summary>
        /// 过滤字符(Filter characters)
        /// </summary>
        /// <param name="Param"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        public static bool XFilterStr(String Param, Regex regex)
        {
            return regex.IsMatch(Param);
        }
        /// <summary>
        /// 反系列化XML(XmlDeserialize)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Xml"></param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(String Xml)
        {
            using (StringReader reader = new StringReader(Xml))
            {
                return (T)new XmlSerializer(typeof(T)).Deserialize(reader);
            }
        }
        /// <summary>
        /// 将对象序列化为XML(XmlSerializer)
        /// 说明：此方法序列化复杂类，如果没有声明XmlInclude等特性，可能会引发“使用 XmlInclude 或 SoapInclude 特性静态指定非已知的类型。”的错误。
        /// (Description: This method serializes complex classes. If you do not declare features such as XmlInclude, you may get an error "Use the XmlInclude or SoapInclude feature to statically specify a non-known type.")
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static string XmlSerializer<T>(T Param)
        {
            MemoryStream Stream = new MemoryStream();
            XmlSerializer xml = new XmlSerializer(typeof(T));
            try
            {
                //序列化对象
                xml.Serialize(Stream, Param);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            Stream.Position = 0;
            StreamReader reader = new StreamReader(Stream);
            string str = reader.ReadToEnd();
            reader.Dispose();
            Stream.Dispose();
            return str;
        }
        /// <summary>
        /// 计算两点GPS坐标的距离（单位：米）
        /// Calculate the distance between two GPS coordinates (unit: meter)
        /// </summary>
        /// <param name="lat1">第一点的纬度坐标</param>
        /// <param name="lng1">第一点的经度坐标</param>
        /// <param name="lat2">第二点的纬度坐标</param>
        /// <param name="lng2">第二点的经度坐标</param>
        /// <returns></returns>
        public static double XDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double a = (lat1 * Math.PI / 180.00) - (lat2 * Math.PI / 180.00);//两点纬度之差
            double b = (lng1 * Math.PI / 180.00) - (lng2 * Math.PI / 180.00); //经度之差
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(lat1 * Math.PI / 180.00) * Math.Cos(lat2 * Math.PI / 180.00) * Math.Pow(Math.Sin(b / 2), 2)));//计算两点距离的公式
            s *= 6378137.0;//弧长乘地球半径（半径为米）
            return Math.Round(s * 10000d) / 10000d;//精确距离的数值
        }
        /// <summary>
        /// 采用Base64编码(LzString)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string XCompressToBase64(string input)
        {
            return LzStringEncryption.CompressToBase64(input);
        }
        /// <summary>
        /// 解析Base64编码(LzString)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string XDecompressFromBase64(string input)
        {
            return LzStringEncryption.DecompressFromBase64(input);
        }
        /// <summary>
        /// 采用UTF16编码(LzString)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string XCompressToUTF16(string input)
        {
            return LzStringEncryption.CompressToUTF16(input);
        }
        /// <summary>
        /// 解析UTF16编码(LzString)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string XDecompressFromUTF16(string input)
        {
            return LzStringEncryption.DecompressFromUTF16(input);
        }
        /// <summary>
        /// 采用URI编码(LzString)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string XCompressToEncodedURIComponent(string input)
        {
            return LzStringEncryption.CompressToEncodedURIComponent(input);
        }
        /// <summary>
        /// 解析URI编码(LzString)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string XDecompressFromEncodedURIComponent(string input)
        {
            return LzStringEncryption.DecompressFromEncodedURIComponent(input);
        }
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <param name="type">位数：32 16</param>
        /// <returns></returns>
        public static string MD5(string input, int type = 32)
        {
            if (type != 32 && type != 16)
                return "Please enter the MD5 encryption digits,for example：16、32";
            return type == 32 ? MD5Encryption.MD5_32(input) : MD5Encryption.MD5_16(input);
        }
        /// <summary>
        /// SHA加密
        /// </summary>
        /// <param name="input"></param>
        /// <param name="type">位数：1 256 384 512</param>
        /// <returns></returns>
        public static string SHA(string input, int type = 1)
        {
            if (type != 1 && type != 256 && type != 384 && type != 512)
                return "Please enter the number of SHA encryption bits, for example：1、256、384、512";
            switch (type)
            {
                case 1:
                    return SHAEncryption.SHA1Encrypt(input);
                case 256:
                    return SHAEncryption.SHA256Encrypt(input);
                case 384:
                    return SHAEncryption.SHA384Encrypt(input);
                case 512:
                    return SHAEncryption.SHA512Encrypt(input);
                default:
                    return SHAEncryption.SHA1Encrypt(input);
            }
        }
        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RSAEncryp(string input)
        {
            return RSAEncryption.RSAEncrypt(input);
        }
        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RSADecryp(string input)
        {
            return RSAEncryption.RSADecrypt(input);
        }
        #endregion
    }
}
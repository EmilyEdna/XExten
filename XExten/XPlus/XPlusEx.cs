using ProtoBuf;
using QRCoder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using System.Xml.Serialization;
using XExten.Encryption;
using XExten.XCore;

namespace XExten.XPlus
{
    /// <summary>
    /// Common Extension Class
    /// </summary>
    public class XPlusEx
    {
        #region Func

        /// <summary>
        /// 时间戳转时间
        /// </summary>
        /// <param name="TimeStamp"></param>
        /// <returns></returns>
        public static DateTime XConvertStamptime(string TimeStamp)
        {
            DateTime StartTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Utc, TimeZoneInfo.Local);
            TimeSpan Span = new TimeSpan(long.Parse(TimeStamp + "0000000"));
            return StartTime.Add(Span);
        }

        /// <summary>
        /// 时间转时间戳
        /// </summary>
        /// <param name="TimeStamp"></param>
        /// <returns></returns>
        public static string XConvertDateTime(DateTime TimeStamp)
        {
            DateTime StartTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Utc, TimeZoneInfo.Local);
            return (((TimeStamp - StartTime).TotalMilliseconds) / 1000).ToString();
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="Path">网址</param>
        /// <param name="Pixel">像素</param>
        /// <returns></returns>
        public static Bitmap XCreateQRCode(string Path, int Pixel)
        {
            QRCodeGenerator Generator = new QRCodeGenerator();
            QRCodeData CodeData = Generator.CreateQrCode(HttpUtility.UrlEncode(Path), QRCodeGenerator.ECCLevel.Q);
            using QRCode Code = new QRCode(CodeData);
            return Code.GetGraphic(Pixel, Color.Black, Color.White, true);
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="Path">网址</param>
        /// <param name="Pixel">像素</param>
        /// <param name="ImgType">1：PNG，2：JPEG，3：GIF，不在此区间默认PNG</param>
        /// <returns></returns>
        public static string XCreateQRCode(string Path, int Pixel, int ImgType = 1)
        {
            QRCodeGenerator Generator = new QRCodeGenerator();
            QRCodeData CodeData = Generator.CreateQrCode(HttpUtility.UrlEncode(Path), QRCodeGenerator.ECCLevel.Q);
            using Base64QRCode QrCode = new Base64QRCode(CodeData);
            return ImgType switch
            {
                1 => QrCode.GetGraphic(Pixel, Color.Black, Color.White, true),
                2 => QrCode.GetGraphic(Pixel, Color.Black, Color.White, true, Base64QRCode.ImageType.Jpeg),
                3 => QrCode.GetGraphic(Pixel, Color.Black, Color.White, true, Base64QRCode.ImageType.Gif),
                _ => QrCode.GetGraphic(Pixel, Color.Black, Color.White, true),
            };
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="Type">位数：32 16</param>
        /// <returns></returns>
        public static string XMD5(string Input, int Type = 32)
        {
            if (Type != 32 && Type != 16)
                return "Please enter the MD5 encryption digits,for example：16、32";
            return Type == 32 ? MD5Encryption.MD5_32(Input) : MD5Encryption.MD5_16(Input);
        }

        /// <summary>
        /// 使用Protobuf反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Bytes"></param>
        /// <returns></returns>
        public static T XProtobufDeSerialize<T>(byte[] Bytes)
        {
            try
            {
                using (MemoryStream Stream = new MemoryStream())
                {
                    //将消息写入流中
                    Stream.Write(Bytes, 0, Bytes.Length);
                    //将流的位置归0
                    Stream.Position = 0;
                    //使用工具反序列化对象
                    return Serializer.Deserialize<T>(Stream);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 使用Protobuf序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Poco"></param>
        /// <returns></returns>
        public static byte[] XProtobufSerialize<T>(T Poco)
        {
            try
            {
                //涉及格式转换，需要用到流，将二进制序列化到流中
                using (MemoryStream Stream = new MemoryStream())
                {
                    //使用ProtoBuf工具的序列化方法
                    Serializer.Serialize<T>(Stream, Poco);
                    //定义二级制数组，保存序列化后的结果
                    byte[] result = new byte[Stream.Length];
                    //将流的位置设为0，起始点
                    Stream.Position = 0;
                    //将流中的内容读取到二进制数组中
                    Stream.Read(result, 0, result.Length);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 读取XML内容
        /// </summary>
        /// <param name="NodeItem">根节点</param>
        /// <param name="NodeKey">根节点下Key节点</param>
        /// <param name="NodeValue">根节点下Value节点</param>
        public static Dictionary<String, String> XReadXml(string NodeItem = null, string NodeKey = null, string NodeValue = null)
        {
            Dictionary<String, String> XmlMap = new Dictionary<String, String>();
            string XmlPath = Directory.GetDirectories(Directory.GetCurrentDirectory()).Where(t => t.ToLower().Contains("xml")).FirstOrDefault();
            string[] XmlFilePath = Directory.GetFiles(XmlPath, "*.xml");
            XmlFilePath.ToEach<string>(item =>
            {
                XElement XNodes = XElement.Load(item);
                var elements = XNodes.Elements(NodeItem.IsNullOrEmpty() ? "Item" : NodeItem);
                if (elements.Count() > 0)
                {
                    elements.ToEachs<XElement>(element =>
                    {
                        string Key = element.Element(NodeKey.IsNullOrEmpty() ? "Key" : NodeKey).Value;
                        string Value = element.Element(NodeValue.IsNullOrEmpty() ? "Value" : NodeValue).Value;
                        if (!XmlMap.ContainsKey(Key))
                            XmlMap.Add(Key, Value);
                    });
                }
            });
            return XmlMap;
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string XRSADecryp(string Input)
        {
            return RSAEncryption.Instance.RSADecrypt(Input);
        }

        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string XRSAEncryp(string Input)
        {
            return RSAEncryption.Instance.RSAEncrypt(Input);
        }

        /// <summary>
        /// SHA加密
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="Type">位数：1 256 384 512</param>
        /// <returns></returns>
        public static string XSHA(string Input, int Type = 1)
        {
            if (Type != 1 && Type != 256 && Type != 384 && Type != 512)
                return "Please enter the number of SHA encryption bits, for example：1、256、384、512";
            switch (Type)
            {
                case 1:
                    return SHAEncryption.SHA1Encrypt(Input);

                case 256:
                    return SHAEncryption.SHA256Encrypt(Input);

                case 384:
                    return SHAEncryption.SHA384Encrypt(Input);

                case 512:
                    return SHAEncryption.SHA512Encrypt(Input);

                default:
                    return SHAEncryption.SHA1Encrypt(Input);
            }
        }

        /// <summary>
        ///  返回条形码(Return barcode)
        /// </summary>
        /// <param name="Param"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <returns></returns>
        public static string XBarHtml(string Param, int Width, int Height)
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

            #endregion 39码 12位

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
        /// 采用Base64编码(LzString)
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string XCompressToBase64(string Input)
        {
            return LzStringEncryption.CompressToBase64(Input);
        }

        /// <summary>
        /// 采用URI编码(LzString)
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string XCompressToEncodedURIComponent(string Input)
        {
            return LzStringEncryption.CompressToEncodedURIComponent(Input);
        }

        /// <summary>
        /// 采用UTF16编码(LzString)
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string XCompressToUTF16(string Input)
        {
            return LzStringEncryption.CompressToUTF16(Input);
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
        /// 解析Base64编码(LzString)
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string XDecompressFromBase64(string Input)
        {
            return LzStringEncryption.DecompressFromBase64(Input);
        }

        /// <summary>
        /// 解析URI编码(LzString)
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string XDecompressFromEncodedURIComponent(string Input)
        {
            return LzStringEncryption.DecompressFromEncodedURIComponent(Input);
        }

        /// <summary>
        /// 解析UTF16编码(LzString)
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string XDecompressFromUTF16(string Input)
        {
            return LzStringEncryption.DecompressFromUTF16(Input);
        }

        /// <summary>
        /// 计算两点GPS坐标的距离（单位：米）
        /// Calculate the distance between two GPS coordinates (unit: meter)
        /// </summary>
        /// <param name="LatStar">第一点的纬度坐标</param>
        /// <param name="LngStar">第一点的经度坐标</param>
        /// <param name="LatEnd">第二点的纬度坐标</param>
        /// <param name="LngEnd">第二点的经度坐标</param>
        /// <returns></returns>
        public static double XDistance(double LatStar, double LngStar, double LatEnd, double LngEnd)
        {
            double a = (LatStar * Math.PI / 180.00) - (LatEnd * Math.PI / 180.00);//两点纬度之差
            double b = (LngStar * Math.PI / 180.00) - (LngEnd * Math.PI / 180.00); //经度之差
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(LatStar * Math.PI / 180.00) * Math.Cos(LatEnd * Math.PI / 180.00) * Math.Pow(Math.Sin(b / 2), 2)));//计算两点距离的公式
            s *= 6378137.0;//弧长乘地球半径（半径为米）
            return Math.Round(s * 10000d) / 10000d;//精确距离的数值
        }

        /// <summary>
        /// 过滤字符(Filter characters)
        /// </summary>
        /// <param name="Param"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        public static bool XFilterStr(string Param, Regex regex)
        {
            return regex.IsMatch(Param);
        }

        /// <summary>
        /// 反系列化XML(XmlDeserialize)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Xml"></param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(string Xml)
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
        /// 取一个随机手机号(Return a random phone number)
        /// </summary>
        /// <returns></returns>
        public static string XTel()
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
        public static string XVerifyCode()
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
        /// 检查字符串是否有中文字符
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static bool XIsChineseStr(string Param)
        {
            return Param.ToArray().Any(item => (int)item > 127);
        }

        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="Executer"></param>
        /// <param name="Exception"></param>
        /// <param name="Finally"></param>
        public static void XTry(Action Executer, Action<Exception> Exception, Action Finally = null)
        {
            try
            {
                Executer.Invoke();
            }
            catch (Exception ex)
            {
                Exception.Invoke(ex);
            }
            finally
            {
                if (Finally != null) Finally.Invoke();
            }
        }

        /// <summary>
        /// 异常处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Executer"></param>
        /// <param name="Exception"></param>
        /// <param name="Finally"></param>
        /// <returns></returns>
        public static T XTry<T>(Func<T> Executer, Func<Exception, T> Exception, Action Finally = null)
        {
            try
            {
                return Executer.Invoke();
            }
            catch (Exception ex)
            {
                return Exception.Invoke(ex);
            }
            finally
            {
                if (Finally != null) Finally.Invoke();
            }
        }

        /// <summary>
        /// 异常处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Executer"></param>
        /// <param name="Exception"></param>
        /// <param name="Finally"></param>
        /// <returns></returns>
        public static async Task<T> XTry<T>(Func<Task<T>> Executer, Func<Exception, Task<T>> Exception, Action Finally = null) 
        {
            try
            {
                return await Executer.Invoke();
            }
            catch (Exception ex)
            {
                return await Exception.Invoke(ex);
            }
            finally
            {
                if (Finally != null) Finally.Invoke();
            }
        }
        #endregion Func
    }
}
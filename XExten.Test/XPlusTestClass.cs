using XExten.Test.TestModel;
using XExten.XPlus;
using Xunit;
using System;

namespace XExten.Test
{
    public class XPlusTestClass
    {

        [Fact]
        public void XMail_Test()
        {
            var res = XPlus.XPlus.XSendMail(Item =>
            {
                Item.Content = "hello world!";
                Item.AcceptedAddress = "xxx@gmail.com";
                Item.Title = "mail test";
            });
        }

        [Fact]
        public void XConvertDateTime_Test()
        {
            var res = XPlus.XPlus.XConvertDateTime(DateTime.Now);
            var res1 = XPlus.XPlus.XConvertStamptime(res);
        }

        [Fact]
        public void XTel_Test()
        {
            var res = XPlus.XPlus.XTel();
        }

        [Fact]
        public void XVerifyCode_Test()
        {
            var res = XPlus.XPlus.XVerifyCode();
        }

        [Fact]
        public void XBarHtml_Test()
        {
            var res = XPlus.XPlus.XBarHtml("ABC", 3, 50);
        }

        [Fact]
        public void XConvertCHN_Test()
        {
            decimal data = 4.85M;
            var res = XPlus.XPlus.XConvertCHN(data);
        }

        [Fact]
        public void XCompressToBase64AndXDeCompressToBase64_Test()
        {
            string text = "张三!";
            var res = XPlus.XPlus.XCompressToBase64(text);
            string defaults = XPlus.XPlus.XDecompressFromBase64(res);
        }

        [Fact]
        public void XCompressToUTF16AndXDecompressFromUTF16_Test()
        {
            string text = "张三!";
            var res = XPlus.XPlus.XCompressToUTF16(text);
            string defaults = XPlus.XPlus.XDecompressFromUTF16(res);
        }

        [Fact]
        public void XEncodedURIAndXDencodedURI_Test()
        {
            string text = "张三!";
            var res = XPlus.XPlus.XCompressToEncodedURIComponent(text);
            string defaults = XPlus.XPlus.XDecompressFromEncodedURIComponent(res);
        }

        [Fact]
        public void XMD5_Test()
        {
            string text = "张三!";
            var res32 = XPlus.XPlus.XMD5(text);
            var res16 = XPlus.XPlus.XMD5(text, 16);
        }

        [Fact]
        public void XSHA_Test()
        {
            string text = "张三!";
            var res32 = XPlus.XPlus.XSHA(text);
        }

        [Fact]
        public void XProtoBuf_Test()
        {
            TestB t = new TestB() { Account = "张三", Id = 1, Name = "李四" };
            var data = XPlus.XPlus.XProtobufSerialize(t);
            var reslt = XPlus.XPlus.XProtobufDeSerialize<TestB>(data);
        }

        [Fact]
        public void XReadXml_Test()
        {
            var dic = XPlus.XPlus.XReadXml();
        }

        [Fact]
        public void XReadXml_Test2()
        {
            var dic = XPlus.XPlus.XReadXml("Node", "Name", "Email");
        }

        [Fact]
        public void XQrCode_Test()
        {
            var qr = XPlus.XPlus.XCreateQRCode("https://www.google.com", 10);
        }

        [Fact]
        public void XIsChineseCode_Test()
        {
            string text = "张三!";
            var res = XPlus.XPlus.XIsChineseStr(text);
        }
        [Fact]
        public void XTryCatch_Test1()
        {
            XPlus.XPlus.XTry(() => throw new Exception("test exception"), (ex) => Console.WriteLine(ex.Message));
        }

        [Fact]
        public void XTryCatch_Test2()
        {
            XPlus.XPlus.XTry(() =>
            {
                return Convert.ToInt32("q");
            }, (ex) =>
            {
                Console.WriteLine(ex.Message);
                return 1;
            });
        }
    }
}
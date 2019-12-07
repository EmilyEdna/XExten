using XExten.Test.TestModel;
using XExten.XPlus;
using Xunit;

namespace XExten.Test
{
    public class XPlusTestClass
    {
        [Fact]
        public void XTel_Test()
        {
            var res = XPlusEx.XTel();
        }

        [Fact]
        public void XVerifyCode_Test()
        {
            var res = XPlusEx.XVerifyCode();
        }

        [Fact]
        public void XBarHtml_Test()
        {
            var res = XPlusEx.XBarHtml("ABC", 3, 50);
        }

        [Fact]
        public void XConvertCHN_Test()
        {
            decimal data = 4.85M;
            var res = XPlusEx.XConvertCHN(data);
        }

        [Fact]
        public void XCompressToBase64AndXDeCompressToBase64_Test()
        {
            string text = "中国的崛起!";
            var res = XPlusEx.XCompressToBase64(text);
            string defaults = XPlusEx.XDecompressFromBase64(res);
        }

        [Fact]
        public void XCompressToUTF16AndXDecompressFromUTF16_Test()
        {
            string text = "中国的崛起!";
            var res = XPlusEx.XCompressToUTF16(text);
            string defaults = XPlusEx.XDecompressFromUTF16(res);
        }

        [Fact]
        public void XEncodedURIAndXDencodedURI_Test()
        {
            string text = "中国的崛起!";
            var res = XPlusEx.XCompressToEncodedURIComponent(text);
            string defaults = XPlusEx.XDecompressFromEncodedURIComponent(res);
        }

        [Fact]
        public void MD5_Test()
        {
            string text = "中国的崛起!";
            var res32 = XPlusEx.MD5(text);
            var res16 = XPlusEx.MD5(text, 16);
        }

        [Fact]
        public void SHA_Test()
        {
            string text = "中国的崛起!";
            var res32 = XPlusEx.SHA(text);
        }

        [Fact]
        public void ProtoBuf_Test()
        {
            TestB t = new TestB() { Account = "张三", Id = 1, Name = "李四" };
            var data = XPlusEx.ProtobufSerialize(t);
            var reslt = XPlusEx.ProtobufDeSerialize<TestB>(data);
        }

        [Fact]
        public void ReadXml_Test()
        {
            var dic = XPlusEx.ReadXml();
        }

        [Fact]
        public void ReadXml_Test2()
        {
            var dic = XPlusEx.ReadXml("Node", "Name", "Email");
        }

        [Fact]
        public void QrCode_Test()
        {
            var qr = XPlusEx.CreateQRCode("https://www.google.com", 10);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
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
        public void XEncodedURIAndXDencodedURI()
        {
            string text = "中国的崛起!";
            var res = XPlusEx.XCompressToEncodedURIComponent(text);
            string defaults = XPlusEx.XDecompressFromEncodedURIComponent(res);
        }
    }
}

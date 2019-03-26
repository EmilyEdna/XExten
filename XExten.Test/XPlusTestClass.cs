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
        public void XCheckMatch_Test()
        {
            var res = XPlusEx.XCheckMatch("你好", "嗯嗯额");
        }
        [Fact]
        public void XConvertCh_Test()
        {
            var res = XPlusEx.XConvertCh("你好");
        }
        [Fact]
        public void XConvertChFirst_Test()
        {
            var res = XPlusEx.XConvertChFirst("你好");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.SocketProxy.SocketInterface.DefaultImpl
{
    public class SocketResultDefault : ISocketResult
    {
        public string Router { get; set; }
        public string SocketJsonData { get; set; }
        public static SocketResultDefault SetValue(string Router, string SocketJsonData)
        {
            return new SocketResultDefault
            {
                Router = Router,
                SocketJsonData = SocketJsonData
            };
        }
    }
}

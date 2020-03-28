using XExten.SocketProxyServer.MiddleView.ViewInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.SocketProxyServer.MiddleView.ViewImpl
{
    public class SocketResult : ISocketResult
    {
        public string Router { get; set; }
        public string SocketJsonData { get; set; }
    }
}

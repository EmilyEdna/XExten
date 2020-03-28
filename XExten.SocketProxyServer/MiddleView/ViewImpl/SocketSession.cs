using XExten.SocketProxyServer.MiddleView.ViewInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.SocketProxyServer.MiddleView.ViewImpl
{
    public class SocketSession : ISocketSession
    {
        public string PrimaryKey { get; set; }
        public string SessionAccount { get; set; }
        public string SessionRole { get; set; }
        public object CustomizeData { get; set; }
    }
}

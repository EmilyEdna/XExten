using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.SocketProxyServer.MiddleView.ViewInterface
{
    public interface ISocketResult
    {
        string Router { get; set; }
        string SocketJsonData { get; set; }
    }
}

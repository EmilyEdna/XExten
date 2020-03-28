using XExten.SocketProxyServer.MiddleSocket;
using System;

namespace XExten.SocketProxyServer
{
    /// <summary>
    /// 启动
    /// </summary>
    public class Start
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            SocketBasic.Bootstrap();
        }
    }
}
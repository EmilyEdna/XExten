using BeetleX;
using XExten.SocketProxyServer.MiddleConfig;
using XExten.SocketProxyServer.MiddleSocket.Setting;
using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.SocketProxyServer.MiddleSocket
{
    public class SocketBasic
    {
        /// <summary>
        /// 启动Socket服务
        /// </summary>
        public static void Bootstrap()
        {
            IServer Serv = SocketFactory.CreateTcpServer<SocketHandlerBase, SocketPacket>();
            Serv.Setting(option =>
            {
                option.DefaultListen.Host = Configuration.TCP_Host;
                option.DefaultListen.Port = Configuration.TCP_Port;
            });
            Serv.Open();
        }
    }
}

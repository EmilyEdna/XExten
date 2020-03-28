using BeetleX.EventArgs;
using XExten.SocketProxyServer.MiddleView;
using System;
using System.Collections.Generic;
using System.Text;
using XExten.XCore;

namespace XExten.SocketProxyServer.MiddleHandler.IntegrationHandler
{
    public class RequestHandler
    {
        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="Event"></param>
        /// <param name="Param"></param>
        internal static void ExecuteSocketRequest(PacketDecodeCompletedEventArgs Event, SocketMiddleData Param)
        {
            Event.Server.Send(Param.ToJson(), Event.Session);
        }
    }
}

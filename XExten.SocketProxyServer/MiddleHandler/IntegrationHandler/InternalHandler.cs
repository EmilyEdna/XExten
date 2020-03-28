using BeetleX.EventArgs;
using XExten.SocketProxyServer.MiddleView;
using XExten.SocketProxyServer.MiddleView.ViewEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XExten.SocketProxyServer.MiddleHandler.IntegrationHandler
{
    public class InternalHandler
    {
        /// <summary>
        /// 转发数据
        /// </summary>
        /// <param name="Param"></param>
        /// <param name="PacketCache"></param>
        internal static void ExecuteSocketIniternalInfo(SocketMiddleData Param, Dictionary<String, PacketDecodeCompletedEventArgs> PacketCache)
        {
            var Router = Param.MiddleResult.Router.Split("/").FirstOrDefault();
            Param.SendType = SendTypeEnum.RequestInfo;
            var Keys = PacketCache.Keys.Where(t => t.Contains(Router)).FirstOrDefault();
            var Event = PacketCache[Keys];
           var NewEvent =  Event.SetInfo(Event.Session, Param);
            Event.Session.Server.Handler.SessionPacketDecodeCompleted(Event.Server, NewEvent);
        }
    }
}

using XExten.SocketProxyServer.MiddleView;
using XExten.SocketProxyServer.MiddleView.ViewEnum;
using XExten.XCore;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text;
using BeetleX.EventArgs;
using System.Linq;
using XExten.SocketProxyServer.MiddleHandler.IntegrationHandler;
using BeetleX;

namespace XExten.SocketProxyServer.MiddleHandler
{
    public class ExecuteDependency
    {
        /// <summary>
        /// 缓存Seesion
        /// </summary>
        private readonly static Dictionary<String, PacketDecodeCompletedEventArgs> PacketCache = new Dictionary<String, PacketDecodeCompletedEventArgs>();

        /// <summary>
        /// 缓存Session
        /// </summary>
        /// <param name="Provider"></param>
        /// <param name="Event"></param>
        public static void ExecutePacketCache(SocketMiddleData Provider, PacketDecodeCompletedEventArgs Event)
        {
            if (Provider.SendType == SendTypeEnum.Init)
            {
                var Keys = Provider.MiddleResult.SocketJsonData.ToModel<Dictionary<string, List<string>>>().Keys.ToList();
                PacketCache.Add(string.Join(",", Keys), Event);
            }
        }
        /// <summary>
        /// 处理内部消息
        /// </summary>
        /// <param name="Param"></param>
        public static void ExecuteInternalInfo(PacketDecodeCompletedEventArgs Event,SocketMiddleData Param)
        {

            switch (Param.SendType)
            {
                case SendTypeEnum.Init:
                    InitHandler.ExecuteSocketApiJson(Param.MiddleResult);
                    break;
                case SendTypeEnum.InternalInfo:
                    InternalHandler.ExecuteSocketIniternalInfo(Param, PacketCache);
                    break;
                case SendTypeEnum.RequestInfo:
                    RequestHandler.ExecuteSocketRequest(Event, Param);
                    break;
                case SendTypeEnum.CallBack:
                    CallHandler.ExecuteSocketCallBack(PacketCache.Values.ToList(), Param);
                    break;
                default:
                    break;
            }
        }

    }
}

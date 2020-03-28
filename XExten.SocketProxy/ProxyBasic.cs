using BeetleX;
using BeetleX.Clients;
using System;
using System.Net;
using XExten.SocketProxy.ProxyAbstract;
using XExten.SocketProxy.SocketCall;
using XExten.SocketProxy.SocketConfig;
using XExten.SocketProxy.SocketConfig.ConstConfig;
using XExten.SocketProxy.SocketDependency;
using XExten.SocketProxy.SocketEnum;
using XExten.SocketProxy.SocketEvent;
using XExten.XCore;

namespace XExten.SocketProxy
{
    /// <summary>
    /// 代理基础类
    /// </summary>
    public class ProxyBasic
    {
        #region Basic Config
        /// <summary>
        /// 通信中心IP
        /// </summary>
        public string SockInfoIP { get; set; }
        /// <summary>
        /// 通信中心端口
        /// </summary>
        public int SockInfoPort { get; set; }
        /// <summary>
        /// 客服端地址
        /// </summary>
        public string ClientPath { get; set; }
        /// <summary>
        /// 客服端端口
        /// </summary>
        public int? ClientPort { get; set; }
        #endregion

        #region 抽象处理器
        /// <summary>
        /// 回调处理器
        /// </summary>
        public CallHandleAbstract CallHandle { get; set; }
        #endregion

        /// <summary>
        /// 初始化通信中心Socket
        /// </summary>
        /// <param name="Action"></param>
        /// <param name="UseServer"></param>
        public static void InitInternalSocket(Action<ProxyBasic> Action, bool UseServer = false)
        {
            ProxyBasic Client = new ProxyBasic();
            Action(Client);
            SocketConstConfig.ClientPort = Client.ClientPort;
            if (UseServer)
            {
                CallEvent.Instance().Changed += new CallEvent.ResultEventHandler(CallEventAction.Instance().OnResponse);
                Client.InitInternalSocket(Client.SockInfoIP, Client.SockInfoPort, DependencyExecute.Instance.FindLibrary());
            }
        }
        /// <summary>
        /// 重新连接通信中心
        /// </summary>
        /// <param name="Action"></param>
        public static void ReOpenInternalSocket(Action<ProxyBasic> Action)
        {
            ProxyBasic Client = new ProxyBasic();
            Action(Client);
            if (Call.SocketClient.IsConnected)
                Call.SocketClient.DisConnect();
            CallEvent.Instance().Changed += new CallEvent.ResultEventHandler(CallEventAction.Instance().OnResponse);
            Client.InitInternalSocket(Client.SockInfoIP, Client.SockInfoPort, DependencyExecute.Instance.FindLibrary());
        }
        /// <summary>
        /// 初始化内部通信
        /// </summary>
        /// <param name="Ip"></param>
        /// <param name="Port"></param>
        /// <param name="MiddleData"></param>
        protected virtual void InitInternalSocket(string Ip, int Port, SocketMiddleData MiddleData)
        {
            if (CallHandle == null)
                CallHandle = new CallHandle();
            AsyncTcpClient ClientAsnyc = SocketFactory.CreateClient<AsyncTcpClient, SocketPacket>(Ip, Port);
            if (!ClientPath.IsNullOrEmpty() && ClientPort.HasValue)
                ClientAsnyc.LocalEndPoint = new IPEndPoint(IPAddress.Parse(ClientPath), ClientPort.Value);
            ClientAsnyc.Connect(out bool Connect);
            Call.SocketClient = ClientAsnyc;
            ClientAsnyc.PacketReceive = (Client, Data) =>
            {
                DependencyCondition Instance = DependencyCondition.Instance;
                if (Instance.ExecuteIsCall(Data) != SendTypeEnum.CallBack)
                {
                    var MiddleData = Instance.ExecuteMapper(Data);
                    if (Client.IsConnected)
                        Call.CallBackHandler(MiddleData, CallHandle);
                }
                else
                    Instance.ExecuteCallData(Data);
            };
            ClientAsnyc.ClientError = (Client, Error) =>
            {
                DependencyError.Instance.ExecuteRecordLog(Error);
            };
            if (MiddleData.SendType == SendTypeEnum.Init)
                ClientAsnyc.Send(MiddleData.ToJson());
        }
    }
}

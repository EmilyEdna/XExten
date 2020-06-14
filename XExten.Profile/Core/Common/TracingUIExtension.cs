using BeetleX;
using BeetleX.Clients;
using XExten.Profile.Tracing.Entry;
using System.Net;
using XExten.XCore;
using System.Collections.Generic;
using System;

namespace XExten.Profile.Core.Common
{
    internal static class TracingUIExtension
    {
        /// <summary>
        /// UI界面地址
        /// </summary>
        internal static string UIHost { get; set; }
        /// <summary>
        /// 客户端
        /// </summary>
        private static readonly Dictionary<String, TcpClient> Clients = new Dictionary<String, TcpClient>();
        /// <summary>
        /// 将数据绘制到UI界面
        /// </summary>
        /// <param name="Context"></param>
        internal static void OpenUI(this PartialContext Context)
        {
            string[] IP = UIHost.Split(":");
            TcpClient Client;
            if (Clients.ContainsKey(UIHost))
            {
                Client = Clients[UIHost];
            }
            else
            {
                Client = SocketFactory.CreateClient<TcpClient>(IP[0], int.Parse(IP[1]));
                Clients.Add(UIHost, Client);
            }
            Client.LocalEndPoint = new IPEndPoint(IPAddress.Any, 9373);
            Client.Stream.ToPipeStream().WriteLine(Context.ToJson());
            Client.Stream.Flush();
        }
    }
}

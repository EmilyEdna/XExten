using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XExten.SocketProxyServer.MiddleConfig
{
    /// <summary>
    /// 配置
    /// </summary>
    public class Configuration
    {
        public static IConfiguration Builder => GetSetting();
        public static string TCP_Host { get; set; } = Builder["TCPServerProxyIP"] ?? "0.0.0.0";
        public static int TCP_Port { get; set; } = Convert.ToInt32(Builder["TCPServerProxyPort"] ?? "9090");
        private static IConfiguration GetSetting()
        {
            IConfigurationBuilder Buidler = new ConfigurationBuilder();
            Buidler.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("setting.json", false, true);
            return Buidler.Build();
        }
    }
}

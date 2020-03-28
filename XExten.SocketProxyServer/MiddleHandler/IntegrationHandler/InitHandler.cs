using XExten.SocketProxyServer.MiddleView.ViewInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using XExten.XCore;

namespace XExten.SocketProxyServer.MiddleHandler.IntegrationHandler
{
    public class InitHandler
    {
        /// <summary>
        /// 把服务注入的API持久化
        /// </summary>
        /// <param name="Param"></param>
        internal static void ExecuteSocketApiJson(ISocketResult Provider)
        {
            var Directories = Path.Combine(AppContext.BaseDirectory, "SocketApi");
            if (!Directory.Exists(Directories))
                Directory.CreateDirectory(Directories);
            var JsonData = Provider.SocketJsonData.ToModel<Dictionary<string, List<string>>>();
            foreach (var Item in JsonData)
            {
                var FilePath = Path.Combine(Directories, $"{Item.Key}.json");
                if (File.Exists(FilePath))
                {
                    File.Delete(FilePath);
                    ExecuteSocketApiFile(FilePath, Provider.SocketJsonData);
                }
                else ExecuteSocketApiFile(FilePath, Provider.SocketJsonData);

            }
        }
        /// <summary>
        /// API写入文件
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="Data"></param>
        private static void ExecuteSocketApiFile(string FilePath, string Data)
        {
            using FileStream Fs = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite);
            using StreamWriter Sw = new StreamWriter(Fs);
            Sw.Write(Data);
            Sw.Flush();
            Sw.Close();
            Fs.Close();
        }
    }
}

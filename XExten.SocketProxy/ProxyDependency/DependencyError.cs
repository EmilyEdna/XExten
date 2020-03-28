using BeetleX.Clients;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XExten.SocketProxy.SocketDependency
{
    public class DependencyError
    {
        public static DependencyError Instance => new DependencyError();
        /// <summary>
        /// 记录异常错误
        /// </summary>
        /// <param name="Error"></param>
        public void ExecuteRecordLog(ClientErrorArgs Error)
        {
            String ExceptionInfomations = $"Service errored with exception：【{Error.Message}】====write time：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\n";
            var Diretories = Path.Combine(AppContext.BaseDirectory, "SocketError");
            if (!Directory.Exists(Diretories))
                Directory.CreateDirectory(Diretories);
            File.AppendAllText(Path.Combine(Diretories, "SocketErrorInfo.log"), ExceptionInfomations);
            Console.WriteLine(ExceptionInfomations);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace XExten.HttpFactory.MultiInterface
{
    /// <summary>
    /// 构建
    /// </summary>
    public interface IBuilder
    {
        /// <summary>
        /// 客服端
        /// </summary>
        HttpClient Client => HttpMultiClientWare.FactoryClient;
        /// <summary>
        /// 构建
        /// </summary>
        /// <returns></returns>
        IBuilder Build();
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        Task<List<String>> RunAsync();
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        List<String> Run();
    }
}

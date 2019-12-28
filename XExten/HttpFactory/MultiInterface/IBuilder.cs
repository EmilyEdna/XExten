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
        /// 构建
        /// </summary>
        /// <param name="TimeOut">超时:秒</param>
        /// <returns></returns>
        IBuilder Build(int TimeOut=60);
        /// <summary>
        /// 设置缓存时间
        /// </summary>
        /// <param name="CacheSeconds"></param>
        /// <returns></returns>
        IBuilder CacheTime(int CacheSeconds = 60);
        /// <summary>
        /// 执行 default UTF-8
        /// </summary>
        /// <returns></returns>
        List<String> RunString();
        /// <summary>
        /// 执行 default UTF-8
        /// </summary>
        /// <returns></returns>
        Task<List<String>> RunStringAsync();
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        List<Byte[]> RunBytes();
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        Task<List<Byte[]>> RunBytesAsync();
    }
}

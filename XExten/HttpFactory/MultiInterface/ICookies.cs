using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XExten.HttpFactory.MultiInterface
{
    /// <summary>
    /// Cookie
    /// </summary>
    public interface ICookies
    {
        /// <summary>
        /// 构建
        /// </summary>
        /// <returns></returns>
        IBuilder Build();
        /// <summary>
        /// Add URL
        /// </summary>
        /// <param name="Path"></param>
        ///<param name="Weight"></param>
        /// <returns></returns>
        INode AddNode(string Path, int Weight);
        /// <summary>
        /// Add Path
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Param"></param>
        /// <param name="Weight"></param>
        /// <returns></returns>
        INode AddNode(string Path, string Param, int Weight = 50);
        /// <summary>
        /// Add Path
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Path"></param>
        /// <param name="Param">实体模型</param>
        /// <param name="MapFied">映射字段</param>
        /// <param name="Weight"></param>
        /// <returns></returns>
        INode AddNode<T>(string Path, T Param, IDictionary<string, string> MapFied = null, int Weight = 50) where T : class, new();
        /// <summary>
        /// Add Header
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IHeaders Header(string key, string value);
        /// <summary>
        /// Add Header
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        IHeaders Header(Dictionary<string, string> headers);
        /// <summary>
        /// Add Cookie
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ICookies Cookie(string name, string value);
        /// <summary>
        /// Add Cookie
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        ICookies Cookie(string name, string value,string path);
        /// <summary>
        /// Add Cookie
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="path"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        ICookies Cookie(string name, string value,string path,string domain);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using XExten.HttpFactory.MultiInterface;

namespace XExten.HttpFactory.MultiImplement
{
    /// <summary>
    /// Cookie
    /// </summary>
    public class Cookies : ICookies
    {
        internal IHeaders Headers;
        internal INode Nodes;
        internal IBuilder Builder;

        /// <summary>
        /// Constructor
        /// </summary>
        public Cookies()
        {
            Headers = new Headers();
            Nodes = new Node();
            Builder = new Builder();
        }

        /// <summary>
        /// 构建
        /// </summary>
        /// <returns></returns>
        public IBuilder Build()
        {
            return Builder.Build();
        }

        /// <summary>
        /// Add Cookie
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ICookies Cookie(string name, string value)
        {
            Cookie Cookie = new Cookie(name, value);
            HttpMultiClientWare.Container.Add(Cookie);
            return this;
        }

        /// <summary>
        /// Add Cookie
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public ICookies Cookie(string name, string value, string path)
        {
            Cookie Cookie = new Cookie(name, value, path);
            HttpMultiClientWare.Container.Add(Cookie);
            return this;
        }

        /// <summary>
        /// Add Cookie
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="path"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public ICookies Cookie(string name, string value, string path, string domain)
        {
            Cookie Cookie = new Cookie(name, value, path, domain);
            HttpMultiClientWare.Container.Add(Cookie);
            return this;
        }

        /// <summary>
        /// Add Header
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IHeaders Header(string key, string value)
        {
            return Headers.Header(key, value);
        }

        /// <summary>
        /// Add Header
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public IHeaders Header(Dictionary<string, string> headers)
        {
            return Headers.Header(headers);
        }

        /// <summary>
        /// Add Uri
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Weight"></param>
        /// <returns></returns>
        public INode AddNode(string Path, int Weight = 50)
        {
            return Nodes.AddNode(Path, Weight);
        }
        /// <summary>
        /// Add Path
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Param"></param>
        /// <param name="Weight"></param>
        /// <returns></returns>
        public INode AddNode(string Path, string Param, int Weight = 50)
        {
            return Nodes.AddNode(Path, Param, Weight);
        }

        /// <summary>
        /// Add Path
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Path"></param>
        /// <param name="Param">实体模型</param>
        /// <param name="MapFied">映射字段</param>
        /// <param name="Weight"></param>
        /// <returns></returns>
        public INode AddNode<T>(string Path, T Param, IDictionary<string, string> MapFied = null, int Weight = 50) where T : class, new()
        {
            return Nodes.AddNode(Path, Param, MapFied, Weight);
        }
    }
}

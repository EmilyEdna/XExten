using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XExten.HttpFactory.MultiInterface;


namespace XExten.HttpFactory.MultiImplement
{
    /// <summary>
    /// Header
    /// </summary>
    public class Headers : IHeaders
    {
        internal HttpClient ClientHeader;
        internal ICookies Cookies;
        internal INode Nodes;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Cookie"></param>
        public Headers(ICookies Cookie)
        {
            ClientHeader = HttpMultiClient.Factory.FirstOrDefault();
            Cookies = Cookie;
            Nodes = new Node();
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public Headers()
        {
            ClientHeader = HttpMultiClient.Factory.FirstOrDefault();
            Cookies = new Cookies();
            Nodes = new Node();
        }

        /// <summary>
        /// Add Header
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IHeaders Header(string key, string value)
        {
            ClientHeader.DefaultRequestHeaders.Add(key, value);
            return this;
        }

        /// <summary>
        /// Add Header
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public IHeaders Header(Dictionary<string, string> headers)
        {
            foreach (var item in headers)
            {
                ClientHeader.DefaultRequestHeaders.Add(item.Key, item.Value);
            }
            return this;
        }

        /// <summary>
        /// Add Cookie
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ICookies Cookie(string name, string value)
        {
           return  Cookies.Cookie(name, value);
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
            return Cookies.Cookie(name, value, path);
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
            return Cookies.Cookie(name, value, path, domain);
        }

        /// <summary>
        /// AddUri
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Weight"></param>
        /// <returns></returns>
        public INode AddNode(string Path, int Weight = 50)
        {
            return Nodes.AddNode(Path, Weight);
        }
    }
}

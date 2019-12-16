using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XExten.HttpFactory.MultiInterface;

namespace XExten.HttpFactory.MultiImplement
{
    /// <summary>
    /// URL
    /// </summary>
    public class Node : INode
    {
        internal IHeaders Headers;
        internal ICookies Cookies;
        /// <summary>
        /// Constructor
        /// </summary>
        public Node()
        {
            Headers = new Headers();
            Cookies = new Cookies();
        }
        /// <summary>
        /// Add Uri
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Weight"></param>
        /// <returns></returns>
        public INode AddNode(string Path, int Weight = 50)
        {
            if (!HttpMultiClient.UriMap.ContainsKey(Path + 50))
                HttpMultiClient.UriMap.TryAdd(Path + Weight, new Uri(Path));
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
            return Cookies.Cookie(name, value);
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
    }
}

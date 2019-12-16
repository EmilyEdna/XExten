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
        internal CookieContainer Containers;
        internal IHeaders Headers;
        internal INode Nodes;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Header"></param>
        public Cookies(IHeaders Header)
        {
            Containers = HttpMultiClient.Container.FirstOrDefault(); ;
            Headers = Header;
            Nodes = new Node();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Cookies()
        {
            Containers = HttpMultiClient.Container.FirstOrDefault();
            Headers = new Headers();
            Nodes = new Node();
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
            Containers.Add(Cookie);
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
            Containers.Add(Cookie);
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
            Containers.Add(Cookie);
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
    }
}

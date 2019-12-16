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
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Header"></param>
        public Cookies(IHeaders Header)
        {
            Containers = HttpMultiClient.Container.FirstOrDefault(); ;
            Headers = Header;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Cookies()
        {
            Containers = HttpMultiClient.Container.FirstOrDefault();
            Headers = new Headers();
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

        public INode AddUrl(string Path)
        {
            throw new NotImplementedException();
        }
    }
}

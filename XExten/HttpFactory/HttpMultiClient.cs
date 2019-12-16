using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XExten.HttpFactory.MultiImplement;
using XExten.HttpFactory.MultiInterface;

namespace XExten.HttpFactory
{
    /// <summary>
    /// 负载请求
    /// </summary>
    public class HttpMultiClient
    {
        internal HttpClient Client;

        /// <summary>
        /// Instance
        /// </summary>
        public static HttpMultiClient HttpMulti => new HttpMultiClient();
        /// <summary>
        /// Constructor
        /// </summary>
        public HttpMultiClient()
        {
            Client = new HttpClient();
        }

        #region Header
        /// <summary>
        /// Add Header
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IHeaders Headers(string key, string value)
        {
            Client.DefaultRequestHeaders.Add(key, value);
            return new Headers(Client);
        }
        /// <summary>
        /// Add Header
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public IHeaders Headers(Dictionary<string,string> headers)
        {
            foreach (var item in headers)
            {
                Client.DefaultRequestHeaders.Add(item.Key, item.Value);
            }
            return new Headers(Client);
        }
        #endregion Header

        #region Cookie
        /// <summary>
        /// Add Cookie
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ICookies Cookies(string name,string value) {
            CookieContainer Container = new CookieContainer();
            Cookie Cookie = new Cookie(name,value);
            Container.Add(Cookie);
            return new Cookies(Container);
        }
        /// <summary>
        /// Add Cookie
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public ICookies Cookies(string name, string value, string path)
        {
            CookieContainer Container = new CookieContainer();
            Cookie Cookie = new Cookie(name, value, path);
            Container.Add(Cookie);
            return new Cookies(Container);
        }
        /// <summary>
        /// Add Cookie
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="path"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public ICookies Cookies(string name, string value, string path, string domain)
        {
            CookieContainer Container = new CookieContainer();
            Cookie Cookie = new Cookie(name, value, path, domain);
            Container.Add(Cookie);
            return new Cookies(Container);
        }
        #endregion Cookie
    }
}

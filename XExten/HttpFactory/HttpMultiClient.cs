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
        public static List<HttpClient> Factory = new List<HttpClient>();
        public static List<CookieContainer> Container = new List<CookieContainer>();
        internal IHeaders HeadersInstance = null;
        internal ICookies CookiesInstance = null;
        /// <summary>
        /// Instance
        /// </summary>
        public static HttpMultiClient HttpMulti => new HttpMultiClient();
        /// <summary>
        /// Constructor
        /// </summary>
        public HttpMultiClient()
        {
            Factory.Add(new HttpClient());
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
            Factory.FirstOrDefault().DefaultRequestHeaders.Add(key, value);
            HeadersInstance = new Headers(CookiesInstance ?? new Cookies(new CookieContainer()));
            return HeadersInstance;
        }
        /// <summary>
        /// Add Header
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public IHeaders Headers(Dictionary<string, string> headers)
        {
            foreach (var item in headers)
            {
                Factory.FirstOrDefault().DefaultRequestHeaders.Add(item.Key, item.Value);
            }
            HeadersInstance = new Headers(CookiesInstance?? new Cookies(new CookieContainer()));
            return HeadersInstance;
        }
        #endregion Header

        #region Cookie
        /// <summary>
        /// Add Cookie
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ICookies Cookies(string name, string value)
        {
            Container .Add(new CookieContainer());
            Cookie Cookie = new Cookie(name, value);
            Container.FirstOrDefault().Add(Cookie);
            CookiesInstance = new Cookies(HeadersInstance ?? new Headers());
            return CookiesInstance;
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
            Container.Add(new CookieContainer());
            Cookie Cookie = new Cookie(name, value, path);
            Container.FirstOrDefault().Add(Cookie);
            CookiesInstance = new Cookies(HeadersInstance ?? new Headers());
            return CookiesInstance;
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
            Container.Add(new CookieContainer());
            Cookie Cookie = new Cookie(name, value, path, domain);
            Container.FirstOrDefault().Add(Cookie);
            CookiesInstance = new Cookies(HeadersInstance ?? new Headers());
            return CookiesInstance;
        }
        #endregion Cookie
    }
}

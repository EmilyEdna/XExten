using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XExten.HttpFactory.MultiImplement;
using XExten.HttpFactory.MultiInterface;
using System.Net.Http.Headers;
using XExten.XPlus;

namespace XExten.HttpFactory
{
    /// <summary>
    /// 负载请求
    /// </summary>
    public class HttpMultiClient
    {
        /// <summary>
        /// Instance
        /// </summary>
        public static HttpMultiClient HttpMulti => new HttpMultiClient();

        /// <summary>
        /// Constructor
        /// </summary>
        public HttpMultiClient()
        {
            HttpMultiClientWare.Builder = new Builder();
            HttpMultiClientWare.Headers = new Headers();
            HttpMultiClientWare.Cookies = new Cookies();
            HttpMultiClientWare.Nodes = new Node();
        }

        /// <summary>
        /// 初始化Cookie容器
        /// </summary>
        /// <returns></returns>
        public HttpMultiClient InitCookieContainer()
        {
            HttpMultiClientWare.Container = new CookieContainer();
            return this;
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
            HttpMultiClientWare.HeaderMaps.Add(new Dictionary<string, string>() { { key, value } });
            return HttpMultiClientWare.Headers;
        }
        /// <summary>
        /// Add Header
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public IHeaders Headers(Dictionary<string, string> headers)
        {
            HttpMultiClientWare.HeaderMaps.Add(headers);
            return HttpMultiClientWare.Headers;
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
            Cookie Cookie = new Cookie(name, value);
            if (HttpMultiClientWare.Container == null) throw new NullReferenceException("Please initialize the InitCookieContainer method before calling the cookie method");
            HttpMultiClientWare.Container.Add(Cookie);
            return HttpMultiClientWare.Cookies;
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
            Cookie Cookie = new Cookie(name, value, path);
            if (HttpMultiClientWare.Container == null) throw new NullReferenceException("Please initialize the InitCookieContainer method before calling the cookie method");
            HttpMultiClientWare.Container.Add(Cookie);
            return HttpMultiClientWare.Cookies;
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
            Cookie Cookie = new Cookie(name, value, path, domain);
            if (HttpMultiClientWare.Container == null) throw new NullReferenceException("Please initialize the InitCookieContainer method before calling the cookie method");
            HttpMultiClientWare.Container.Add(Cookie);
            return HttpMultiClientWare.Cookies;
        }
        #endregion Cookie

        #region URL
        /// <summary>
        /// Add Path
        /// </summary>
        /// <param name="Path">请求地址</param>
        /// <param name="Type">请求类型</param>
        /// <param name="UseCache">使用缓存</param>
        /// <param name="Weight">1~100区间</param>
        /// <returns></returns>
        public INode AddNode(string Path, RequestType Type = RequestType.GET, bool UseCache = false, int Weight = 50)
        {
            WeightURL WeightUri = new WeightURL
            {
                Weight = Weight,
                URL = new Uri(Path),
                Request = Type,
                UseCache = UseCache
            };
            HttpMultiClientWare.WeightPath.Add(WeightUri);
            return HttpMultiClientWare.Nodes;
        }

        /// <summary>
        /// Add Path
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Param"></param>
        ///  <param name="Type">请求类型</param>
        /// <param name="UseCache">使用缓存</param>
        /// <param name="Weight"></param>
        /// <returns></returns>
        public INode AddNode(string Path, string Param, RequestType Type = RequestType.GET, bool UseCache = false, int Weight = 50)
        {
            WeightURL WeightUri = new WeightURL
            {
                Weight = Weight,
                URL = new Uri(Path),
                Request = Type,
                Contents = new StringContent(Param),
                UseCache = UseCache,
                MediaTypeHeader = new MediaTypeHeaderValue("application/json")
            };
            HttpMultiClientWare.WeightPath.Add(WeightUri);
            return HttpMultiClientWare.Nodes;
        }

        /// <summary>
        /// Add Path
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Param"></param>
        /// <param name="Type">请求类型</param>
        /// <param name="UseCache">使用缓存</param>
        /// <param name="Weight">1~100区间</param>
        /// <returns></returns>
        public INode AddNode(string Path, List<KeyValuePair<String, String>> Param, RequestType Type = RequestType.GET, bool UseCache = false, int Weight = 50)
        {
            return XPlus.XPlus.XTry(() =>
             {
                 WeightURL WeightUri = new WeightURL
                 {
                     Weight = Weight,
                     URL = new Uri(Path),
                     Request = Type,
                     Contents = new FormUrlEncodedContent(Param),
                     UseCache = UseCache,
                     MediaTypeHeader = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
                 };
                 HttpMultiClientWare.WeightPath.Add(WeightUri);
                 return HttpMultiClientWare.Nodes;
             }, (Ex) => throw new Exception("The parameter type is incorrect. The parameter can only be a solid model."));
        }

        /// <summary>
        /// Add Path
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Path"></param>
        /// <param name="Param">实体模型</param>
        /// <param name="MapFied">映射字段</param>
        ///  <param name="Type">请求类型</param>
        /// <param name="UseCache">使用缓存</param>
        /// <param name="Weight">1~100区间</param>
        /// <returns></returns>
        public INode AddNode<T>(string Path, T Param, IDictionary<string, string> MapFied = null, RequestType Type = RequestType.GET, bool UseCache = false, int Weight = 50) where T : class, new()
        {
            return XPlus.XPlus.XTry(() =>
            {
                WeightURL WeightUri = new WeightURL
                {
                    Weight = Weight,
                    URL = new Uri(Path),
                    Request = Type,
                    UseCache = UseCache,
                    Contents = new FormUrlEncodedContent(HttpKeyPairs.KeyValuePairs(Param, MapFied)),
                    MediaTypeHeader = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
                };
                HttpMultiClientWare.WeightPath.Add(WeightUri);
                return HttpMultiClientWare.Nodes;
            }, (Ex) => throw new Exception("The parameter type is incorrect. The parameter can only be a solid model."));
        }
        #endregion
    }
}

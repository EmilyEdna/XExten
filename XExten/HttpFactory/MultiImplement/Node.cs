using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
        internal IBuilder Builder;

        /// <summary>
        /// Constructor
        /// </summary>
        public Node()
        {
            Headers = new Headers();
            Cookies = new Cookies();
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
        /// Add Uri
        /// </summary>
        /// <param name="Path"></param>
        ///<param name="Weight"></param>
        /// <returns></returns>
        public INode AddNode(string Path, int Weight)
        {
            WeightURL WeightUri = new WeightURL
            {
                Weight = Weight,
                URL = new Uri(Path)
            };
            HttpMultiClientWare.WeightPath.Add(WeightUri);
            return this;
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
            WeightURL WeightUri = new WeightURL
            {
                Weight = Weight,
                URL = new Uri(Path),
                StringContents = new StringContent(Param)
            };
            HttpMultiClientWare.WeightPath.Add(WeightUri);
            return this;
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
            try
            {
                WeightURL WeightUri = new WeightURL
                {
                    Weight = Weight,
                    URL = new Uri(Path),
                    FormContent = new FormUrlEncodedContent(HttpKeyPairs.KeyValuePairs(Param, MapFied))
                };
                HttpMultiClientWare.WeightPath.Add(WeightUri);
                return this;
            }
            catch (Exception)
            {
                throw new Exception("参数类型不正确，参数只能是实体模型。");
            }
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

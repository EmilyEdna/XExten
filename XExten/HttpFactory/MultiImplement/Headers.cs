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
    /// Header
    /// </summary>
    public class Headers : IHeaders
    {
        internal HttpClient ClientHeader;

        /// <summary>
        /// Constructor
        /// </summary>
        public Headers()
        {
            ClientHeader = new HttpClient();
        }

        /// <summary>
        /// Constructor with HttpClient
        /// </summary>
        /// <param name="Client"></param>
        public Headers(HttpClient Client)
        {
            ClientHeader = Client;
        }

        /// <summary>
        /// Add Cookie
        /// </summary>
        /// <returns></returns>
        public ICookies Cookie()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add Path
        /// </summary>
        /// <returns></returns>
        public INode Node()
        {
            throw new NotImplementedException();
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
    }
}

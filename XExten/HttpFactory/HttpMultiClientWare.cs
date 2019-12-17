using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace XExten.HttpFactory
{
    /// <summary>
    /// 中间服务
    /// </summary>
    public class HttpMultiClientWare
    {
        /// <summary>
        /// Client
        /// </summary>
        public static HttpClient FactoryClient { get; set; }
        /// <summary>
        /// Container
        /// </summary>
        public static CookieContainer Container { get; set; }
        /// <summary>
        /// URL
        /// </summary>
        public static List<WeightURL> WeightPath { get; set; }
    }
}

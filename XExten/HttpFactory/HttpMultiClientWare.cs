using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XExten.HttpFactory.MultiInterface;

namespace XExten.HttpFactory
{
    internal class HttpMultiClientWare
    {
        internal static List<Dictionary<String, String>> HeaderMaps = new List<Dictionary<string, string>>();
        internal static List<WeightURL> WeightPath = new List<WeightURL>();
        internal static CookieContainer Container { get; set; }
        internal static HttpClient FactoryClient { get; set; }
        internal static IBuilder Builder { get; set; }
        internal static ICookies Cookies { get; set; }
        internal static IHeaders Headers { get; set; }
        internal static INode Nodes { get; set; }
    }
}

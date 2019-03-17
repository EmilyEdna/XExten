using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LinqX.Core.HttpFactory
{
    public class HttpRequestClient
    {
        /// <summary>
        /// 将数据制作表单数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Entity"></param>
        /// <param name="Map"></param>
        /// <returns></returns>
        public static IList<KeyValuePair<String, String>> KeyValuePairs<T>(T Entity, IDictionary<string, string> Map = null) where T : class, new()
        {
            IList<KeyValuePair<String, String>> keyValuePairs = new List<KeyValuePair<string, string>>();
            Entity.GetType().GetProperties().ToList().ForEach(t =>
            {
                var flag = t.CustomAttributes.Where(x => x.AttributeType == typeof(JsonIgnoreAttribute)).FirstOrDefault();
                if (Map != null)
                    foreach (KeyValuePair<String, String> item in Map)
                    {
                        if (item.Key.Equals(t.Name))
                            keyValuePairs.Add(new KeyValuePair<string, string>(item.Value, t.GetValue(Entity).ToString()));
                    }
                else if (flag == null)
                    keyValuePairs.Add(new KeyValuePair<string, string>(t.Name, t.GetValue(Entity).ToString()));
            });
            return keyValuePairs;
        }
        /// <summary>
        /// Post异步请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="headers"></param>
        /// <param name="contentType"></param>
        /// <param name="timeout"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static async Task<String> HttpPostAsync(string url, IList<KeyValuePair<String, String>> data, Dictionary<string, string> headers = null, string contentType = null, int timeout = 0, Encoding encoding = null)
        {
            using (HttpClient Client = new HttpClient())
            {
                if (headers != null)
                    foreach (KeyValuePair<string, string> header in headers)
                    {
                        Client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                if (timeout > 0)
                    Client.Timeout = new TimeSpan(0, 0, timeout);
                HttpContent content = new FormUrlEncodedContent(data);
                if (contentType != null)
                    content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.67 Safari/537.36");
                HttpResponseMessage responseMessage = await Client.PostAsync(url, content);
                Byte[] resultBytes = await responseMessage.Content.ReadAsByteArrayAsync();
                return Encoding.UTF8.GetString(resultBytes);
            }
        }
        /// <summary>
        /// Get异步请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<String> HttpGetAsync(string url, Dictionary<string, string> headers = null, int timeout = 0)
        {
            using (HttpClient Client = new HttpClient())
            {
                if (headers != null)
                    foreach (KeyValuePair<string, string> header in headers)
                    {
                        Client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                if (timeout > 0)
                    Client.Timeout = new TimeSpan(0, 0, timeout);
                Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.67 Safari/537.36");
                Byte[] resultBytes = await Client.GetByteArrayAsync(url);
                return Encoding.Default.GetString(resultBytes);
            }
        }
    }
}

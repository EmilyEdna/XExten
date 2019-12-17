using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace XExten.HttpFactory
{
    public class HttpRequestClient
    {

        /// <summary>
        /// Http by post default UTF8
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="UseCookie"></param>
        /// <param name="IsDispose"></param>
        /// <param name="headers"></param>
        /// <param name="contentType"></param>
        /// <param name="timeout"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static async Task<String> HttpPostAsync(string url, IList<KeyValuePair<String, String>> data, bool UseCookie = false, bool IsDispose = true, Dictionary<string, string> headers = null, string contentType = null, int timeout = 0, Encoding encoding = null)
        {
            Byte[] ResultBytes = null;
            if (UseCookie)
                ResultBytes = await KeepSessionByHttpPostBytesAsync(url, data, IsDispose, headers, contentType, timeout, encoding);
            else
                ResultBytes = await HttpPostBytesAsync(url, data, headers, contentType, timeout, encoding);
            return Encoding.UTF8.GetString(ResultBytes);
        }

        /// <summary>
        /// Http by post default Bytes
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="headers"></param>
        /// <param name="contentType"></param>
        /// <param name="timeout"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static async Task<Byte[]> HttpPostBytesAsync(string url, IList<KeyValuePair<String, String>> data, Dictionary<string, string> headers = null, string contentType = null, int timeout = 0, Encoding encoding = null)
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
                return await responseMessage.Content.ReadAsByteArrayAsync();
            }
        }

        /// <summary>
        /// http by post default bytes keep session
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="IsDispose"></param>
        /// <param name="headers"></param>
        /// <param name="contentType"></param>
        /// <param name="timeout"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static async Task<Byte[]> KeepSessionByHttpPostBytesAsync(string url, IList<KeyValuePair<String, String>> data, bool IsDispose = true, Dictionary<string, string> headers = null, string contentType = null, int timeout = 0, Encoding encoding = null)
        {
            CookieContainer Cookie = new CookieContainer();
            HttpClientHandler Handler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = Cookie
            };
            if (IsDispose)
            {
                using (HttpClient Client = new HttpClient(Handler))
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
                    return await responseMessage.Content.ReadAsByteArrayAsync();
                }
            }
            else
            {
                HttpClient Client = new HttpClient(Handler);
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
                return await responseMessage.Content.ReadAsByteArrayAsync();
            }
        }

        /// <summary>
        /// Http by get default UTF8
        /// </summary>
        /// <param name="url"></param>
        /// <param name="UseCookie"></param>
        /// <param name="IsDispose"></param>
        /// <param name="headers"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<String> HttpGetAsync(string url, bool UseCookie = false, bool IsDispose = true, Dictionary<string, string> headers = null, int timeout = 0)
        {
            Byte[] ResultBytes = null;
            if (UseCookie)
                ResultBytes = await KeepSessionByHttpGetBytesAsync(url, IsDispose, headers, timeout);
            else
                ResultBytes = await HttpGetBytesAsync(url, headers, timeout);
            return Encoding.Default.GetString(ResultBytes);
        }

        /// <summary>
        /// Http by get default Bytes
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<Byte[]> HttpGetBytesAsync(string url, Dictionary<string, string> headers = null, int timeout = 0)
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
                return await Client.GetByteArrayAsync(url);
            }
        }

        /// <summary>
        /// http by get default bytes keep session
        /// </summary>
        /// <param name="url"></param>
        /// <param name="IsDispose"></param>
        /// <param name="headers"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<Byte[]> KeepSessionByHttpGetBytesAsync(string url, bool IsDispose = true, Dictionary<string, string> headers = null, int timeout = 0)
        {
            CookieContainer Cookie = new CookieContainer();
            HttpClientHandler Handler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = Cookie
            };
            if (IsDispose)
            {
                using (HttpClient Client = new HttpClient(Handler))
                {
                    if (headers != null)
                        foreach (KeyValuePair<string, string> header in headers)
                        {
                            Client.DefaultRequestHeaders.Add(header.Key, header.Value);
                        }
                    if (timeout > 0)
                        Client.Timeout = new TimeSpan(0, 0, timeout);
                    Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.67 Safari/537.36");
                    return await Client.GetByteArrayAsync(url);
                }
            }
            else
            {
                HttpClient Client = new HttpClient(Handler);
                if (headers != null)
                    foreach (KeyValuePair<string, string> header in headers)
                    {
                        Client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                if (timeout > 0)
                    Client.Timeout = new TimeSpan(0, 0, timeout);
                Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.67 Safari/537.36");
                return await Client.GetByteArrayAsync(url);
            }
        }
    }
}
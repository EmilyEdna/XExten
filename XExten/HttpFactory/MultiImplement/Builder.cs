using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XExten.HttpFactory.MultiInterface;
using XExten.XCore;

namespace XExten.HttpFactory.MultiImplement
{
    /// <summary>
    /// 构建器
    /// </summary>
    public class Builder : IBuilder
    {
        /// <summary>
        /// 构建
        /// </summary>
        /// <param name="TimeOut">超时:秒</param>
        /// <returns></returns>
        public IBuilder Build(int TimeOut = 60)
        {
            if (HttpMultiClientWare.WeightPath.FirstOrDefault().URL == null)
                throw new Exception("Request address is not set!");
            if (HttpMultiClientWare.Container != null)
            {
                HttpClientHandler Handler = new HttpClientHandler
                {
                    AllowAutoRedirect = true,
                    UseCookies = true,
                    CookieContainer = HttpMultiClientWare.Container
                };
                HttpClient Client = new HttpClient(Handler);
                if (HttpMultiClientWare.HeaderMaps.Count != 0)
                    HttpMultiClientWare.HeaderMaps.ForEach(item =>
                    {
                        foreach (var KeyValuePair in item)
                        {
                            Client.DefaultRequestHeaders.Add(KeyValuePair.Key, KeyValuePair.Value);
                        }
                    });
                HttpMultiClientWare.FactoryClient = Client;
            }
            else
            {
                HttpClient Client = new HttpClient();
                if (HttpMultiClientWare.HeaderMaps.Count != 0)
                    HttpMultiClientWare.HeaderMaps.ForEach(item =>
                    {
                        foreach (var KeyValuePair in item)
                        {
                            Client.DefaultRequestHeaders.Add(KeyValuePair.Key, KeyValuePair.Value);
                        }
                    });
                HttpMultiClientWare.FactoryClient = Client;
            }
            HttpMultiClientWare.FactoryClient.Timeout = new TimeSpan(0, 0, TimeOut);
            return HttpMultiClientWare.Builder;
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        public List<Byte[]> RunBytes()
        {
            List<Byte[]> Result = new List<Byte[]>();
            HttpMultiClientWare.WeightPath.OrderByDescending(t => t.Weight).ToEachs(item =>
            {
                if (item.Request == RequestType.GET)
                    Result.Add(HttpMultiClientWare.FactoryClient.GetAsync(item.URL).Result.Content.ReadAsByteArrayAsync().Result);
                else if (item.Request == RequestType.DELETE)
                    Result.Add(HttpMultiClientWare.FactoryClient.DeleteAsync(item.URL).Result.Content.ReadAsByteArrayAsync().Result);
                else if (item.Request == RequestType.POST)
                    Result.Add(HttpMultiClientWare.FactoryClient.PostAsync(item.URL, item.Contents).Result.Content.ReadAsByteArrayAsync().Result);
                else
                    Result.Add(HttpMultiClientWare.FactoryClient.PutAsync(item.URL, item.Contents).Result.Content.ReadAsByteArrayAsync().Result);
            });
            HttpMultiClientWare.FactoryClient.Dispose();
            return Result;
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        public async Task<List<Byte[]>> RunBytesAsync()
        {
            return await Task.FromResult(RunBytes());
        }

        /// <summary>
        /// 执行 default UTF-8
        /// </summary>
        /// <returns></returns>
        public List<string> RunString()
        {
            List<string> Result = new List<string>();
            HttpMultiClientWare.WeightPath.OrderByDescending(t => t.Weight).ToEachs(item =>
            {
                if (item.Request == RequestType.GET)
                    Result.Add(HttpMultiClientWare.FactoryClient.GetAsync(item.URL).Result.Content.ReadAsStringAsync().Result);
                else if (item.Request == RequestType.DELETE)
                    Result.Add(HttpMultiClientWare.FactoryClient.DeleteAsync(item.URL).Result.Content.ReadAsStringAsync().Result);
                else if (item.Request == RequestType.POST)
                    Result.Add(HttpMultiClientWare.FactoryClient.PostAsync(item.URL, item.Contents).Result.Content.ReadAsStringAsync().Result);
                else
                    Result.Add(HttpMultiClientWare.FactoryClient.PutAsync(item.URL, item.Contents).Result.Content.ReadAsStringAsync().Result);
            });
            HttpMultiClientWare.FactoryClient.Dispose();
            return Result;
        }

        /// <summary>
        /// 执行 default UTF-8
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> RunStringAsync()
        {
            return await Task.FromResult(RunString());
        }
    }
}

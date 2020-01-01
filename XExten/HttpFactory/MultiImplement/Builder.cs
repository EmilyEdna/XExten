using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XExten.CacheFactory;
using XExten.HttpFactory.MultiInterface;
using XExten.XCore;

namespace XExten.HttpFactory.MultiImplement
{
    /// <summary>
    /// 构建器
    /// </summary>
    public class Builder : IBuilder
    {

        private static int CacheSecond = 30;

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
        /// 设置缓存时间
        /// </summary>
        /// <param name="CacheSeconds">单位：Seconds</param>
        /// <returns></returns>
        public IBuilder CacheTime(int CacheSeconds = 60)
        {
            CacheSecond = CacheSeconds;
            return this;
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
                if (item.UseCache)
                {
                    var Data = Caches.RunTimeCacheGet<Byte[]>(item.URL.AbsoluteUri);
                    if (Data == null)
                    {
                        Result.Add(RequestBytes(item));
                        Caches.RunTimeCacheSet(item.URL.AbsoluteUri, Result.FirstOrDefault(), CacheSecond, true);
                    }
                    else
                        Result.Add(Data);
                }
                else
                    Result.Add(RequestBytes(item));
            });
            Dispose();
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
                if (item.UseCache)
                {
                    var Data = Caches.RunTimeCacheGet<string>(item.URL.AbsoluteUri);
                    if (Data.IsNullOrEmpty())
                    {
                        Result.Add(RequestString(item));
                        Caches.RunTimeCacheSet(item.URL.AbsoluteUri, Result.FirstOrDefault(), CacheSecond, true);
                    }
                    else
                        Result.Add(Data);
                }
                else
                    Result.Add(RequestString(item));
            });
            Dispose();
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

        /// <summary>
        /// 请求
        /// </summary>
        /// <param name="Item"></param>
        /// <returns></returns>
        private string RequestString(WeightURL Item)
        {
            if (Item.Request == RequestType.GET)
                return HttpMultiClientWare.FactoryClient.GetAsync(Item.URL).Result.Content.ReadAsStringAsync().Result;
            else if (Item.Request == RequestType.DELETE)
                return HttpMultiClientWare.FactoryClient.DeleteAsync(Item.URL).Result.Content.ReadAsStringAsync().Result;
            else if (Item.Request == RequestType.POST)
                return HttpMultiClientWare.FactoryClient.PostAsync(Item.URL, Item.Contents).Result.Content.ReadAsStringAsync().Result;
            else
                return HttpMultiClientWare.FactoryClient.PutAsync(Item.URL, Item.Contents).Result.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// 请求
        /// </summary>
        /// <param name="Item"></param>
        /// <returns></returns>
        private Byte[] RequestBytes(WeightURL Item)
        {
            if (Item.Request == RequestType.GET)
                return HttpMultiClientWare.FactoryClient.GetAsync(Item.URL).Result.Content.ReadAsByteArrayAsync().Result;
            else if (Item.Request == RequestType.DELETE)
                return HttpMultiClientWare.FactoryClient.DeleteAsync(Item.URL).Result.Content.ReadAsByteArrayAsync().Result;
            else if (Item.Request == RequestType.POST)
                return HttpMultiClientWare.FactoryClient.PostAsync(Item.URL, Item.Contents).Result.Content.ReadAsByteArrayAsync().Result;
            else
                return HttpMultiClientWare.FactoryClient.PutAsync(Item.URL, Item.Contents).Result.Content.ReadAsByteArrayAsync().Result;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        private void Dispose()
        {
            HttpMultiClientWare.FactoryClient.Dispose();
            HttpMultiClientWare.Container = null;
            HttpMultiClientWare.HeaderMaps.Clear();
            HttpMultiClientWare.WeightPath.Clear();
        }
    }
}

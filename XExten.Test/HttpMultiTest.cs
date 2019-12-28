using System;
using System.Collections.Generic;
using System.Text;
using XExten.HttpFactory;
using Xunit;

namespace XExten.Test
{
    public class HttpMultiTest
    {
        [Fact]
        public void Test1()
        {
            var data = HttpMultiClient.HttpMulti.AddNode("https://www.baidu.com")
                .AddNode("https://fanyi.baidu.com/?aldtype=16047#auto/zh")
                .Build().RunString();
        }
        [Fact]
        public void Test2()
        {
            var data = HttpMultiClient.HttpMulti
                .InitCookieContainer()
                .Headers("name","test")
                .AddNode("https://api.uixsj.cn/hitokoto/w.php?code=json",RequestType.GET,true)
                .Build().CacheTime().RunBytes();

            var data1 = HttpMultiClient.HttpMulti
              .InitCookieContainer()
              .Headers("name", "test")
              .AddNode("https://api.uixsj.cn/hitokoto/w.php?code=json", RequestType.GET, true)
              .Build().CacheTime().RunBytes();
        }
    }
}

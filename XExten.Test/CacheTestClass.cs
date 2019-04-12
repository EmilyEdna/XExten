using System;
using System.Collections.Generic;
using System.Text;
using XExten.CacheFactory;
using XExten.Test.TestModel;
using Xunit;

namespace XExten.Test
{
    public class CacheTestClass
    {
        [Fact]
        public void MemoryCache_Test()
        {
            TestA A = new TestA { Id = 1, Name = "123", PassWord = "123" };
            Caches.SetCache("key", A);
            var res = Caches.GetCache<TestA>("key");
        }
        [Fact]
        public void RedisCache_Test()
        {
            Caches.CacheType = 1;
            Caches.ConnectionString = "127.0.0.1:6379";
            TestA A = new TestA { Id = 1, Name = "123", PassWord = "123" };
            Caches.SetCache("key", A);
            var res = Caches.GetCache<TestA>("key");
        }
    }
}

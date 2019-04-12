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
            Caches.RunTimeCacheSet("key", A);
            Caches.RunTimeCacheGet<TestA>("key");
            Caches.RunTimeCacheRemove("key");
        }
        [Fact]
        public void RedisCache_Test()
        {
            Caches.RedisConnectionString = "127.0.0.1:6379";
            TestA A = new TestA { Id = 1, Name = "123", PassWord = "123" };
            Caches.RedisCacheSet("key", A);
            Caches.RedisCacheGet<TestA>("key");
            Caches.RedisCacheRemove("key");
        }
        [Fact]
        public void MongoDbCache_Test() {
            Caches.MongoDBConnectionString = "mongodb://sa:123@127.0.0.1";
            Caches.DbName = "Test";
            TestA A = new TestA { Id = 1, Name = "123", PassWord = "123" };
            Caches.MongoDBCacheSet<TestA>(A);
            Caches.MongoDBCacheGet<TestA>(t => t.Id == 1);
            Caches.MongoDBCacheRemove<TestA>(t => t.Id == 1);
        }
    }
}

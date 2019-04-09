using System;
using System.Collections.Generic;
using XExten.Test.TestModel;
using XExten.XCore;
using Xunit;

namespace XExten.Test
{
    public class LinqXTestClass
    {
        [Fact]
        public void ByUnic_Test()
        {
            string str = "我是测试文字";
            str.ByUnic();
        }
        [Fact]
        public void ByUTF8_Test()
        {
            string str = "\\u6211\\u662f\\u6d4b\\u8bd5\\u6587\\u5b57";
            str.ByUTF8();
        }
        [Fact]
        public void ByMap_Test()
        {
            TestA A = new TestA() { Id = 1, Name = "admin" };
            A.ByMap<TestA, TestB>();
        }
        [Fact]
        public void ByNames_Test()
        {
            TestA A = new TestA();
            A.ByNames();
        }
        [Fact]
        public void ByValues_Test()
        {
            TestA A = new TestA() { Id = 1, Name = "admin" };
            A.ByValues();
        }
        [Fact]
        public void ByDic_Test()
        {
            TestA A = new TestA() { Id = 1, Name = "admin" };
            A.ByDic();
        }
        [Fact]
        public void Null_Test()
        {
            List<TestA> Li = new List<TestA>();
            Li.IsNullOrEmpty();
        }
        [Fact]
        public void ByDes_Test()
        {
            TestA A = new TestA();
            A.ByDes(t => t.Id);
        }
        [Fact]
        public void BySet_Test()
        {
            TestA A = new TestA();
            A.BySet(t => t.Id, 1);
        }
        [Fact]
        public void BySend_Test()
        {
            List<int> Li = new List<int>() { 1, 2 };
            var p = Li.BySend(t => t + 1);
        }
        [Fact]
        public void ByPage_Test()
        {
            List<int> Li = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Li.ByPage(1, 3);
        }
        [Fact]
        public void ByDicEach_Test()
        {
            Dictionary<int, string> keyValues = new Dictionary<int, string>
            {
                { 1, "张三" },
                { 2, "李四" }
            };
            keyValues.ByDicEach((t, k) =>
            {
                int key = t;
                string value = k;
            });
        }
        [Fact]
        public void ByOver_Test()
        {
            List<TestA> Li = new List<TestA>();
            Li.ByOver(t => t.Name);
        }
        [Fact]
        public void ByTable_Test()
        {
            TestA A = new TestA { Id = 1, Name = "张三" };
            List<TestA> Li = new List<TestA>
            {
                A
            };
            var dt = A.ByTable();
            var dts = Li.ByTables();
            var A1 = dt.ByEntity<TestA>();
            var As = dts.ByEntities<TestA>();
        }
        [Fact]
        public void ByJsonAndObject_Test()
        {
            TestA A = new TestA { Id = 1, Name = "mike" };
            var res = A.ByJson();
            var data = res.ByModel<TestA>();
        }
        [Fact]
        public void ByCache_Test()
        {
            TestA A = new TestA
            {
                Id = 1,
                Name = "123",
                PassWord = "123"
            };
            A.ByCache();
            var res = A.ByCacheData();
            LinqX.ClearCache();
        }
    }
}

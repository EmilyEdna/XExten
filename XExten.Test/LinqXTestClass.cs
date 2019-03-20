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
        public void GetExpression_Test()
        {
            string[] arr = new[] { "Id", "Name" };
            LinqX.GetExpression<TestA>(arr);

        }
        [Fact]
        public void SetProptertiesValue_Test()
        {
            Dictionary<string, Object> keyValues = new Dictionary<string, Object>
            {
                {"Id" , 1 },
                { "Name", "李四" }
            };
            TestA A = new TestA();
            LinqX.SetProptertiesValue<TestA>(keyValues, A);
        }
        [Fact]
        public void GetGetExpression_Test2()
        {
            TestA A = new TestA { Id = 10, Name = "测试" };
            LinqX.GetExpression<TestA>("Name", "123", QType.NotLike);
        }
        [Fact]
        public void ByOver_Test()
        {
            List<TestA> Li = new List<TestA>();
            Li.ByOver(t => t.Name);
        }
    }
}

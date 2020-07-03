using System.Collections.Generic;
using XExten.Test.TestModel;
using XExten.XCore;
using Xunit;

namespace XExten.Test
{
    public class LinqXTestClass
    {
        [Fact]
        public void ToUnic_Test()
        {
            string str = "我是测试文字";
            str.ToUnicode();
        }

        [Fact]
        public void ToUTF8_Test()
        {
            string str = "\\u6211\\u662f\\u6d4b\\u8bd5\\u6587\\u5b57";
            str.ToUTF8();
        }

        [Fact]
        public void ToMap_Test()
        {
            TestA A = new TestA() { Id = 1, Name = "admin" };
            A.ToMapper<TestA, TestB>();
        }

        [Fact]
        public void ToAutoMap_Test()
        {
            TestA A = new TestA() { Id = 1, Name = "admin" };
            var res = A.ToAutoMapper<TestB>();
        }

        [Fact]
        public void ToAutoMaps_Test()
        {
            TestA A = new TestA() { Id = 1, Name = "admin" };
            List<TestA> LA = new List<TestA>() { A };
            var res = LA.ToAutoMapper<TestA,TestB>();
        }

        [Fact]
        public void ToNames_Test()
        {
            TestA A = new TestA();
            A.ToNames();
        }

        [Fact]
        public void ToValues_Test()
        {
            TestA A = new TestA() { Id = 1, Name = "admin" };
            A.ToValues();
        }

        [Fact]
        public void ToDic_Test()
        {
            TestA A = new TestA() { Id = 1, Name = "admin" };
            A.ToDic();
        }

        [Fact]
        public void Null_Test()
        {
            List<TestA> Li = new List<TestA>();
            Li.IsNullOrEmpty();
        }

        [Fact]
        public void ToDes_Test()
        {
            TestA A = new TestA();
            A.ToDes(t => t.Id);
        }

        [Fact]
        public void ToSet_Test()
        {
            TestA A = new TestA();
            A.ToSet(t => t.Id, 1);
        }

        [Fact]
        public void ToPage_Test()
        {
            List<int> Li = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Li.ToPage(1, 3);
        }

        [Fact]
        public void ToDicEach_Test()
        {
            Dictionary<int, string> keyValues = new Dictionary<int, string>
            {
                { 1, "张三" },
                { 2, "李四" }
            };
            keyValues.ToDicEach((t, k) =>
            {
                int key = t;
                string value = k;
            });
        }

        [Fact]
        public void ToOver_Test()
        {
            var T = new TestA
            {
                Id = 1,
                Name = "张三",
                PassWord = "123"
            };
            var T1 = new TestA
            {
                Id = 1,
                Name = "李四",
                PassWord = "123"
            };
            List<TestA> Li = new List<TestA>() { T, T1 };
            var res = Li.ToOver(t => t.Name);
        }

        [Fact]
        public void ToTable_Test()
        {
            TestA A = new TestA { Id = 1, Name = "张三" };
            List<TestA> Li = new List<TestA>
            {
                A
            };
            var dt = A.ToTable();
            var dts = Li.ToTables();
            var A1 = dt.ToEntity<TestA>();
            var As = dts.ToEntities<TestA>();
        }

        [Fact]
        public void ToJsonAndObject_Test()
        {
            TestA A = new TestA { Id = 1, Name = "mike" };
            var res = A.ToJson();
            var data = res.ToModel<TestA>();
        }

        [Fact]
        public void Encryption_Test()
        {
            string name = "jojo";
            var a1 = name.ToLzStringEnc();
            var r1 = a1.ToLzStringDec();
            var a2 = name.ToMD5();
            var a3 = name.ToSHA(256);
        }
        [Fact]
        public void ToSelectDes_Test()
        {
            var res = TestC.A.ToDescription();
        }
        [Fact]
        public void ToMsgByte_Test()
        {
            var test = new
            {
                Id = 1,
                Name = "emily",
                PassWord = "emily"
            };
            var bytes = test.ToMsgByte();
            var json1 = test.ToMsgJson();
            var json = bytes.ToMsgJson();
            var model = bytes.ToMsgModel<object>();
        }
    }
}
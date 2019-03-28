using System;
using System.Collections.Generic;
using System.Text;
using XExten.Test.TestModel;
using XExten.XExpres;
using Xunit;

namespace XExten.Test
{
    public class XExpTestClass
    {
        [Fact]
        public void SetProptertiesValue_Test()
        {
            Dictionary<string, Object> keyValues = new Dictionary<string, Object>
            {
                {"Id" , 1 },
                { "Name", "李四" }
            };
            TestA A = new TestA();
            XExp.SetProptertiesValue<TestA>(keyValues, A);
        }
        [Fact]
        public void GetExpression_Test()
        {
            string[] arr = new[] { "Id", "Name" };
            XExp.GetExpression<TestA>(arr);
        }
        [Fact]
        public void GetExpression_Test1()
        {
            TestA A = new TestA { Id = 10, Name = "测试" };
            XExp.GetExpression<TestA>("Name", "123", QType.NotLike);
        }
        [Fact]
        public void GetExpression_Test2()
        {
            var res = XExp.GetExpression<TestA, TestB>((t, x) => new { t, x });
        }
    }
}

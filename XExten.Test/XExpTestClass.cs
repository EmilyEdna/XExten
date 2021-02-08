using System;
using System.Collections.Generic;
using XExten.DynamicType;
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
            XExpres.XExpres.SetProptertiesValue<TestA>(keyValues, A);
        }

        [Fact]
        public void GetExpression_Test()
        {
            string[] arr = new[] { "Id", "Name" };
            var res = XExpres.XExpres.GetExpression<TestA>(arr);
        }

        [Fact]
        public void GetExpression_Test1()
        {
            TestA A = new TestA { Id = 10, Name = "测试" };
            XExpres.XExpres.GetExpression<TestA>("Name", "123", QType.NotLike);
        }

        [Fact]
        public void GetCombineClass_Test()
        {
            var res = XExpres.XExpres.CombineClass<TestA, TestB>((t, x) => (new { t, x }));
            var instance = Activator.CreateInstance(res);
            //Instantiate first and assign later
        }

        [Fact]
        public void GetCombineClassValue_Test()
        {
            List<DynamicPropertyValue> dynamics = new List<DynamicPropertyValue>
            {
               new DynamicPropertyValue("Id",typeof(int),1)
            };
            var res = XExpres.XExpres.CombineClassWithValue<TestA, TestB>((t, x) => (new { t, x }), dynamics);
        }

        [Fact]
        public void CreateType_Test()
        {
            List<DynamicProperty> properties = new List<DynamicProperty> {
                new DynamicProperty("age",typeof(int))
            };
            var res = DynamicClassBuilder.Instance.GetDynamicClass(properties, "Test");
            var Instance = Activator.CreateInstance(res);
        }
    }
}
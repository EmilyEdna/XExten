using LinqX.Core.DynamicType;
using System;
using System.Dynamic;
using System.Linq;
using Xunit;

namespace LinqXTest
{
    public class PropertyTest
    {
        [Fact]
        public void SetProperty()
        {
            DynamicProperty[] property = new DynamicProperty[]
            {  new DynamicProperty("name", typeof(string)) };
            DynamicTypeBuilder builder = new DynamicTypeBuilder("textModel");
            var ps = builder.Create("textClass", property);
            var p = Activator.CreateInstance(typeof(ExpandoObject), true, new Object[] { "name", "Id" });
        }
    }
    public class TestClass
    {
        public string Name { get; set; }
    }
}

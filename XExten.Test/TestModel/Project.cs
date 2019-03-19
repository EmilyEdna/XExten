using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace XExten.Test.TestModel
{
    public class TestA
    {
        [Description("Test")]
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class TestB
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

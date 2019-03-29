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
        [Description("Test")]
        public string Name { get; set; }
        [Description("Test")]
        public string PassWord { get; set; }
    }
    public class TestB
    {
        [Description("Test")]
        public int Id { get; set; }
        [Description("Test")]
        public string Name { get; set; }
        [Description("Test")]
        public string Account { get; set; }
    }
}

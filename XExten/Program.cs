using XExten.XCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace XExten
{
    public class Program
    {
        public static void Main(string[] args)
        {
            List<TestA> ta = new List<TestA>();
            TestA t1 = new TestA();
            t1.Id = 1;
            t1.Name = "2";
            ta.Add(t1);
            var pp = ta.ByMap<TestA,TestB>();
        }

    }
    public class TestA
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class TestB
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using XExten.Office;
using XExten.Office.Enums;
using XExten.Test.TestModel;
using Xunit;

namespace XExten.Test
{
    public class ExcelTestClass
    {
        [Fact]
        public void ExcelExportTest()
        {
            List<TestA> data = new List<TestA> {
            new TestA
            {
                Id = 1,
                Name = "张三",
                PassWord = "123"
            },
            new TestA
            {
                Id = 2,
                Name = "里斯",
                PassWord = "456"
            }
        };
            using var fs = new FileStream("testA.xlsx", FileMode.Create);
            ExcelFactory.ExportExcel(data, ExcelType.xlsx, fs, "testA");
        }
    }
}

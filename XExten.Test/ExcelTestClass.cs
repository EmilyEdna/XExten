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
            List<TestD> data = new List<TestD> {
            new TestD
            {
                Id = 1,
                Name = "张三",
                IsMan = true,
                Tc=TestC.A
            },
            new TestD
            {
                Id = 2,
                Name = "里斯",
                IsMan = false,
                Tc=TestC.B
            }
        };
            using var fs = new FileStream("TestD.xlsx", FileMode.Create);
            ExcelFactory.ExportExcel(data, ExcelType.xlsx, fs, "TestD");
        }
        [Fact]
        public void ExcelImportTest()
        {
            using var fs = new FileStream("TestD.xlsx", FileMode.Open, FileAccess.Read);
            var data = ExcelFactory.ImportExcel<TestD>(fs, ExcelType.xlsx,true);
        }
    }
}

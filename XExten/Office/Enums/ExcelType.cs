using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace XExten.Office.Enums
{
    /// <summary>
    /// Excel 类型
    /// </summary>
    public enum ExcelType
    {
        [Description("Excel2003")]
        xls = 2003,
        [Description("Excel2007+")]
        xlsx = 2007
    }
}

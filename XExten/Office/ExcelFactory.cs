using Org.BouncyCastle.Crypto.Tls;
using XExten.Office.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using XExten.XCore;
using System.ComponentModel;
using XExten.Common;

namespace XExten.Office
{
    /// <summary>
    /// Excel工厂
    /// </summary>
    public class ExcelFactory
    {
        /// <summary>
        /// 导出EXCEL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Data"></param>
        /// <param name="Types"></param>
        /// <param name="St"></param>
        /// <param name="SheetName"></param>
        /// <param name="DateFormat"></param>
        public static void ExportExcel<T>(IEnumerable<T> Data, ExcelType Types, Stream St, string SheetName, string DateFormat = "yyyy-MM-dd") where T : class, new()
        {
            int Rows = Data.Count();//数据行
            var PropNames = Data.FirstOrDefault().ToNames();
            var Cols = PropNames.Count;
            IExcel excel = new Excel(Types, DateFormat);
            excel.CreateWorkBook().CreateSheet(SheetName);
            #region 创建头
            excel.CreateRows(0);
            for (int Col = 0; Col < Cols; Col++)
            {
                var First = Data.FirstOrDefault();
                var Index = PropNames[Col];
                var Name = First.ToAttribute<T, DescriptionAttribute>(Index,true).Description;
                var Ingore = First.ToAttribute<T, IgnoreMappedAttribute>(Index, true)?.Ingore;
                if (Ingore.HasValue == false || Ingore.Value == false)
                {
                    excel.CreateCells(Col, Name);
                }
            }
            excel.HeadStyle(Cols-1);
            #endregion
            #region 创建内容
            for (int Row = 1; Row <= Rows; Row++)
            {
                excel.CreateRows(Row);

                for (int Col = 0; Col < Cols; Col++)
                {
                    var First = Data.ToArray()[Row-1];
                    var Index = PropNames[Col];
                    var Ingore = First.ToAttribute<T, IgnoreMappedAttribute>(Index, true)?.Ingore;
                    if (Ingore.HasValue == false || Ingore.Value == false)
                    {
                       var data =  First.GetType().GetProperty(Index).GetValue(First);
                        excel.CreateCells(Col, data).BodyStyle(Row, Cols-1);
                    }
                }
            }
            #endregion
            excel.WriteStream(St);
        }
    }
}

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
        /// <param name="Data">需要被导出的数据</param>
        /// <param name="Types">Excel类型</param>
        /// <param name="St">流</param>
        /// <param name="SheetName">工作表名称</param>
        /// <param name="DateFormat">事件格式</param>
        public static void ExportExcel<T>(IEnumerable<T> Data, ExcelType Types, Stream St, string SheetName, string DateFormat = "yyyy-MM-dd") where T : class, new()
        {
            int Rows = Data.Count();//数据行
            var PropNames = Data.FirstOrDefault().ToNames();
            var Cols = PropNames.Count;
            List<string> NotIngoreNames = new List<string>();
            //获取忽略字段
            Data.FirstOrDefault().GetType().GetProperties().ToEachs(item =>
            {
                var Ingore = Data.FirstOrDefault().ToAttribute<T, IgnoreMappedAttribute>(item.Name, true)?.Ingore;
                if (Ingore != null && Ingore.Value == true)
                {
                    Cols -= 1;
                }
                else {
                    NotIngoreNames.Add(item.Name);
                }
            });
            if (Cols == 0)
                return;
            IExcel excel = new Excel(Types, DateFormat);
            excel.CreateWorkBook().CreateSheet(SheetName);
            #region 创建头
            excel.CreateRows(0);
            for (int Col = 0; Col < Cols; Col++)
            {
                var First = Data.FirstOrDefault();
                var Index = NotIngoreNames[Col];
                var Name = First.ToAttribute<T, DescriptionAttribute>(Index, true).Description;
                excel.CreateCells(Col, Name);
            }
            excel.HeadStyle(Cols - 1);
            #endregion
            #region 创建内容
            for (int Row = 1; Row <= Rows; Row++)
            {
                excel.CreateRows(Row);

                for (int Col = 0; Col < Cols; Col++)
                {
                    var First = Data.ToArray()[Row - 1];
                    var Index = NotIngoreNames[Col];
                    var data = First.GetType().GetProperty(Index).GetValue(First);
                    excel.CreateCells(Col, data).BodyStyle(Row, Cols - 1);
                }
            }
            #endregion
            #region 创建页脚
            excel.CreateRows(Rows + 1).CreateCells(0, "页脚");
            var LastCol = Cols - 1;
            var LastRow = Rows + 1;
            if (LastCol != 0) {
                excel.MergeCell(Rows + 1, Rows + 1, 0, LastCol).FootStyle(Rows + 1, LastCol);
            }
            else
                excel.FootStyle(Rows + 1, LastCol);
            #endregion
            excel.WriteStream(St);
        }
    }
}

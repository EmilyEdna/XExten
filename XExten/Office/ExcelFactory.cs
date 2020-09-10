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
                var Ingore = Data.FirstOrDefault().ToAttribute<T, OfficeAttribute>(item.Name, true)?.IngoreField;
                if (Ingore != null && Ingore.Value == true)
                {
                    Cols -= 1;
                }
                else
                {
                    NotIngoreNames.Add(item.Name);
                }
            });
            if (Cols == 0)
                return;
            IExcel excel = new Excel(Types, DateFormat);
            excel.CreateExportWorkBook().CreateExportSheet(SheetName);
            #region 创建头
            excel.CreateExportRows(0);
            for (int Col = 0; Col < Cols; Col++)
            {
                var First = Data.FirstOrDefault();
                var Index = NotIngoreNames[Col];
                var Name = First.ToAttribute<T, OfficeAttribute>(Index, true).MapperField;
                excel.CreateExportCells(Col, Name);
            }
            excel.HeadExportStyle(Cols - 1);
            #endregion
            #region 创建内容
            for (int Row = 1; Row <= Rows; Row++)
            {
                excel.CreateExportRows(Row);

                for (int Col = 0; Col < Cols; Col++)
                {
                    var First = Data.ToArray()[Row - 1];
                    var Index = NotIngoreNames[Col];
                    var data = First.GetType().GetProperty(Index).GetValue(First);
                    excel.CreateExportCells(Col, data).BodyExportStyle(Row, Cols - 1);
                }
            }
            #endregion
            #region 创建页脚
            excel.CreateExportRows(Rows + 1).CreateExportCells(0, "页脚");
            var LastCol = Cols - 1;
            var LastRow = Rows + 1;
            if (LastCol != 0)
            {
                excel.MergeExportCell(Rows + 1, Rows + 1, 0, LastCol).FootExportStyle(Rows + 1, LastCol);
            }
            else
                excel.FootExportStyle(Rows + 1, LastCol);
            #endregion
            excel.WriteExportStream(St);
        }
        /// <summary>
        /// 导入EXCEL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fs">流</param>
        /// <param name="Types">类型</param>
        /// <param name="HasPageFooter">文档是否有页脚</param>
        /// <param name="SheetIndex">数据表索引</param>
        /// <returns></returns>
        public static List<T> ImportExcel<T>(Stream fs, ExcelType Types,bool HasPageFooter=false, int SheetIndex = 0) where T: new()
        {
            IExcel excel = new Excel(fs, Types, HasPageFooter);
            var data = excel.CreateImportWorkBook().CreateImportSheet(SheetIndex).CreateImportHead<T>().CreateImportBody<T>().ImportData();
            return data.ToEntities<T>();
        }
    }
}

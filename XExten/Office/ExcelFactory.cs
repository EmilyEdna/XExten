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
using XExten.XPlus;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.Reflection;
using NPOI.SS.Util;
using NPOI.HSSF.UserModel;

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
                var Name = First.ToAttribute<T, OfficeAttribute>(Index, true)?.MapperField;
                if (Name.IsNullOrEmpty())
                    throw new NullReferenceException("实体未打上OfficeAttribute特性");
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
                    var Entity = First.GetType().GetProperty(Index);
                    var data = Entity.GetValue(First);
                    CreateDropDwonData(excel, Types, First.GetType().GetProperty(Index), Col, Col);
                    var WasEnum = (Entity.GetCustomAttribute(typeof(OfficeAttribute)) as OfficeAttribute).IsEnum;
                    if (WasEnum)
                    {
                        var Attr = Entity.PropertyType.GetField(data.ToString()).GetCustomAttribute(typeof(DescriptionAttribute));
                        if (Attr == null) throw new NullReferenceException("导出枚举对象时没有设置对应的DescriptionAttribute");
                        var result = (Attr as DescriptionAttribute).Description;
                        excel.CreateExportCells(Col, result).BodyExportStyle(Row, Cols - 1);
                    }
                    else
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
        public static List<T> ImportExcel<T>(Stream fs, ExcelType Types, bool HasPageFooter = false, int SheetIndex = 0) where T : new()
        {
            IExcel excel = new Excel(fs, Types, HasPageFooter);
            return excel.CreateImportWorkBook().CreateImportSheet(SheetIndex).CreateImportHead<T>().CreateImportBody<T>();
        }

        #region Private Function
        /// <summary>
        /// 获取下拉值
        /// </summary>
        /// <param name="Info"></param>
        /// <returns></returns>
        private static List<string> GetEnumDatas(PropertyInfo Info)
        {
            bool? WasEnum = (Info.GetCustomAttributes(typeof(OfficeAttribute), false).FirstOrDefault() as OfficeAttribute)?.IsEnum;
            if (WasEnum != null && WasEnum.Value == true)
            {
                List<string> Datas = new List<string>();
                Info.PropertyType.GetFields().ToEachs(Item =>
                {
                    var Attr = Item.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();
                    if (Attr != null)
                    {
                        var Des = (Attr as DescriptionAttribute).Description;
                        Datas.Add(Des);
                    }
                });
                return Datas;
            }
            return null;
        }
        /// <summary>
        /// EXCEL2007下拉值
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="data"></param>
        /// <param name="StarCol"></param>
        /// <param name="EndCol"></param>
        private static void CreateDropDwonListForXLSX(XSSFSheet sheet, List<string> data, int StarCol, int EndCol)
        {
            XSSFDataValidationHelper Validation = new XSSFDataValidationHelper(sheet);
            XSSFDataValidationConstraint Constraint = (XSSFDataValidationConstraint)Validation.CreateExplicitListConstraint(data.ToArray());
            CellRangeAddressList AddressList = new CellRangeAddressList(1, 65535, StarCol, EndCol);
            var XSSF = Validation.CreateValidation(Constraint, AddressList);
            sheet.AddValidationData(XSSF);
        }
        /// <summary>
        /// EXCEL2003下拉值
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="data"></param>
        /// <param name="StarCol"></param>
        /// <param name="EndCol"></param>
        private static void CreateDropDwonListForXLS(HSSFSheet sheet, List<string> data, int StarCol, int EndCol)
        {
            HSSFDataValidationHelper Validation = new HSSFDataValidationHelper(sheet);
            DVConstraint Constraint = (DVConstraint)Validation.CreateExplicitListConstraint(data.ToArray());
            CellRangeAddressList AddressList = new CellRangeAddressList(1, 65535, StarCol, EndCol);
            var HSSF = Validation.CreateValidation(Constraint, AddressList);
            sheet.AddValidationData(HSSF);
        }
        /// <summary>
        /// 创建下拉值
        /// </summary>
        /// <param name="excel"></param>
        /// <param name="type"></param>
        /// <param name="info"></param>
        /// <param name="StarCol"></param>
        /// <param name="EndCol"></param>
        private static void CreateDropDwonData(IExcel excel, ExcelType type, PropertyInfo info, int StarCol, int EndCol)
        {
            if (type == ExcelType.xlsx)
            {
                XSSFSheet Sheet = (excel.GetSheet() as XSSFSheet);
                var data = GetEnumDatas(info);
                if (data == null) return;
                CreateDropDwonListForXLSX(Sheet, data, StarCol, EndCol);
            }
            else
            {
                HSSFSheet Sheet = (excel.GetSheet() as HSSFSheet);
                var data = GetEnumDatas(info);
                if (data == null) return;
                CreateDropDwonListForXLS(Sheet, data, StarCol, EndCol);
            }
        }
        #endregion
    }
}

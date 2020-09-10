using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using XExten.Office.Enums;
using System;
using System.IO;
using System.Data;
using XExten.XCore;
using System.Collections.Generic;
using XExten.Common;
using System.Linq;

namespace XExten.Office
{
    /// <summary>
    /// Excel导出
    /// </summary>
    public partial class Excel : IExcel
    {
        #region NPOI
        private IWorkbook Workbook;
        private ISheet Sheet;
        private IRow Row;
        private ICell Cell;
        private ICellStyle HStyle;
        private ICellStyle BStyle;
        private ICellStyle FStyle;
        private ExcelType EnumType;
        private string DateFormat;
        private DataTable Table;
        private List<int> FullCols;
        private Stream St;
        private bool HasFooter;
        #endregion

        #region 导出
        /// <summary>
        /// 导出构造
        /// </summary>
        /// <param name="type"></param>
        /// <param name="Format"></param>
        public Excel(ExcelType type, string Format = "yyyy-MM-dd")
        {
            EnumType = type;
            DateFormat = Format;
        }
        /// <summary>
        /// 创建工作薄
        /// </summary>
        /// <returns></returns>
        public IExcel CreateExportWorkBook()
        {
            if (Workbook != null) return this;
            if (EnumType == ExcelType.xls)
                Workbook = new HSSFWorkbook();
            else
                Workbook = new XSSFWorkbook();
            return this;
        }
        /// <summary>
        /// 创建工作表
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IExcel CreateExportSheet(string name)
        {
            CheckWorkBook();
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("参数不能为空");
            Sheet = Workbook.GetSheet(name) ?? Workbook.CreateSheet(name);
            return this;
        }
        /// <summary>
        /// 创建行
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IExcel CreateExportRows(int index)
        {
            Row = Sheet.GetRow(index) ?? Sheet.CreateRow(index);
            return this;
        }
        /// <summary>
        /// 创建单元格
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IExcel CreateExportCells(int index, object value)
        {
            Cell = Row.GetCell(index) ?? Row.CreateCell(index);
            if (value.GetType() == typeof(DateTime))
            {
                Cell.SetCellValue(((DateTime)value).ToString(DateFormat));
                Cell.CellStyle.DataFormat = Workbook.CreateDataFormat().GetFormat(DateFormat);
            }
            Cell.SetCellValue(value.ToString());
            return this;
        }
        /// <summary>
        /// 表头样式
        /// </summary>
        /// <param name="EndCol"></param>
        /// <returns></returns>
        public IExcel HeadExportStyle(int EndCol)
        {
            HStyle = Workbook.CreateCellStyle();
            SetBorder(HStyle);
            SetBorderColor(HStyle);
            SetAlignment(HStyle);
            SetFont(HStyle);
            SetStyle(0, 0, 0, EndCol, HStyle);
            return this;
        }
        /// <summary>
        /// 内容样式
        /// </summary>
        /// <param name="EndRow"></param>
        /// <param name="EndCol"></param>
        /// <returns></returns>
        public IExcel BodyExportStyle(int EndRow, int EndCol)
        {
            BStyle = Workbook.CreateCellStyle();
            SetBorder(BStyle);
            SetBorderColor(BStyle, false);
            SetAlignment(BStyle);
            SetFont(BStyle, false);
            SetStyle(1, EndRow, 0, EndCol, BStyle);
            return this;
        }
        /// <summary>
        /// 页脚样式
        /// </summary>
        /// <param name="EndRow"></param>
        /// <param name="EndCol"></param>
        /// <returns></returns>
        public IExcel FootExportStyle(int EndRow, int EndCol)
        {
            FStyle = Workbook.CreateCellStyle();
            SetBorder(FStyle);
            SetBorderColor(FStyle, false);
            SetAlignment(FStyle);
            SetFont(FStyle, false);
            SetStyle(EndRow, EndRow, 0, EndCol, FStyle);
            return this;
        }
        /// <summary>
        /// 合并单元格
        /// </summary>
        /// <param name="SRI">起始行</param>
        /// <param name="ERI">结束行</param>
        /// <param name="SCI">起始列</param>
        /// <param name="ECI">结束列</param>
        /// <returns></returns>
        public IExcel MergeExportCell(int SRI, int ERI, int SCI, int ECI)
        {
            Sheet.AddMergedRegion(new CellRangeAddress(SRI, ERI, SCI, ECI));
            return this;
        }
        /// <summary>
        /// 写入文件流
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        public IExcel WriteExportStream(Stream st)
        {
            Workbook.Write(st);
            return this;
        }
        #endregion

        #region 导入
        /// <summary>
        /// 导入构造
        /// </summary>
        /// <param name="st"></param>
        /// <param name="type"></param>
        /// <param name="HasPageFooter"></param>
        public Excel(Stream st, ExcelType type, bool HasPageFooter)
        {
            St = st;
            EnumType = type;
            HasFooter = HasPageFooter;
            Table = new DataTable();
        }
        /// <summary>
        /// 创建导入工作薄
        /// </summary>
        /// <returns></returns>
        public IExcel CreateImportWorkBook()
        {
            if (St == null) return this;
            if (EnumType == ExcelType.xls)
                Workbook = new HSSFWorkbook(St);
            else
                Workbook = new XSSFWorkbook(St);
            return this;
        }
        /// <summary>
        /// 获取导入的工作表
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        public IExcel CreateImportSheet(int Index)
        {
            Sheet = Workbook.GetSheetAt(Index);
            return this;
        }
        /// <summary>
        /// 获取导入表格头
        /// </summary>
        /// <returns></returns>
        public IExcel CreateImportHead<T>()
        {
            Row = Sheet.GetRow(Sheet.FirstRowNum);
            Dictionary<string, string> FieldNames = new Dictionary<string, string>();
            FullCols = new List<int>();
            typeof(T).GetProperties().ToEachs(item =>
            {
                var AttrField = (item.GetCustomAttributes(typeof(OfficeAttribute), false).FirstOrDefault() as OfficeAttribute).MapperField;
                FieldNames.Add(AttrField, item.Name);
            });
            for (int index = 0; index < Row.LastCellNum; index++)
            {
                var HeaderKey = Row.GetCell(index).StringCellValue;
                if (!FieldNames.ContainsKey(HeaderKey))
                    FieldNames.Remove(HeaderKey);
                Table.Columns.Add(FieldNames[HeaderKey]);
                FullCols.Add(index);
            }
            return this;
        }
        /// <summary>
        /// 创建导入内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IExcel CreateImportBody<T>()
        {
            var SecondRow = Sheet.FirstRowNum + 1;
            var LastRow = HasFooter ? Sheet.LastRowNum - 1 : Sheet.LastRowNum;//排除页脚
            for (int index = SecondRow; index <= LastRow; index++)
            {
                if (Sheet.GetRow(index) == null)
                    return this;
                DataRow DtRow = Table.NewRow();
                foreach (var item in FullCols)
                {
                    DtRow[item] = Sheet.GetRow(index).GetCell(item).StringCellValue;
                }
                Table.Rows.Add(DtRow);
            }
            return this;
        }
        /// <summary>
        /// 导出数据
        /// </summary>
        /// <returns></returns>
        public DataTable ImportData()
        {
            return Table;
        }
        #endregion
    }
}

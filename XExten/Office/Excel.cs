using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using XExten.Office.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
        #endregion

        /// <summary>
        /// 构造
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
        public IExcel CreateWorkBook()
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
        public IExcel CreateSheet(string name)
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
        public IExcel CreateRows(int index)
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
        public IExcel CreateCells(int index, object value)
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
        public IExcel HeadStyle(int EndCol)
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
        public IExcel BodyStyle(int EndRow, int EndCol)
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
        public IExcel FootStyle(int EndRow, int EndCol)
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
        public IExcel MergeCell(int SRI, int ERI, int SCI, int ECI)
        {
            Sheet.AddMergedRegion(new CellRangeAddress(SRI, ERI, SCI, ECI));
            return this;
        }
        /// <summary>
        /// 写入文件流
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        public IExcel WriteStream(Stream st)
        {
            Workbook.Write(st);
            return this;
        }
    }
}

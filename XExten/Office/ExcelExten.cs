using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.Office
{
    public partial class Excel
    {
        /// <summary>
        /// 检查工作薄是否被创建
        /// </summary>
        private void CheckWorkBook()
        {
            if (Workbook == null) throw new NullReferenceException("CreateWorkBook()是否优先被调用过");
        }
        /// <summary>
        /// 创建边框颜色
        /// </summary>
        private void SetBorderColor(ICellStyle Style, bool IsHead = true)
        {
            Style.TopBorderColor = IsHead ? HSSFColor.LightBlue.Index : HSSFColor.Black.Index;
            Style.LeftBorderColor = IsHead ? HSSFColor.LightBlue.Index : HSSFColor.Black.Index;
            Style.RightBorderColor = IsHead ? HSSFColor.LightBlue.Index : HSSFColor.Black.Index;
            Style.BottomBorderColor = IsHead ? HSSFColor.LightBlue.Index : HSSFColor.Black.Index;
        }
        /// <summary>
        /// 创建布局方式
        /// </summary>
        private void SetAlignment(ICellStyle Style)
        {
            Style.Alignment = HorizontalAlignment.Center;
            Style.VerticalAlignment = VerticalAlignment.Center;
        }
        /// <summary>
        /// 创建边框
        /// </summary>
        private void SetBorder(ICellStyle Style)
        {

            Style.BorderBottom = BorderStyle.Thin;
            Style.BorderLeft = BorderStyle.Thin;
            Style.BorderRight = BorderStyle.Thin;
            Style.BorderTop = BorderStyle.Thin;
        }
        /// <summary>
        /// 设置字体
        /// </summary>
        private void SetFont(ICellStyle Style, bool IsHead = true)
        {
            IFont Font = Workbook.CreateFont();
            Font.Color = IsHead ? HSSFColor.Red.Index : HSSFColor.Black.Index;
            Font.IsBold = IsHead ? true : false;
            Style.SetFont(Font);
        }
        /// <summary>
        /// 为当前行设置样式
        /// </summary>
        /// <param name="StartRowIndex">起始行索引</param>
        /// <param name="EndRowIndex">结束行索引</param>
        /// <param name="StartColIndex">起始列索引</param>
        /// <param name="EndColIndex">结束列索引</param>
        /// <param name="Style">单元格样式</param>
        private void SetStyle(int StartRowIndex, int EndRowIndex, int StartColIndex, int EndColIndex, ICellStyle Style)
        {
            for (int row = StartRowIndex; row <= EndRowIndex; row++)
            {
                var Row = Sheet.GetRow(row) ?? Sheet.CreateRow(row);
                for (int col = StartColIndex; col <= EndColIndex; col++)
                {
                    var Cell = Row.GetCell(col) ?? Row.CreateCell(col);
                    Cell.CellStyle = Style;
                }
            }
        }
    }
}

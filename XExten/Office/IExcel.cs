using XExten.Office.Enums;
using System.IO;

namespace XExten.Office
{
    /// <summary>
    /// Excel导出
    /// </summary>
    public interface IExcel
    {
        /// <summary>
        /// 创建工作薄
        /// </summary>
        /// <returns></returns>
        IExcel CreateWorkBook();
        /// <summary>
        /// 创建工作表
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IExcel CreateSheet(string name);
        /// <summary>
        /// 创建行
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IExcel CreateRows(int index);
        /// <summary>
        /// 创建单元格
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IExcel CreateCells(int index, object value);
        /// <summary>
        /// 表头样式
        /// </summary>
        /// <param name="EndCol"></param>
        /// <returns></returns>
        IExcel HeadStyle(int EndCol);
        /// <summary>
        /// 内容样式
        /// </summary>
        /// <param name="EndRow"></param>
        /// <param name="EndCol"></param>
        /// <returns></returns>
        IExcel BodyStyle(int EndRow, int EndCol);
        /// <summary>
        /// 页脚样式
        /// </summary>
        /// <param name="EndRow"></param>
        /// <param name="EndCol"></param>
        /// <returns></returns>
        IExcel FootStyle(int EndRow, int EndCol);
        /// <summary>
        /// 合并单元格
        /// </summary>
        /// <param name="SRI">起始行</param>
        /// <param name="ERI">结束行</param>
        /// <param name="SCI">起始列</param>
        /// <param name="ECI">结束列</param>
        /// <returns></returns>
        IExcel MergeCell(int SRI, int ERI, int SCI, int ECI);
        /// <summary>
        /// 写入文件流
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        IExcel WriteStream(Stream st);
    }
}

using System.IO;
using System.Data;
using NPOI.SS.UserModel;
using System.Collections.Generic;

namespace XExten.Office
{
    /// <summary>
    /// Excel导出
    /// </summary>
    public interface IExcel
    {
        #region 获取对象
        /// <summary>
        /// 获取工作薄
        /// </summary>
        /// <returns></returns>
        IWorkbook GetWorkbook();
        /// <summary>
        /// 获取工作表
        /// </summary>
        /// <returns></returns>
        ISheet GetSheet();
        /// <summary>
        /// 获取行
        /// </summary>
        /// <returns></returns>
        IRow GetRow();
        /// <summary>
        /// 获取列
        /// </summary>
        /// <returns></returns>
        ICell GetCell();
        #endregion
        #region 导出
        /// <summary>
        /// 创建工作薄
        /// </summary>
        /// <returns></returns>
        IExcel CreateExportWorkBook();
        /// <summary>
        /// 创建工作表
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IExcel CreateExportSheet(string name);
        /// <summary>
        /// 创建行
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IExcel CreateExportRows(int index);
        /// <summary>
        /// 创建单元格
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IExcel CreateExportCells(int index, object value);
        /// <summary>
        /// 表头样式
        /// </summary>
        /// <param name="EndCol"></param>
        /// <returns></returns>
        IExcel HeadExportStyle(int EndCol);
        /// <summary>
        /// 内容样式
        /// </summary>
        /// <param name="EndRow"></param>
        /// <param name="EndCol"></param>
        /// <returns></returns>
        IExcel BodyExportStyle(int EndRow, int EndCol);
        /// <summary>
        /// 页脚样式
        /// </summary>
        /// <param name="EndRow"></param>
        /// <param name="EndCol"></param>
        /// <returns></returns>
        IExcel FootExportStyle(int EndRow, int EndCol);
        /// <summary>
        /// 合并单元格
        /// </summary>
        /// <param name="SRI">起始行</param>
        /// <param name="ERI">结束行</param>
        /// <param name="SCI">起始列</param>
        /// <param name="ECI">结束列</param>
        /// <returns></returns>
        IExcel MergeExportCell(int SRI, int ERI, int SCI, int ECI);
        /// <summary>
        /// 写入文件流
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        IExcel WriteExportStream(Stream st);
        #endregion
        #region 导入
        /// <summary>
        /// 创建导入工作薄
        /// </summary>
        /// <returns></returns>
        IExcel CreateImportWorkBook();
        /// <summary>
        /// 获取导入的工作表
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        IExcel CreateImportSheet(int Index);
        /// <summary>
        /// 获取导入表格头
        /// </summary>
        /// <returns></returns>
        IExcel CreateImportHead<T>();
        /// <summary>
        /// 创建导入内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        List<T> CreateImportBody<T>() where T : new();
        #endregion
    }
}

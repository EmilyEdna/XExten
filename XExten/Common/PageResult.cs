using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XExten.Common
{
    /// <summary>
    /// 分页结果
    /// </summary>
    public class PageResult<T>
    {
        /// <summary>
        /// Total
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// CurrentPage
        /// </summary>
        public int CurrentPage { get; set; }
        /// <summary>
        /// <summary>
        /// TotalPage
        /// </summary>
        public int TotalPage { get; set; }
        /// <summary>
        ///  Result
        /// </summary>
        public List<T> Queryable { get; set; }
    }
}

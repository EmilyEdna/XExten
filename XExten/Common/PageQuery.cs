using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XExten.Common
{
    /// <summary>
    /// 分页查询
    /// </summary>
    public class PageQuery
    {
        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 条数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 关键字
        /// </summary>
        public Dictionary<String, Object> KeyWord { get; set; }
    }
}

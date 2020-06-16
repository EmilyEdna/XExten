using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.ProfileUI.Model.EFModel
{
    public class TraceModel
    {
        public int Id { get; set; }
        /// <summary>
        /// 追踪的数据
        /// </summary>
        public string Result { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}

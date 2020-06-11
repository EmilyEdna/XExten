using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace XExten.Profile.Tracing.Entry.Enum
{
    public enum SqlHandlerType
    {
        /// <summary>
        /// 查询
        /// </summary>
        [Description("Sql_Select")]
        Select,
        /// <summary>
        /// 更新
        /// </summary>
        [Description("Sql_Update")]
        Update,
        /// <summary>
        /// 删除
        /// </summary>
        [Description("Sql_Delete")]
        Delete,
        /// <summary>
        /// 新增
        /// </summary>
        [Description("Sql_Insert")]
        Insert,
        /// <summary>
        /// 存储过程
        /// </summary>
        [Description("Sql_Store")]
        Store,
        /// <summary>
        /// 其他
        /// </summary>
        [Description("Sql_Directive")]
        Onthor
    }
}

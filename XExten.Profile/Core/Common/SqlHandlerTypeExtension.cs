using System;
using System.Collections.Generic;
using System.Text;
using XExten.Profile.Tracing.Entry.Enum;
using XExten.XCore;

namespace XExten.Profile.Core.Common
{
    public static class SqlHandlerTypeExtension
    {
        /// <summary>
        /// 获取执行数据库命令时的操作类型
        /// </summary>
        /// <param name="SqlHandle"></param>
        /// <returns>操作类型</returns>
        public static string GetSqlHandlerType(this string SqlHandle)
        {
            return (SqlHandle.ToLower()) switch
            {
                "select" => SqlHandlerType.Select.ToDescription(),
                "update" => SqlHandlerType.Update.ToDescription(),
                "delete" => SqlHandlerType.Delete.ToDescription(),
                "insert" => SqlHandlerType.Insert.ToDescription(),
                "excute" => SqlHandlerType.Store.ToDescription(),
                _ => SqlHandlerType.Onthor.ToDescription(),
            };
        }
    }
}

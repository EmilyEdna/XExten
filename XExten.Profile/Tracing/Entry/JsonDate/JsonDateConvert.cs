using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.Profile.Tracing.Entry.JsonDate
{
    /// <summary>
    /// Json时间转换格式
    /// </summary>
    public class JsonDateConvert : IsoDateTimeConverter
    {
        /// <summary>
        /// 
        /// </summary>
        public JsonDateConvert() : base()
        {
            base.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        }
    }
}

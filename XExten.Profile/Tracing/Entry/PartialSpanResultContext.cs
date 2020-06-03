using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.Profile.Tracing.Entry
{
    public class PartialSpanResultContext
    {
        /// <summary>
        /// 结果
        /// </summary>
        public Object Results { get; private set; }

        public void SetResult(object data) 
        {
            Results = data;
        }
    }
}

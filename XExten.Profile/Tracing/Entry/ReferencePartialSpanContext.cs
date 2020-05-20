using System;
using System.Collections.Generic;
using System.Text;
using XExten.Profile.Tracing.Entry.Enum;

namespace XExten.Profile.Tracing.Entry
{
    public class ReferencePartialSpanContext
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public Guid RequirId { get; set; }
        /// <summary>
        /// 请求Id
        /// </summary>
        public Guid EntryServiceId { get; set; }
        /// <summary>
        /// 行为
        /// </summary>
        public string OperationName { get; set; }
        /// <summary>
        /// 组件
        /// </summary>
        public string Component { get; set; }
        /// <summary>
        /// 方式
        /// </summary>
        public ChannelLayerType LayerType { get; set; }
        /// <summary>
        /// 标签
        /// </summary>
        public PartialSpanContextTagCollection Tags { get; set; }
    }
}

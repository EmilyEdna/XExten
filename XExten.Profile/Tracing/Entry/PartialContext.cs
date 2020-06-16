using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using XExten.Profile.Tracing.Entry.Enum;
using XExten.Profile.Tracing.Entry.JsonDate;
using XExten.Profile.Tracing.Entry.Struct;

namespace XExten.Profile.Tracing.Entry
{
    public class PartialContext
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public Guid RequirId { get; }
        /// <summary>
        /// 行为
        /// </summary>
        public string OperationName { get; }
        /// <summary>
        /// 开始时间
        /// </summary>
        [JsonConverter(typeof(JsonDateConvert))]
        public DateTime BeginTime { get; }
        /// <summary>
        /// 通道类型
        /// </summary>
        public ChannelType Channel { get; }
        /// <summary>
        /// 头信息
        /// </summary>
        public List<HeaderValue> HeaderValue { get; }
        /// <summary>
        /// 跟踪Id
        /// </summary>
        public UniqueId TraceId { get; }
        /// <summary>
        /// 引用
        /// </summary>
        public ReferencePartialSpanContextCollection References { get; } = new ReferencePartialSpanContextCollection();
        /// <summary>
        /// 请求信息
        /// </summary>
        public PartialSpanContext Context { get; }
        /// <summary>
        /// 结果信息
        /// </summary>
        public PartialSpanResultContext ResultContext { get; }

        public PartialContext(UniqueId traceId, List<HeaderValue> headers, ChannelType channel, string operationName)
        {
            RequirId = Guid.NewGuid();
            TraceId = traceId;
            BeginTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            Channel = channel;
            HeaderValue = headers;
            OperationName = operationName;
            Context = new PartialSpanContext();
            ResultContext = new PartialSpanResultContext();
        }
    }
}

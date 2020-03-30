using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.TracingClient.Client.DataContract
{
    public class Span
    {

        public string SpanId { get; set; }

        public string TraceId { get; set; }

        public bool Sampled { get; set; }

        /// <summary>
        ///  操作名称
        /// </summary>
        public string OperationName { get; set; }

        /// <summary>
        /// 间隔时间：毫秒
        /// </summary>
        public long Duration { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTimeOffset StartTimestamp { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTimeOffset FinishTimestamp { get; set; }

        /// <summary>
        /// 日志
        /// </summary>
        public ICollection<Log> Logs { get; set; } = new List<Log>();

        /// <summary>
        /// 标签
        /// </summary>
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();

        /// <summary>
        /// 满载
        /// </summary>
        public ICollection<Baggage> Baggages { get; set; } = new List<Baggage>();

        /// <summary>
        /// 应用
        /// </summary>
        public ICollection<SpanReference> References { get; set; } = new List<SpanReference>();
    }
}

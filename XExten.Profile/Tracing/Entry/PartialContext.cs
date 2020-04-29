using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.Profile.Tracing.Entry
{
    public class PartialContext
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public Guid RequirId { get; set; }
        /// <summary>
        /// 行为
        /// </summary>
        public string OperationName { get; set; }
        private DateTime _BeginTime;
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginTime
        {
            get
            {
                return _BeginTime;
            }
            set
            {
                _BeginTime = Convert.ToDateTime(value.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }
        /// <summary>
        /// 通道类型
        /// </summary>
        public ChannelType Channel { get; set; }
        /// <summary>
        /// 头信息
        /// </summary>
        public List<HeaderValue> HeaderValue { get; set; }
        /// <summary>
        /// 请求信息
        /// </summary>
        public PartialSpanContext Context { get; set; }
    }
}

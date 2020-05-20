using System;
using System.Collections.Generic;
using System.Text;
using XExten.Profile.Tracing.Entry.Enum;

namespace XExten.Profile.Tracing.Entry
{
    public class PartialSpanContext
    {
        /// <summary>
        /// 方式
        /// </summary>
        public ChannelLayerType LayerType { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string URL { get; set; }
        /// <summary>
        /// 方法
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// 组件
        /// </summary>
        public string Component { get; set; }
        /// <summary>
        /// IP
        /// </summary>
        public string Router { get; set; }
        /// <summary>
        /// 路劲
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 请求时间
        /// </summary>
        public long PenddingStar { get; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        /// <summary>
        ///状态码
        /// </summary>
        public int StatusCode { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public long PenddingEnd { get; set; }
        /// <summary>
        /// 请求间隔
        /// </summary>
        public long RequestMilliseconds { get; set; }
        /// <summary>
        /// 标签
        /// </summary>
        public PartialSpanContextTagCollection Tags { get; } = new PartialSpanContextTagCollection();
        /// <summary>
        /// 异常信息
        /// </summary>
        public PartialSpanContextExceptionCollection Exceptions { get; } = new PartialSpanContextExceptionCollection();

        public PartialSpanContext Add(string key, string value)
        {
            Tags.Add(key, value);
            return this;
        }

        public PartialSpanContext Add(Exception exception)
        {
            Exceptions.Add(exception);
            return this;
        }
    }
}

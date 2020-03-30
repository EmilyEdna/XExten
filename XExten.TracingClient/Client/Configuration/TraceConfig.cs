using System;

namespace XExten.TracingClient.Client.Configuration
{
    /// <summary>
    /// 追踪链配置
    /// </summary>
    public class TraceConfig
    {

        /// <summary>
        /// 服务
        /// </summary>
        public string Service { get; set; }

        /// <summary>
        /// 服务依赖
        /// </summary>
        public string ServiceIdentity { get; set; }

        /// <summary>
        /// 采集的URL
        /// </summary>
        public string CollectorUrl { get; set; }

        /// <summary>
        /// 受限容量
        /// </summary>
        public int BoundedCapacity { get; set; }

        /// <summary>
        /// 连接数
        /// </summary>
        public int ConsumerCount { get; set; }

        /// <summary>
        /// 清空间隔
        /// </summary>
        public int FlushInterval { get; set; }

        /// <summary>
        /// 路由忽略正则
        /// </summary>
        public string[] IgnoredRoutesRegexPatterns { get; set; }

    }
}

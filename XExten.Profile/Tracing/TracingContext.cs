using System;
using System.Collections.Generic;
using System.Text;
using XExten.Profile.Abstractions;
using XExten.Profile.Tracing.Entry;
using XExten.XCore;

namespace XExten.Profile.Tracing
{
    public class TracingContext : ITracingContext
    {
        /// <summary>
        /// 创建请求
        /// </summary>
        /// <param name="operationName"></param>
        /// <param name="carrierHeader"></param>
        /// <returns></returns>
        public PartialContext CreateEntryPartialContext(string operationName, ICarrierHeaderCollection carrierHeader)
        {
            if (operationName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(operationName));
            return new PartialContext
            {
                RequirId = Guid.NewGuid(),
                OperationName = operationName,
                BeginTime = DateTime.Now,
                Channel = ChannelType.Entry,
                HeaderValue= carrierHeader.CurrentSpan,
               Context= new PartialSpanContext()
            };
        }

        /// <summary>
        /// 创建退出
        /// </summary>
        /// <param name="operationName"></param>
        /// <param name="networkAddress"></param>
        /// <param name="carrierHeader"></param>
        /// <returns></returns>
        public PartialContext CreateExitPartialContext(string operationName, string networkAddress, ICarrierHeaderCollection carrierHeader = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 创建本地
        /// </summary>
        /// <param name="operationName"></param>
        /// <returns></returns>
        public PartialContext CreateLocalPartialContext(string operationName)
        {
            throw new NotImplementedException();
        }

        public void Release(PartialContext partialContext)
        {
            throw new NotImplementedException();
        }
    }
}

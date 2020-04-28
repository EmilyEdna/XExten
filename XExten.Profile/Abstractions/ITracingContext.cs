using System;
using System.Collections.Generic;
using System.Text;
using XExten.Profile.Abstractions.Common;
using XExten.Profile.Tracing.Entry;

namespace XExten.Profile.Abstractions
{
    public interface ITracingContext: IDependency
    {
        /// <summary>
        /// 创建登录
        /// </summary>
        /// <param name="operationName"></param>
        /// <param name="carrierHeader"></param>
        /// <returns></returns>
        PartialContext CreateEntryPartialContext(string operationName, ICarrierHeaderCollection carrierHeader);
        /// <summary>
        /// 创建执行
        /// </summary>
        /// <param name="operationName"></param>
        /// <returns></returns>
        PartialContext CreateLocalPartialContext(string operationName);
        /// <summary>
        /// 创建退出
        /// </summary>
        /// <param name="operationName"></param>
        /// <param name="networkAddress"></param>
        /// <param name="carrierHeader"></param>
        /// <returns></returns>
        PartialContext CreateExitPartialContext(string operationName, string networkAddress, ICarrierHeaderCollection carrierHeader = default);
        void Release(PartialContext partialContext);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using XExten.Profile.Abstractions;
using XExten.Profile.Tracing.Entry;
using XExten.XCore;
using XExten.Profile.Tracing.Entry.Enum;
using XExten.Profile.Tracing.Entry.Struct;
using System.Threading;
using System.Linq;
using XExten.Profile.Core.Common;

namespace XExten.Profile.Tracing
{
    public class TracingContext : ITracingContext
    {
        private readonly IExitContextAccessor ExitAccessor;
        private readonly ILocalContextAccessor LocalAccessor;
        private readonly IEntryContextAccessor EntryAccessor;
        private readonly ThreadLocal<long> Sequence = new ThreadLocal<long>(() => 0);
        public TracingContext(IExitContextAccessor exit, ILocalContextAccessor local, IEntryContextAccessor entry)
        {
            ExitAccessor = exit;
            LocalAccessor = local;
            EntryAccessor = entry;
        }
        /// <summary>
        /// 创建请求
        /// </summary>
        /// <param name="operationName"></param>
        /// <param name="carrierHeader"></param>
        /// <returns></returns>
        public PartialContext CreateEntryPartialContext(string operationName, ICarrierHeaderCollection carrierHeader)
        {
            if (operationName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(operationName));
            PartialContext Partial = new PartialContext(GetTraceId(), carrierHeader.CurrentSpan, ChannelType.Entry, operationName);
            EntryAccessor.Context = Partial;
            return Partial;
        }

        /// <summary>
        /// 创建退出
        /// </summary>
        /// <param name="operationName"></param>
        /// <param name="networkAddress"></param>
        /// <param name="carrierHeader"></param>
        /// <returns></returns>
        public PartialContext CreateExitPartialContext(string operationName)
        {
            PartialContext Context = GetParentPartialContext(ChannelType.Exit);
            PartialContext Partial = new PartialContext(GetTraceId(Context), Context.HeaderValue, ChannelType.Exit, operationName);
            if (Context != null)
            {
                var ParentReference = Context.References.FirstOrDefault();
                ReferencePartialSpanContext Reference = new ReferencePartialSpanContext
                {
                    Component= Context.Context.Component,
                    EntryServiceId=Context.RequirId,
                    LayerType = Context.Context.LayerType,
                    OperationName = Context.OperationName,
                    Tags = Context.Context.Tags,
                    RequirId = Guid.NewGuid()
                };
                Partial.References.Add(Reference);
                if(ParentReference!=null) Partial.References.Add(ParentReference);
            }
            ExitAccessor.Context = Partial;
            return Partial;
        }

        /// <summary>
        /// 创建本地
        /// </summary>
        /// <param name="operationName"></param>
        /// <returns></returns>
        public PartialContext CreateLocalPartialContext(string operationName)
        {
            if (operationName == null) throw new ArgumentNullException(nameof(operationName));
            PartialContext Context = GetParentPartialContext(ChannelType.Local);
            PartialContext Partial = new PartialContext(GetTraceId(Context), Context.HeaderValue, ChannelType.Local, operationName);
            if (Context != null)
            {
                ReferencePartialSpanContext Reference = new ReferencePartialSpanContext
                {
                    Component= Context.Context.Component,
                    EntryServiceId=Context.RequirId,
                    LayerType=Context.Context.LayerType,
                    OperationName=Context.OperationName,
                    Tags=Context.Context.Tags,
                    RequirId=Guid.NewGuid()
                };
                Partial.References.Add(Reference);
            }
            LocalAccessor.Context = Partial;
            return Partial;
        }

        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="partialContext"></param>
        public void Release(PartialContext partialContext)
        {
            if (partialContext == null) return;
            partialContext.OpenUI();
            switch (partialContext.Channel)
            {
                case ChannelType.Entry:
                    EntryAccessor.Context = null;
                    break;
                case ChannelType.Local:
                    LocalAccessor.Context = null;
                    break;
                case ChannelType.Exit:
                    ExitAccessor.Context = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(ChannelType), partialContext.Channel, "Invalid Chanel");
            }
        }

        #region Private Method
        private PartialContext GetParentPartialContext(ChannelType channel)
        {
            switch (channel)
            {
                case ChannelType.Entry:
                    return null;
                case ChannelType.Local:
                    return EntryAccessor.Context;
                case ChannelType.Exit:
                    return LocalAccessor.Context ?? EntryAccessor.Context;
                default:
                    throw new ArgumentOutOfRangeException(nameof(ChannelType), channel, "Invalid Chanel");
            }
        }

        private UniqueId GetTraceId(PartialContext context = null) => context?.TraceId ?? new UniqueId(0, Thread.CurrentThread.ManagedThreadId, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1000 + GetSequence());

        private long GetSequence()
        {
            if (Sequence.Value++ >= 9999) return 0;
            return Sequence.Value;
        }
        #endregion
    }
}

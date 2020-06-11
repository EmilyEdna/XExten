using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using XExten.Profile.Abstractions;
using XExten.Profile.AspNetCore.DiagnosticProcessorName;
using XExten.Profile.Attributes;
using XExten.Profile.Core.Common;
using XExten.Profile.Tracing.Entry;
using XExten.Profile.Tracing.Entry.Enum;

namespace XExten.Profile.AspNetCore
{
    public class EFCoreTracingDiagnosticProcessor : ITracingDiagnosticProcessor
    {
        public string ListenerName { get; } = ProcessorName.EntityFrameworkCore;

        private readonly ITracingContext TracingContext;
        private readonly IExitContextAccessor ExitAccessor;
        private readonly ILocalContextAccessor LocalAccessor;
        private readonly IEnumerable<IEFCoreDiagnosticHandler> EFCoreDiagnosticHandler;

        public EFCoreTracingDiagnosticProcessor(ITracingContext tracingContext, IExitContextAccessor exitAccessor,
            ILocalContextAccessor localAccessor, IEnumerable<IEFCoreDiagnosticHandler> EfDiagnosticHandler)
        {
            TracingContext = tracingContext;
            ExitAccessor = exitAccessor;
            LocalAccessor = localAccessor;
            EFCoreDiagnosticHandler = EfDiagnosticHandler;
        }

        [DiagnosticName(ProcessorName.EntityFrameworkCoreCommandExecuting)]
        public void CommandExecuting([Object] CommandEventData EventData)
        {
            var CommandType = EventData.Command.CommandText?.Split(' ');
            var operationName = $"DB {CommandType.FirstOrDefault() ?? EventData.ExecuteMethod.ToString()}";
            PartialContext Context = CreateContext(EventData.Command, operationName);
            Context.Context.LayerType = ChannelLayerType.DB;
            Context.Context.Add("DataSource", EventData.Command.Connection.DataSource);
            Context.Context.Add("Type", (CommandType.FirstOrDefault() ?? EventData.ExecuteMethod.ToString()).GetSqlHandlerType());
            Context.Context.Add("DbInstance", EventData.Command.Connection.Database);
            Context.Context.Add("BindData", EventData.Command.Parameters?.FormatParameters(false));
            Context.Context.Add("TSQL", EventData.Command.CommandText);
        }

        [DiagnosticName(ProcessorName.EntityFrameworkCoreCommandExecuted)]
        public void CommandExecuted([Object] CommandExecutedEventData EventData)
        {
            if (EventData == null) return;
            var Context = GetCurrentContext(EventData.Command);
            Context.Context.Add("Duration", Math.Truncate(EventData.Duration.TotalMilliseconds).ToString());
            if (Context != null) TracingContext.Release(Context);
        }

        [DiagnosticName(ProcessorName.EntityFrameworkCoreCommandError)]
        public void CommandError([Object] CommandErrorEventData EventData)
        {
            if (EventData == null) return;
            var Context = GetCurrentContext(EventData.Command);
            if (Context != null)
            {
                Context?.Context?.Exceptions?.Add(EventData.Exception);
                TracingContext.Release(Context);
            }
        }

        private PartialContext CreateContext(DbCommand Command, string operationName)
        {
            PartialContext Context;
            foreach (var provider in EFCoreDiagnosticHandler)
            {
                if (provider.Match(Command.Connection))
                {
                    Context = TracingContext.CreateExitPartialContext(operationName);
                    Context.Context.Component = "EntityFrameWorkCore.Complate";
                    return Context;
                }
            }
            Context = TracingContext.CreateLocalPartialContext(operationName);
            Context.Context.Component = "EntityFrameWorkCore";
            return Context;
        }

        private PartialContext GetCurrentContext(DbCommand Command)
        {
            foreach (var provider in EFCoreDiagnosticHandler)
                if (provider.Match(Command.Connection))
                    return ExitAccessor.Context;
            return LocalAccessor.Context;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using XExten.Profile.Abstractions;
using XExten.Profile.AspNetCore.DiagnosticProcessorName;
using XExten.Profile.Attributes;
using System.Linq;
using XExten.Profile.Tracing.Entry.Enum;

namespace XExten.Profile.AspNetCore
{
    public class SqlClientTracingDiagnosticProcessor : ITracingDiagnosticProcessor
    {
        public string ListenerName { get; } = ProcessorName.SqlClient;

        private readonly ITracingContext TracingContext;
        private readonly IExitContextAccessor Accessor;
        public SqlClientTracingDiagnosticProcessor(ITracingContext tracingContext, IExitContextAccessor accessor)
        {
            TracingContext = tracingContext;
            Accessor = accessor;
        }

        [DiagnosticName(ProcessorName.SqlBeforeExecuteCommand)]
        public void BeforeExecuteCommand([Property(Name = "Command")] SqlCommand sqlCommand)
        {
            var commandType = $"SqlClient.{sqlCommand.CommandText?.Split(' ')?.FirstOrDefault()}";
            var ExitPartial = TracingContext.CreateExitPartialContext(commandType);
            ExitPartial.Context.Component = "SqlClient";
            ExitPartial.Context.LayerType = ChannelLayerType.DB;
            ExitPartial.Context.Add("DataSource", sqlCommand.Connection.DataSource);
            ExitPartial.Context.Add("Type", "SQL");
            ExitPartial.Context.Add("DbInstance", sqlCommand.Connection.Database);
            ExitPartial.Context.Add("Statement", sqlCommand.CommandText);
        }
        [DiagnosticName(ProcessorName.SqlAfterExecuteCommand)]
        public void AfterExecuteCommand()
        {
            var Context = Accessor.Context;
            if (Context == null) return;
            TracingContext.Release(Context);
        }
        [DiagnosticName(ProcessorName.SqlBeforeExecuteCommand)]
        public void ErrorExecuteCommand([Property(Name = "Exception")] Exception exception)
        {
            Accessor.Context?.Context?.Add(exception);
        }
    }
}

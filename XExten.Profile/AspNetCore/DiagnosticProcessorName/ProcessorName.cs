using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.Profile.AspNetCore.DiagnosticProcessorName
{
    public class ProcessorName
    {
        public const string MicrosoftAspNetCore = "Microsoft.AspNetCore";
        public const string BeginRequest = "Microsoft.AspNetCore.Hosting.BeginRequest";
        public const string EndRequest = "Microsoft.AspNetCore.Hosting.EndRequest";
        public const string UnhandledException = "Microsoft.AspNetCore.Diagnostics.UnhandledException";
        public const string HostingException = "Microsoft.AspNetCore.Hosting.UnhandledException";

        public const string EntityFrameworkCore = "Microsoft.EntityFrameworkCore";
        public const string EntityFrameworkCoreCommandExecuting = "Microsoft.EntityFrameworkCore.Database.Command.CommandExecuting";
        public const string EntityFrameworkCoreCommandExecuted = "Microsoft.EntityFrameworkCore.Database.Command.CommandExecuted";
        public const string EntityFrameworkCoreCommandError = "Microsoft.EntityFrameworkCore.Database.Command.CommandError";

        public const string SqlClient = "SqlClientDiagnosticListener";
        public const string SqlBeforeExecuteCommand = "System.Data.SqlClient.WriteCommandBefore";
        public const string SqlAfterExecuteCommand = "System.Data.SqlClient.WriteCommandAfter";
        public const string SqlErrorExecuteCommand = "System.Data.SqlClient.WriteCommandError";

        public const string MySqlClient = "MySqlClientDiagnosticListener";
        public const string MySqlBeforeExecuteCommand = "System.Data.MySqlClient.WriteCommandBefore";
        public const string MySqlAfterExecuteCommand = "System.Data.MySqlClient.WriteCommandAfter";
        public const string MySqlErrorExecuteCommand = "System.Data.MySqlClient.WriteCommandError";

        public const string NpgSqlClient = "Npgsql.Command";
        public const string NpgsqlExecuteCommandStart = "ExecuteCommandStart";
        public const string NpgsqlExecuteCommandStop = "ExecuteCommandStop";
        public const string NpgsqlExecuteCommandError = "ExecuteCommandError";

        public const string HttpClinet = "HttpHandlerDiagnosticListener";
        public const string HttpClientRequest = "System.Net.Http.Request";
        public const string HttpClientResponse = "System.Net.Http.Response";
        public const string HttpClientException = "System.Net.Http.Exception";

        public const string MethodClient = "MethodHandlerDiagnosticListener";
        public const string MethodBegin = "ExecuteCommandMethodStar";
        public const string MethodEnd = "ExecuteCommandMethodEnd";
        public const string MethodException = "ExecuteCommandMethodException";
    }
}

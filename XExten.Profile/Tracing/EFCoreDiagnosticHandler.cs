using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using XExten.Profile.Abstractions;

namespace XExten.Profile.Tracing
{
    public class EFCoreDiagnosticHandler : IEFCoreDiagnosticHandler
    {

        public string GetPeer(DbConnection connection)
        {
            return connection.DataSource;
        }

        public bool Match(DbConnection connection)
        {
            return connection.GetType().FullName switch
            {
                "Microsoft.Data.Sqlite.SqliteConnection" => true,
                "MySql.Data.MySqlClient.MySqlConnection" => true,
                "Npgsql.NpgsqlConnection" => true,
                "Microsoft.Data.SqlClient.SqlConnection" => true,
                _ => false,
            };
        }

    }
}

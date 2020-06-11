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
            throw new NotImplementedException();
        }

        public bool Match(DbConnection connection)
        {
            switch (connection.GetType().FullName)
            {
                case "Microsoft.Data.Sqlite.SqliteConnection":
                    return true;
                case "MySql.Data.MySqlClient.MySqlConnection":
                    return true;
                case "Npgsql.NpgsqlConnection":
                    return true;
                case "Microsoft.Data.SqlClient.SqlConnection":
                    return true;
                default:
                    return false;
            }
        }


    }
}

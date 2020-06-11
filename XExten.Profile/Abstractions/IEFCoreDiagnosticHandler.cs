using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using XExten.Profile.Abstractions.Common;

namespace XExten.Profile.Abstractions
{
    public interface IEFCoreDiagnosticHandler: IDependency
    {

        bool Match(DbConnection connection);

        string GetPeer(DbConnection connection);
    }
}

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using XExten.Profile.Abstractions.Common;

namespace XExten.Profile.Abstractions
{
    public interface IRequestDiagnosticHandler: IDependency
    {
        bool OnlyMatch(HttpRequestMessage request);

        void Handle(ITracingContext tracingContext, HttpRequestMessage request);
    }
}

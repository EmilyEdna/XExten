﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using XExten.Profile.Abstractions.Common;
using XExten.Profile.Tracing.Entry;

namespace XExten.Profile.Abstractions
{
    public interface IHostingDiagnosticHandler: IDependency
    {
        bool OnlyMatch(HttpContext httpContext);

        void BeginRequest(ITracingContext tracingContext, HttpContext httpContext);

        void EndRequest(PartialContext partialContext, HttpContext httpContext);
    }
}

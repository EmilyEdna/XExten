using System;
using System.Collections.Generic;
using System.Text;
using XExten.Profile.Abstractions.Common;

namespace XExten.Profile.Abstractions
{
    public interface IMethondDiagnosticHandler: IDependency
    {
        bool OnlyMatch(Object Data);
        void Handle(ITracingContext tracingContext, Object Provider);
    }
}

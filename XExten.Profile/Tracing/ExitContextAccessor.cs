using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using XExten.Profile.Abstractions;
using XExten.Profile.Tracing.Entry;

namespace XExten.Profile.Tracing
{
    public class ExitContextAccessor : IExitContextAccessor
    {
        private readonly AsyncLocal<PartialContext> PartialContext = new AsyncLocal<PartialContext>();
        public PartialContext Context { get => PartialContext.Value; set => PartialContext.Value = value; }
    }
}

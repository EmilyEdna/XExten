using System;
using System.Collections.Generic;
using System.Text;
using XExten.Profile.Abstractions;
using XExten.Profile.Tracing.Entry;

namespace XExten.Profile.Tracing
{
    public class TracingContext : ITracingContext
    {
        public PartialContext CreateEntryPartialContext(string operationName, ICarrierHeaderCollection carrierHeader)
        {
            throw new NotImplementedException();
        }

        public PartialContext CreateExitPartialContext(string operationName, string networkAddress, ICarrierHeaderCollection carrierHeader = null)
        {
            throw new NotImplementedException();
        }

        public PartialContext CreateLocalPartialContext(string operationName)
        {
            throw new NotImplementedException();
        }

        public void Release(PartialContext partialContext)
        {
            throw new NotImplementedException();
        }
    }
}

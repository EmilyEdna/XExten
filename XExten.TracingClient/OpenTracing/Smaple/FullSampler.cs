using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.TracingClient.OpenTracing.Smaple
{
    public class FullSampler : ISampler
    {
        public bool ShouldSample()
        {
            return true;
        }
    }
}

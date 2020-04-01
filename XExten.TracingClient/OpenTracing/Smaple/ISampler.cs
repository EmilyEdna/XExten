using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.TracingClient.OpenTracing.Smaple
{
    public interface ISampler
    {
        bool ShouldSample();
    }
}

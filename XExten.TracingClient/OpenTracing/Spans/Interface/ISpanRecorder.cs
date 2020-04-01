using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.TracingClient.OpenTracing.Spans.Interface
{
    public interface ISpanRecorder
    {
        void Record(ISpan span);
    }
}

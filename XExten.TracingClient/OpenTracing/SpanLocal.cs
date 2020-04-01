using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using XExten.TracingClient.OpenTracing.Spans.Interface;

namespace XExten.TracingClient.OpenTracing
{
    internal static class SpanLocal
    {
        private static readonly AsyncLocal<ISpan> AsyncLocal = new AsyncLocal<ISpan>();

        public static ISpan Current
        {
            get
            {
                return AsyncLocal.Value;
            }
            set
            {
                AsyncLocal.Value = value;
            }
        }
    }
}

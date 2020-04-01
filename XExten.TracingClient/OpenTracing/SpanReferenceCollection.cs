using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace XExten.TracingClient.OpenTracing
{
    public class SpanReferenceCollection : Collection<SpanReference>
    {
        public static readonly SpanReferenceCollection Empty = new SpanReferenceCollection();
    }
}

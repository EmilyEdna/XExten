using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.TracingClient.Client.DataContract
{
    public class SpanReference
    {
        public string Reference { get; set; }

        public string ParentId { get; set; }
    }
}

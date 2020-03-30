using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.TracingClient.Client.DataContract
{
    public class Log
    {
        public DateTimeOffset Timestamp { get; set; }

        public ICollection<LogField> Fields { get; set; } = new List<LogField>();
    }
}

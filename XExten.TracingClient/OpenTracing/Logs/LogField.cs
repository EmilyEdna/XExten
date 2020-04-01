using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.TracingClient.OpenTracing.Logs
{
    public class LogField : Dictionary<string, object>
    {
        public LogField() : base()
        {
        }

        public LogField(IDictionary<string, object> collection)
            : base(collection)
        {
        }

        public static LogField CreateNew()
        {
            return new LogField();
        }
    }
}

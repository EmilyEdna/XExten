using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.TracingClient.OpenTracing.Carrier.Interface
{
    public interface ICarrier : IEnumerable<KeyValuePair<string, string>>
    {
        string this[string key] { get; set; }
    }
}

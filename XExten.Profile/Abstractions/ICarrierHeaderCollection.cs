using System;
using System.Collections.Generic;
using System.Text;
using XExten.Profile.Tracing.Entry;

namespace XExten.Profile.Abstractions
{
    public interface ICarrierHeaderCollection : IEnumerable<KeyValuePair<string, string>>
    {
        List<KeyValuePair<string, string>> GetAll();
        void Add(string key, string value);
        List<HeaderValue> CurrentSpan { get; }
    }
}

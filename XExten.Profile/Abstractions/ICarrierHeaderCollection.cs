using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.Profile.Abstractions
{
    public interface ICarrierHeaderCollection : IEnumerable<KeyValuePair<string, string>>
    {
        void Add(string key, string value);
    }
}

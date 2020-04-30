using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace XExten.Profile.Tracing.Entry
{
    public class PartialSpanContextTagCollection : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly Dictionary<string, string> tags = new Dictionary<string, string>();

        internal void Add(string key, string value)
        {
            tags[key] = value;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return tags?.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return tags?.GetEnumerator();
        }
    }
}

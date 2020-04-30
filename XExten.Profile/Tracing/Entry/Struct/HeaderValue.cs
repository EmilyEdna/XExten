using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.Profile.Tracing.Entry.Struct
{
    public struct HeaderValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public HeaderValue(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}

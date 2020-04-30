using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using XExten.Profile.Abstractions;
using System.Linq;
using XExten.Profile.Tracing.Entry.Struct;

namespace XExten.Profile.Tracing
{
    public class CarrierHeaderCollection : ICarrierHeaderCollection
    {
        private readonly IEnumerable<KeyValuePair<string, string>> _headers;
        public CarrierHeaderCollection(HttpContext httpContext)
        {
            _headers = httpContext.Request.Headers.Select(t => new KeyValuePair<string, string>(t.Key, t.Value)).ToArray();
        }
        public List<HeaderValue> CurrentSpan => _headers.Select(t => new HeaderValue(t.Key, t.Value)).ToList();
        public void Add(string key, string value)
        {
            _headers.ToList().Add(new KeyValuePair<string, string>(key, value));
        }
        public List<KeyValuePair<string, string>> GetAll() => _headers.ToList();
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _headers?.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _headers?.GetEnumerator();
    }
}

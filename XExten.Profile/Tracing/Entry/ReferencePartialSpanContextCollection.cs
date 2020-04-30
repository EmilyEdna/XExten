using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace XExten.Profile.Tracing.Entry
{
    public class ReferencePartialSpanContextCollection : IEnumerable<ReferencePartialSpanContext>
    {
        private readonly HashSet<ReferencePartialSpanContext> References = new HashSet<ReferencePartialSpanContext>();

        public int Count => References.Count;

        public void Add(ReferencePartialSpanContext Context)
        {
            References.Add(Context);
        }

        public IEnumerator<ReferencePartialSpanContext> GetEnumerator()
        {
            return References?.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return References?.GetEnumerator();
        }
    }
}

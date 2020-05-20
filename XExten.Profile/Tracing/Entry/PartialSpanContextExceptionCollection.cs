using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace XExten.Profile.Tracing.Entry
{
    public class PartialSpanContextExceptionCollection : IEnumerable<Exception>
    {
        private readonly List<Exception> exceptions = new List<Exception>();

        public void Add(Exception exception) {
            exceptions.Add(exception);
        }

        public IEnumerator<Exception> GetEnumerator()
        {
            return exceptions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return exceptions.GetEnumerator();
        }
    }
}

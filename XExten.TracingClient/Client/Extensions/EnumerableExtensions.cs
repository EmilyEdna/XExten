using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XExten.TracingClient.Client.Extensions
{
    internal static class EnumerableExtensions
    {
        internal static IEnumerable<IEnumerable<T>> Chunked<T>(this IEnumerable<T> source, int chunkedCapacity)
        {
            while (source.Any())
            {
                yield return source.Take(chunkedCapacity);
                source = source.Skip(chunkedCapacity);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Twisted.Extensions
{
    public static class ImmutableExtensions
    {
        public static IReadOnlyList<T> AsReadOnly<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            var array = enumerable.ToArray();

            var collection = new ReadOnlyCollection<T>(array);

            return collection;
        }
    }
}
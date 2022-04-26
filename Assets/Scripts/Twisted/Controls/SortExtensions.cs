using System;
using System.Collections.Generic;
using System.Linq;

namespace Twisted.Controls
{
    public static class SortExtensions
    {
        public static IOrderedEnumerable<TSource> Sort<TSource, TKey>(
            this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey>? comparer, bool descending)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            return source is IOrderedEnumerable<TSource> enumerable
                ? enumerable.CreateOrderedEnumerable(selector, comparer, descending)
                : descending
                    ? source.OrderByDescending(selector, comparer)
                    : source.OrderBy(selector, comparer);
        }
    }
}
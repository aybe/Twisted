using System;
using System.Collections.Generic;

namespace Twisted.Extensions
{
    /// <summary>
    ///     Compares arrays by length and by content.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of elements in arrays to be compared.
    /// </typeparam>
    public sealed class ArrayComparer<T> : Comparer<T[]> where T : IComparable<T>
    {
        public static ArrayComparer<T> Instance { get; } = new ArrayComparer<T>();

        public override int Compare(T[]? x, T[]? y)
        {
            if (x == null && y == null)
            {
                return default;
            }

            if (x == null)
            {
                return int.MinValue;
            }

            if (y == null)
            {
                return int.MaxValue;
            }

            var length = x.Length.CompareTo(y.Length);

            if (length != default)
            {
                return length;
            }

            for (var i = 0; i < x.Length; i++)
            {
                var a = x[i];
                var b = y[i];
                var c = a.CompareTo(b);

                if (c != default)
                {
                    return c;
                }
            }

            return default;
        }
    }
}
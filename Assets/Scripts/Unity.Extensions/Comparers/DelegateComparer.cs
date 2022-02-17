using System;
using System.Collections.Generic;

namespace Unity.Extensions.Comparers
{
    public sealed class DelegateComparer<T> : Comparer<T>
    {
        public DelegateComparer(Func<T?, T?, int> compareFunc)
        {
            CompareFunc = compareFunc ?? throw new ArgumentNullException(nameof(compareFunc));
        }

        private Func<T?, T?, int> CompareFunc { get; }

        public override int Compare(T? x, T? y)
        {
            return CompareFunc(x, y);
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Unity.Extensions
{
    public static class TreeExtensions
    {
        public static IEnumerable<T> TraverseDfs<T>(T source, Func<T, IEnumerable<T>?> children) where T : class
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            if (children is null)
                throw new ArgumentNullException(nameof(children));

            var stack = new Stack<T>();

            stack.Push(source);

            while (stack.Count > 0)
            {
                var pop = stack.Pop();

                yield return pop;

                foreach (var item in children(pop) ?? Array.Empty<T>())
                {
                    Assert.IsNotNull(item, "WTF");
                    stack.Push(item);
                }
            }
        }
    }
}
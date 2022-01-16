using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Twisted.Tests.PS.V2.Extensions;

[PublicAPI]
public static class TraverseExtensions
{
    public static void TraverseBFS<T>(
        this T node, Action<T> action) where T : IEnumerable<T>
    {
        if (node == null)
            throw new ArgumentNullException(nameof(node));

        if (action == null)
            throw new ArgumentNullException(nameof(action));

        var queue = new Queue<T>();

        queue.Enqueue(node);

        while (queue.Any())
        {
            var current = queue.Dequeue();

            action(current);

            foreach (var item in current)
            {
                queue.Enqueue(item);
            }
        }
    }

    public static void TraverseDFS<T>(
        this T node, Action<T> action) where T : IEnumerable<T>
    {
        if (node == null)
            throw new ArgumentNullException(nameof(node));

        if (action == null)
            throw new ArgumentNullException(nameof(action));

        var stack = new Stack<T>();

        stack.Push(node);

        while (stack.Any())
        {
            var current = stack.Pop();

            action(current);

            foreach (var item in current.Reverse())
            {
                stack.Push(item);
            }
        }
    }
}
﻿namespace Twisted;

public static class TreeNodeExtensions
    // NOTE having these as extensions removes the need from specifying T in caller
{
    public static void TraverseBFS<T>(this T node, Action<T> visitor) where T : TreeNode
    {
        if (node == null)
            throw new ArgumentNullException(nameof(node));

        if (visitor == null)
            throw new ArgumentNullException(nameof(visitor));

        var queue = new Queue<T>();

        queue.Enqueue(node);

        while (queue.Any())
        {
            var dequeue = queue.Dequeue();

            visitor(dequeue);

            foreach (var item in dequeue)
            {
                queue.Enqueue((T)item);
            }
        }
    }

    public static void TraverseDFS<T>(this T node, Action<T> visitor) where T : TreeNode
    {
        if (node == null)
            throw new ArgumentNullException(nameof(node));

        if (visitor == null)
            throw new ArgumentNullException(nameof(visitor));

        var stack = new Stack<T>();

        stack.Push(node);

        while (stack.Any())
        {
            var pop = stack.Pop();

            visitor(pop);

            foreach (var item in pop.Reverse())
            {
                stack.Push((T)item);
            }
        }
    }
}
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace Twisted.PS.V1;

public static class Tree
{
    public static void TraversePreOrder<T>(
        [NotNull] T parent, Expression<Func<T, IEnumerable<T>>> children,
        Action<(T Node, int Depth, int Count)> action)
    {
        if (parent == null)
            throw new ArgumentNullException(nameof(parent));

        if (children == null)
            throw new ArgumentNullException(nameof(children));

        if (action == null)
            throw new ArgumentNullException(nameof(action));

        var queue = new Stack<(T Value, int Depth)>();
        var fetch = children.Compile();
        var count = 0;

        queue.Push((parent, 0));

        while (queue.Any())
        {
            var (node, depth) = queue.Pop();

            count++;

            action((node, depth, count));

            var descendants = fetch(node);

            foreach (var descendant in descendants)
            {
                queue.Push((descendant, depth + 1));
            }
        }
    }
}
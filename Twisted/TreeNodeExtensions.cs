namespace Twisted;

public static class TreeNodeExtensions
    // NOTE having these as extensions removes the need from specifying T in caller
{
    public static IEnumerable<T> TraverseBfs<T>(this T node) where T : TreeNode
    {
        if (node == null)
            throw new ArgumentNullException(nameof(node));

        var queue = new Queue<T>();

        queue.Enqueue(node);

        while (queue.Any())
        {
            var dequeue = queue.Dequeue();

            yield return dequeue;

            foreach (var item in dequeue)
            {
                queue.Enqueue((T)item);
            }
        }
    }

    public static IEnumerable<T> TraverseDfsPreOrder<T>(this T node) where T : TreeNode
    {
        if (node == null)
            throw new ArgumentNullException(nameof(node));

        var stack = new Stack<T>();

        stack.Push(node);

        while (stack.Any())
        {
            var pop = stack.Pop();

            yield return pop;

            foreach (var item in pop.Reverse())
            {
                stack.Push((T)item);
            }
        }
    }
}
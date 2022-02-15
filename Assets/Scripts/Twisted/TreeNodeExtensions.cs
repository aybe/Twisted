using System;
using System.Collections.Generic;
using System.Linq;

namespace Twisted
{
    public static class TreeNodeExtensions
    {
        public static IEnumerable<TreeNode> TraverseBfs(this TreeNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            var queue = new Queue<TreeNode>();

            queue.Enqueue(node);

            while (queue.Any())
            {
                var dequeue = queue.Dequeue();

                yield return dequeue;

                foreach (var item in dequeue)
                {
                    queue.Enqueue(item);
                }
            }
        }

        public static IEnumerable<TreeNode> TraverseDfs(this TreeNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            var stack = new Stack<TreeNode>();

            stack.Push(node);

            while (stack.Any())
            {
                var pop = stack.Pop();

                yield return pop;

                foreach (var item in pop.Reverse())
                {
                    stack.Push(item);
                }
            }
        }
    }
}
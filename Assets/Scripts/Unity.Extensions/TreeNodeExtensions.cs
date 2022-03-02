using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unity.Extensions
{
    public static class TreeNodeExtensions
    {
        public static string PrintHierarchyBackward(this TreeNode node)
        {
            if (node is null)
                throw new ArgumentNullException(nameof(node));

            var builder = new StringBuilder();

            var current = node;

            while (current != null)
            {
                if (builder.Length != 0)
                {
                    builder.Insert(0, Environment.NewLine);
                }

                builder.Insert(0, current);

                if (current.Parent != null)
                {
                    builder.Insert(0, "└───");

                    for (var i = 0; i < current.Depth - 1; i++)
                    {
                        builder.Insert(0, "    ");
                    }
                }

                current = current.Parent;
            }

            var result = builder.ToString();

            return result;
        }

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
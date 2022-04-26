using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Twisted.Controls
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

        public static string PrintHierarchyForward(this TreeNode node)
        {
            if (node is null)
                throw new ArgumentNullException(nameof(node));

            var stack = new Stack<TreeNode>();

            stack.Push(node);

            var builder = new StringBuilder();

            while (stack.Count > 0)
            {
                var pop = stack.Pop();

                var current = pop;

                var position = builder.Length;

                while (current.Parent != null)
                {
                    var count = current.Parent.Count;
                    var index = current.Parent.IndexOf(current);
                    var close = index != current.Parent.Count - 1;

                    if (node.Depth < current.Depth) // make node as if root
                    {
                        if (current.Depth < pop.Depth)
                        {
                            if (count > 1 && close)
                            {
                                builder.Insert(position, "│   ");
                            }
                            else
                            {
                                builder.Insert(position, "    ");
                            }
                        }
                        else
                        {
                            if (count > 1)
                            {
                                if (close)
                                {
                                    builder.Insert(position, "├───");
                                }
                                else
                                {
                                    builder.Insert(position, "└───");
                                }
                            }
                            else
                            {
                                builder.Insert(position, "└───");
                            }
                        }
                    }

                    current = current.Parent;
                }

                builder.Append(pop);

                builder.Append(Environment.NewLine);

                for (var i = pop.Count - 1; i >= 0; i--)
                {
                    stack.Push(pop[i]);
                }
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
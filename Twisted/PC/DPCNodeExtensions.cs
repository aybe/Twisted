using System.Text;

namespace Twisted.PC;

public static class DPCNodeExtensions
{
    public static string Print(this TreeNode node)
    {
        if (node is null)
            throw new ArgumentNullException(nameof(node));

        var builder = new StringBuilder();

        var stack = new Stack<TreeNode>();

        stack.Push(node);

        while (stack.Any())
        {
            var pop = stack.Pop();

            var str = PrintPrivate(pop);

            builder.AppendLine(str);

            foreach (var item in pop.Reverse())
            {
                stack.Push(item);
            }
        }

        var print = builder.ToString();

        return print;
    }

    private static string PrintPrivate(TreeNode node)
    {
        if (node is null)
            throw new ArgumentNullException(nameof(node));

        var builder = new StringBuilder();

        builder.Append(node);

        var current = node;

        while (current.Parent != null)
        {
            var count = current.Parent.Count;
            var index = current.Parent.IndexOf(current);
            var close = index != current.Parent.Count - 1;

            if (current.Depth < node.Depth)
            {
                if (count > 1 && close)
                {
                    builder.Insert(0, "│   ");
                }
                else
                {
                    builder.Insert(0, "    ");
                }
            }
            else
            {
                if (count > 1)
                {
                    if (close)
                    {
                        builder.Insert(0, "├───");
                    }
                    else
                    {
                        builder.Insert(0, "└───");
                    }
                }
                else
                {
                    builder.Insert(0, "└───");
                }
            }

            current = current.Parent;
        }

        var text = builder.ToString();

        return text;
    }
}
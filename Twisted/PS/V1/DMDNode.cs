using System.Diagnostics;
using System.Text;
using JetBrains.Annotations;
using Twisted.Extensions;

namespace Twisted.PS.V1;

public abstract class DMDNode
{
    // NOTE a cache could be interesting but what about parents ??

    protected DMDNode(DMD dmd, DMDNode? parent)
    {
        if (dmd == null)
            throw new ArgumentNullException(nameof(dmd));

        Parent = parent;
        Parent?.Children.Add(this);

        Position = dmd.Position;

        PrintMe();
    }

    public long Position { get; }

    public int Depth
    {
        get
        {
            var depth = 0;

            var parent = this;

            while (parent.Parent != null)
            {
                parent = parent.Parent;
                depth++;
            }

            return depth;
        }
    }

    public DMDNode Root
    {
        get
        {
            var parent = this;

            while (parent.Parent != null)
            {
                parent = parent.Parent;
            }

            return parent;
        }
    }

    public DMDNode? Parent { get; }

    public List<DMDNode> Children { get; } = new();

    public string Print()
    {
        var builder = new StringBuilder();
        var types   = new SortedDictionary<Type, int>(TypeComparer.Instance);
        var nodes   = 0;

        void Action((DMDNode Node, int Depth, int Count) tuple)
        {
            var (node, depth, count) = tuple;

            builder.AppendLine($"{new string('\t', depth)}{node}");

            nodes = count;

            var type = node.GetType();

            if (types.ContainsKey(type))
            {
                types[type]++;
            }
            else
            {
                types.Add(type, 1);
            }
        }

        Tree.TraversePreOrder(this, s => s.Children, Action);

        builder.AppendLine($"Total nodes: {nodes}");

        foreach (var (type, count) in types)
        {
            builder.AppendLine($"\t{type.Name}: {count}");
        }

        var print = builder.ToString();

        return print;
    }

    public void PrintMe()
    {
        Trace.IndentLevel = Depth;
        Trace.WriteLine(this);
    }

    public override string ToString()
    {
        return
            $"{GetType().Name}, {nameof(Depth)}: {Depth}, {nameof(Children)}: {Children.Count}, {nameof(Position)}: {Position}";
    }
}
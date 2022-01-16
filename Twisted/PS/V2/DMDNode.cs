using System.Collections;
using System.Text;
using JetBrains.Annotations;
using Twisted.Extensions;

namespace Twisted.PS.V2;

public abstract class DMDNode : IReadOnlyList<DMDNode>
{
    protected DMDNode([CanBeNull] DMDNode parent, BinaryReader reader, ushort? type = null)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Parent   = parent;
        Position = reader.BaseStream.Position;
        Type     = type ?? reader.ReadUInt16(Endianness.BigEndian);
    }

    [PublicAPI]
    public ushort Type { get; }

    [PublicAPI]
    public long Position { get; }

    [PublicAPI]
    [CanBeNull]
    public DMDNode Parent { get; }

    private IList<DMDNode> Children { get; } = new List<DMDNode>();

    [PublicAPI]
    public DMDNode Root
    {
        get
        {
            var root = this;

            while (root.Parent != null)
            {
                root = root.Parent;
            }

            return root;
        }
    }

    public override string ToString()
    {
        return $"0x{Type:X4} @ {Position}";
    }

    protected static uint ReadAddress(BinaryReader reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        const uint @base = 0x800188B8;

        var position = reader.BaseStream.Position;

        var address = reader.ReadUInt32(Endianness.LittleEndian);

        address -= @base;

        return address;
    }

    protected void ReadAddressesThenNodes(BinaryReader reader, int count)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        var addresses = new uint[count];

        for (var i = 0; i < count; i++)
        {
            addresses[i] = ReadAddress(reader);
        }

        foreach (var address in addresses)
        {
            reader.BaseStream.Position = address;

            var node = ReadNode(this, reader);

            Children.Add(node);
        }
    }

    private static DMDNode ReadNode([CanBeNull] DMDNode parent, BinaryReader reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var position = reader.BaseStream.Position;

        var peek = reader.Peek(s => s.ReadUInt16(Endianness.BigEndian));

        DMDNode node = peek switch
        {
            0x0010 => new DMDNode0010(parent, reader),
            0x00F0 => new DMDNode0010(parent, reader),

            0x0009 => new DMDNodeXXXX(parent, reader),
            0x0019 => new DMDNodeXXXX(parent, reader),
            0x0040 => new DMDNodeXXXX(parent, reader),
            0x0064 => new DMDNodeXXXX(parent, reader),
            0x0071 => new DMDNodeXXXX(parent, reader),
            0x00E1 => new DMDNodeXXXX(parent, reader),
            0x00E4 => new DMDNodeXXXX(parent, reader),
            0x00F9 => new DMDNodeXXXX(parent, reader),
            0x093D => new DMDNodeXXXX(parent, reader),
            0x0417 => new DMDNodeXXXX(parent, reader),
            0x00C4 => new DMDNodeXXXX(parent, reader),
            0x1027 => new DMDNodeXXXX(parent, reader),
            0x1055 => new DMDNodeXXXX(parent, reader),
            0x105C => new DMDNodeXXXX(parent, reader),
            0x105E => new DMDNodeXXXX(parent, reader),
            0x107A => new DMDNodeXXXX(parent, reader),
            0x108C => new DMDNodeXXXX(parent, reader),
            0x24F4 => new DMDNodeXXXX(parent, reader),
            0x409C => new DMDNodeXXXX(parent, reader),
            0x4070 => new DMDNodeXXXX(parent, reader),
            0x407E => new DMDNodeXXXX(parent, reader),
            0x4042 => new DMDNodeXXXX(parent, reader),
            0x4038 => new DMDNodeXXXX(parent, reader),
            0x40E8 => new DMDNodeXXXX(parent, reader),
            0x40F4 => new DMDNodeXXXX(parent, reader),
            0x4495 => new DMDNodeXXXX(parent, reader),
            0x4406 => new DMDNodeXXXX(parent, reader),
            0x44D2 => new DMDNodeXXXX(parent, reader),
            0x5125 => new DMDNodeXXXX(parent, reader),
            0x5195 => new DMDNodeXXXX(parent, reader),
            0x643F => new DMDNodeXXXX(parent, reader),
            0x6472 => new DMDNodeXXXX(parent, reader),
            0x6927 => new DMDNodeXXXX(parent, reader),
            0x84DE => new DMDNodeXXXX(parent, reader),
            0x904A => new DMDNodeXXXX(parent, reader),
            0x90D0 => new DMDNodeXXXX(parent, reader),
            0x90C9 => new DMDNodeXXXX(parent, reader),
            0x905F => new DMDNodeXXXX(parent, reader),
            0x9076 => new DMDNodeXXXX(parent, reader),
            0x907E => new DMDNodeXXXX(parent, reader),
            0x90EB => new DMDNodeXXXX(parent, reader),
            0xC1C5 => new DMDNodeXXXX(parent, reader),
            0xA177 => new DMDNodeXXXX(parent, reader),
            0xE457 => new DMDNodeXXXX(parent, reader),

            0x00FF => new DMDNode00FF(parent, reader),

            0x0107 => new DMDNode0107(parent, reader),

            0x0206 => new DMDNode020X(parent, reader),
            0x0207 => new DMDNode020X(parent, reader),
            0x0208 => new DMDNode020X(parent, reader),
            0x0209 => new DMDNode020X(parent, reader),
            0x020A => new DMDNode020X(parent, reader),

            0x0305 => new DMDNode0305(parent, reader),

            0x040B => new DMDNode040B(parent, reader),

            0x050B => new DMDNode050B(parent, reader),

            0x07FF => new DMDNode07FF(parent, reader),

            0x08FF => new DMDNode08FF(parent, reader),

            0x0903 => new DMDNode0903(parent, reader),

            0x0B06 => new DMDNode0B06(parent, reader),

            _ => throw new NotSupportedException($"Type = 0x{peek:X4}, Position = {position}")
        };

        return node;
    }

    [PublicAPI]
    public string PrintTree()
    {
        var stack = new Stack<(DMDNode, int)>();

        stack.Push((this, 0));

        var builder = new StringBuilder();

        while (stack.Any())
        {
            var (item, depth) = stack.Pop();

            builder.AppendLine($"{new string('\t', depth)}{item}");

            for (var i = item.Count - 1; i >= 0; i--)
            {
                var child = item[i];
                stack.Push((child, depth + 1));
            }
        }

        var tree = builder.ToString();

        return tree;
    }

    #region IReadOnlyList<DMDNode>

    public DMDNode this[int index] => Children[index];

    public int Count => Children.Count;

    public IEnumerator<DMDNode> GetEnumerator()
    {
        return Children.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Children).GetEnumerator();
    }

    #endregion
}
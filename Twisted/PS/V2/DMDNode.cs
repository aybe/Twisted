using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twisted.Extensions;

namespace Twisted.PS.V2;

public abstract class DMDNode : TreeNode
{
    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameterInConstructor")]
    protected DMDNode(DMDNode? parent, BinaryReader reader, uint? nodeType = null) : base(parent)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Position = reader.BaseStream.Position;
        NodeType = nodeType ?? reader.ReadUInt32(Endianness.BigEndian);
    }

    public long Position { get; }

    public long Length { get; private set; }

    public uint NodeType { get; }

    protected void SetLength(BinaryReader reader)
    {
        Length = reader.BaseStream.Position - Position;
    }

    public override string ToString()
    {
        return $"{GetType().Name}, {nameof(NodeType)}: 0x{NodeType:X8}, {nameof(Position)}: {Position}, {nameof(Length)}: {Length}";
    }

    protected static uint ReadAddress(BinaryReader reader, bool validate = true)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var position = reader.BaseStream.Position;
        var address1 = reader.ReadUInt32(Endianness.LittleEndian);
        var address2 = address1 - DMD.BaseAddress;

        if (validate)
        {
            Assert.IsFalse(address2 >= reader.BaseStream.Length, $"{address2} >= {reader.BaseStream.Length} @ {position}");
        }

        return address2;
    }

    protected static uint[] ReadAddresses(BinaryReader reader, int count, bool validate = true)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        var addresses = new uint[count];

        for (var i = 0; i < count; i++)
        {
            addresses[i] = ReadAddress(reader, validate);
        }

        return addresses;
    }

    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Local")]
    private static DMDNode ReadNode(DMDNode? parent, BinaryReader reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var position = reader.BaseStream.Position;

        var peek = reader.Peek(s => s.ReadUInt16(Endianness.BigEndian));

        /*
         * according to dbScanForInteractiveStuff
         * 0 ok
         * 1 ok
         * 2 ok
         * 3 ok
         * 4 ok
         * 5 ok
         * 6 never encountered, 7 ok, 8 ok
         * 9 ok
         * B ok
         * anything else is bad op code
         */

        DMDNode node = peek switch
        {
            // TODO are these geometry?

            0x0010 => new DMDNode0010(parent, reader),
            0x00F0 => new DMDNode0010(parent, reader),

            // TODO this geometry stuff should be refactored

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

            _ => throw new NotSupportedException($"{nameof(NodeType)} = 0x{peek:X4}, {nameof(Position)} = {position}")
        };

        return node;
    }

    protected static DMDNode[] ReadNodes(DMDNode? parent, BinaryReader reader, uint[] addresses)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        if (addresses == null)
            throw new ArgumentNullException(nameof(addresses));

        var nodes = new DMDNode[addresses.Length];

        for (var i = 0; i < nodes.Length; i++)
        {
            reader.BaseStream.Position = addresses[i];

            var node = ReadNode(parent, reader);

            nodes[i] = node;
        }

        return nodes;
    }
}
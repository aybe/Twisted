using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Twisted.Controls;
using Twisted.Formats.Binary;
using Unity.Mathematics;
using UnityEngine.Assertions;

// ReSharper disable CommentTypo

namespace Twisted.Formats.Database
{
    public abstract class DMDNode : TreeNode
    {
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameterInConstructor")]
        protected DMDNode(DMDNode? parent, BinaryReader reader) : base(parent)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            Position = reader.BaseStream.Position;
            NodeType = reader.ReadUInt32(Endianness.BE);
            NodeKind = (ushort)((NodeType >> 16) & 0xFFFF);
            NodeRole = (ushort)((NodeType >> 00) & 0xFFFF);
        }

        private byte[] ObjectData { get; set; } = null!;

        /// <summary>
        ///     <see cref="NodeKind" /> &lt;&lt; 16 | <see cref="NodeRole" /> &lt;&lt; 00.
        /// </summary>
        public uint NodeType { get; }

        /// <summary>
        ///     See 'dbScanForInteractiveStuff'.
        /// </summary>
        public ushort NodeKind { get; }

        /// <summary>
        ///     See 'dbProcessInteractives'.
        /// </summary>
        public ushort NodeRole { get; }

        public long Position { get; }

        public long Length { get; private set; }

        protected virtual uint BaseAddress => (Root as DMDNode)!.BaseAddress;

        [PublicAPI]
        public virtual float4x4 LocalTransform { get; init; } = float4x4.identity;

        [PublicAPI]
        public float4x4 WorldTransform
        {
            get
            {
                var stack = new Stack<float4x4>();

                var value = this;

                while (value is not null)
                {
                    stack.Push(value.LocalTransform);

                    value = value.Parent as DMDNode;
                }

                var transform = stack.Aggregate(float4x4.identity, math.mul);

                return transform;
            }
        }

        [SuppressMessage("ReSharper", "ReturnTypeCanBeEnumerable.Global")]
        public IReadOnlyList<byte> GetObjectData()
        {
            return ObjectData.ToArray();
        }

        protected void SetupBinaryObject(BinaryReader reader)
            // TODO there's a way to do that in ctor but it'd need an extra reader for each node to avoid 'Virtual member call in constructor'
        {
            Length = reader.BaseStream.Position - Position;

            Assert.AreNotEqual(0, Length, "Invalid node length.");

            byte[] data;

            using (new BinaryReaderPositionScope(reader, Position))
            {
                data = reader.ReadBytes(Length.ToInt32());
            }

            ObjectData = data;
        }

        public override string ToString()
        {
            return $"{GetType().Name}, {nameof(NodeType)}: 0x{NodeType:X8}, {nameof(Position)}: {Position}, {nameof(Length)}: {Length}";
        }

        protected uint ReadAddress(BinaryReader reader, bool validate = true)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var position = reader.BaseStream.Position;
            var address1 = reader.ReadUInt32(Endianness.LE);
            var address2 = address1 - BaseAddress;

            if (validate)
            {
                Assert.IsFalse(address2 >= reader.BaseStream.Length, $"{address2} >= {reader.BaseStream.Length} @ {position}");
            }

            return address2;
        }

        protected uint[] ReadAddresses(BinaryReader reader, int count, bool validate = true)
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
        [SuppressMessage("ReSharper", "ConvertSwitchStatementToSwitchExpression",       Justification = "Code coverage")]
        [SuppressMessage("Style",     "IDE0066:Convert switch statement to expression", Justification = "Code coverage")]
        private static DMDNode ReadNode(DMDNode? parent, BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var position = reader.BaseStream.Position;

            var peek = reader.Peek(s => s.ReadUInt16(Endianness.BE));

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

        // @formatter:off
        switch (peek)
        {
            case 0x0010: return new DMDNode0010(parent, reader);
            case 0x00F0: return new DMDNode0010(parent, reader);
            case 0x0009: return new DMDNodeXXXX(parent, reader);
            case 0x0019: return new DMDNodeXXXX(parent, reader);
            case 0x0040: return new DMDNodeXXXX(parent, reader);
            case 0x0064: return new DMDNodeXXXX(parent, reader);
            case 0x0071: return new DMDNodeXXXX(parent, reader);
            case 0x00E1: return new DMDNodeXXXX(parent, reader);
            case 0x00E4: return new DMDNodeXXXX(parent, reader);
            case 0x00F9: return new DMDNodeXXXX(parent, reader);
            case 0x093D: return new DMDNodeXXXX(parent, reader);
            case 0x0417: return new DMDNodeXXXX(parent, reader);
            case 0x00C4: return new DMDNodeXXXX(parent, reader);
            case 0x1027: return new DMDNodeXXXX(parent, reader);
            case 0x1055: return new DMDNodeXXXX(parent, reader);
            case 0x105C: return new DMDNodeXXXX(parent, reader);
            case 0x105E: return new DMDNodeXXXX(parent, reader);
            case 0x107A: return new DMDNodeXXXX(parent, reader);
            case 0x108C: return new DMDNodeXXXX(parent, reader);
            case 0x24F4: return new DMDNodeXXXX(parent, reader);
            case 0x409C: return new DMDNodeXXXX(parent, reader);
            case 0x4070: return new DMDNodeXXXX(parent, reader);
            case 0x407E: return new DMDNodeXXXX(parent, reader);
            case 0x4042: return new DMDNodeXXXX(parent, reader);
            case 0x4038: return new DMDNodeXXXX(parent, reader);
            case 0x40E8: return new DMDNodeXXXX(parent, reader);
            case 0x40F4: return new DMDNodeXXXX(parent, reader);
            case 0x4495: return new DMDNodeXXXX(parent, reader);
            case 0x4406: return new DMDNodeXXXX(parent, reader);
            case 0x44D2: return new DMDNodeXXXX(parent, reader);
            case 0x5125: return new DMDNodeXXXX(parent, reader);
            case 0x5195: return new DMDNodeXXXX(parent, reader);
            case 0x643F: return new DMDNodeXXXX(parent, reader);
            case 0x6472: return new DMDNodeXXXX(parent, reader);
            case 0x6927: return new DMDNodeXXXX(parent, reader);
            case 0x84DE: return new DMDNodeXXXX(parent, reader);
            case 0x904A: return new DMDNodeXXXX(parent, reader);
            case 0x90D0: return new DMDNodeXXXX(parent, reader);
            case 0x90C9: return new DMDNodeXXXX(parent, reader);
            case 0x905F: return new DMDNodeXXXX(parent, reader);
            case 0x9076: return new DMDNodeXXXX(parent, reader);
            case 0x907E: return new DMDNodeXXXX(parent, reader);
            case 0x90EB: return new DMDNodeXXXX(parent, reader);
            case 0xC1C5: return new DMDNodeXXXX(parent, reader);
            case 0xA177: return new DMDNodeXXXX(parent, reader);
            case 0xE457: return new DMDNodeXXXX(parent, reader);
            case 0x00FF: return new DMDNode00FF(parent, reader);
            case 0x0107: return new DMDNode0107(parent, reader);
            case 0x0206: return new DMDNode020X(parent, reader);
            case 0x0207: return new DMDNode020X(parent, reader);
            case 0x0208: return new DMDNode020X(parent, reader);
            case 0x0209: return new DMDNode020X(parent, reader);
            case 0x020A: return new DMDNode020X(parent, reader);
            case 0x0305: return new DMDNode0305(parent, reader);
            case 0x040B: return new DMDNode040B(parent, reader);
            case 0x050B: return new DMDNode050B(parent, reader);
            case 0x07FF: return new DMDNode07FF(parent, reader);
            case 0x08FF: return new DMDNode08FF(parent, reader);
            case 0x0903: return new DMDNode0903(parent, reader);
            case 0x0B06: return new DMDNode0B06(parent, reader);
            default: throw new NotSupportedException($"{nameof(NodeType)} = 0x{peek:X4}, {nameof(Position)} = {position}");
        }
            // @formatter:on
        }

        [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
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

        public virtual string? GetNodeInfo()
        {
            return null;
        }
    }
}
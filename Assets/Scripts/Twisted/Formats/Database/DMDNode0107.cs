using System;
using System.IO;
using System.Runtime.InteropServices;
using Twisted.Formats.Binary;
using Unity.Mathematics;
using UnityEngine.Assertions;

namespace Twisted.Formats.Database
{
    public sealed class DMDNode0107 : DMDNode
    {
        private readonly int Unknown1;

        private readonly float3 Vector1;

        public DMDNode0107(DMDNode? parent, BinaryReader reader)
            : base(parent, reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var bytes = reader.ReadBytes(20);

            Assert.AreEqual((byte)0, bytes[19]);

            var ints = MemoryMarshal.Cast<byte, int>(bytes);

            Vector1 = new float3(ints[0], ints[1], ints[2]);

            Unknown1 = ints[3];

            var count = bytes[16];

            var addresses = ReadAddresses(reader, count);

            // TODO there may be 76 more bytes in here

            SetupBinaryObject(reader);

            ReadNodes(this, reader, addresses);
        }

        public override float4x4 LocalTransform
        {
            get
            {
                return NodeRole switch // 8010087C
                {
                    1 => float4x4.Translate(Vector1),
                    _ => float4x4.identity
                };
            }
        }

        public override string? GetNodeInfo()
        {
            var role = NodeRole.ReverseEndianness();

            return role switch
            {
                0x0001 => "3D environment",
                _      => null
            };
        }
    }
}
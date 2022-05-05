using System;
using System.IO;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

namespace Twisted.Formats.Database
{
    public sealed class DMDNode050B : DMDNode
    {
        public readonly byte Flag1;

        public readonly float3x3 Rotation;

        public readonly byte Unknown1;

        public readonly float3 Vector1;

        public DMDNode050B(DMDNode? parent, BinaryReader reader)
            : base(parent, reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var bytes = reader.ReadBytes(36);

            var i16 = MemoryMarshal.Cast<byte, short>(bytes);
            var i32 = MemoryMarshal.Cast<byte, int>(bytes);

            var m00 = i16[0];
            var m01 = i16[1];
            var m02 = i16[2];
            var m10 = i16[3];
            var m11 = i16[4];
            var m12 = i16[5];
            var m20 = i16[6];
            var m21 = i16[7];
            var m22 = i16[8];
            var c0  = new int3(m00, m01, m02);
            var c1  = new int3(m10, m11, m12);
            var c2  = new int3(m20, m21, m22);

            Vector1 = new float3(i32[5], i32[6], i32[7]);

            Unknown1 = bytes[32];

            Assert.AreEqual((byte)0, bytes[33]);

            var flag1 = bytes[35];

            Flag1 = flag1 switch
            {
                0 => flag1,
                1 => flag1,
                _ => throw new InvalidOperationException($"Unknown {nameof(Flag1)}: {Flag1}.")
            };

            var x1 = new float3x3(m00, m01, m02, m10, m11, m12, m20, m21, m22);
            var x2 = new float3x3(c0,  c1,  c2);

            Debug.Log(x1);
            Debug.Log(x2);

            Rotation = Flag1 switch
            {
                0 => x1,
                1 => x2,
                _ => throw new InvalidOperationException($"Unknown {nameof(Flag1)}: {Flag1}.")
            };

            var addressesCount = bytes[34];
            var addresses      = ReadAddresses(reader, addressesCount);

            SetupBinaryObject(reader);

            ReadNodes(this, reader, addresses);
        }

        public static float4x4 TRS(float3x3 rotation)
            // this is 800FE9C4 but as 4x4 so that we can transform stuff
        {
            const float scale = 1.0f / 4096f;

            var translation = float3.zero;

            return math.float4x4(
                math.float4(rotation.c0 * scale, 0.0f),
                math.float4(rotation.c1 * scale, 0.0f),
                math.float4(rotation.c2 * scale, 0.0f),
                math.float4(translation,         1.0f)
            );
        }
    }
}
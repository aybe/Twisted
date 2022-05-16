using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Twisted.Formats.Binary;
using Unity.Mathematics;

namespace Twisted.Formats.Database
{
    public static class DMDExtensions
    {
        public static float4x4 GetWorldTransform(this DMDNode node)
        {
            if (node is null)
                throw new ArgumentNullException(nameof(node));

            var stack = new Stack<float4x4>();

            var value = node;

            while (value is not null)
            {
                stack.Push(value.GetLocalTransform());

                value = value.Parent as DMDNode;
            }

            var transform = stack.Aggregate(float4x4.identity, math.mul);

            return transform;
        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public static float4x4 GetLocalTransform(this DMDNode node)
        {
            if (node is null)
                throw new ArgumentNullException(nameof(node));

            return node switch // TODO arbitrary scale 
            {
                DMD                  => math.mul(float4x4.RotateX(math.radians(-90.0f)), float4x4.Scale(1.0f)),
                DMDNode0010          => float4x4.identity,
                DMDNode00FF          => float4x4.identity,
                DMDNode0107          => float4x4.identity,
                DMDNode020X          => float4x4.identity,
                DMDNode0305          => float4x4.identity,
                DMDNode040B node040B => float4x4.Translate(node040B.Vector1), // this looks correct, e.g. ground stands
                DMDNode050B          => float4x4.identity,                    // this one participates in hierarchy in a weird way, not done here
                DMDNode07FF          => float4x4.identity,
                DMDNode08FF          => float4x4.identity,
                DMDNode0903          => float4x4.identity,
                DMDNode0B06          => float4x4.identity,
                DMDNodeXXXX          => float4x4.identity,
                _                    => throw new NotSupportedException()
            };
        }

        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public static string? TryGetNodeInfo(this DMDNode node)
        {
            var info = node.NodeType switch
            {
                0x0107_0100 => "3D environment",
                0x0107_0200 => "Sky",
                0x0107_0300 => "Ground",
                0x0107_0A00 => "Sweet Tooth",
                0x0107_1400 => "Yellow Jacket",
                0x0107_1E00 => "Darkside",
                0x0107_2800 => "Hammerhead",
                0x0107_3200 => "Outlaw",
                0x0107_3C00 => "Crimson Fury",
                0x0107_4600 => "Warthog",
                0x0107_5000 => "Mr Grimm",
                0x0107_5A00 => "Pit Viper",
                0x0107_6400 => "Thumper",
                0x0107_6E00 => "Spectre",
                0x0107_7800 => "Road Kill",
                0x040B_9101 => "Power up",
                0x040B_9201 => "Power up",
                0x040B_9301 => "Power up",
                0x040B_9501 => "Power up",
                0x040B_9A01 => "Power up",
                0x040B_9C01 => "Power up",
                0x040B_9E01 => "Power up",
                _           => null
            };

            var role = node.NodeRole.ReverseEndianness();

            info ??= role switch
            {
                0x00C9 => "Bullet",
                0x0138 => "Bullet",
                0x00D2 => "Weapon texture",
                0x00D4 => "Weapon texture",
                0x00D5 => "Weapon texture",
                0x00D6 => "Weapon texture",
                0x028A => "Flame animation",
                0x028B => "Smoke animation",
                0x028C => "Contrail animation",
                0x028D => "Spark animation",
                0x028E => "Explosion animation",
                0x028F => "Burst animation",
                0x0290 => "Burn animation",
                0x029E => "Frag texture",
                0x029F => "Frag texture",
                0x02A0 => "Frag texture",
                0x02A1 => "Frag texture",
                0x02EF => "Pedestrian",
                0x02F4 => "Pedestrian",
                0x02F8 => "Pedestrian",
                0x0304 => "Pedestrian",
                0x0305 => "Pedestrian",
                0x0307 => "Pedestrian",
                0x0308 => "Pedestrian",
                0x0309 => "Pedestrian",
                0x030A => "Pedestrian",
                0x030D => "Pedestrian",
                0x030E => "Pedestrian",
                0x02EE => "Hover cop",
                0x02F0 => "Hover cop",
                0x02F1 => "Static cop",
                0x02F2 => "Static cop",
                0x02F3 => "Static cop",
                0x04D8 => "Dashboard",
                0x04DF => "Rear view mirror",
                0x04EC => "Dashboard icons",
                0x0500 => "Steering wheel",
                0x012D => "Special bullet",
                0x012E => "Special bullet",
                0x0134 => "Special bullet",
                0x0135 => "Special bullet",
                0x0398 => "Health stand",
                0x0399 => "Health stand lightning",
                0x0514 => "Protagonist",
                _      => null
            };

            return info;
        }
    }
}
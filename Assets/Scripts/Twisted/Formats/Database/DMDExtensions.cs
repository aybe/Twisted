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
        public static DMDInfo? TryGetInfo(this DMDNode node)
        {
            var info = node.NodeType switch
            {
                0x0107_0100 => new DMDInfo(DMDInfoCategory.Environment, "3D environment"),
                0x0107_0200 => new DMDInfo(DMDInfoCategory.Environment, "Sky"),
                0x0107_0300 => new DMDInfo(DMDInfoCategory.Environment, "Ground"),
                0x0107_0A00 => new DMDInfo(DMDInfoCategory.Character,   "Sweet Tooth"),
                0x0107_1400 => new DMDInfo(DMDInfoCategory.Character,   "Yellow Jacket"),
                0x0107_1E00 => new DMDInfo(DMDInfoCategory.Character,   "Darkside"),
                0x0107_2800 => new DMDInfo(DMDInfoCategory.Character,   "Hammerhead"),
                0x0107_3200 => new DMDInfo(DMDInfoCategory.Character,   "Outlaw"),
                0x0107_3C00 => new DMDInfo(DMDInfoCategory.Character,   "Crimson Fury"),
                0x0107_4600 => new DMDInfo(DMDInfoCategory.Character,   "Warthog"),
                0x0107_5000 => new DMDInfo(DMDInfoCategory.Character,   "Mr Grimm"),
                0x0107_5A00 => new DMDInfo(DMDInfoCategory.Character,   "Pit Viper"),
                0x0107_6400 => new DMDInfo(DMDInfoCategory.Character,   "Thumper"),
                0x0107_6E00 => new DMDInfo(DMDInfoCategory.Character,   "Spectre"),
                0x0107_7800 => new DMDInfo(DMDInfoCategory.Character,   "Road Kill"),
                0x040B_9101 => new DMDInfo(DMDInfoCategory.Bonus,       "Power up"),
                0x040B_9201 => new DMDInfo(DMDInfoCategory.Bonus,       "Power up"),
                0x040B_9301 => new DMDInfo(DMDInfoCategory.Bonus,       "Power up"),
                0x040B_9501 => new DMDInfo(DMDInfoCategory.Bonus,       "Power up"),
                0x040B_9A01 => new DMDInfo(DMDInfoCategory.Bonus,       "Power up"),
                0x040B_9C01 => new DMDInfo(DMDInfoCategory.Bonus,       "Power up"),
                0x040B_9E01 => new DMDInfo(DMDInfoCategory.Bonus,       "Power up"),
                _           => default(DMDInfo?)
            };

            var role = node.NodeRole.ReverseEndianness();

            info ??= role switch
            {
                0x00C9 => new DMDInfo(DMDInfoCategory.Weapon,      "Bullet"),
                0x0138 => new DMDInfo(DMDInfoCategory.Weapon,      "Bullet"),
                0x00D2 => new DMDInfo(DMDInfoCategory.Texture,     "Weapon texture"),
                0x00D4 => new DMDInfo(DMDInfoCategory.Texture,     "Weapon texture"),
                0x00D5 => new DMDInfo(DMDInfoCategory.Texture,     "Weapon texture"),
                0x00D6 => new DMDInfo(DMDInfoCategory.Texture,     "Weapon texture"),
                0x028A => new DMDInfo(DMDInfoCategory.Animation,   "Flame animation"),
                0x028B => new DMDInfo(DMDInfoCategory.Animation,   "Smoke animation"),
                0x028C => new DMDInfo(DMDInfoCategory.Animation,   "Contrail animation"),
                0x028D => new DMDInfo(DMDInfoCategory.Animation,   "Spark animation"),
                0x028E => new DMDInfo(DMDInfoCategory.Animation,   "Explosion animation"),
                0x028F => new DMDInfo(DMDInfoCategory.Animation,   "Burst animation"),
                0x0290 => new DMDInfo(DMDInfoCategory.Animation,   "Burn animation"),
                0x029E => new DMDInfo(DMDInfoCategory.Texture,     "Frag texture"),
                0x029F => new DMDInfo(DMDInfoCategory.Texture,     "Frag texture"),
                0x02A0 => new DMDInfo(DMDInfoCategory.Texture,     "Frag texture"),
                0x02A1 => new DMDInfo(DMDInfoCategory.Texture,     "Frag texture"),
                0x02EF => new DMDInfo(DMDInfoCategory.Enemy,       "Pedestrian"),
                0x02F4 => new DMDInfo(DMDInfoCategory.Enemy,       "Pedestrian"),
                0x02F8 => new DMDInfo(DMDInfoCategory.Enemy,       "Pedestrian"),
                0x0304 => new DMDInfo(DMDInfoCategory.Enemy,       "Pedestrian"),
                0x0305 => new DMDInfo(DMDInfoCategory.Enemy,       "Pedestrian"),
                0x0307 => new DMDInfo(DMDInfoCategory.Enemy,       "Pedestrian"),
                0x0308 => new DMDInfo(DMDInfoCategory.Enemy,       "Pedestrian"),
                0x0309 => new DMDInfo(DMDInfoCategory.Enemy,       "Pedestrian"),
                0x030A => new DMDInfo(DMDInfoCategory.Enemy,       "Pedestrian"),
                0x030D => new DMDInfo(DMDInfoCategory.Enemy,       "Pedestrian"),
                0x030E => new DMDInfo(DMDInfoCategory.Enemy,       "Pedestrian"),
                0x02EE => new DMDInfo(DMDInfoCategory.Enemy,       "Hover cop"),
                0x02F0 => new DMDInfo(DMDInfoCategory.Enemy,       "Hover cop"),
                0x02F1 => new DMDInfo(DMDInfoCategory.Enemy,       "Static cop"),
                0x02F2 => new DMDInfo(DMDInfoCategory.Enemy,       "Static cop"),
                0x02F3 => new DMDInfo(DMDInfoCategory.Enemy,       "Static cop"),
                0x04D8 => new DMDInfo(DMDInfoCategory.Interface,   "Dashboard"),
                0x04DF => new DMDInfo(DMDInfoCategory.Interface,   "Rear view mirror"),
                0x04EC => new DMDInfo(DMDInfoCategory.Interface,   "Dashboard icons"),
                0x0500 => new DMDInfo(DMDInfoCategory.Interface,   "Steering wheel"),
                0x012D => new DMDInfo(DMDInfoCategory.Weapon,      "Special bullet"),
                0x012E => new DMDInfo(DMDInfoCategory.Weapon,      "Special bullet"),
                0x0134 => new DMDInfo(DMDInfoCategory.Weapon,      "Special bullet"),
                0x0135 => new DMDInfo(DMDInfoCategory.Weapon,      "Special bullet"),
                0x0398 => new DMDInfo(DMDInfoCategory.Environment, "Health stand"),
                0x0399 => new DMDInfo(DMDInfoCategory.Environment, "Health stand lightning"),
                0x0514 => new DMDInfo(DMDInfoCategory.Character,   "Thumbnail"),
                _      => default(DMDInfo?)
            };

            return info;
        }
    }
}
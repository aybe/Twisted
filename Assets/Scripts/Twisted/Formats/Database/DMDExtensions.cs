using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
    }
}
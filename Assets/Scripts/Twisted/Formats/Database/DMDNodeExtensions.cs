using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Unity.Mathematics;

namespace Twisted.Formats.Database
{
    public static class DMDNodeExtensions
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

        [SuppressMessage("ReSharper", "ConvertSwitchStatementToSwitchExpression")]
        public static float4x4 GetLocalTransform(this DMDNode node)
        {
            if (node is null)
                throw new ArgumentNullException(nameof(node));

            switch (node)
            {
                case DMD:
                    return math.mul(float4x4.RotateX(math.radians(-90.0f)), float4x4.Scale(0.1f));
                case DMDNode0010:
                    return float4x4.identity;
                case DMDNode00FF:
                    return float4x4.identity;
                case DMDNode0107:
                    return float4x4.identity;
                case DMDNode020X:
                    return float4x4.identity;
                case DMDNode0305:
                    return float4x4.identity;
                case DMDNode040B node040B: // this appears to be correct as it positions most of the objects correctly
                    return float4x4.Translate(node040B.Vector1);
                case DMDNode050B node050B: // this is 800FE9C4 + translation, best results but still needs some offset
                    return TRS(node050B.Vector1.yxz, node050B.Rotation, new float3(1.0f / 4096.0f));
                case DMDNode07FF:
                    return float4x4.identity;
                case DMDNode08FF:
                    return float4x4.identity;
                case DMDNode0903:
                    return float4x4.identity;
                case DMDNode0B06:
                    return float4x4.identity;
                case DMDNodeXXXX:
                    return float4x4.identity;
                default:
                    throw new NotSupportedException();
            }
        }

        private static float4x4 TRS(float3 translate, float3x3 rotate, float3 scale)
        {
            return math.float4x4(
                math.float4(rotate.c0 * scale.x, 0.0f),
                math.float4(rotate.c1 * scale.y, 0.0f),
                math.float4(rotate.c2 * scale.z, 0.0f),
                math.float4(translate,           1.0f)
            );
        }
    }
}
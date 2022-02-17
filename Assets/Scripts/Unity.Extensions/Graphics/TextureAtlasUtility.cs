using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace Unity.Extensions.Graphics
{
    internal static class TextureAtlasUtility
    {
        public static IReadOnlyDictionary<TextureTransform, Matrix4x4> Matrices { get; } = new ReadOnlyDictionary<TextureTransform, Matrix4x4>(GetMatrices());

        private static Dictionary<TextureTransform, Matrix4x4> GetMatrices()
        {
            static float GetRotation(TextureTransform transform)
            {
                if (transform.HasFlagFast(TextureTransform.Rotate270))
                    return 270.0f;

                if (transform.HasFlagFast(TextureTransform.Rotate180))
                    return 180.0f;

                if (transform.HasFlagFast(TextureTransform.Rotate90))
                    return 90.0f;

                return 0.0f;
            }

            static Vector2 GetScale(TextureTransform transform)
            {
                var x = transform.HasFlagFast(TextureTransform.FlipX) ? -1.0f : +1.0f;
                var y = transform.HasFlagFast(TextureTransform.FlipY) ? -1.0f : +1.0f;
                var s = new Vector2(x, y);

                return s;
            }

            var matrices = new Dictionary<TextureTransform, Matrix4x4>();

            var transforms = Enumerable.Range(0, (int)Mathf.Pow(2, Enum.GetValues(typeof(TextureTransform)).Length - 1)).Select(s => (TextureTransform)s);

            foreach (var transform in transforms)
            {
                var fr = GetRotation(transform);
                var fs = GetScale(transform);
                var m1 = Matrix4x4.TRS(new Vector3(-0.5f, -0.5f), Quaternion.identity, Vector3.one);
                var m2 = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, fs);
                var m3 = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0.0f, 0.0f, fr), Vector3.one);
                var m4 = Matrix4x4.TRS(new Vector3(+0.5f, +0.5f), Quaternion.identity, Vector3.one);
                var m5 = m4 * m3 * m2 * m1;

                matrices.Add(transform, m5);
            }

            return matrices;
        }
    }
}
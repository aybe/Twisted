using System;
using System.Linq;
using UnityEngine;

namespace Unity.Extensions.Graphics
{
    public sealed class TextureAtlas : ScriptableObject
    {
        [SerializeField]
        [HideInInspector]
        private Rect[] Rects = null!;

        [SerializeField]
        [HideInInspector]
        private Vector2[] Sizes = null!;

        public static bool TryCreate(
            Texture2D[] textures, out TextureAtlas atlas, out Texture2D atlasTexture, int padding = 5, int bleeding = 5, int maximumAtlasSize = 4096, bool makeNoLongerReadable = false)
        {
            if (textures is null)
                throw new ArgumentNullException(nameof(textures));

            if (textures.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(textures));

            if (padding < 0)
                throw new ArgumentOutOfRangeException(nameof(padding));

            if (bleeding < 0)
                throw new ArgumentOutOfRangeException(nameof(bleeding));

            if (maximumAtlasSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(maximumAtlasSize));

            atlas        = default!;
            atlasTexture = default!;

            var texture = new Texture2D(0, 0);

            if (bleeding > 0)
            {
                textures = textures.Select(s => s.Bleed(bleeding)).ToArray();
            }

            var rects = texture.PackTextures(textures, padding, maximumAtlasSize, makeNoLongerReadable);

            if (bleeding > 0)
            {
                foreach (var item in textures)
                {
                    DestroyImmediate(item);
                }
            }

            if (rects is null)
            {
                DestroyImmediate(texture);
                return false;
            }

            if (bleeding > 0)
            {
                var uv = new Vector2((float)bleeding / texture.width, (float)bleeding / texture.height);
                rects = rects.Select(s => new Rect(s.position + uv, s.size - uv * 2.0f)).ToArray();
            }

            atlas       = CreateInstance<TextureAtlas>();
            atlas.Rects = rects;
            atlas.Sizes = rects.Select(s => new Vector2(1.0f / (texture.width * s.width - 1.0f), 1.0f / (texture.height * s.height - 1.0f))).ToArray();

            atlasTexture = texture;

            return true;
        }

        [Obsolete("This is plain wrong")] // TODO remove
        public int Count => Rects.Length;

        /// <summary>
        ///     Gets UV coordinates for a texture.
        /// </summary>
        /// <param name="index">
        ///     The index of the texture.
        /// </param>
        /// <param name="uv">
        ///     The UV coordinates.
        /// </param>
        /// <param name="normalized">
        ///     Does <paramref name="uv" /> denotes normalized coordinates?
        /// </param>
        /// <param name="transform">
        ///     Transform to apply to UV coordinates.
        /// </param>
        /// <returns>
        ///     The transformed UV coordinates.
        /// </returns>
        public Vector2 GetUV(int index, Vector2 uv, bool normalized, TextureTransform transform)
        {
            if (Rects is null)
                throw new NullReferenceException($"{nameof(Rects)} is null, did you create an instance with {nameof(TryCreate)}?");

            if (Sizes is null)
                throw new NullReferenceException($"{nameof(Sizes)} is null, did you create an instance with {nameof(TryCreate)}?");

            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), index, null);

            if (index >= Rects.Length)
                throw new ArgumentOutOfRangeException(nameof(index), index, null);

            if (index >= Sizes.Length)
                throw new ArgumentOutOfRangeException(nameof(index), index, null);

            var rect   = Rects[index];
            var size   = Sizes[index];
            var coords = normalized ? uv : Vector2.Scale(uv, size);
            var matrix = TextureAtlasUtility.Matrices[transform];
            var point  = matrix.MultiplyPoint(coords);
            var scale  = Vector2.Scale(point, rect.size);
            var result = rect.position + scale;

            return result;
        }
    }
}
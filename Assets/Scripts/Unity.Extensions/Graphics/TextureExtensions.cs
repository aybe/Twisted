using System;
using UnityEngine;

namespace Unity.Extensions.Graphics
{
    public static class TextureExtensions
    {
        public static Texture2D Bleed(this Texture2D source, int margin)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            if (margin <= 0)
                throw new ArgumentOutOfRangeException(nameof(margin));

            var sw = source.width;
            var sh = source.height;
            var tw = sw + margin * 2;
            var th = sh + margin * 2;
            var sp = source.GetPixels32();
            var tp = new Color32[tw * th];

            for (var i = 0; i < tw * th; i++)
            {
                var tx = i % tw;
                var ty = i / tw;
                var sx = tx < margin ? 0 : tx > sw + margin - 1 ? sw - 1 : tx - margin;
                var sy = ty < margin ? 0 : ty > sh + margin - 1 ? sh - 1 : ty - margin;
                tp[ty * tw + tx] = sp[sy * sw + sx];
            }

            var target = new Texture2D(tw, th, source.format, source.mipmapCount is not 0);

            target.SetPixels32(tp);
            target.Apply();

            return target;
        }
    }
}
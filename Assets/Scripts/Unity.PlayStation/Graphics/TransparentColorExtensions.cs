﻿#if UNITY_2017_1_OR_NEWER
using UnityEngine;

namespace Unity.PlayStation.Graphics
{
    public static class TransparentColorExtensions
    {
        public static Color ToColor(this TransparentColor color, TransparentColorMode mode = TransparentColorMode.None)
            // BUG this is conflicting with System.Drawing extension method
        {
            return color.ToColor32(mode);
        }

        public static Color32 ToColor32(this TransparentColor color, TransparentColorMode mode = TransparentColorMode.None)
        {
            var color1 = color.ToColor(mode);
            var color2 = new Color32(color1.R, color1.G, color1.B, color1.A);
            return color2;
        }
    }
}
#endif
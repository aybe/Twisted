#if UNITY_2017_1_OR_NEWER
using UnityEngine;

namespace Unity.Extensions.General
{
    public static class ColorExtensions
    {
        public static Color ToColor(this System.Drawing.Color color)
        {
            return color.ToColor32();
        }

        public static Color32 ToColor32(this System.Drawing.Color color)
        {
            return new Color32(color.R, color.G, color.B, color.A);
        }
    }
}
#endif
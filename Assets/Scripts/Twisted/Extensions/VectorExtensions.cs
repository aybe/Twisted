using UnityEngine;
using Vector4 = System.Numerics.Vector4;

namespace Twisted.Extensions
{
    public static class VectorExtensions
    {
        public static Vector3 ToVector3(this Vector4 value)
        {
            return new Vector3(value.X, value.Y, value.Z);
        }
    }
}
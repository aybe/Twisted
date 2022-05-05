using System;
using UnityEngine.UIElements;

namespace Twisted.Editor
{
    public static class Vector3FieldExtensions
    {
        public static void SetLabels(this Vector3Field field, string x, string y, string z)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            field.Q<FloatField>("unity-x-input").label = x;
            field.Q<FloatField>("unity-y-input").label = y;
            field.Q<FloatField>("unity-z-input").label = z;
        }
    }
}
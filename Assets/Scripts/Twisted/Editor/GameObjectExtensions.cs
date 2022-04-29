using System;
using UnityEngine;

namespace Twisted.Editor
{
    public static class GameObjectExtensions
    {
        public static GameObject CreateChild(this GameObject parent, string? name = null, PrimitiveType? primitiveType = null)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            var child = primitiveType != null ? GameObject.CreatePrimitive(primitiveType.Value) : new GameObject();

            if (name is not null)
            {
                child.name = name;
            }

            child.transform.SetParent(parent.transform);

            return child;
        }
    }
}
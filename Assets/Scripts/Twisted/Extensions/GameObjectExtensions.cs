using System;
using UnityEngine;

namespace Twisted.Extensions
{
    public static class GameObjectExtensions
    {
        public static GameObject CreateChild(this GameObject parent, string? name = null)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            var child = new GameObject();

            if (name is not null)
            {
                child.name = name;
            }

            child.transform.SetParent(parent.transform);

            return child;
        }
    }
}
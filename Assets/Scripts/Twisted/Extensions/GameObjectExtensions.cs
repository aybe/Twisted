using System;
using UnityEngine;

namespace Twisted.Extensions
{
    public static class GameObjectExtensions
    {
        public static GameObject CreateChild(this GameObject parent, string? name = null, params Type[] components)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            var child = new GameObject(name, components)
            {
                transform =
                {
                    parent = parent.transform
                }
            };

            return child;
        }
    }
}
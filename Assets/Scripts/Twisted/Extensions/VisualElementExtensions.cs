using System;
using UnityEngine.UIElements;

namespace Twisted.Extensions
{
    public static class VisualElementExtensions
    {
        public static T? GetParent<T>(this VisualElement element) where T : VisualElement
        {
            if (element is null)
                throw new ArgumentNullException(nameof(element));

            var current = element.parent;

            while (current != null)
            {
                if (current is T parent)
                {
                    return parent;
                }

                current = current.parent;
            }

            return null;
        }
    }
}
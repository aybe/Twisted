using System;
using UnityEditor;
using UnityEngine;

namespace Unity.Extensions.Editor
{
    public static class GenericMenuExtensions
    {
        public static void AddItem(this GenericMenu menu, bool enabled, GUIContent content, bool on, GenericMenu.MenuFunction2 func, object userData)
        {
            if (menu is null)
                throw new ArgumentNullException(nameof(menu));

            if (content is null)
                throw new ArgumentNullException(nameof(content));

            if (func is null)
                throw new ArgumentNullException(nameof(func));

            if (userData is null)
                throw new ArgumentNullException(nameof(userData));

            if (enabled)
            {
                menu.AddItem(content, on, func, userData);
            }
            else
            {
                menu.AddDisabledItem(content);
            }
        }
    }
}
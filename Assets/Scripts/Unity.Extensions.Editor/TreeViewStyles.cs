using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.Extensions.Editor
{
    public static class TreeViewStyles
    {
        public static IReadOnlyDictionary<TextAnchor, GUIStyle> Label { get; } = CreateDictionary(EditorStyles.label);

        private static IReadOnlyDictionary<TextAnchor, GUIStyle> CreateDictionary(GUIStyle style)
        {
            if (style is null)
                throw new ArgumentNullException(nameof(style));

            var dictionary = Enum
                .GetValues(typeof(TextAnchor))
                .Cast<TextAnchor>()
                .ToDictionary(s => s, s => new GUIStyle(style) { alignment = s });

            return new ReadOnlyDictionary<TextAnchor, GUIStyle>(dictionary);
        }
    }
}
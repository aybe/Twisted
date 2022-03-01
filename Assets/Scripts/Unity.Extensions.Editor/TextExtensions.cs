using System;
using UnityEditor;
using UnityEngine;

namespace Unity.Extensions.Editor
{
    public static class TextExtensions
    {
        public static GUIContent Ellipsis(string? text, float width, GUIStyle style)
        {
            if (style is null)
                throw new ArgumentNullException(nameof(style));

            var source = text;
            var target = text;

            while (source != null && !string.IsNullOrWhiteSpace(source))
            {
                if (Event.current.type != EventType.Repaint)
                    break; // CalcSize only works on repaint

                var size = style.CalcSize(EditorGUIUtility.TrTempContent(target));

                if (size.x < width)
                    break;

                source = source[..^1];
                target = $"{source}...";
            }

            return EditorGUIUtility.TrTextContent(target);
        }
    }
}
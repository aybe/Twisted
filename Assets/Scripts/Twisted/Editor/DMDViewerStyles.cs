using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Twisted.Editor
{
    internal static class DMDViewerStyles
    {
        public static GUIContent Help { get; } = EditorGUIUtility.TrIconContent("_Help", "Open help for DMD Viewer.");

        public static GUIContent ColorVertexContent { get; } = EditorGUIUtility.TrIconContent("Material Icon", "Toggle vertex color tinting.");

        public static GlobalKeyword ColorVertexKeyword { get; } = GlobalKeyword.Create("DMD_VIEWER_COLOR_VERTEX");

        public static GUIContent ColorPolygonContent { get; } = EditorGUIUtility.TrIconContent("Grid.FillTool", "Toggle polygon color tinting.");

        public static GlobalKeyword ColorPolygonKeyword { get; } = GlobalKeyword.Create("DMD_VIEWER_COLOR_POLYGON");

        public static bool ToggleButton(bool value, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            if (content is null)
                throw new ArgumentNullException(nameof(content));

            if (style is null)
                throw new ArgumentNullException(nameof(style));

            using var scope = new EditorGUI.ChangeCheckScope();

            var toggle = GUILayout.Toggle(value, content, style, options);

            if (!scope.changed)
            {
                return false;
            }

            return toggle;
        }
    }
}
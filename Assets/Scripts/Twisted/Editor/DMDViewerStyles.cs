using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Twisted.Editor
{
    internal static class DMDViewerStyles
    {
        public static GUIContent HelpContent { get; } = EditorGUIUtility.TrIconContent("_Help", "Open help for DMD Viewer.");

        public static GUIContent ColorVertexContent { get; } = EditorGUIUtility.TrIconContent("Material Icon", "Toggle vertex color tinting.");

        public static GlobalKeyword ColorVertexKeyword { get; } = GlobalKeyword.Create("DMD_VIEWER_COLOR_VERTEX");

        public static GUIContent ColorPolygonContent { get; } = EditorGUIUtility.TrIconContent("Grid.FillTool", "Toggle polygon color tinting.");

        public static GlobalKeyword ColorPolygonKeyword { get; } = GlobalKeyword.Create("DMD_VIEWER_COLOR_POLYGON");
    }
}
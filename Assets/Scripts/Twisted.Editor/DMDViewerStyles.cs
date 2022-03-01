using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Twisted.Editor
{
    internal static class DMDViewerStyles
    {
        public static GUIContent HelpContent { get; } = EditorGUIUtility.TrIconContent("_Help", "Open help for DMD Viewer.");

        public static GUIContent OpenContent { get; } = EditorGUIUtility.TrIconContent("FolderOpened Icon", "Open a DMD file.");

        public static GUIContent ColorVertexContent { get; } = EditorGUIUtility.TrIconContent("Material Icon", "Toggle vertex color.");

        public static GlobalKeyword ColorVertexKeyword { get; } = GlobalKeyword.Create("DMD_VIEWER_COLOR_VERTEX");

        public static GUIContent ColorPolygonContent { get; } = EditorGUIUtility.TrIconContent("Grid.FillTool", "Toggle polygon color.");

        public static GlobalKeyword ColorPolygonKeyword { get; } = GlobalKeyword.Create("DMD_VIEWER_COLOR_POLYGON");

        public static GUIContent ModelSplitContent { get; } = EditorGUIUtility.TrIconContent("Grid Icon", "Toggle model split.");

        public static GUIContent SceneViewFraming { get; } = EditorGUIUtility.TrIconContent("Camera Icon", "Toggle scene view framing.");

        public static GUIContent TextureContent { get; } = EditorGUIUtility.TrIconContent("Texture2D Icon", "Toggle texture.");

        public static GlobalKeyword TextureKeyword { get; } = GlobalKeyword.Create("DMD_VIEWER_TEXTURE");
    }
}
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Twisted.Editor
{
    internal static class DMDViewerStyles
    {
        #region Actions

        public static GUIContent Help { get; } =
            EditorGUIUtility.TrIconContent(
                "_Help",
                "Open help window."
            );

        public static GUIContent Open { get; } =
            EditorGUIUtility.TrIconContent(
                "FolderOpened Icon",
                "Open an existing DMD file."
            );

        #endregion

        #region Settings

        public static GUIContent FilteringDistinct { get; } =
            EditorGUIUtility.TrIconContent(
                "FilterByType",
                "Enable distinct filtering for search mode."
            );

        public static GUIContent FilteringRegex { get; } =
            EditorGUIUtility.TrIconContent(
                "FilterByLabel",
                "Enable regular expressions for search mode."
            );

        public static GUIContent Splitting { get; } =
            EditorGUIUtility.TrIconContent(
                "Grid Icon",
                "Enable splitting of models as polygons."
            );

        public static GUIContent Framing { get; } =
            EditorGUIUtility.TrIconContent(
                "Camera Icon",
                "Enable selection framing."
            );

        #endregion

        #region Shader

        public static GUIContent VertexColor { get; } =
            EditorGUIUtility.TrIconContent(
                "Material Icon",
                "Enable vertex colors."
            );

        public static GlobalKeyword VertexColorKeyword { get; } =
            GlobalKeyword.Create(
                "DMD_VIEWER_COLOR_VERTEX"
            );

        public static GUIContent PolygonColor { get; } =
            EditorGUIUtility.TrIconContent(
                "Grid.FillTool",
                "Enable polygon family coloring."
            );

        public static GlobalKeyword PolygonColorKeyword { get; } =
            GlobalKeyword.Create(
                "DMD_VIEWER_COLOR_POLYGON"
            );

        public static GUIContent ModelTexture { get; } =
            EditorGUIUtility.TrIconContent(
                "Texture2D Icon",
                "Enable texturing."
            );

        public static GlobalKeyword ModelTextureKeyword { get; } =
            GlobalKeyword.Create(
                "DMD_VIEWER_TEXTURE"
            );

        #endregion
    }
}
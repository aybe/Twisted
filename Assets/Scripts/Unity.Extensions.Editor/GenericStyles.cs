using UnityEditor;
using UnityEngine;

namespace Unity.Extensions.Editor
{
    public static class GenericStyles
    {
        public static GUIStyle LabelMiniCentered { get; } = new(EditorStyles.miniLabel)
        {
            alignment = TextAnchor.MiddleLeft
        };
    }
}
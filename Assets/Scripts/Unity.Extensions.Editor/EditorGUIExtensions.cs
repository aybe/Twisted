using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;

namespace Unity.Extensions.Editor
{
    public static class EditorGUIExtensions
    {
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

        public static void ToggleButtonShaderKeyword(GlobalKeyword keyword, GUIContent content, GUIStyle? style = null)
        {
            var enabled = Shader.IsKeywordEnabled(keyword);

            using var scope = new EditorGUI.ChangeCheckScope();

            var value = ToggleButton(enabled, content, style ?? GUI.skin.button);

            if (!scope.changed)
                return;

            Shader.SetKeyword(keyword, value);

            InternalEditorUtility.RepaintAllViews();
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Twisted.Editor.Extensions
{
    internal sealed class ProjectGenerationSettingsProvider : SettingsProvider
    {
        private SerializedProperty Nullable = null!;
        private SerializedProperty NullableWarningsDisabled = null!;
        private SerializedProperty PreserveProjectStructure = null!;
        private SerializedProperty RemoveRootNamespace = null!;
        private SerializedObject SerializedObject = null!;

        private ProjectGenerationSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
            : base(path, scopes, keywords)
        {
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            var settings = ProjectGenerationSettings.instance;

            settings.Save();

            SerializedObject = new SerializedObject(settings);

            Nullable = SerializedObject.FindProperty(nameof(ProjectGenerationSettings.Nullable));

            NullableWarningsDisabled = SerializedObject.FindProperty(nameof(ProjectGenerationSettings.NullableWarningsDisabled));

            PreserveProjectStructure = SerializedObject.FindProperty(nameof(ProjectGenerationSettings.PreserveProjectStructure));

            RemoveRootNamespace = SerializedObject.FindProperty(nameof(ProjectGenerationSettings.RemoveEmptyRootNamespace));
        }

        public override void OnGUI(string searchContext)
        {
            SerializedObject.Update();

            const float width = 250.0f;

            using (new SettingsGUIScope(width))
            {
                using (var cs = new EditorGUI.ChangeCheckScope())
                {
                    DrawBoolProperty(Nullable, Styles.Nullable);

                    DrawBoolProperty(NullableWarningsDisabled, Styles.NullableWarningsDisabled);

                    if (false)
#pragma warning disable CS0162
                        // ReSharper disable once HeuristicUnreachableCode
                    {
                        DrawBoolProperty(PreserveProjectStructure, Styles.PreserveProjectStructure);

                        DrawBoolProperty(RemoveRootNamespace, Styles.RemoveRootNamespace);
                    }
#pragma warning restore CS0162
                    
                    if (cs.changed)
                    {
                        SerializedObject.ApplyModifiedProperties();
                        ProjectGenerationSettings.instance.Save();
                    }
                }

                EditorGUILayout.Space();

                if (GUILayout.Button("Regenerate Project Files", GUILayout.Width(width)))
                {
                    Unity.CodeEditor.CodeEditor.CurrentEditor.SyncAll();
                }
            }
        }

        private static void DrawBoolProperty(SerializedProperty property, GUIContent label)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            if (property.propertyType != SerializedPropertyType.Boolean)
                throw new ArgumentOutOfRangeException(nameof(property));

            if (label == null)
                throw new ArgumentNullException(nameof(label));

            var rect = EditorGUILayout.GetControlRect();

            using var ps = new EditorGUI.PropertyScope(rect, label, property);
            using var cs = new EditorGUI.ChangeCheckScope();

            var value = EditorGUI.ToggleLeft(rect, ps.content, property.boolValue);

            if (cs.changed)
            {
                property.boolValue = value;
            }
        }

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new ProjectGenerationSettingsProvider(
                "Project/Project Generation",
                SettingsScope.Project,
                new[] { "c#", "csproj", "project" }
            );
        }

        private static class Styles
        {
            public static readonly GUIContent Nullable = new(
                "Enable #nullable enable at project-level.",
                "Equivalent to adding #nullable enable at the top of each file."
            );

            public static readonly GUIContent NullableWarningsDisabled = new(
                "Enable CS8632 at project-level.",
                "Removes false warnings about missing nullable context in Visual Studio."
            );

            public static readonly GUIContent PreserveProjectStructure = new(
                "Rebuild project structure for local packages.",
                "Rebuilds project directory structure ignored by Unity while generating a project file."
            );

            public static readonly GUIContent RemoveRootNamespace = new(
                "Remove empty root namespace.",
                "Removes false warnings about incorrect namespaces in ReSharper."
            );
        }

        private sealed class SettingsGUIScope : GUI.Scope
        {
            private readonly float LabelWidth;

            public SettingsGUIScope(float labelWidth = 300.0f)
            {
                LabelWidth = EditorGUIUtility.labelWidth;

                EditorGUIUtility.labelWidth = labelWidth;

                GUILayout.BeginHorizontal();
                GUILayout.Space(9.0f);

                GUILayout.BeginVertical();
                GUILayout.Space(12.0f);
            }

            protected override void CloseScope()
            {
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                EditorGUIUtility.labelWidth = LabelWidth;
            }
        }
    }
}
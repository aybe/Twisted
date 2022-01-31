using Unity.CodeEditor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Twisted.Editor
{
    [FilePath("ProjectSettings/" + nameof(ProjectGenerationSettings) + ".asset", FilePathAttribute.Location.ProjectFolder)]
    internal sealed class ProjectGenerationSettings : ScriptableSingleton<ProjectGenerationSettings>
    {
        private const string Header = "Project Generation";

        [Tooltip("Enables '#nullable enable' at project-level and disables CS8632 warnings.")]
        public bool Nullable;

        private void OnEnable()
        {
            hideFlags &= ~HideFlags.NotEditable;
        }

        [SettingsProvider]
        public static SettingsProvider GetSettingsProvider()
        {
            var provider = new SettingsProvider($"Project/{Header}", SettingsScope.Project)
            {
                label = Header,
                activateHandler = (context, root) =>
                {
                    if (!string.IsNullOrWhiteSpace(context))
                    {
                        // TODO actually, only IMGUI has the ability to highlight text
                    }

                    var container = new VisualElement
                    {
                        style =
                        {
                            marginLeft = 1.0f,
                            marginTop = 9.0f
                        }
                    };

                    root.Add(container);

                    var title = new Label
                    {
                        text = Header,
                        style =
                        {
                            fontSize = 19.0f,
                            unityFontStyleAndWeight = FontStyle.Bold,
                            marginBottom = 12.0f
                        }
                    };

                    container.Add(title);

                    var properties = new VisualElement
                    {
                        style =
                        {
                            flexDirection = FlexDirection.Column
                        }
                    };

                    container.Add(properties);

                    var button = new Button(() => CodeEditor.CurrentEditor.SyncAll())
                    {
                        text = "Regenerate project files",
                        style =
                        {
                            alignSelf = Align.FlexStart,
                            marginTop = 12.0f,
                            paddingLeft = 12.0f,
                            paddingRight = 12.0f
                        }
                    };

                    container.Add(button);

                    var settings = new SerializedObject(instance);

                    properties.Add(new PropertyField(settings.FindProperty(nameof(Nullable))));

                    root.Bind(settings);

                    root.RegisterCallback<SerializedPropertyChangeEvent>(_ => { instance.Save(true); });
                },
                keywords = new[] { "project", "C#", "csharp", "csproj", "nullable" }
            };

            return provider;
        }
    }
}
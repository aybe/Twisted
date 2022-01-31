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

        public bool Nullable;

        public bool NullableWarningDisable;

        public bool PreserveDirectoryStructure;

        private void Reset()
        {
            Nullable = true;
            NullableWarningDisable = true;
            PreserveDirectoryStructure = true;
        }

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

                    var toggle1 = CreateToggleLeft("Define #nullable enable at project-level.");
                    toggle1.BindProperty(settings.FindProperty(nameof(Nullable)));
                    properties.Add(toggle1);

                    var toggle2 = CreateToggleLeft("Define CS8632 at project-level.");
                    toggle2.BindProperty(settings.FindProperty(nameof(NullableWarningDisable)));
                    properties.Add(toggle2);

                    var toggle3 = CreateToggleLeft("Preserve project structure for local assembly definitions.");
                    toggle3.BindProperty(settings.FindProperty(nameof(PreserveDirectoryStructure)));
                    properties.Add(toggle3);

                    root.Bind(settings);

                    root.RegisterCallback<SerializedPropertyChangeEvent>(_ => { instance.Save(true); });
                },
                keywords = new[] { "project", "C#", "csharp", "csproj", "nullable" }
            };

            return provider;
        }

        private static Toggle CreateToggleLeft(string label)
        {
            var toggle = new Toggle(label)
            {
                style =
                {
                    alignSelf = Align.FlexStart,
                    flexDirection = FlexDirection.RowReverse
                }
            };

            var element1 = toggle.Q(className: Toggle.inputUssClassName);

            element1.style.flexGrow = 0.0f;

            var element2 = toggle.Q(className: Toggle.labelUssClassName);

            element2.style.marginLeft = 2.0f;
            element2.style.paddingLeft = 2.0f;

            return toggle;
        }
    }
}
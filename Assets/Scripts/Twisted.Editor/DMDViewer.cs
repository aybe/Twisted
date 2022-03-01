using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Twisted.Graphics;
using Unity.Extensions.Editor;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Twisted.Editor
{
    internal sealed class DMDViewer : EditorWindow
    {
        #region Methods

        private void InitializeFactory(string? path)
        {
            if (File.Exists(path))
            {
                State.Factory     = DMDFactory.Create(path!);
                State.FactoryPath = path;
                titleContent.text = Path.GetFileName(path);
            }
            else
            {
                State.Factory     = null;
                State.FactoryPath = null;
                titleContent.text = "DMD Viewer";
            }
        }

        [MenuItem("Twisted/DMD Viewer")]
        private static void InitializeWindow()
        {
            var type   = typeof(EditorWindow).Assembly.GetType("UnityEditor.ProjectBrowser");
            var types  = type != null ? new[] { type } : Array.Empty<Type>();
            var window = GetWindow<DMDViewer>(types);
            window.titleContent = new GUIContent(EditorGUIUtility.IconContent("CustomTool"))
            {
                text = "DMD Viewer"
            };
        }

        #endregion

        #region Fields

        private static DMDPreview Preview => Singleton<DMDPreview>.instance;

        private static DMDViewerSettings Settings => DMDViewerSettings.instance;

        [SerializeField]
        private DMDViewerState State = new();

        private TreeView<DMDNode> View = null!;

        private SearchField ViewSearch = null!;

        #endregion

        #region Unity

        private void Awake()
        {
            // initialize the shader once with sensible defaults that looks as intended

            Shader.SetKeyword(DMDViewerStyles.TextureKeyword,     true);
            Shader.SetKeyword(DMDViewerStyles.ColorVertexKeyword, true);
        }

        private void OnDisable()
        {
            Settings.Save();
        }

        private void OnEnable()
        {
            // initialize factory from last opened file if any

            InitializeFactory(State.FactoryPath);

            // initialize the view and search field

            State.ViewState ??= new TreeViewState();

            var columns = new MultiColumnHeaderState.Column[]
            {
                new TreeViewColumn<DMDNode>(s => s.GetType().Name)
                {
                    headerContent   = EditorGUIUtility.TrTextContent("Name"),
                    width           = 350.0f,
                    minWidth        = 200.0f,
                    maxWidth        = 500.0f,
                    TextAnchor      = TextAnchor.MiddleLeft,
                    IsPrimaryColumn = true
                },
                new TreeViewColumn<DMDNode>(s => $"0x{(s.NodeType >> 16) & 0xFFFF:X4}")
                {
                    headerContent = EditorGUIUtility.TrTextContent("Type 1"),
                    width         = 75.0f,
                    minWidth      = 75.0f,
                    maxWidth      = 75.0f,
                    TextAnchor    = TextAnchor.MiddleRight
                },
                new TreeViewColumn<DMDNode>(s => $"0x{(s.NodeType >> 00) & 0xFFFF:X4}")
                {
                    headerContent = EditorGUIUtility.TrTextContent("Type 2"),
                    width         = 75.0f,
                    minWidth      = 75.0f,
                    maxWidth      = 75.0f,
                    TextAnchor    = TextAnchor.MiddleRight
                },
                new TreeViewColumn<DMDNode>(s => s.Position)
                {
                    headerContent = EditorGUIUtility.TrTextContent("Position"),
                    width         = 75.0f,
                    minWidth      = 75.0f,
                    maxWidth      = 75.0f,
                    TextAnchor    = TextAnchor.MiddleRight
                },
                new TreeViewColumn<DMDNode>(s => s.Length)
                {
                    headerContent = EditorGUIUtility.TrTextContent("Length"),
                    minWidth      = 75.0f,
                    maxWidth      = 75.0f,
                    width         = 75.0f,
                    TextAnchor    = TextAnchor.MiddleRight
                },
                new TreeViewColumn<DMDNode>(s => (s as DMDNode00FF)?.GetPolygonsString() ?? "N/A")
                {
                    headerContent = EditorGUIUtility.TrTextContent("Polygons"),
                    width         = 500.0f,
                    minWidth      = 100.0f,
                    maxWidth      = 999.0f,
                    TextAnchor    = TextAnchor.MiddleLeft
                }
            };

            foreach (var column in columns)
            {
                column.allowToggleVisibility = true;
                column.autoResize            = false; // BUG disable column auto-resize otherwise stupid control horizontal scrollbar will constantly flicker
                column.canSort               = true;
            }

            var headerState = new TreeViewColumnHeaderState(columns);
            var header      = new TreeViewColumnHeader(headerState);

            headerState.maximumNumberOfSortedColumns = 1;

            if (MultiColumnHeaderState.CanOverwriteSerializedFields(State.ViewStateHeader, headerState))
                MultiColumnHeaderState.OverwriteSerializedFields(State.ViewStateHeader, headerState);

            if (State.ViewStateHeader is null)
            {
                header.ResizeToFit();
            }

            State.ViewStateHeader ??= headerState;

            View = new TreeView<DMDNode>(State.ViewState, header)
            {
                OnCanMultiSelect = _ => false,
                Root             = State.Factory?.DMD,
                RowHeight        = State.ViewHeight,
                searchString     = State.ViewState!.searchString
            };

            View.NodeMouseContextClick += (_, e) =>
            {
                var menu = new GenericMenu();

                menu.AddItem(
                    e.Node is DMDNode00FF, EditorGUIUtility.TrTextContent("Refresh"),
                    false,
                    s =>
                    {
                        DMDPreview.Instance.SetNode(State.Factory, s as DMDNode00FF, State.FactorySplit, false); // don't frame now or it'll be choppy
                    },
                    e.Node
                );

                menu.AddSeparator("/");

                var data = string.Concat(((DMDNode)e.Node).GetObjectData().Select(s => s.ToString("X2")));

                menu.AddItem(
                    EditorGUIUtility.TrTextContent("Hex Dump/Clipboard"),
                    false,
                    s =>
                    {
                        EditorGUIUtility.systemCopyBuffer = s as string;
                    },
                    data
                );

                menu.AddItem(
                    EditorGUIUtility.TrTextContent("Hex Dump/Console"),
                    false,
                    s =>
                    {
                        Debug.Log(s.ToString());
                    },
                    data
                );

                menu.ShowAsContext();

                EditorApplication.delayCall += () => DMDPreview.Instance.FrameSelection(); // frame now, it won't be choppy
            };

            View.NodeSelectionChanged += (_, e) =>
            {
                DMDPreview.Instance.SetNode(State.Factory, e.Nodes.OfType<DMDNode00FF>().FirstOrDefault(), State.FactorySplit, Event.current.button == 0); // above will frame
            };

            ViewSearch = new SearchField();

            ViewSearch.downOrUpArrowKeyPressed += () => View.SetFocusAndEnsureSelectedItem();
        }

        private void OnDestroy()
        {
            DestroyImmediate(DMDPreview.Instance.gameObject); // leave scene clear after we close
        }

        private void OnGUI()
        {
            var disabled = State.Factory is null; // enable relevant UI only when a file has been loaded

            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                using (new EditorGUIUtility.IconSizeScope(new Vector2(16.0f, 16.0f)))
                {
                    if (GUILayout.Button(DMDViewerStyles.OpenContent, EditorStyles.toolbarButton))
                    {
                        var path = EditorUtility.OpenFilePanel(null, null, "DMD");

                        if (string.IsNullOrEmpty(path))
                            return;

                        InitializeFactory(path);

                        View.Root = State.Factory!.DMD;

                        // before reloading, clear active state so that handlers may perform some cleanup too

                        View.CollapseAll();

                        View.SetSelection(Array.Empty<int>(), TreeViewSelectionOptions.FireSelectionChanged);

                        View.Reload();
                    }

                    EditorGUILayout.Space();

                    using (new EditorGUI.DisabledScope(disabled))
                    {
                        using (var scope = new EditorGUI.ChangeCheckScope())
                        {
                            var value = EditorGUIExtensions.ToggleButton(State.FactorySplit,
                                DMDViewerStyles.ModelSplitContent,
                                EditorStyles.toolbarButton
                            );

                            if (scope.changed)
                            {
                                State.FactorySplit = value;
                                View.SetSelection(View.GetSelection(), TreeViewSelectionOptions.FireSelectionChanged);
                            }
                        }

                        EditorGUILayout.Space();

                        EditorGUIExtensions.ToggleButtonShaderKeyword(
                            DMDViewerStyles.TextureKeyword, DMDViewerStyles.TextureContent,
                            EditorStyles.toolbarButton
                        );

                        EditorGUILayout.Space();

                        EditorGUIExtensions.ToggleButtonShaderKeyword(
                            DMDViewerStyles.ColorVertexKeyword, DMDViewerStyles.ColorVertexContent,
                            EditorStyles.toolbarButton
                        );

                        EditorGUILayout.Space();

                        EditorGUIExtensions.ToggleButtonShaderKeyword(
                            DMDViewerStyles.ColorPolygonKeyword, DMDViewerStyles.ColorPolygonContent,
                            EditorStyles.toolbarButton
                        );

                        GUILayout.FlexibleSpace();

                        if (!string.IsNullOrWhiteSpace(View.searchString))
                        {
                            GUILayout.Label(
                                EditorGUIUtility.TrTempContent($"{View.GetRows().Count} items found"),
                                GenericStyles.LabelMiniCentered,
                                GUILayout.Height(EditorGUIUtility.singleLineHeight)
                            );
                        }

                        using (var scope = new EditorGUI.ChangeCheckScope())
                        {
                            var value = GUILayout.HorizontalSlider(State.ViewHeight, 16, 32, GUILayout.Width(75.0f));

                            if (scope.changed)
                            {
                                View.RowHeight = State.ViewHeight = value;
                            }
                        }

                        EditorGUILayout.Space();

                        using (var scope = new EditorGUI.ChangeCheckScope())
                        {
                            var value = ViewSearch.OnToolbarGUI(View.searchString, GUILayout.Width(125.0f));

                            if (scope.changed)
                            {
                                View.searchString = value;
                            }
                        }
                    }
                }
            }

            if (disabled is false)
            {
                View.OnGUI(GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandHeight(true)));
            }
        }

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members")]
        private void ShowButton(Rect rect)
        {
            if (GUI.Button(rect, DMDViewerStyles.HelpContent, EditorStyles.iconButton))
            {
                EditorUtility.DisplayDialog("DMD Viewer", "TODO", "Close"); // TODO add help
            }
        }

        #endregion
    }
}
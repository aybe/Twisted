using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Twisted.Graphics;
using Unity.Extensions.Editor;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Twisted.Editor
{
    internal sealed class DMDViewer : EditorWindow
    {
        #region Methods

        private void OpenNode(DMDNode00FF? node, bool frame = true)
        {
            Singleton<DMDPreview>.instance.SetNode(Factory, node, frame: frame);
        }

        #endregion

        private static MultiColumnHeaderState.Column[] GetColumns()
        {
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
                column.autoResize            = false; // BUG control is fucked up, horizontal scrollbar keeps flickering otherwise
                column.canSort               = true;
            }

            return columns;
        }

        private static class Styles
        {
            public static GUIStyle MiniLabelCentered { get; } = new(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleLeft
            };
        }

        #region Unity

        [MenuItem("Twisted/DMD Viewer")]
        private static void Initialize()
        {
            var type   = typeof(EditorWindow).Assembly.GetType("UnityEditor.ProjectBrowser");
            var types  = type != null ? new[] { type } : Array.Empty<Type>();
            var window = GetWindow<DMDViewer>(types);
            window.titleContent = new GUIContent(EditorGUIUtility.IconContent("CustomTool"))
            {
                text = "DMD Viewer"
            };
        }

        private void Awake()
        {
            // enable both texture and vertex color once to make it look like it's intended
            Shader.SetKeyword(DMDViewerStyles.TextureKeyword,     true);
            Shader.SetKeyword(DMDViewerStyles.ColorVertexKeyword, true);
        }

        [SuppressMessage("ReSharper", "ConstantNullCoalescingCondition")]
        private void OnEnable()
        {
            // reload previous DMD if any, initialize the view, fetch settings from UI

            UpdateFactory(FactoryPath);

            ViewState ??= new TreeViewState();

            var headerState = new TreeViewColumnHeaderState(GetColumns());
            var header      = new TreeViewColumnHeader(headerState);

            headerState.maximumNumberOfSortedColumns = 1;

            if (MultiColumnHeaderState.CanOverwriteSerializedFields(ViewHeaderState, headerState))
                MultiColumnHeaderState.OverwriteSerializedFields(ViewHeaderState, headerState);

            if (ViewHeaderState is null)
            {
                header.ResizeToFit();
            }

            ViewHeaderState ??= headerState;

            View = new TreeView<DMDNode>(ViewState, header)
            {
                OnCanMultiSelect  = _ => false,
                OnGetNewSelection = ViewGetNewSelectionOverride,
                Root              = Factory?.DMD
            };

            View.NodeMouseContextClick += OnViewNodeMouseContextClick;

            View.NodeSelectionChanged += OnViewNodeSelectionChanged;

            ViewSearch = new SearchField();

            ViewSearch.downOrUpArrowKeyPressed += OnViewSearchKeyPressed;

            UpdateViewRowHeight();
            UpdateViewSearchString();
        }

        private List<int> ViewGetNewSelectionOverride(TreeViewItem clickedItem, bool keepMultiSelection, bool useActionKeyAsShift)
        {
            // we want a user-friendly context click behavior, i.e. one that doesn't sadistically changes actual selection

            var selection = new List<int>();

            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                selection.AddRange(View.GetSelection());
            }
            else
            {
                selection.Add(clickedItem.id);
            }

            return selection;
        }

        private void OnDisable()
        {
            View.NodeMouseContextClick -= OnViewNodeMouseContextClick;

            View.NodeSelectionChanged -= OnViewNodeSelectionChanged;

            ViewSearch.downOrUpArrowKeyPressed -= OnViewSearchKeyPressed;
        }

        private void OnDestroy()
        {
            DestroyImmediate(Singleton<DMDPreview>.instance.gameObject); // clear scene on close
        }

        private void OnGUI()
        {
            var disabled = Factory is null; // for enabling relevant UI only when a file has been loaded

            // main toolbar

            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                using (new EditorGUIUtility.IconSizeScope(new Vector2(16.0f, 16.0f)))
                {
                    if (GUILayout.Button(DMDViewerStyles.OpenContent, EditorStyles.toolbarButton))
                    {
                        var path = EditorUtility.OpenFilePanel(null, null, "DMD");

                        if (string.IsNullOrEmpty(path))
                            return;

                        UpdateFactory(path);

                        // View.SetRoot(Factory!.DMD); // BUG in fact there should be no need to reload at this point because View.OnGUI does

                        View.Root = Factory!.DMD;

                        View.CollapseAll();

                        View.SetSelection(Array.Empty<int>(), TreeViewSelectionOptions.FireSelectionChanged);

                        View.Reload();
                    }

                    EditorGUILayout.Space();

                    using (new EditorGUI.DisabledScope(disabled))
                    {
                        EditorGUIExtensions.ToggleButtonShaderKeyword(DMDViewerStyles.TextureKeyword, DMDViewerStyles.TextureContent, EditorStyles.toolbarButton);

                        EditorGUILayout.Space();

                        EditorGUIExtensions.ToggleButtonShaderKeyword(DMDViewerStyles.ColorVertexKeyword, DMDViewerStyles.ColorVertexContent, EditorStyles.toolbarButton);

                        EditorGUILayout.Space();

                        EditorGUIExtensions.ToggleButtonShaderKeyword(DMDViewerStyles.ColorPolygonKeyword, DMDViewerStyles.ColorPolygonContent, EditorStyles.toolbarButton);

                        GUILayout.FlexibleSpace();

                        if (!string.IsNullOrWhiteSpace(View.searchString))
                        {
                            GUILayout.Label(
                                EditorGUIUtility.TrTempContent($"{View.GetRows().Count} items found"),
                                Styles.MiniLabelCentered,
                                GUILayout.Height(EditorGUIUtility.singleLineHeight)
                            ); // already forgot how painful it was to center a label...    
                        }

                        using (var scope = new EditorGUI.ChangeCheckScope())
                        {
                            var value = GUILayout.HorizontalSlider(ViewRowHeight, 16, 32, GUILayout.Width(50.0f));

                            if (scope.changed)
                            {
                                ViewRowHeight = value;
                                UpdateViewRowHeight();
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

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private void ShowButton(Rect rect)
        {
            if (GUI.Button(rect, DMDViewerStyles.HelpContent, EditorStyles.iconButton))
            {
                EditorUtility.DisplayDialog("DMD Viewer", "TODO", "Close"); // TODO add help
            }
        }

        #endregion

        #region View

        private DMDFactory? Factory;

        [SerializeField]
        private string? FactoryPath;

        private TreeView<DMDNode> View = null!;

        [SerializeField]
        private float ViewRowHeight = 24;

        [SerializeField]
        private TreeViewState? ViewState;

        [SerializeField]
        private MultiColumnHeaderState? ViewHeaderState;

        private SearchField ViewSearch = null!;

        private void UpdateFactory(string? path)
        {
            if (File.Exists(path))
            {
                Factory           = DMDFactory.Create(path!);
                FactoryPath       = path;
                titleContent.text = Path.GetFileName(path);
            }
            else
            {
                Factory           = null;
                FactoryPath       = null;
                titleContent.text = "DMD Viewer";
            }
        }

        private void UpdateViewRowHeight()
        {
            View.RowHeight = ViewRowHeight;
        }

        private void UpdateViewSearchString()
        {
            View.searchString = ViewState!.searchString;
        }

        #endregion

        #region Handlers

        private void OnViewNodeMouseContextClick(object sender, TreeViewMouseClickEventArgs e)
        {
            var menu = new GenericMenu();

            {
                var content = EditorGUIUtility.TrTextContent("Refresh");

                if (e.Node is DMDNode00FF node)
                {
                    menu.AddItem(content, false, s => OpenNode(s as DMDNode00FF, false), node);
                }
                else
                {
                    menu.AddDisabledItem(content);
                }
            }

            menu.AddSeparator("/");

            var data = string.Concat(((DMDNode)e.Node).GetObjectData().Select(s => s.ToString("X2")));

            menu.AddItem(
                EditorGUIUtility.TrTextContent("Hex Dump/Clipboard"),
                false,
                s => { EditorGUIUtility.systemCopyBuffer = s as string; },
                data
            );

            menu.AddItem(
                EditorGUIUtility.TrTextContent("Hex Dump/Console"),
                false,
                Debug.Log,
                data
            );

            menu.ShowAsContext();

            Repaint(); // coz' totally fucked up Unity may not show menu if one stays still...
        }

        private void OnViewNodeSelectionChanged(object sender, TreeViewSelectionEventArgs<DMDNode> e)
        {
            // animations and context menus combined don't play well at all, the former will get paused by the latter when it opens
            // furthermore, it's rather sadistic from end-user perspective to lose his actual selection for a simple context click

            var current = Event.current;

            if (current.type == EventType.MouseDown && current.button == 1)
                return;

            OpenNode(e.Nodes.OfType<DMDNode00FF>().FirstOrDefault());
        }

        private void OnViewSearchKeyPressed()
        {
            View.SetFocusAndEnsureSelectedItem();
        }

        #endregion
    }
}
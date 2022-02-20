using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Twisted.PS;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;

namespace Twisted.Editor
{
    internal sealed class DMDViewer : EditorWindow
    {
        #region Unity

        [MenuItem("Twisted/DMD Viewer")]
        private static void Initialize()
        {
            var window = GetWindow<DMDViewer>();

            window.titleContent = new GUIContent(EditorGUIUtility.IconContent("CustomTool"))
            {
                text = "DMD Viewer"
            };
        }

        [SuppressMessage("ReSharper", "ConstantNullCoalescingCondition")]
        private void OnEnable()
        {
            // reload previous DMD if any, initialize the view, fetch settings from UI

            UpdateFactory(FactoryPath);

            ViewState ??= new TreeViewState();

            View = new TreeNodeView(ViewState, Factory?.DMD);

            View.NodeMouseContextClick += OnViewNodeMouseContextClick;

            View.NodeSelectionChanged += OnViewNodeSelectionChanged;

            View.SearchFilter = (_, items) =>
            {
                return items // filter dupe nodes during filtering
                    .Cast<TreeViewItem<TreeNode>>()
                    .GroupBy(s => s.Data, DMDViewerNodePositionComparer.Instance)
                    .Select(s => s.First())
                    .Cast<TreeViewItem>()
                    .ToList();
            };

            ViewSearch = new SearchField();

            ViewSearch.downOrUpArrowKeyPressed += OnViewSearchKeyPressed;

            UpdateViewRowHeight();
            UpdateViewSearchString();
        }

        private void OnDisable()
        {
            View.NodeMouseContextClick -= OnViewNodeMouseContextClick;

            View.NodeSelectionChanged -= OnViewNodeSelectionChanged;

            ViewSearch.downOrUpArrowKeyPressed -= OnViewSearchKeyPressed;
        }

        private void OnGUI()
        {
            // main toolbar

            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                var content = EditorGUIUtility.TrTextContent("File");
                var style   = EditorStyles.toolbarDropDown;
                var rect    = GUILayoutUtility.GetRect(content, style);

                if (EditorGUI.DropdownButton(rect, content, FocusType.Passive, style))
                {
                    var menu = new GenericMenu();

                    menu.AddItem(new GUIContent("Open..."), false, () =>
                    {
                        var path = EditorUtility.OpenFilePanel(null, null, "DMD");

                        if (string.IsNullOrEmpty(path))
                            return;

                        UpdateFactory(path);

                        View.SetRoot(Factory.DMD);
                    });

                    menu.DropDown(rect);
                }

                EditorGUILayout.Space();

                using (new EditorGUIUtility.IconSizeScope(new Vector2(16.0f, 16.0f)))
                {
                    DrawGlobalKeywordToggle(Styles.ColorVertexKeyword, Styles.ColorVertexContent, EditorStyles.toolbarButton);

                    EditorGUILayout.Space();

                    DrawGlobalKeywordToggle(Styles.ColorPolygonKeyword, Styles.ColorPolygonContent, EditorStyles.toolbarButton);
                }

                GUILayout.FlexibleSpace();

                using (new EditorGUI.DisabledScope(!View.HasRoot))
                {
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

            View.OnGUI(GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandHeight(true)));
        }

        #endregion

        #region View

        private DMDFactory Factory;

        [SerializeField]
        private string? FactoryPath;

        private TreeNodeView View = null!;

        [SerializeField]
        private float ViewRowHeight = 24;

        [SerializeField]
        private TreeViewState ViewState = null!;

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
            View.SetRowHeight(ViewRowHeight);
        }

        private void UpdateViewSearchString()
        {
            View.searchString = ViewState.searchString;
        }

        #endregion

        #region Methods

        private void OpenNode(DMDNode00FF? node, bool frame = true)
        {
            Singleton<DMDPreview>.instance.SetNode(Factory, node, frame: frame);
        }

        #endregion

        #region Handlers

        private void OnViewNodeMouseContextClick(object sender, TreeNodeClickEventArgs e)
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

        private void OnViewNodeSelectionChanged(object sender, TreeNodeSelectionEventArgs e)
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

        #region Help

        private void ShowButton(Rect rect)
        {
            if (GUI.Button(rect, Styles.Help, EditorStyles.iconButton))
            {
                EditorUtility.DisplayDialog("DMD Viewer", "TODO", "Close"); // TODO add help
            }
        }

        private static void DrawGlobalKeywordToggle(GlobalKeyword keyword, GUIContent content, GUIStyle? style = null)
        {
            var enabled = Shader.IsKeywordEnabled(keyword);

            using var scope = new EditorGUI.ChangeCheckScope();

            var value = Styles.ToggleButton(enabled, content, style ?? GUI.skin.button);

            if (!scope.changed)
                return;

            Shader.SetKeyword(keyword, value);

            InternalEditorUtility.RepaintAllViews();
        }

        private static class Styles
        {
            public static GUIContent Help { get; } = EditorGUIUtility.TrIconContent("_Help", "Open help for DMD Viewer.");

            public static GUIContent ColorVertexContent { get; } = EditorGUIUtility.TrIconContent("Material Icon", "Toggle vertex color tinting.");

            public static GlobalKeyword ColorVertexKeyword { get; } = GlobalKeyword.Create("DMD_VIEWER_COLOR_VERTEX");

            public static GUIContent ColorPolygonContent { get; } = EditorGUIUtility.TrIconContent("Grid.FillTool", "Toggle polygon color tinting.");

            public static GlobalKeyword ColorPolygonKeyword { get; } = GlobalKeyword.Create("DMD_VIEWER_COLOR_POLYGON");

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
        }

        #endregion
    }
}
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Twisted.PS;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

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
                        var value = ViewSearch.OnToolbarGUI(ViewSearchString, GUILayout.Width(125.0f));

                        if (scope.changed)
                        {
                            ViewSearchString = value;
                            UpdateViewSearchString();
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

        [SerializeField]
        private string ViewSearchString = null!;

        private void UpdateFactory(string? path)
        {
            Factory     = File.Exists(path) ? DMDFactory.Create(path!) : null;
            FactoryPath = path;

            titleContent.text = Path.GetFileName(path);
        }

        private void UpdateViewRowHeight()
        {
            View.SetRowHeight(ViewRowHeight);
        }

        private void UpdateViewSearchString()
        {
            View.searchString = ViewSearchString;
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

        private static class Styles
        {
            public static GUIContent Help { get; } = EditorGUIUtility.TrIconContent("_Help", "Open help for DMD Viewer.");
        }

        #endregion
    }
}
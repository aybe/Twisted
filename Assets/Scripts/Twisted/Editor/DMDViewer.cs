using System;
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
            // initialize the view, reload previous DMD if any, fetch settings from UI

            ViewState ??= new TreeViewState();

            View = new TreeNodeView(ViewState, string.IsNullOrEmpty(ViewPath) ? null : OpenFile(ViewPath!));

            View.NodeSelectionChanged += OnNodeClicked;

            View.NodeMouseContextClick += ViewOnNodeMouseContextClick;

            ViewSearch = new SearchField();

            ViewSearch.downOrUpArrowKeyPressed += OnViewSearchKeyPressed;

            ViewUpdateRowHeight();
            ViewUpdateSearchString();
        }

        private void OnDisable()
        {
            View.NodeSelectionChanged -= OnNodeClicked;

            View.NodeMouseContextClick -= ViewOnNodeMouseContextClick;
            
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

                        View.SetRoot(OpenFile(path));

                        ViewPath = path;
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
                            ViewUpdateRowHeight();
                        }
                    }

                    EditorGUILayout.Space();

                    using (var scope = new EditorGUI.ChangeCheckScope())
                    {
                        var value = ViewSearch.OnToolbarGUI(ViewSearchString, GUILayout.Width(125.0f));

                        if (scope.changed)
                        {
                            ViewSearchString = value;
                            ViewUpdateSearchString();
                        }
                    }
                }
            }

            View.OnGUI(GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandHeight(true)));
        }

        #endregion

        #region View

        private TreeNodeView View = null!;

        [SerializeField]
        private string? ViewPath;

        [SerializeField]
        private float ViewRowHeight = 24;

        [SerializeField]
        private TreeViewState ViewState = null!;

        private SearchField ViewSearch = null!;

        [SerializeField]
        private string ViewSearchString = null!;

        private void OnNodeClicked(object sender, TreeNodeSelectionEventArgs e)
        {
           OpenNode(e.Nodes.OfType<DMDNode00FF>().FirstOrDefault());
        }

        private void OnViewSearchKeyPressed()
        {
            View.SetFocusAndEnsureSelectedItem();
        }

        private void ViewOnNodeMouseContextClick(object sender, TreeNodeClickEventArgs e)
        {
            var data = string.Concat(((DMDNode)e.Node).GetObjectData().Select(s => s.ToString("X2")));

            var menu = new GenericMenu();

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

        private void ViewUpdateRowHeight()
        {
            View.SetRowHeight(ViewRowHeight);
        }

        private void ViewUpdateSearchString()
        {
            View.searchString = ViewSearchString;
        }

        #endregion

        #region Methods

        private static DMD OpenFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));

            using var reader = new BinaryReader(new MemoryStream(File.ReadAllBytes(path)));

            var dmd = new DMD(reader);

            return dmd;
        }

        private static void OpenNode(DMDNode00FF node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            Singleton<DMDViewerPreview>.instance.SetNode(node);
        }

        #endregion
    }
}
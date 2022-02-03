﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Twisted.PS;
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

            View.OnSelectionChanged += OnViewSelectionChanged;

            ViewSearch = new SearchField();

            ViewSearch.downOrUpArrowKeyPressed += OnViewSearchKeyPressed;

            ViewUpdateRowHeight();
            ViewUpdateSearchString();
        }

        private void OnDisable()
        {
            View.OnSelectionChanged -= OnViewSelectionChanged;

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

        private void OnViewSelectionChanged(object sender, TreeNodeViewSelectionEventArgs e)
        {
            foreach (var node in e.Nodes)
            {
                Debug.Log(node);
            }
        }

        private void OnViewSearchKeyPressed()
        {
            View.SetFocusAndEnsureSelectedItem();
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

        #endregion
    }
}
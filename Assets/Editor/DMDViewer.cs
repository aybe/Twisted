using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Twisted.Graphics;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    public sealed class DMDViewer : EditorWindow
        // TODO column sorting + arrows
        // TODO filtering + toolbar hint
        // TODO context menus
        // TODO try fix horizontal scroll bar
        // TODO save state
        // TODO when another file is loaded, reset internal tree state
        // TODO what becomes primary column should be left aligned
        // TODO everything missing from legacy viewer
        // TODO
        // BUG when tree view loses focus, one of its expander may turn blue at any time
        // BUG tree view keyboard expand/collapse may stop working for no reason at all
        // BUG horizontal scroll bar flickers and partially hides selected item at bottom
        // BUG 
        // BUG multi-column sort destroys expand state
        // BUG 
        // BUG when sorting, selected item is lost
        // BUG 
    {
        [SerializeField]
        private DMDViewerModel Model = null!;

        [SerializeField]
        private VisualTreeAsset VisualTreeAsset = null!;

        private Label ToolbarSearchResults =>
            rootVisualElement.Q<Label>("toolbarLabelSearchResults");

        private ToolbarSearchField ToolbarSearchField =>
            rootVisualElement.Q<ToolbarSearchField>("toolbarSearchField");

        private Toolbar ToolbarBreadcrumbsHost =>
            rootVisualElement.Q<Toolbar>("toolbarBreadcrumbsHost");

        private ToolbarBreadcrumbs ToolbarBreadcrumbs =>
            rootVisualElement.Q<ToolbarBreadcrumbs>("toolbarBreadcrumbs");

        private DMDTreeView TreeView =>
            rootVisualElement.Q<DMDTreeView>();

        private void OnDisable()
        {
            TreeView.onSelectionChange -= OnTreeViewSelectionChange;

            TreeView.Dispose(); // for columns callback
        }

        public void CreateGUI()
        {
            var root = rootVisualElement;

            var container = VisualTreeAsset.Instantiate();

            container.style.flexGrow = 1;

            root.Add(container);

            var toolbarButtonOpenFile          = root.Q<ToolbarButton>("toolbarButtonOpenFile");
            InitializeModel();
            InitializeSearch();

            InitializeTreeView();

            toolbarButtonOpenFile.clicked += OnToolbarOpenFile;
            // TODO other properties

            root.Bind(Model.SerializedObject);

            var searchField = root.Q<ToolbarSearchField>();

            root.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode is KeyCode.F && evt.ctrlKey)
                {
                    searchField.Focus();
                }
            });

            searchField.RegisterValueChangedCallback(evt =>
            {
                Debug.Log(evt.newValue); // TODO update tree view
            });

            searchField.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode is not KeyCode.DownArrow)
                    return;

                TreeView.Q<ScrollView>().contentContainer.Focus(); // shamelessly stolen from source
            });

            UpdateTreeViewAndBreadcrumbs();
            UpdateWindowTitle();
        }

        private void InitializeModel()
        {
            if (Model == null)
            {
                Model = CreateInstance<DMDViewerModel>();
            }

            Model.Initialize();
        }

        [MenuItem("Twisted/DMD Viewer (UI Elements)")]
        private static void InitializeWindow()
        {
            GetWindow<DMDViewer>();
        }

        private void UpdateTreeViewAndBreadcrumbs()
        {
            // try populate the tree

            var dmd = Model.DMDFactory?.DMD;

            TreeView.SetRoot(dmd); // more subtle than you think: it initializes their internal shit

            TreeView.sortColumnDescriptions.Clear(); // clean up garbage from previous file, if any

            TreeView.CollapseAll(); // also but stupid method NRE unless root is set, hence why here

            if (dmd is not null)
            {
                TreeView.SelectNode(dmd, true, true); // get breadcrumbs to show or UI will look bad
            }

            // show or hide depending DMD, because dumb ass tree will NRE when clicked but is empty

            var visible = dmd is not null;

            TreeView.visible = visible;

            ToolbarBreadcrumbsHost.visible = visible;
        }

        private void UpdateWindowTitle()
        {
            titleContent = new GUIContent(DMDViewerStyles.WindowTitle)
            {
                text = File.Exists(Model.CurrentFile) ? Path.GetFileName(Model.CurrentFile) : "DMD Viewer"
            };
        }

        private void InitializeTreeView()
        {
            TreeView.selectionType = SelectionType.Single;

            TreeView.sortingEnabled = true;

            TreeView.SetColumns(DMDTreeView.GetColumns());

            TreeView.onSelectionChange += OnTreeViewSelectionChange;
        }

        #region Event handlers

        private void OnToolbarOpenFile()
        {
            Model.OpenFile();
            UpdateTreeViewAndBreadcrumbs();
            UpdateWindowTitle();
        }

        private void OnToolbarSearchFieldValueChanged(ChangeEvent<string> evt)
        {
            var element = evt.target as VisualElement ?? throw new InvalidOperationException();

            var valid = TreeView.IsSearchPatternValid(evt.newValue);

            if (valid)
            {
                element.style.borderBottomColor = new StyleColor(new Color32(0xB7, 0xB7, 0xB7, 0xFF));
                element.style.borderLeftColor   = new StyleColor(new Color32(0xB7, 0xB7, 0xB7, 0xFF));
                element.style.borderRightColor  = new StyleColor(new Color32(0xB7, 0xB7, 0xB7, 0xFF));
                element.style.borderTopColor    = new StyleColor(new Color32(0xA0, 0xA0, 0xA0, 0xFF));
            }
            else
            {
                element.style.borderBottomColor = new StyleColor(new Color32(0xFF, 0x00, 0x00, 0xFF));
                element.style.borderLeftColor   = new StyleColor(new Color32(0xFF, 0x00, 0x00, 0xFF));
                element.style.borderRightColor  = new StyleColor(new Color32(0xFF, 0x00, 0x00, 0xFF));
                element.style.borderTopColor    = new StyleColor(new Color32(0xFF, 0x00, 0x00, 0xFF));
            }

            if (valid)
            {
                RemoveNotification();
            }
            else
            {
                ShowNotification(EditorGUIUtility.TrTempContent("Search pattern is not valid."));
            }

            if (valid)
            {
                TreeView.SetSearchFilter(evt.newValue);
            }

            ToolbarSearchResults.text = $"{TreeView.GetRowCount()} items found";
        }

        private void OnToolbarBreadcrumbsItemClick(ClickEvent evt)
        {
            // update breadcrumbs: clicking, elements, node stack

            var target = evt.target as VisualElement ?? throw new InvalidOperationException();
            var parent = target.parent;
            var index  = parent.IndexOf(target);

            for (var i = BreadcrumbsNodes.Count - 1; i > index; i--)
            {
                var element = parent.ElementAt(i);
                element.UnregisterCallback<ClickEvent>(OnToolbarBreadcrumbsItemClick);
                ToolbarBreadcrumbs.PopItem();
                BreadcrumbsNodes.RemoveAt(i);
            }

            // sync tree view selection with clicked breadcrumbs

            var node = BreadcrumbsNodes[index];

            TreeView.SelectNode(node, true, true);
        }

        private void OnTreeViewSelectionChange(IEnumerable<object> objects)
        {
            // update breadcrumbs: clicking, elements, node stack

            for (var i = 0; i < ToolbarBreadcrumbs.childCount; i++)
            {
                var element = ToolbarBreadcrumbs.ElementAt(i);
                element.UnregisterCallback<ClickEvent>(OnToolbarBreadcrumbsItemClick);
            }

            while (ToolbarBreadcrumbs.childCount > 0)
            {
                ToolbarBreadcrumbs.PopItem();
                BreadcrumbsNodes.RemoveAt(BreadcrumbsNodes.Count - 1);
            }

            var current = objects.Single() as DMDNode;

            while (current != null)
            {
                BreadcrumbsNodes.Insert(0, current);
                current = current.Parent as DMDNode;
            }

            foreach (var node in BreadcrumbsNodes)
            {
                ToolbarBreadcrumbs.PushItem($"0x{node.NodeType:X8}");
            }

            for (var i = 0; i < ToolbarBreadcrumbs.childCount; i++)
            {
                var element = ToolbarBreadcrumbs.ElementAt(i);
                element.RegisterCallback<ClickEvent>(OnToolbarBreadcrumbsItemClick);
            }
        }

        #endregion

        #region Search

        private void InitializeSearch()
        {
            ToolbarSearchField.RegisterValueChangedCallback(OnToolbarSearchFieldValueChanged);
        }

        #endregion

        #region Breadcrumbs

        private readonly List<DMDNode> BreadcrumbsNodes = new();

        #endregion
    }
}
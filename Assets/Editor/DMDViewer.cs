using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Twisted;
using Twisted.Graphics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace Editor
{
    public sealed class DMDViewer : EditorWindow
        // TODO save state
        // TODO when another file is loaded, reset internal tree state
        // TODO everything missing from legacy viewer
        // BUG when tree view loses focus, one of its expander may turn blue at any time
        // BUG tree view keyboard expand/collapse may stop working for no reason at all
        // BUG when clearing search breadcrumbs are not updated -> hide them
    {
        [SerializeField]
        private DMDViewerModel Model = null!;

        [SerializeField]
        private VisualTreeAsset VisualTreeAsset = null!;

        private static DMDPreview Preview => Singleton<DMDPreview>.instance;

        private void OnDisable()
        {
            CleanupTreeView();
        }

        private void OnDestroy()
        {
            DestroyImmediate(Preview.gameObject); // don't leave garbage on scene
        }

        public void CreateGUI()
        {
            InitializeModel();
            InitializeRoot();
            InitializeTreeView();
            InitializeToolbar();

            UpdateControls();
            UpdateTitle();
        }

        #region Controls

        private ToolbarButton ToolbarOpenFile =>
            rootVisualElement.Q<ToolbarButton>("toolbarOpenFile");

        private ToolbarToggle ToolbarDistinctFiltering =>
            rootVisualElement.Q<ToolbarToggle>("toolbarDistinctFiltering");

        private ToolbarToggle ToolbarSelectionFraming =>
            rootVisualElement.Q<ToolbarToggle>("toolbarSelectionFraming");

        private ToolbarToggle ToolbarModelSplitting =>
            rootVisualElement.Q<ToolbarToggle>("toolbarModelSplitting");

        private ToolbarToggle ToolbarTexturing =>
            rootVisualElement.Q<ToolbarToggle>("toolbarTexturing");

        private ToolbarToggle ToolbarVertexColors =>
            rootVisualElement.Q<ToolbarToggle>("toolbarVertexColors");

        private ToolbarToggle ToolbarPolygonColoring =>
            rootVisualElement.Q<ToolbarToggle>("toolbarPolygonColoring");

        private Label ToolbarSearchLabel =>
            rootVisualElement.Q<Label>("toolbarSearchLabel");

        private ToolbarSearchField ToolbarSearchField =>
            rootVisualElement.Q<ToolbarSearchField>("toolbarSearchField");

        private ToolbarBreadcrumbs ToolbarBreadcrumbs =>
            rootVisualElement.Q<ToolbarBreadcrumbs>("toolbarBreadcrumbs");

        private Toolbar ToolbarBreadcrumbsHost =>
            rootVisualElement.Q<Toolbar>("toolbarBreadcrumbsHost");

        private List<DMDNode> ToolbarBreadcrumbsNodes { get; } = new();

        private DMDTreeView TreeView =>
            rootVisualElement.Q<DMDTreeView>();

        #endregion

        #region Initialization/cleanup

        [MenuItem("Twisted/DMD Viewer (UI Elements)")]
        private static void InitializeWindow()
        {
            GetWindow<DMDViewer>();
        }

        private void InitializeModel()
        {
            if (Model == null)
            {
                Model = CreateInstance<DMDViewerModel>();
            }

            Model.Initialize();
        }

        private void InitializeRoot()
        {
            var root = rootVisualElement;

            var container = VisualTreeAsset.Instantiate();

            container.style.flexGrow = 1;

            root.Add(container);

            root.Bind(Model.SerializedObject);

            root.RegisterCallback<KeyDownEvent>(OnRootKeyDown);
        }

        private void InitializeToolbar()
        {
            ToolbarOpenFile.clicked += OnToolbarOpenFile;

            InitializeToolbarToggle(ToolbarDistinctFiltering, Model.UseDistinctFilteringProperty, OnToolbarDistinctFilteringValueChanged);

            ToolbarSelectionFraming.BindProperty(Model.UseSelectionFramingProperty);

            ToolbarSelectionFraming.RegisterValueChangedCallback(OnToolbarSelectionFramingValueChanged);

            ToolbarModelSplitting.BindProperty(Model.UseModelSplittingProperty);

            ToolbarModelSplitting.RegisterValueChangedCallback(OnToolbarModelSplittingValueChanged);

            InitializeToolbarToggle(ToolbarTexturing,       Model.UseTexturingProperty,     OnToolbarTexturingValueChanged);
            InitializeToolbarToggle(ToolbarVertexColors,    Model.UseVertexColorsProperty,  OnToolbarVertexColorsValueChanged);
            InitializeToolbarToggle(ToolbarPolygonColoring, Model.UsePolygonColorsProperty, OnToolbarPolygonColoringValueChanged);

            ToolbarSearchField.RegisterCallback<KeyDownEvent>(OnToolbarSearchFieldKeyDown);

            ToolbarSearchField.RegisterValueChangedCallback(OnToolbarSearchFieldValueChanged);
        }

        private static void InitializeToolbarToggle(
            ToolbarToggle toggle, SerializedProperty property, EventCallback<ChangeEvent<bool>> callback)
        {
            if (toggle == null)
                throw new ArgumentNullException(nameof(toggle));

            if (property == null)
                throw new ArgumentNullException(nameof(property));

            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            toggle.BindProperty(property);

            toggle.RegisterValueChangedCallback(callback);

            using var @event = ChangeEvent<bool>.GetPooled(property.boolValue, property.boolValue);

            callback(@event);
        }

        private void InitializeTreeView()
        {
            TreeView.selectionType = SelectionType.Single;

            TreeView.sortingEnabled = true;

            TreeView.SetColumns(DMDTreeView.GetColumns());

            TreeView.SelectionChanged += OnTreeViewSelectionChanged;

            TreeView.ContextMenuHandler = OnTreeViewContextMenuHandler;

            // instead of their buggy implementation, always show scrolling, that's cleaner

            var scrollView = TreeView.Q<ScrollView>();

            const ScrollerVisibility visibility = ScrollerVisibility.AlwaysVisible;

            scrollView.horizontalScrollerVisibility = visibility;
            scrollView.verticalScrollerVisibility   = visibility;

            // and also some fixes put in USS side as things aren't even properly centered
        }

        private void CleanupTreeView()
        {
            TreeView.SelectionChanged -= OnTreeViewSelectionChanged;

            TreeView.Dispose(); // for columns callback
        }

        #endregion

        #region Updaters

        private void UpdateControls()
        {
            // try populate the tree

            var dmd = Model.DMDFactory?.DMD;

            TreeView.RootNode = dmd;
            
            TreeView.Rebuild();

            // clean up garbage from previous file, if any

            TreeView.sortColumnDescriptions.Clear();

            TreeView.CollapseAll();

            // show or hide tree depending DMD, because dumb ass tree will NRE if clicked but empty

            var visible = dmd is not null;

            TreeView.visible = visible;

            ToolbarBreadcrumbsHost.visible = visible;

            // update our controls and select something to update breadcrumbs or they'll look weird

            UpdateToolbarSearchLabel();

            if (dmd is not null)
            {
                TreeView.SelectNode(dmd, true, true);
            }
        }

        private void UpdateToolbarSearchLabel()
        {
            var count = TreeView.GetRowCount();

            ToolbarSearchLabel.text = count is 0 ? string.Empty : $"{count} item{(count is not 1 ? "s" : string.Empty)}";
        }

        private void UpdateToolbarBreadcrumbs()
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
                ToolbarBreadcrumbsNodes.RemoveAt(ToolbarBreadcrumbsNodes.Count - 1);
            }

            var current = TreeView.selectedItem as DMDNode;

            while (current != null)
            {
                ToolbarBreadcrumbsNodes.Insert(0, current);
                current = current.Parent as DMDNode;

                if (!string.IsNullOrWhiteSpace(ToolbarSearchField.value))
                    current = null;
            }

            foreach (var item in ToolbarBreadcrumbsNodes)
            {
                ToolbarBreadcrumbs.PushItem($"0x{item.NodeType:X8}");
            }

            for (var i = 0; i < ToolbarBreadcrumbs.childCount; i++)
            {
                var element = ToolbarBreadcrumbs.ElementAt(i);
                element.RegisterCallback<ClickEvent>(OnToolbarBreadcrumbsItemClick);
            }
        }

        private void UpdateTitle()
        {
            titleContent = new GUIContent(DMDViewerStyles.WindowTitle)
            {
                text = File.Exists(Model.CurrentFile) ? Path.GetFileName(Model.CurrentFile) : "DMD Viewer"
            };
        }

        #endregion

        #region Event handlers

        [SuppressMessage("ReSharper", "SwitchStatementMissingSomeEnumCasesNoDefault")]
        private void OnRootKeyDown(KeyDownEvent evt)
        {
            switch (evt.keyCode)
            {
                case KeyCode.F when evt.ctrlKey:

                    // ensure that CTRL+F always do something, i.e. start editing when field is focused but is not editing

                    var element = rootVisualElement.focusController.focusedElement != ToolbarSearchField
                        ? ToolbarSearchField
                        : ToolbarSearchField.Q(className: "unity-text-element--inner-input-field-component");

                    element.Focus();

                    break;
            }
        }

        private void OnToolbarOpenFile()
        {
            Model.OpenFile();
            UpdateControls();
            UpdateTitle();
        }

        private void OnToolbarSelectionFramingValueChanged(ChangeEvent<bool> evt)
        {
            if (Model.UseSelectionFraming)
            {
                EditorApplication.delayCall += () => Preview.FrameSelection();
            }
        }

        private void OnToolbarDistinctFilteringValueChanged(ChangeEvent<bool> evt)
        {
            TreeView.SearchFilterComparer = evt.newValue ? DMDViewerNodeEqualityComparer.Instance : null;
            TreeView.Rebuild();

            UpdateToolbarSearchLabel();
        }

        private void OnToolbarModelSplittingValueChanged(ChangeEvent<bool> evt)
        {
            TreeView.SetSelection(TreeView.GetSelection());
        }

        private void OnToolbarTexturingValueChanged(ChangeEvent<bool> evt)
        {
            Shader.SetKeyword(GlobalKeyword.Create("DMD_VIEWER_TEXTURE"), evt.newValue);
        }

        private void OnToolbarVertexColorsValueChanged(ChangeEvent<bool> evt)
        {
            Shader.SetKeyword(GlobalKeyword.Create("DMD_VIEWER_COLOR_VERTEX"), evt.newValue);
        }

        private void OnToolbarPolygonColoringValueChanged(ChangeEvent<bool> evt)
        {
            Shader.SetKeyword(GlobalKeyword.Create("DMD_VIEWER_COLOR_POLYGON"), evt.newValue);
        }

        private void OnToolbarSearchFieldKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode is KeyCode.UpArrow or KeyCode.DownArrow)
                TreeView.Focus();
        }

        private void OnToolbarSearchFieldValueChanged(ChangeEvent<string> evt)
        {
            var element = evt.target as VisualElement ?? throw new InvalidOperationException();

            var valid = TreeView.SearchFilterValidate(evt.newValue);

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
                TreeView.SearchFilter = evt.newValue;
                TreeView.Rebuild();
            }

            UpdateToolbarSearchLabel();
            UpdateToolbarBreadcrumbs();
        }

        private void OnToolbarBreadcrumbsItemClick(ClickEvent evt)
        {
            // update breadcrumbs: clicking, elements, node stack

            var target = evt.target as VisualElement ?? throw new InvalidOperationException();
            var parent = target.parent;
            var index  = parent.IndexOf(target);

            for (var i = ToolbarBreadcrumbsNodes.Count - 1; i > index; i--)
            {
                var element = parent.ElementAt(i);
                element.UnregisterCallback<ClickEvent>(OnToolbarBreadcrumbsItemClick);
                ToolbarBreadcrumbs.PopItem();
                ToolbarBreadcrumbsNodes.RemoveAt(i);
            }

            // sync tree view selection with clicked breadcrumbs

            var node = ToolbarBreadcrumbsNodes[index];

            TreeView.SelectNode(node, true, true);
        }

        private void OnTreeViewContextMenuHandler(DMDNode node, DropdownMenu menu)
        {
            menu.AppendAction("Dump/Hexadecimal", _ =>
            {
                Model.DumpHexadecimal(node);
            });

            menu.AppendAction("Dump/Hierarchy (Backward)", _ =>
            {
                Model.DumpHierarchyBackward(node);
            });

            menu.AppendAction("Dump/Hierarchy (Forward)", _ =>
            {
                Model.DumpHierarchyForward(node);
            });
        }

        private void OnTreeViewSelectionChanged(object sender, TreeViewSelectionChangedEventArgs<DMDNode> e)
        {
            UpdateToolbarBreadcrumbs();

            Preview.SetNode(Model.DMDFactory!, e.Items.FirstOrDefault() as DMDNode00FF, Model.UseModelSplitting, Model.UseSelectionFraming);
        }

        #endregion
    }
}
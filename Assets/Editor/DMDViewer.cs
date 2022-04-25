using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Twisted;
using Twisted.Graphics;
using Unity.Extensions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace Editor
{
    public sealed partial class DMDViewer : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset VisualTreeAsset = null!;

        private List<DMDNode> Breadcrumbs { get; } = new();

        private DMDFactory? Factory { get; set; }

        private static DMDPreview Preview => Singleton<DMDPreview>.instance;

        private static DMDViewerSettings Settings => DMDViewerSettings.instance;

        private void OnDisable()
        {
            rootVisualElement
                .UnregisterCallback<KeyDownEvent>(
                    OnRootVisualKeyDown
                );

            ToolbarOpenFile.clicked -= OnToolbarOpenFile;

            ToolbarDistinctFiltering
                .UnregisterValueChangedCallback(
                    OnToolbarDistinctFilteringValueChanged
                );

            ToolbarSelectionFraming
                .UnregisterValueChangedCallback(
                    OnToolbarSelectionFramingValueChanged
                );

            ToolbarModelSplitting
                .UnregisterValueChangedCallback(
                    OnToolbarModelSplittingValueChanged
                );

            ToolbarTexturing
                .UnregisterValueChangedCallback(
                    OnToolbarTexturingValueChanged
                );

            ToolbarVertexColors
                .UnregisterValueChangedCallback(
                    OnToolbarVertexColorsValueChanged
                );

            ToolbarPolygonColoring
                .UnregisterValueChangedCallback(
                    OnToolbarPolygonColoringValueChanged
                );

            ToolbarSearchField
                .UnregisterValueChangedCallback(
                    OnToolbarSearchFieldValueChanged
                );

            ToolbarSearchField
                .UnregisterCallback<KeyDownEvent>(
                    OnToolbarSearchFieldKeyDown
                );

            TreeView.SelectionChanged -= OnTreeViewSelectionChanged;

            TreeView.Dispose();

            Settings.Save();
        }

        private void OnDestroy()
        {
            DestroyImmediate(Preview.gameObject); // don't leave garbage on scene
        }

        public void CreateGUI()
        {
            InitializeRootVisual();
            InitializeTreeView();
            InitializeToolbar();

            UpdateFactory();
            UpdateInterface();
            UpdateTitle();
        }

        #region Initialization/cleanup

        [MenuItem("Twisted/DMD Viewer (UI Elements)")]
        private static void InitializeWindow()
        {
            GetWindow<DMDViewer>();
        }

        private void InitializeRootVisual()
        {
            var root = rootVisualElement;

            var container = VisualTreeAsset.Instantiate();

            container.style.flexGrow = 1;

            root.Add(container);

            root.RegisterCallback<KeyDownEvent>(OnRootVisualKeyDown);
        }

        private void InitializeToolbar()
        {
            InitializeToolbarButtons();
            InitializeToolbarToggles();
            InitializeToolbarSearch();
        }

        private void InitializeToolbarButtons()
        {
            ToolbarOpenFile.clicked += OnToolbarOpenFile;
        }

        private void InitializeToolbarToggles()
        {
            InitializeToolbarToggle(ToolbarDistinctFiltering, Settings.UseFilterDistinctProperty, OnToolbarDistinctFilteringValueChanged);
            InitializeToolbarToggle(ToolbarSelectionFraming,  Settings.UseSceneFrameProperty,     OnToolbarSelectionFramingValueChanged);
            InitializeToolbarToggle(ToolbarModelSplitting,    Settings.UseSplitModelProperty,     OnToolbarModelSplittingValueChanged);
            InitializeToolbarToggle(ToolbarTexturing,         Settings.UseModelTextureProperty,   OnToolbarTexturingValueChanged);
            InitializeToolbarToggle(ToolbarVertexColors,      Settings.UseVertexColorsProperty,   OnToolbarVertexColorsValueChanged);
            InitializeToolbarToggle(ToolbarPolygonColoring,   Settings.UsePolygonColorsProperty,  OnToolbarPolygonColoringValueChanged);
        }

        private static void InitializeToolbarToggle(
            ToolbarToggle toggle, SerializedProperty property, EventCallback<ChangeEvent<bool>> callback)
        {
            if (toggle is null)
                throw new ArgumentNullException(nameof(toggle));

            if (property is null)
                throw new ArgumentNullException(nameof(property));

            if (callback is null)
                throw new ArgumentNullException(nameof(callback));

            toggle.BindProperty(property);

            toggle.RegisterValueChangedCallback(callback);

            using var @event = ChangeEvent<bool>.GetPooled(property.boolValue, property.boolValue);

            callback(@event);
        }

        private void InitializeToolbarSearch()
        {
            ToolbarSearchField
                .Q<TextField>(className: ToolbarSearchField.textUssClassName)
                .BindProperty(Settings.LastFilterProperty);

            ToolbarSearchField.RegisterCallback<KeyDownEvent>(OnToolbarSearchFieldKeyDown);

            ToolbarSearchField.RegisterValueChangedCallback(OnToolbarSearchFieldValueChanged);
        }

        private void InitializeTreeView()
        {
            TreeView.selectionType = SelectionType.Single;

            TreeView.sortingEnabled = true;

            TreeView.SetColumns(DMDTreeView.GetColumns());

            TreeView.SelectionChanged += OnTreeViewSelectionChanged;

            TreeView.ContextMenuHandler = OnTreeViewContextMenuHandler;

            TreeView.SearchFilter = Settings.LastFilterProperty.stringValue;

            // instead of their buggy implementation, always show scrolling, that's cleaner

            var scrollView = TreeView.Q<ScrollView>();

            const ScrollerVisibility visibility = ScrollerVisibility.AlwaysVisible;

            scrollView.horizontalScrollerVisibility = visibility;
            scrollView.verticalScrollerVisibility   = visibility;

            // and also some fixes put in USS side as things aren't even properly centered
        }

        #endregion

        #region Updaters

        private void UpdateFactory()
        {
            var path = Settings.LastDatabaseProperty.stringValue;

            if (File.Exists(path))
            {
                Factory = DMDFactory.Create(path);
            }
        }

        private void UpdateInterface()
        {
            // try populate the tree

            var dmd = Factory?.DMD;

            TreeView.RootNode = dmd;

            TreeView.Rebuild();

            // clean up garbage from previous file, if any

            TreeView.sortColumnDescriptions.Clear();

            TreeView.CollapseAll();

            // show or hide tree depending DMD, because dumb ass tree will NRE if clicked but empty

            var visible = dmd is not null;

            TreeView.visible = visible;

            ToolbarBreadcrumbsHost.visible = visible;

            ToolbarBreadcrumbsHost.style.display = DisplayStyle.None; // TODO remove as it's useless

            // finally, update our controls

            UpdateSearchLabel();
            UpdateBreadcrumbs();
        }

        private void UpdateTitle()
        {
            var path = Settings.LastDatabaseProperty.stringValue;

            titleContent = new GUIContent(EditorGUIUtility.IconContent("CustomTool"))
            {
                text = File.Exists(path) ? Path.GetFileName(path) : "DMD Viewer"
            };
        }

        private void UpdateBreadcrumbs()
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
                Breadcrumbs.RemoveAt(Breadcrumbs.Count - 1);
            }

            var current = TreeView.selectedItem as DMDNode;

            while (current != null)
            {
                Breadcrumbs.Insert(0, current);
                current = current.Parent as DMDNode;

                if (!string.IsNullOrWhiteSpace(ToolbarSearchField.value))
                    current = null;
            }

            foreach (var item in Breadcrumbs)
            {
                ToolbarBreadcrumbs.PushItem($"0x{item.NodeType:X8}");
            }

            for (var i = 0; i < ToolbarBreadcrumbs.childCount; i++)
            {
                var element = ToolbarBreadcrumbs.ElementAt(i);
                element.RegisterCallback<ClickEvent>(OnToolbarBreadcrumbsItemClick);
            }
        }

        private void UpdateSearchLabel()
        {
            var count = TreeView.GetRowCount();

            ToolbarSearchLabel.text = count is 0 ? string.Empty : $"{count} item{(count is not 1 ? "s" : string.Empty)}";
        }

        #endregion

        #region Event handlers

        [SuppressMessage("ReSharper", "SwitchStatementMissingSomeEnumCasesNoDefault")]
        private void OnRootVisualKeyDown(KeyDownEvent evt)
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
            var path = EditorUtility.OpenFilePanel(null, null, "DMD");

            if (string.IsNullOrEmpty(path))
                return;

            Settings.LastDatabaseProperty.stringValue = path;
            Settings.SerializedObject.ApplyModifiedPropertiesWithoutUndo();

            UpdateFactory();
            UpdateInterface();
            UpdateTitle();
        }

        private void OnToolbarSelectionFramingValueChanged(ChangeEvent<bool> evt)
        {
            if (Settings.UseSceneFrameProperty.boolValue && TreeView.GetSelection().OfType<DMDNode00FF>().Any())
            {
                EditorApplication.delayCall += () => Preview.FrameSelection();
            }
        }

        private void OnToolbarDistinctFilteringValueChanged(ChangeEvent<bool> evt)
        {
            TreeView.SearchFilterComparer = evt.newValue ? DMDViewerNodeEqualityComparer.Instance : null;
            TreeView.Rebuild();

            UpdateSearchLabel();
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
            if (evt.keyCode is not (KeyCode.UpArrow or KeyCode.DownArrow))
                return;

            TreeView.Focus();

            // depending selection and search history the selection might become null
            // thus, when control gets focus, navigation will not work until 2nd time
            // ensure that something is selected so that navigation works on 1st time

            if (TreeView.selectedItem is not null)
                return;

            if (TreeView.GetItemDataForIndex<DMDNode>(0) is { } node)
            {
                TreeView.SelectNode(node, true, true);
            }
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

            UpdateSearchLabel();
            UpdateBreadcrumbs();
        }

        private void OnToolbarBreadcrumbsItemClick(ClickEvent evt)
        {
            // update breadcrumbs: clicking, elements, node stack

            var target = evt.target as VisualElement ?? throw new InvalidOperationException();
            var parent = target.parent;
            var index  = parent.IndexOf(target);

            for (var i = Breadcrumbs.Count - 1; i > index; i--)
            {
                var element = parent.ElementAt(i);
                element.UnregisterCallback<ClickEvent>(OnToolbarBreadcrumbsItemClick);
                ToolbarBreadcrumbs.PopItem();
                Breadcrumbs.RemoveAt(i);
            }

            // sync tree view selection with clicked breadcrumbs

            var node = Breadcrumbs[index];

            TreeView.SelectNode(node, true, true);
        }

        private void OnTreeViewContextMenuHandler(DMDNode node, DropdownMenu menu)
        {
            menu.AppendAction("Dump/Hexadecimal", _ =>
            {
                var data = string.Concat(node.GetObjectData().Select(s => s.ToString("X2")));
                EditorGUIUtility.systemCopyBuffer = data;
            });

            menu.AppendAction("Dump/Hierarchy (Backward)", _ =>
            {
                EditorGUIUtility.systemCopyBuffer = node.PrintHierarchyBackward();
            });

            menu.AppendAction("Dump/Hierarchy (Forward)", _ =>
            {
                EditorGUIUtility.systemCopyBuffer = node.PrintHierarchyForward();
            });
        }

        private void OnTreeViewSelectionChanged(object sender, TreeViewSelectionChangedEventArgs<DMDNode> e)
        {
            if (Factory is null)
                return;

            UpdateBreadcrumbs();

            Preview.SetNode(
                Factory,
                e.Items.FirstOrDefault() as DMDNode00FF,
                Settings.UseModelTextureProperty.boolValue,
                Settings.UseSceneFrameProperty.boolValue
            );
        }

        #endregion
    }

    public sealed partial class DMDViewer
    {
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

        private DMDTreeView TreeView =>
            rootVisualElement.Q<DMDTreeView>();

        #endregion
    }
}
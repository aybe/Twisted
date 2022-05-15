using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Twisted.Controls;
using Twisted.Formats.Database;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace Twisted.Editor
{
    public sealed partial class Viewer : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset VisualTreeAsset = null!;

        private static GameObject Container
        {
            get
            {
                var gameObject = GameObject.Find("DMD Viewer");

                if (gameObject == null)
                {
                    gameObject = new GameObject("DMD Viewer") { hideFlags = HideFlags.DontSave };
                }

                return gameObject;
            }
        }

        private ViewerFactory? Factory { get; set; }

        private static ViewerSettings Settings => ViewerSettings.instance;

        private void OnDisable()
        {
            TreeView.SelectionChanged -= OnTreeViewSelectionChanged;

            TreeView.Dispose();

            Settings.Save();
        }

        private void OnDestroy()
        {
            DestroyImmediate(Container);
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
            InitializeToolbarToggle(ToolbarDistinctFiltering, Settings.EnableFilteredSearchProperty,    OnToolbarDistinctFilteringValueChanged);
            InitializeToolbarToggle(ToolbarSelectionFraming,  Settings.EnableFramingProperty,           OnToolbarSelectionFramingValueChanged);
            InitializeToolbarToggle(ToolbarModelSplitting,    Settings.EnablePolygonGenerationProperty, OnToolbarModelSplittingValueChanged);
            InitializeToolbarToggle(ToolbarTexturing,         Settings.EnableTextureProperty,           OnToolbarTexturingValueChanged);
            InitializeToolbarToggle(ToolbarTextureAlpha,      Settings.EnableTextureAlphaProperty,      OnToolbarTextureAlphaValueChanged);
            InitializeToolbarToggle(ToolbarVertexColors,      Settings.EnableVertexColorsProperty,      OnToolbarVertexColorsValueChanged);
            InitializeToolbarToggle(ToolbarPolygonColoring,   Settings.EnablePolygonColorsProperty,     OnToolbarPolygonColoringValueChanged);
            InitializeToolbarToggle(ToolbarNodeFilter,        Settings.EnableFilteredNodesProperty,     OnToolbarNodeFilterValueChanged);
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
            TreeView.selectionType = SelectionType.Multiple;

            TreeView.sortingEnabled = true;

            TreeView.SetColumns(ViewerTreeView.GetColumns());

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
                Factory = ViewerFactory.Create(path);
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

            // finally, update our controls

            UpdateSearchLabel();
        }

        private void UpdateTitle()
        {
            var path = Settings.LastDatabaseProperty.stringValue;

            titleContent = new GUIContent(EditorGUIUtility.IconContent("CustomTool"))
            {
                text = File.Exists(path) ? Path.GetFileName(path) : "DMD Viewer"
            };
        }

        private void UpdateSearchLabel()
        {
            var count = TreeView.GetRowCount();

            ToolbarSearchLabel.text = count is 0 ? string.Empty : $"{count} item{(count is not 1 ? "s" : string.Empty)}";
        }

        private void UpdateSelection()
        {
            var selection = TreeView.GetSelection();
            if (selection.Any())
            {
                TreeView.SetSelection(selection);
            }
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
            if (Settings.EnableFramingProperty.boolValue && TreeView.GetSelection().OfType<DMDNode00FF>().Any())
            {
                EditorApplication.delayCall += () => ViewerUtility.Frame(Container);
            }
        }

        private void OnToolbarDistinctFilteringValueChanged(ChangeEvent<bool> evt)
        {
            TreeView.SearchFilterComparer = evt.newValue ? ViewerComparer.Instance : null;
            TreeView.Rebuild();

            UpdateSearchLabel();
        }

        private void OnToolbarModelSplittingValueChanged(ChangeEvent<bool> evt)
        {
            UpdateSelection();
        }

        [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local")]
        private void OnToolbarTexturingValueChanged(ChangeEvent<bool> evt)
        {
            Shader.SetKeyword(GlobalKeyword.Create("DMD_VIEWER_TEXTURE"), evt.newValue);
        }

        [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local")]
        private void OnToolbarTextureAlphaValueChanged(ChangeEvent<bool> evt)
        {
            Shader.SetKeyword(GlobalKeyword.Create("DMD_VIEWER_COLOR_ALPHA"), evt.newValue);
        }

        [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local")]
        private void OnToolbarVertexColorsValueChanged(ChangeEvent<bool> evt)
        {
            Shader.SetKeyword(GlobalKeyword.Create("DMD_VIEWER_COLOR_VERTEX"), evt.newValue);
        }

        [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local")]
        private void OnToolbarPolygonColoringValueChanged(ChangeEvent<bool> evt)
        {
            Shader.SetKeyword(GlobalKeyword.Create("DMD_VIEWER_COLOR_POLYGON"), evt.newValue);
        }

        private void OnToolbarNodeFilterValueChanged(ChangeEvent<bool> evt)
        {
            UpdateSelection();
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
        }

        [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local")]
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

            // no task here is overall better: progress bar modal behavior is kept, no context switch to call onto Unity API

            var progress = new Progress();

            progress.ProgressChanged += (_, args) =>
            {
                var percent1 = args.Leaf.GetProgress();
                var percent2 = args.Head.GetProgress();
                EditorUtility.DisplayProgressBar("Generating scene, please be patient...", $"{args.Leaf.Header}: {percent1:P0}", percent2);
            };

            ViewerPreview.ConfigureNodes(
                Container,
                Factory,
                e.Items,
                Settings.EnableFramingProperty.boolValue,
                Settings.EnablePolygonGenerationProperty.boolValue,
                Settings.EnableFilteredNodesProperty.boolValue,
                progress
            );

            EditorUtility.ClearProgressBar();
        }

        #endregion
    }

    public sealed partial class Viewer
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

        private ToolbarToggle ToolbarTextureAlpha =>
            rootVisualElement.Q<ToolbarToggle>("toolbarTextureAlpha");

        private ToolbarToggle ToolbarVertexColors =>
            rootVisualElement.Q<ToolbarToggle>("toolbarVertexColors");

        private ToolbarToggle ToolbarPolygonColoring =>
            rootVisualElement.Q<ToolbarToggle>("toolbarPolygonColoring");

        private ToolbarToggle ToolbarNodeFilter =>
            rootVisualElement.Q<ToolbarToggle>("toolbarNodeFilter");

        private Label ToolbarSearchLabel =>
            rootVisualElement.Q<Label>("toolbarSearchLabel");

        private ToolbarSearchField ToolbarSearchField =>
            rootVisualElement.Q<ToolbarSearchField>("toolbarSearchField");

        private ViewerTreeView TreeView =>
            rootVisualElement.Q<ViewerTreeView>();

        #endregion
    }
}
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
    {
        [SerializeField]
        private DMDViewerModel Model = null!;

        [SerializeField]
        private VisualTreeAsset VisualTreeAsset = null!;

        private GenericTreeView<DMDNode> TreeView = null!;

        private void OnEnable()
        {
            TreeView                   =  new GenericTreeView<DMDNode>(DMDViewerTreeView.GetColumns());
            TreeView.selectionType     =  SelectionType.Single;
            TreeView.onSelectionChange += OnTreeViewSelectionChange;
        }

        private void OnDisable()
        {
            TreeView.Dispose();
        }

        [SuppressMessage("ReSharper", "UnusedVariable")]
        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        public void CreateGUI()
        {
            var root = rootVisualElement;

            var container = VisualTreeAsset.Instantiate();

            container.style.flexGrow = 1;

            root.Add(container);

            var toolbarButtonOpenFile          = root.Q<ToolbarButton>("toolbarButtonOpenFile");
            var toolbarToggleDistinctFiltering = root.Q<ToolbarToggle>("toolbarToggleDistinctFiltering");
            var toolbarToggleRegexSearch       = root.Q<ToolbarToggle>("toolbarToggleRegexSearch");
            var toolbarToggleSelectionFraming  = root.Q<ToolbarToggle>("toolbarToggleSelectionFraming");
            var toolbarToggleModelSplitting    = root.Q<ToolbarToggle>("toolbarToggleModelSplitting");
            var toolbarToggleTexturing         = root.Q<ToolbarToggle>("toolbarToggleTexturing");
            var toolbarToggleVertexColors      = root.Q<ToolbarToggle>("toolbarToggleVertexColors");
            var toolbarTogglePolygonColoring   = root.Q<ToolbarToggle>("toolbarTogglePolygonColoring");
            ToolbarSearchResults = root.Q<Label>("toolbarLabelSearchResults");
            var toolbarSliderItemHeight = root.Q<SliderInt>("toolbarSliderItemHeight");

            ToolbarBreadcrumbsHost = root.Q<Toolbar>("toolbarBreadcrumbsHost");
            ToolbarBreadcrumbs     = root.Q<ToolbarBreadcrumbs>("toolbarBreadcrumbs");

            InitializeModel();
            InitializeSearch();
            InitializeWindowTitle();

            toolbarButtonOpenFile.clicked += () =>
            {
                Model.OpenFile();
                InitializeWindowTitle();
                InitializeTreeView();
            };

            toolbarToggleDistinctFiltering.BindProperty(Model.UseDistinctFilteringProperty);

            toolbarToggleDistinctFiltering.RegisterValueChangedCallback(evt =>
            {
            });

            toolbarToggleRegexSearch.BindProperty(Model.UseRegexSearchProperty);

            toolbarToggleRegexSearch.RegisterValueChangedCallback(evt =>
            {
            });

            toolbarToggleSelectionFraming.BindProperty(Model.UseSelectionFramingProperty);

            toolbarToggleSelectionFraming.RegisterValueChangedCallback(evt =>
            {
            });

            toolbarToggleModelSplitting.BindProperty(Model.UseModelSplittingProperty);

            toolbarToggleModelSplitting.RegisterValueChangedCallback(evt =>
            {
            });

            toolbarToggleTexturing.BindProperty(Model.UseTexturingProperty);

            toolbarToggleTexturing.RegisterValueChangedCallback(evt =>
            {
            });

            toolbarToggleVertexColors.BindProperty(Model.UseVertexColorsProperty);

            toolbarToggleVertexColors.RegisterValueChangedCallback(evt =>
            {
            });

            toolbarTogglePolygonColoring.BindProperty(Model.UsePolygonColorsProperty);

            toolbarTogglePolygonColoring.RegisterValueChangedCallback(evt =>
            {
            });

            // TODO other properties

            root.Bind(Model.SerializedObject);

            root.Q("TreeViewHost").Add(TreeView);

            var searchField = root.Q<ToolbarSearchField>();
            var treeView    = TreeView;

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

                treeView.Q<ScrollView>().contentContainer.Focus(); // shamelessly stolen from source
            });

            InitializeTreeView();
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

        private void InitializeWindowTitle()
        {
            titleContent = new GUIContent(DMDViewerStyles.WindowTitle)
            {
                text = File.Exists(Model.CurrentFile) ? Path.GetFileName(Model.CurrentFile) : "DMD Viewer"
            };
        }

        private void InitializeTreeView()
        {
            var dmd = Model.DMDFactory?.DMD;

            if (dmd is not null)
            {
                TreeView.sortColumnDescriptions.Clear();

                TreeView.CollapseAll();

                TreeView.SetRoot(dmd);

                TreeView.SelectNode(dmd, true, true);
            }

            var visible = dmd is not null; // stupid tree will NRE when clicked and is not populated

            TreeView.visible               = visible;
            ToolbarBreadcrumbsHost.visible = visible;
        }

        #region TreeView

        private void OnTreeViewSelectionChange(IEnumerable<object> objects)
        {
            UpdateBreadcrumbs(objects);
        }

        #endregion

        #region Search

        private Label ToolbarSearchResults = null!;

        private void InitializeSearch()
        {
            var field = rootVisualElement.Q<ToolbarPopupSearchField>("toolbarPopupSearchField");

            field.RegisterValueChangedCallback(evt =>
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
            });

            field.menu.AppendAction(
                "Use Regex",
                _ =>
                {
                    Model.UseRegexSearch = !Model.UseRegexSearch;
                },
                _ => Model.UseRegexSearch ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal
            );
        }

        #endregion

        #region Breadcrumbs

        private Toolbar ToolbarBreadcrumbsHost = null!;

        private ToolbarBreadcrumbs ToolbarBreadcrumbs = null!;

        private readonly List<DMDNode> BreadcrumbsNodes = new();

        private void OnBreadcrumbsItemClick(ClickEvent evt)
        {
            UpdateBreadcrumbsItem(evt);
        }

        private void UpdateBreadcrumbs(IEnumerable<object> objects)
        {
            for (var i = 0; i < ToolbarBreadcrumbs.childCount; i++)
            {
                var element = ToolbarBreadcrumbs.ElementAt(i);
                element.UnregisterCallback<ClickEvent>(OnBreadcrumbsItemClick);
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
                element.RegisterCallback<ClickEvent>(OnBreadcrumbsItemClick);
            }
        }

        private void UpdateBreadcrumbsItem(ClickEvent evt)
        {
            if (evt == null)
                throw new ArgumentNullException(nameof(evt));

            var target = evt.target as VisualElement ?? throw new InvalidOperationException();
            var parent = target.parent;
            var index  = parent.IndexOf(target);

            for (var i = BreadcrumbsNodes.Count - 1; i > index; i--)
            {
                var element = parent.ElementAt(i);
                element.UnregisterCallback<ClickEvent>(OnBreadcrumbsItemClick);
                ToolbarBreadcrumbs.PopItem();
                BreadcrumbsNodes.RemoveAt(i);
            }

            var node = BreadcrumbsNodes[index];

            TreeView.SelectNode(node, true, true);
        }

        #endregion
    }
}
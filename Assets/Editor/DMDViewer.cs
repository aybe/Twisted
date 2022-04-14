using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
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
            TreeView = CreateTreeView();

            [SuppressMessage("ReSharper", "UnusedParameter.Local")]
            static GenericTreeView<DMDNode> CreateTreeView()
            {
                var column1 = new GenericTreeViewColumn<DMDNode>
                {
                    Title          = "Name",
                    Name           = "Name",
                    ValueGetter    = s => s.GetType().Name,
                    ValueComparer  = s => Comparer<string>.Default,
                    ValueFormatter = s => $"{s}",
                    Width          = 200.0f
                };

                var column2 = new GenericTreeViewColumn<DMDNode>
                {
                    Title          = "Type 1",
                    Name           = "Type 1",
                    ValueGetter    = s => s.NodeType,
                    ValueComparer  = s => Comparer<uint>.Default,
                    ValueFormatter = s => s is uint u ? $"0x{(u >> 16) & 0xFFFF:X8}" : null,
                    Width          = 100.0f
                };

                var column3 = new GenericTreeViewColumn<DMDNode>
                {
                    Title          = "Type 2",
                    Name           = "Type 2",
                    ValueGetter    = s => s.NodeType,
                    ValueComparer  = s => Comparer<uint>.Default,
                    ValueFormatter = s => s is uint u ? $"0x{(u >> 00) & 0xFFFF:X8}" : null,
                    Width          = 100.0f
                };

                var column4 = new GenericTreeViewColumn<DMDNode>
                {
                    Title          = "Position",
                    Name           = "Position",
                    ValueGetter    = s => s.Position,
                    ValueComparer  = s => Comparer<long>.Default,
                    ValueFormatter = s => $"{s:N0}",
                    Width          = 100.0f
                };

                var column5 = new GenericTreeViewColumn<DMDNode>
                {
                    Title          = "Length",
                    Name           = "Length",
                    ValueGetter    = s => s.Length,
                    ValueComparer  = s => Comparer<long>.Default,
                    ValueFormatter = s => $"{s:N0}",
                    Width          = 100.0f
                };

                var column6 = new GenericTreeViewColumn<DMDNode>
                {
                    Title          = "Polygons",
                    Name           = "Polygons",
                    ValueGetter    = s => s is DMDNode00FF ff ? ff.GetPolygonsString() : "N/A",
                    ValueComparer  = s => Comparer<string>.Default,
                    ValueFormatter = s => $"{s}",
                    Width          = 200.0f
                };

                var columns = new[]
                {
                    column1,
                    column2,
                    column3,
                    column4,
                    column5,
                    column6
                };

                var treeView = new GenericTreeView<DMDNode>(columns);

                return treeView;
            }
        }

        private void OnDisable()
        {
            //TreeView.columnSortingChanged -= OnTreeViewColumnSortingChanged;
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
            var toolbarLabelSearchResults      = root.Q<Label>("toolbarLabelSearchResults");
            var toolbarSliderItemHeight        = root.Q<SliderInt>("toolbarSliderItemHeight");
            var toolbarSearchField             = root.Q<ToolbarSearchField>("toolbarSearchField");

            InitializeModel();
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

            if (Model.DMDFactory != null)
            {
                InitializeTreeView();
            }
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
            var dmd = Model.DMDFactory?.DMD ?? throw new InvalidOperationException();

            TreeView.SetRoot(dmd);
            TreeView.ExpandRootItems();
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Twisted.Graphics;
using Unity.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    public sealed class DMDViewer : EditorWindow
        // TODO set good size/stretching/etc for columns
        // TODO column sorting + arrows
        // TODO filtering + toolbar hint
        // TODO context menus
        // TODO remove sample text
        // TODO try fix horizontal scroll bar
        // TODO reload tree at reload
        // TODO save state
        // TODO when another file is loaded, reset internal tree state
        // TODO what becomes primary column should be left aligned
        // TODO everything missing from legacy viewer
        // TODO
        // BUG when tree view loses focus, one of its expander may turn blue at any time
        // BUG tree view keyboard expand/collapse may stop working for no reason at all
        // BUG horizontal scroll bar flickers and partially hides selected item at bottom
    {
        public delegate string ContentProvider(DMDNode node, Expression<Func<DMDNode, object>> expression);

        [SerializeField]
        private VisualTreeAsset VisualTreeAsset = null!;

        [SerializeField]
        private DMDViewerModel Model = null!;

        private MultiColumnTreeView TreeView => rootVisualElement.Q<MultiColumnTreeView>();

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
                Debug.Log(true);
                Model.OpenFile();
                InitializeWindowTitle();
                InitializeTreeView();
            };

            toolbarToggleDistinctFiltering.BindProperty(Model.UseDistinctFilteringProperty);

            toolbarToggleDistinctFiltering.RegisterValueChangedCallback(evt =>
            {
                Debug.Log(evt.newValue);
            });

            toolbarToggleRegexSearch.BindProperty(Model.UseRegexSearchProperty);

            toolbarToggleRegexSearch.RegisterValueChangedCallback(evt =>
            {
                Debug.Log(evt.newValue);
            });

            toolbarToggleSelectionFraming.BindProperty(Model.UseSelectionFramingProperty);

            toolbarToggleSelectionFraming.RegisterValueChangedCallback(evt =>
            {
                Debug.Log(evt.newValue);
            });

            toolbarToggleModelSplitting.BindProperty(Model.UseModelSplittingProperty);

            toolbarToggleModelSplitting.RegisterValueChangedCallback(evt =>
            {
                Debug.Log(evt.newValue);
            });

            toolbarToggleTexturing.BindProperty(Model.UseTexturingProperty);

            toolbarToggleTexturing.RegisterValueChangedCallback(evt =>
            {
                Debug.Log(evt.newValue);
            });

            toolbarToggleVertexColors.BindProperty(Model.UseVertexColorsProperty);

            toolbarToggleVertexColors.RegisterValueChangedCallback(evt =>
            {
                Debug.Log(evt.newValue);
            });

            toolbarTogglePolygonColoring.BindProperty(Model.UsePolygonColorsProperty);

            toolbarTogglePolygonColoring.RegisterValueChangedCallback(evt =>
            {
                Debug.Log(evt.newValue);
            });

            // TODO other properties

            root.Bind(Model.SerializedObject);

            var sliderItemHeight = root.Q<SliderInt>();
            var searchField      = root.Q<ToolbarSearchField>();
            var treeView         = root.Q<MultiColumnTreeView>();

            root.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode is KeyCode.F && evt.ctrlKey)
                {
                    searchField.Focus();
                }
            });

            sliderItemHeight.RegisterValueChangedCallback(evt =>
            {
                // how nice is that, we must rebuild the tree for it to pick up fixed item height change...

                if (treeView.virtualizationMethod is not CollectionVirtualizationMethod.FixedHeight)
                    treeView.virtualizationMethod = CollectionVirtualizationMethod.FixedHeight;

                treeView.fixedItemHeight = evt.newValue;

                treeView.Rebuild();
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

            treeView.fixedItemHeight = sliderItemHeight.value; // sync our view with slider

            treeView.columns.Add(new Column
            {
                name          = "Column 1 Name",
                title         = "Column 1 Title",
                icon          = default,
                visible       = true,
                width         = 200,
                minWidth      = 50,
                maxWidth      = 300,
                sortable      = true,
                stretchable   = true,
                optional      = true,
                resizable     = true,
                makeHeader    = () => new Label(),
                bindHeader    = element => ((Label)element).text = "Header 1",
                unbindHeader  = null,
                destroyHeader = null,
                makeCell = () =>
                {
                    var label = new Label();
                    label.AddToClassList("tree-view-cell");
                    return label;
                },
                bindCell    = (element, i) => ((Label)element).text = $"Column 1: {i}",
                unbindCell  = (element, i) => ((Label)element).text = default,
                destroyCell = null
            });

            // TODO bindHeader & title should be column name

            treeView.columns.Add(new Column
            {
                name          = "Column 2 Name",
                title         = "Column 2 Title",
                icon          = default,
                visible       = true,
                width         = 200,
                minWidth      = 50,
                maxWidth      = 300,
                sortable      = true,
                stretchable   = true,
                optional      = true,
                resizable     = true,
                makeHeader    = () => new Label(),
                bindHeader    = element => ((Label)element).text = "Header 2",
                unbindHeader  = null,
                destroyHeader = null,
                makeCell = () =>
                {
                    var label = new Label();
                    label.AddToClassList("tree-view-cell");
                    return label;
                },
                bindCell    = (element, i) => ((Label)element).text = $"Column 2: {i}",
                unbindCell  = (element, i) => ((Label)element).text = default,
                destroyCell = null
            });

            treeView.columns.Add(new Column
            {
                name          = "Column 3 Name",
                title         = "Column 3 Title",
                icon          = default,
                visible       = true,
                width         = 200,
                minWidth      = 50,
                maxWidth      = 300,
                sortable      = true,
                stretchable   = true,
                optional      = true,
                resizable     = true,
                makeHeader    = () => new Label(),
                bindHeader    = element => ((Label)element).text = "Header 3",
                unbindHeader  = null,
                destroyHeader = null,
                makeCell = () =>
                {
                    var label = new Label();
                    label.AddToClassList("tree-view-cell");
                    return label;
                },
                bindCell    = (element, i) => ((Label)element).text = $"Column 3: {i}",
                unbindCell  = (element, i) => ((Label)element).text = default,
                destroyCell = null
            });

            var index = 0; // NOTE: anything negative won't be shown
            var items = new List<TreeViewItemData<string>>
            {
                new(index++, "Root", TreeViewSampleData.Generate(5, 5, (id, depth) => $"{nameof(id)}: {id}, {nameof(depth)}: {depth}", ref index))
            };

            treeView.SetRootItems(items);
        }

        private void InitializeModel()
        {
            if (Model == null)
            {
                Model = CreateInstance<DMDViewerModel>();
            }
        }

        private void InitializeWindowTitle()
        {
            titleContent = new GUIContent(DMDViewerStyles.WindowTitle)
            {
                text = File.Exists(Model.CurrentFile) ? Path.GetFileName(Model.CurrentFile) : "DMD Viewer"
            };
        }

        [MenuItem("Twisted/DMD Viewer (UI Elements)")]
        public static void InitializeWindow()
        {
            GetWindow<DMDViewer>();
        }

        private void InitializeTreeView()
        {
            var view = TreeView;

            var items = InitializeTreeViewItems(Model.DMDFactory?.DMD ?? throw new InvalidOperationException());

            view.SetRootItems(items);

            var column1 = CreateDefaultColumn("Node");

            column1.bindCell = (element, index) =>
            {
                var node = GetNode(element, index);

                ((Label)element).text = node.GetType().Name;
            };

            var column2 = CreateDefaultColumn("Type 1", TextAnchor.MiddleRight);

            column2.bindCell = (element, index) =>
            {
                var node = GetNode(element, index);

                ((Label)element).text = $"0x{(node.NodeType >> 8) & 0xFF:X4}";
            };

            var column3 = CreateDefaultColumn("Type 2", TextAnchor.MiddleRight);

            column3.bindCell = (element, index) =>
            {
                var node = GetNode(element, index);

                ((Label)element).text = $"0x{(node.NodeType >> 0) & 0xFF:X4}";
            };

            var column4 = CreateDefaultColumn("Position", TextAnchor.MiddleRight);

            column4.bindCell = (element, index) =>
            {
                var node = GetNode(element, index);

                ((Label)element).text = $"{node.Position:N0}";
            };

            var column5 = CreateDefaultColumn("Length", TextAnchor.MiddleRight);

            column5.bindCell = (element, index) =>
            {
                var node = GetNode(element, index);

                ((Label)element).text = $"{node.Length:N0}";
            };

            var column6 = CreateDefaultColumn("Polygons");

            column6.bindCell = (element, index) =>
            {
                var node = GetNode(element, index);

                ((Label)element).text = $"{(node is DMDNode00FF ff ? ff.GetPolygonsString() : "N/A")}";
            };

            view.columns.Clear();
            view.columns.Add(column1);
            view.columns.Add(column2);
            view.columns.Add(column3);
            view.columns.Add(column4);
            view.columns.Add(column5);
            view.columns.Add(column6);
        }

        private static DMDNode GetNode(VisualElement element, int index)
        {
            if (element is null)
                throw new ArgumentNullException(nameof(element));

            var view = element.GetParent<MultiColumnTreeView>() ?? throw new InvalidOperationException();
            var item = view.viewController.GetItemForIndex(index);
            var node = item as DMDNode ?? throw new InvalidOperationException();

            return node;
        }

        private static Column CreateDefaultColumn(string header, TextAnchor anchor = TextAnchor.MiddleLeft)
        {
            if (string.IsNullOrWhiteSpace(header))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(header));

            var column = new Column
            {
                name          = header,
                title         = header,
                icon          = default,
                visible       = true,
                width         = 200,
                minWidth      = 100,
                maxWidth      = 300,
                sortable      = true,
                stretchable   = true,
                optional      = true,
                resizable     = true,
                makeHeader    = () => CreateDefaultLabel(TextAnchor.MiddleLeft),
                bindHeader    = element => ((Label)element).text = header,
                unbindHeader  = element => ((Label)element).text = default,
                destroyHeader = null,
                makeCell      = () => CreateDefaultLabel(anchor),
                bindCell      = null,
                unbindCell    = null,
                destroyCell   = null
            };

            return column;
        }

        private static VisualElement CreateDefaultLabel(TextAnchor anchor)
        {
            var label = new Label
            {
                style =
                {
                    flexGrow       = 1,
                    unityTextAlign = anchor
                }
            };

            return label;
        }

        private static List<TreeViewItemData<TreeNode>> InitializeTreeViewItems(DMD dmd)
        {
            if (dmd is null)
                throw new ArgumentNullException(nameof(dmd));

            var id    = 0;
            var list  = new List<TreeViewItemData<TreeNode>>();
            var stack = new Stack<(TreeNode Node, TreeViewItemData<TreeNode>? Container)>();

            stack.Push((dmd, null));

            while (stack.Count > 0)
            {
                var (node, container) = stack.Pop();

                var data = new TreeViewItemData<TreeNode>(id++, node);

                if (container is null)
                {
                    list.Add(data);
                }
                else
                {
                    ((IList<TreeViewItemData<TreeNode>>)container.Value.children).Add(data);
                }

                foreach (var child in node.Reverse())
                {
                    stack.Push((child, data));
                }
            }

            return list;
        }
    }
}
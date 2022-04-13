using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Twisted.Graphics;
using Unity.Extensions;
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
    {
        [SerializeField]
        private DMDViewerModel Model = null!;

        [SerializeField]
        private VisualTreeAsset VisualTreeAsset = null!;

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

            if (Model.DMDFactory != null)
            {
                InitializeTreeView();
            }
            else
            {
                TreeView.visible = false; // otherwise stupid tree view greets you with a NRE when clicked
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
            var view = TreeView;

            if (view.visible is false)
                view.visible = true;

            var items = InitializeTreeViewItems(Model.DMDFactory?.DMD ?? throw new InvalidOperationException());

            view.SetRootItems(items);

            var column1 = CreateDefaultColumn("Node", 200.0f);

            column1.bindCell = (element, index) =>
            {
                var node = GetNodeFromItem(element, index);

                ((Label)element).text = node.GetType().Name;
            };

            var column2 = CreateDefaultColumn("Type 1");

            column2.bindCell = (element, index) =>
            {
                var node = GetNodeFromItem(element, index);

                ((Label)element).text = $"0x{(node.NodeType >> 8) & 0xFF:X4}";
            };

            var column3 = CreateDefaultColumn("Type 2");

            column3.bindCell = (element, index) =>
            {
                var node = GetNodeFromItem(element, index);

                ((Label)element).text = $"0x{(node.NodeType >> 0) & 0xFF:X4}";
            };

            var column4 = CreateDefaultColumn("Position");

            column4.bindCell = (element, index) =>
            {
                var node = GetNodeFromItem(element, index);

                ((Label)element).text = $"{node.Position:N0}";
            };

            var column5 = CreateDefaultColumn("Length");

            column5.bindCell = (element, index) =>
            {
                var node = GetNodeFromItem(element, index);

                ((Label)element).text = $"{node.Length:N0}";
            };

            var column6 = CreateDefaultColumn("Polygons", 200.0f);

            column6.bindCell = (element, index) =>
            {
                var node = GetNodeFromItem(element, index);

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

        private static Column CreateDefaultColumn(string header, float? width = null)
        {
            if (string.IsNullOrWhiteSpace(header))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(header));

            var column = new Column // stretchable sucks big time
            {
                name          = header,
                title         = header,
                icon          = default,
                visible       = true,
                width         = width ?? 100.0f,
                minWidth      = 75.0f,
                sortable      = true,
                optional      = true,
                resizable     = true,
                makeHeader    = () => CreateDefaultLabel(TextAnchor.MiddleLeft),
                bindHeader    = element => ((Label)element).text = header,
                unbindHeader  = element => ((Label)element).text = default,
                destroyHeader = null,
                makeCell      = () => CreateDefaultLabel(TextAnchor.MiddleLeft),
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
                    unityTextAlign = anchor,
                    overflow       = Overflow.Hidden,
                    textOverflow   = TextOverflow.Ellipsis
                }
            };

            return label;
        }

        private static DMDNode GetNodeFromItem(VisualElement element, int index)
        {
            if (element is null)
                throw new ArgumentNullException(nameof(element));

            var view = element.GetParent<MultiColumnTreeView>() ?? throw new InvalidOperationException();
            var item = view.viewController.GetItemForIndex(index);
            var node = item as DMDNode ?? throw new InvalidOperationException();

            return node;
        }
    }
}
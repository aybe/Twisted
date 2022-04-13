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
        // BUG 
        // BUG multi-column sort destroys expand state
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
                TreeView.ExpandAll();
            }
            else
            {
                TreeView.visible = false; // stupid tree view greets you with a NRE when clicked otherwise
                // furthermore that shows an empty block where columns should be
            }

            TreeView.sortingEnabled = true;

            TreeView.columnSortingChanged += () =>
            {
                var list = InitializeTreeViewItemsEx(Model.DMDFactory!.DMD, TreeView.sortedColumns.ToArray());
                TreeView.SetRootItems(list);
                TreeView.Rebuild();
            };
        }

        private static List<TreeViewItemData<DMDNode>> InitializeTreeViewItemsEx(DMD dmd, SortColumnDescription[] descriptions)
        {
            if (dmd is null)
                throw new ArgumentNullException(nameof(dmd));

            var id    = 0;
            var list  = new List<TreeViewItemData<DMDNode>>();
            var stack = new Stack<(DMDNode Node, TreeViewItemData<DMDNode>? Container)>();

            stack.Push((dmd, null));

            while (stack.Count > 0)
            {
                var (node, container) = stack.Pop();

                var data = new TreeViewItemData<DMDNode>(id++, node);

                if (container is null)
                {
                    list.Add(data);
                }
                else
                {
                    ((IList<TreeViewItemData<DMDNode>>)container.Value.children).Add(data);
                }

                var children = node.Cast<DMDNode>().Reverse();

                foreach (var description in descriptions)
                {
                    Func<DMDNode, string> selector = description.columnName switch
                    {
                        "Node"     => GetNodeName,
                        "Type 1"   => GetNodeType1,
                        "Type 2"   => GetNodeType2,
                        "Position" => GetNodePosition,
                        "Length"   => GetNodeLength,
                        "Polygons" => GetNodePolygons,
                        _          => throw new NotSupportedException(description.columnName)
                    };

                    children = children.Sort(selector, null, description.direction is SortDirection.Descending); // TODO could be object.ToString
                }

                foreach (var child in children)
                {
                    stack.Push((child, data));
                }
            }

            return list;
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

            var column1 = CreateDefaultColumn("Node",     200.0f, GetNodeName);
            var column2 = CreateDefaultColumn("Type 1",   100.0f, GetNodeType1);
            var column3 = CreateDefaultColumn("Type 2",   100.0f, GetNodeType2);
            var column4 = CreateDefaultColumn("Position", 100.0f, GetNodePosition);
            var column5 = CreateDefaultColumn("Length",   100.0f, GetNodeLength);
            var column6 = CreateDefaultColumn("Polygons", 200.0f, GetNodePolygons);

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

        private static Column CreateDefaultColumn(string header, float  width, Func<DMDNode, string> getter)
        {
            if (string.IsNullOrWhiteSpace(header))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(header));
            
            if (getter is null)
                throw new ArgumentNullException(nameof(getter));
            
            if (width <= 0.0f)
                throw new ArgumentOutOfRangeException(nameof(width));

            static VisualElement CreateDefaultLabel(TextAnchor anchor)
            {
                return new Label
                {
                    style =
                    {
                        flexGrow       = 1,
                        unityTextAlign = anchor,
                        overflow       = Overflow.Hidden,
                        textOverflow   = TextOverflow.Ellipsis
                    }
                };
            }

            VisualElement MakeHeader()
            {
                return CreateDefaultLabel(TextAnchor.MiddleLeft);
            }

            void BindHeader(VisualElement element)
            {
                ((Label)element).text = header;
            }

            void UnbindHeader(VisualElement element)
            {
                ((Label)element).text = default;
            }

            VisualElement MakeCell()
            {
                return CreateDefaultLabel(TextAnchor.MiddleLeft);
            }

            void BindCell(VisualElement element, int index)
            {
                ((Label)element).text = getter(GetNodeFromItem(element, index));
            }

            var column = new Column // stretchable sucks big time
            {
                name          = header,
                title         = header,
                icon          = default,
                visible       = true,
                width         = width,
                minWidth      = 75.0f,
                sortable      = true,
                optional      = true,
                resizable     = true,
                makeHeader    = MakeHeader,
                bindHeader    = BindHeader,
                unbindHeader  = UnbindHeader,
                destroyHeader = null,
                makeCell      = MakeCell,
                bindCell      = BindCell,
                unbindCell    = null,
                destroyCell   = null
            };

            return column;
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

        #region Getters

        private static string GetNodeName(DMDNode node)
        {
            if (node is null)
                throw new ArgumentNullException(nameof(node));

            return node.GetType().Name;
        }

        private static string GetNodeType1(DMDNode node)
        {
            if (node is null)
                throw new ArgumentNullException(nameof(node));

            return $"0x{(node.NodeType >> 16) & 0xFFFF:X4}";
        }

        private static string GetNodeType2(DMDNode node)
        {
            if (node is null)
                throw new ArgumentNullException(nameof(node));

            return $"0x{(node.NodeType >> 00) & 0xFFFF:X4}";
        }

        private static string GetNodePosition(DMDNode node)
        {
            if (node is null)
                throw new ArgumentNullException(nameof(node));

            return $"{node.Position:N0}";
        }

        private static string GetNodeLength(DMDNode node)
        {
            if (node is null)
                throw new ArgumentNullException(nameof(node));

            return $"{node.Length:N0}";
        }

        private static string GetNodePolygons(DMDNode node)
        {
            if (node is null)
                throw new ArgumentNullException(nameof(node));

            return $"{(node is DMDNode00FF ff ? ff.GetPolygonsString() : "N/A")}";
        }

        #endregion
    }
}
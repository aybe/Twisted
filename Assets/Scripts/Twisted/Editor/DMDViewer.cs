using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Twisted.PS;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Twisted.Editor
{
    internal sealed class DMDViewer : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset TreeAsset = null!;

        private TreeView View = null!;

        private TreeViewItemData<TreeNode> ViewData;

        public void CreateGUI()
        {
            var root = rootVisualElement;

            var container = TreeAsset.Instantiate();

            container.style.flexGrow = 1.0f; // BUG that template container doesn't grow "by design"...

            root.Add(container);
            
            View = root.Q<TreeView>("treeView1");
            View.makeItem = OnTreeViewMakeItem;
            View.bindItem = OnTreeViewBindItem;
            View.onSelectionChange += OnTreeViewSelectionChange;

            View.autoExpand = false;                   // BUG when true it expands totally un-related tree view items
            View.horizontalScrollingEnabled = true;    // BUG when false it still shows a scrollbar that's even buggier
            View.style.visibility = Visibility.Hidden; // BUG just amazing, click an empty tree throws a NRE, hide it then

            {
                // populate the menu

                var toolbarMenuFile = root.Q<ToolbarMenu>("toolbarMenuFile");

                toolbarMenuFile.menu.AppendAction("Open DMD...", _ =>
                {
                    var path = EditorUtility.OpenFilePanel(null, null, "DMD");

                    if (string.IsNullOrEmpty(path))
                        return;

                    OpenFile(path);
                });

                toolbarMenuFile.menu.AppendSeparator();

                toolbarMenuFile.menu.AppendAction(
                    "Settings/Alternating Row Background",
                    _ =>
                    {
                        View.showAlternatingRowBackgrounds = View.showAlternatingRowBackgrounds switch
                        {
                            AlternatingRowBackground.None        => AlternatingRowBackground.All,
                            AlternatingRowBackground.ContentOnly => AlternatingRowBackground.None,
                            AlternatingRowBackground.All         => AlternatingRowBackground.None,
                            _                                    => throw new ArgumentOutOfRangeException()
                        };
                    },
                    _ => View.showAlternatingRowBackgrounds == AlternatingRowBackground.All
                        ? DropdownMenuAction.Status.Checked
                        : DropdownMenuAction.Status.Normal
                );
            }

            {
                // configure tree view item height slider

                var sliderTreeViewItemHeight = root.Q<SliderInt>("sliderTreeViewItemHeight");

                sliderTreeViewItemHeight.RegisterValueChangedCallback(s =>
                {
                    View.fixedItemHeight = s.newValue;
                    View.Rebuild(); // BUG using slow but working Rebuild because RefreshItems is complete shit
                });

                sliderTreeViewItemHeight.value = (int)View.fixedItemHeight;
            }

            if (false)
            {
                // TODO delete this sample initializer

                OpenFile(@"C:/Users/aybe/source/repos/Twisted.Tests/.twisted/TM1PSJAP/UADMD/CARS.DMD");

                View.ExpandAll();
            }
        }

        private void OnTreeViewBindItem(VisualElement element, int index)
        {
            // from tree view item index, find the actual tree node it is wrapping

            var stack = new Stack<TreeViewItemData<TreeNode>>();

            stack.Push(ViewData);

            var id = View.GetIdForIndex(index);

            var data = default(TreeViewItemData<TreeNode>?);

            while (stack.Any())
            {
                var pop = stack.Pop();

                if (pop.id == id)
                {
                    data = pop;
                    break;
                }

                foreach (var child in pop.children.Reverse())
                {
                    stack.Push(child);
                }
            }

            if (data == null)
            {
                throw new InvalidOperationException($"Couldn't find tree node from tree view item index {index}.");
            }

            // update the visual element accordingly

            ((Label)element).text = data.Value.data.ToString();
        }

        [MenuItem("Twisted/DMD Viewer")]
        public static void ShowWindow()
        {
            var window = GetWindow<DMDViewer>();

            window.titleContent = new GUIContent(EditorGUIUtility.IconContent("CustomTool"))
            {
                text = "DMD Viewer"
            };
        }

        private void OpenFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));

            using var reader = new BinaryReader(File.OpenRead(path));

            var dmd = new DMD(reader); // NOTE: there's no need to keep a direct reference to it

            Debug.Log($"Opened {path} with {dmd.TraverseDfs().Count()} nodes.");

            ViewData = GetTreeViewItemData(dmd);

            View.SetRootItems(new List<TreeViewItemData<TreeNode>> { ViewData });

            View.Rebuild();
            
            View.style.visibility = Visibility.Visible; // BUG fix to avoid dumb ass NRE when clicking an empty tree view
        }

        private VisualElement OnTreeViewMakeItem()
        {
            return new Label
            {
                style = // vertically aligned
                {
                    flexGrow = new StyleFloat(1.0f),
                    unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleLeft)
                }
            };
        }

        private void OnTreeViewSelectionChange(IEnumerable<object> objects)
        {
            foreach (var o in objects)
            {
                Debug.Log(o);
            }
        }

        private static TreeViewItemData<TreeNode> GetTreeViewItemData(TreeNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            // thanks guys for making an API even shittier than IMGUI...

            var index = 0;
            var first = new TreeViewItemData<TreeNode>(index++, node);
            var stack = new Stack<TreeViewItemData<TreeNode>>();

            stack.Push(first);

            while (stack.Any())
            {
                var pop = stack.Pop();

                foreach (var child in pop.data)
                {
                    var list = (List<TreeViewItemData<TreeNode>>)pop.children;
                    list.Add(new TreeViewItemData<TreeNode>(index++, child));
                }

                foreach (var item in pop.children.Reverse())
                {
                    stack.Push(item);
                }
            }

            return first;
        }
    }
}
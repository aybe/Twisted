using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Twisted.Controls
{
    internal sealed class TreeViewSorter<T> where T : TreeNode
    {
        public TreeViewSorter(TreeView<T> view)
        {
            View = view ?? throw new ArgumentNullException(nameof(view));
        }

        private TreeView<T> View { get; }

        private Task? BackgroundTask { get; set; }

        public void OnSortChanged()
        {
            // how fucking amazing? these guys raise this event as many times as there are columns + 2

            // that is, unless you hold a fucking modifier and obviously that isn't explained anywhere

            // to protect ourselves from this shit we use a task and it brings smooth performance back

            if (BackgroundTask?.IsCompleted == false)
                return;

            BackgroundTask = Task.Factory.StartNew(
                Sort,
                CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext()
            );
        }

        private void Sort()
        {
            // the deep sorting will screw ids/expanded/selection, first, save this information

            var selection = View.GetSelection();

            SortSaveExpanded(out var collapsed, out var expanded);

            // perform the actual sorting, this will also update our dictionary that we'll need

            View.Rebuild();

            // restore the collapsed/expanded state of the tree view items but in a fast manner

            SortLoadExpanded(collapsed, expanded);

            if (selection.Any())
            {
                var builder = View.Builder;

                // now it's time to restore the selection that was previously made by the user

                View.SetSelectionByIdWithoutNotify(selection.Select(s => builder.GetNodeIdentifier(s)));

                // scroll to something or it'll suck, this isn't perfect because you know why!

                View.ScrollToItemById(builder.GetNodeIdentifier(selection.First()));
            }

            View.RefreshItems(); // try avoid partially redrawn tree! BUG doesn't always work!

            View.Focus();
        }

        private void SortSaveExpanded(out HashSet<T> collapsed, out HashSet<T> expanded)
        {
            // by carefully avoiding to use their incredibly slow junk, we get decent speed

            collapsed = new HashSet<T>();
            expanded  = new HashSet<T>();

            var controller = View.viewController;

            foreach (var item in View.Builder)
            {
                var data = item.data;

                if (controller.IsExpanded(item.id))
                {
                    expanded.Add(data);
                }
                else
                {
                    collapsed.Add(data);
                }
            }
        }

        private void SortLoadExpanded(HashSet<T> collapsed, HashSet<T> expanded)
        {
            // same as when saving the expanded nodes, simple, works fast for 10K+ nodes

            var builder = View.Builder;

            var controller = View.viewController;

            if (expanded.Count > collapsed.Count)
            {
                controller.ExpandAll();

                foreach (var data in collapsed)
                {
                    controller.CollapseItem(builder.GetNodeIdentifier(data), false);
                }
            }
            else
            {
                controller.CollapseAll();

                foreach (var data in expanded)
                {
                    controller.ExpandItem(builder.GetNodeIdentifier(data), false, false);
                }
            }
        }
    }
}
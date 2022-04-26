using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Extensions;

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
            // these code monkeys will raise this event N headers + 2 times in a row for NO FUCKING REASON
            // this, unless you hold a FUCKING modifier such as Shift or Ctrl, obviously, not explained...
            // now let's protect ourselves from their stupid shit by using a task with a rudimentary guard
            // this, along our manual handling brings back SMOOTH performance like there was in IMGUI tree

            if (BackgroundTask?.IsCompleted == false)
                return;

            BackgroundTask = Task.Factory.StartNew(
                Sort,
                CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext() // this is WPF-like, cross-thread stuff
            );
        }

        private void Sort()
        {
            // the deep sorting will screw ids/expanded/selection, first, save this information

            var selection = View.GetSelection();

            SortSaveExpanded(out var collapsed, out var expanded);

            // perform the actual sorting, this will also update our dictionary that we'll need

            View.Rebuild();

            // restore the collapsed/expanded state of the tree view items but in a FAST manner

            SortLoadExpanded(collapsed, expanded);

            if (selection.Any())
            {
                var builder = View.Builder;

                // now it's time to restore the selection that was previously made by the user

                View.SetSelectionById(selection.Select(s => builder.GetNodeIdentifier(s)));

                // scroll to something or it'll suck, not perfect because of their incompetence

                View.ScrollToItemById(builder.GetNodeIdentifier(selection.First()));
            }

            View.RefreshItems(); // now redraw the control or we'll get a nice broken interface

            View.Focus();
        }

        private void SortSaveExpanded(out HashSet<T> collapsed, out HashSet<T> expanded)
        {
            // by using their slow junk at bare minimum we can finally enjoy decent performance

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
            // here we use a simple but effective approach that scale well for 10K+ tree nodes
            // now really stupid: we'd do it in whichever way that will take the shortest time
            // that's because this crap is exponentially longer as there are nodes in the tree

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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Extensions;
using UnityEngine.UIElements;

namespace Twisted.Controls
{
    internal sealed class TreeViewBuilder<T> : IEnumerable<TreeViewItemData<T>> where T : TreeNode
    {
        public TreeViewBuilder(TreeView<T> view)
        {
            View = view ?? throw new ArgumentNullException(nameof(view));
        }

        private TreeView<T> View { get; }

        private List<TreeViewItemData<T>> Items { get; } = new();

        private Dictionary<T, int> Nodes { get; } = new();

        public IEnumerator<TreeViewItemData<T>> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Items).GetEnumerator();
        }

        public int GetNodeCount()
        {
            return Nodes.Count;
        }

        public int GetNodeIdentifier(T node)
        {
            if (node is null)
                throw new ArgumentNullException(nameof(node));

            if (!Nodes.TryGetValue(node, out var id))
            {
                throw new ArgumentOutOfRangeException(nameof(node), node, "The node is neither the root node nor a child of it.");
            }

            return id;
        }

        public void Rebuild()
        {
            if (View.Columns is null || View.Columns.Length is 0)
            {
                throw new InvalidOperationException("There are no columns in the tree view.");
            }

            List<TreeViewItemData<T>> items;

            if (View.RootNode is null)
            {
                items = new List<TreeViewItemData<T>>();
            }
            else
            {
                var descriptions = View.sortedColumns as SortColumnDescription[] ?? View.sortedColumns.ToArray();
                var dictionary   = descriptions.ToDictionary(s => s.columnName, s => View.Columns.Single(t => t.Name == s.columnName));

                items = string.IsNullOrWhiteSpace(View.SearchFilter)
                    ? RebuildTree(descriptions, dictionary)
                    : RebuildList(descriptions, dictionary);
            }

            View.SetRootItems(items);

            Items.Clear();

            Items.AddRange(GetList(items, s => s.children));

            Nodes.Clear();

            foreach (var item in Items)
            {
                Nodes.Add(item.data, item.id);
            }
        }

        private List<TreeViewItemData<T>> RebuildList(
            SortColumnDescription[] descriptions, IReadOnlyDictionary<string, TreeViewColumn<T>> dictionary)
        {
            if (descriptions is null)
                throw new ArgumentNullException(nameof(descriptions));

            if (dictionary is null)
                throw new ArgumentNullException(nameof(dictionary));

            var nodes = new List<T>();
            var stack = new Stack<T>(new[] { View.RootNode! });
            var regex = new Regex(View.SearchFilter!, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            while (stack.Count > 0)
            {
                var pop = stack.Pop();

                foreach (var column in View.Columns!)
                {
                    var o = column.ValueGetter?.Invoke(pop);
                    var s = column.ValueFormatter?.Invoke(o);
                    var b = s != null && regex.IsMatch(s);

                    if (b is false)
                        continue;

                    nodes.Add(pop);
                    break;
                }

                foreach (var child in pop.Cast<T>().Reverse())
                {
                    stack.Push(child);
                }
            }

            var sort = nodes.AsEnumerable();

            foreach (var description in descriptions)
            {
                var column = dictionary[description.columnName];
                var getter = column.ValueGetter;

                if (getter is null)
                {
                    throw new NullReferenceException($"{nameof(TreeViewColumn<T>.ValueGetter)} is null.");
                }

                sort = Sort(sort, getter, null, description.direction is SortDirection.Descending);
            }

            if (View.SearchFilterComparer is not null)
            {
                sort = sort.Distinct(View.SearchFilterComparer);
            }

            return sort.Select((s, t) => new TreeViewItemData<T>(t, s)).ToList();
        }

        private List<TreeViewItemData<T>> RebuildTree(
            SortColumnDescription[] descriptions, IReadOnlyDictionary<string, TreeViewColumn<T>> dictionary)
        {
            if (descriptions is null)
                throw new ArgumentNullException(nameof(descriptions));

            if (dictionary is null)
                throw new ArgumentNullException(nameof(dictionary));

            var list = new List<TreeViewItemData<T>>();

            var stack = new Stack<(T Node, TreeViewItemData<T>? Container)>();

            stack.Push((View.RootNode!, null));

            var id = 0;

            while (stack.Count > 0)
            {
                var (node, container) = stack.Pop();

                var item = new TreeViewItemData<T>(id++, node);

                if (container is null)
                {
                    list.Add(item);
                }
                else
                {
                    ((IList<TreeViewItemData<T>>)container.Value.children).Add(item);
                }

                var children = node.Cast<T>().Reverse();

                foreach (var description in descriptions)
                {
                    var column = dictionary[description.columnName];
                    var getter = column.ValueGetter;

                    if (getter is null)
                    {
                        throw new NullReferenceException($"{nameof(TreeViewColumn<T>.ValueGetter)} is null.");
                    }

                    children = Sort(children, getter, null, description.direction is SortDirection.Descending);
                }

                foreach (var child in children)
                {
                    stack.Push((child, item));
                }
            }

            return list;
        }

        private static List<TSource> GetList<TSource>(
            IEnumerable<TSource> collection, Func<TSource, IEnumerable<TSource>> children)
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));

            if (children is null)
                throw new ArgumentNullException(nameof(children));

            var list = new List<TSource>();

            var stack = new Stack<TSource>();

            foreach (var source in collection)
            {
                stack.Push(source);
            }

            while (stack.Count > 0)
            {
                var pop = stack.Pop();

                list.Add(pop);

                foreach (var child in children(pop).Reverse())
                {
                    stack.Push(child);
                }
            }

            return list;
        }

        private static IOrderedEnumerable<TSource> Sort<TSource, TKey>(
            IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey>? comparer, bool descending)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            return source is IOrderedEnumerable<TSource> enumerable
                ? enumerable.CreateOrderedEnumerable(selector, comparer, @descending)
                : descending
                    ? source.OrderByDescending(selector, comparer)
                    : source.OrderBy(selector, comparer);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Twisted.Controls
{
    public abstract class TreeNode : IList<TreeNode>
    {
        private TreeNode? _parent;

        protected TreeNode(TreeNode? parent = null)
        {
            parent?.Add(this);
            _parent = parent;
        }

        private List<TreeNode> Children { get; } = new List<TreeNode>();

        public int Depth
        {
            get
            {
                var depth = 0;

                for (var node = this; node.Parent != null; node = node.Parent)
                {
                    depth++;
                }

                return depth;
            }
        }

        [SuppressMessage("ReSharper", "ConvertToAutoProperty")]
        public TreeNode? Parent => _parent;

        public TreeNode Root
        {
            get
            {
                var root = this;

                while (root.Parent != null)
                {
                    root = root.Parent;
                }

                return root;
            }
        }

        public int Count => Children.Count;

        public bool IsReadOnly => ((ICollection<TreeNode>)Children).IsReadOnly;

        public TreeNode this[int index]
        {
            get => Children[index];
            set => throw new NotSupportedException();
        }

        public void Add(TreeNode item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            Insert(Count, item);
        }

        public void Clear()
        {
            foreach (var child in Children)
            {
                child._parent = null;
            }

            Children.Clear();
        }

        public bool Contains(TreeNode item)
        {
            var contains = Children.Contains(item);

            return contains;
        }

        public void CopyTo(TreeNode[] array, int arrayIndex)
        {
            Children.CopyTo(array, arrayIndex);
        }

        public IEnumerator<TreeNode> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Children).GetEnumerator();
        }

        public int IndexOf(TreeNode item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var indexOf = Children.IndexOf(item);

            return indexOf;
        }

        public void Insert(int index, TreeNode item)
        {
            ref var parent = ref item._parent;

            if (parent == this)
                throw new ArgumentOutOfRangeException(nameof(item), "Item is already a child of this node.");

            if (parent != null)
                throw new ArgumentOutOfRangeException(nameof(item), "Item is already a child of another node.");

            parent = this;

            Children.Insert(index, item);
        }

        public bool Remove(TreeNode item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var remove = Children.Remove(item);

            if (remove)
            {
                item._parent = null;
            }

            return remove;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            var child = Children[index];

            child._parent = null;

            Children.RemoveAt(index);
        }
    }
}
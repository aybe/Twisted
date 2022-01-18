using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Twisted.Tests;

public abstract class TreeNode<T> : IList<T> where T : TreeNode<T>
{
    private T? _parent;

    protected TreeNode(T? parent = null)
    {
        parent?.Add((T)this);
        _parent = parent;
    }

    private List<T> Children { get; } = new();

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

    [SuppressMessage("ReSharper", "ConvertToAutoPropertyWithPrivateSetter")]
    public T? Parent => _parent;

    public T Root
    {
        get
        {
            var root = this;

            while (root.Parent != null)
            {
                root = root.Parent;
            }

            return (T)root;
        }
    }

    public int Count => Children.Count;

    public bool IsReadOnly => ((ICollection<T>)Children).IsReadOnly;

    public T this[int index]
    {
        get => Children[index];
        set => throw new NotSupportedException();
    }

    public void Add(T item)
    {
        if (item is null)
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

    public bool Contains(T item)
    {
        var contains = Children.Contains(item);

        return contains;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        Children.CopyTo(array, arrayIndex);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return Children.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Children).GetEnumerator();
    }

    public int IndexOf(T item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        var indexOf = Children.IndexOf(item);

        return indexOf;
    }

    public void Insert(int index, T item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        ref var parent = ref item._parent;

        if (parent == this)
            throw new ArgumentOutOfRangeException(nameof(item), "Item is already a child of this node.");

        if (parent != null)
            throw new ArgumentOutOfRangeException(nameof(item), "Item is already a child of another node.");

        parent = (T)this;

        Children.Insert(index, item);
    }

    public bool Remove(T item)
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

    public void TraverseBFS(Action<T> visitor)
    {
        if (visitor == null)
            throw new ArgumentNullException(nameof(visitor));

        TraverseBFS(s =>
        {
            visitor(s);
            return true;
        });
    }

    public void TraverseBFS(Func<T, bool> visitor)
    {
        if (visitor == null)
            throw new ArgumentNullException(nameof(visitor));

        var queue = new Queue<T>();

        queue.Enqueue((T)this);

        while (queue.Any())
        {
            var dequeue = queue.Dequeue();

            if (!visitor(dequeue))
                break;

            foreach (var item in dequeue)
            {
                queue.Enqueue(item);
            }
        }
    }

    public void TraverseDFS(Action<T> visitor)
    {
        if (visitor == null)
            throw new ArgumentNullException(nameof(visitor));

        TraverseDFS(s =>
        {
            visitor(s);
            return true;
        });
    }

    public void TraverseDFS(Func<T, bool> visitor)
    {
        if (visitor == null)
            throw new ArgumentNullException(nameof(visitor));

        var stack = new Stack<T>();

        stack.Push((T)this);

        while (stack.Any())
        {
            var pop = stack.Pop();

            if (!visitor(pop))
                break;

            foreach (var item in pop.Reverse())
            {
                stack.Push(item);
            }
        }
    }
}
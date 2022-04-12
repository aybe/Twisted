using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Editor
{
    public static class TreeViewSampleData
    {
        public delegate T SampleDataValue<out T>(int id, int depth);

        public static int Count(int items, int depth)
        {
            if (items < 0)
                throw new ArgumentOutOfRangeException(nameof(items));

            if (depth < 0)
                throw new ArgumentOutOfRangeException(nameof(depth));

            var count = 0;

            for (var i = 0; i < depth + 1; i++)
            {
                count += (int)Math.Pow(items, i + 1);
            }

            return count;
        }

        public static List<TreeViewItemData<T>> Generate<T>(int items, int depth, SampleDataValue<T> value, ref int index)
        {
            if (items < 0)
                throw new ArgumentOutOfRangeException(nameof(items));

            if (depth < 0)
                throw new ArgumentOutOfRangeException(nameof(depth));

            if (value is null)
                throw new ArgumentNullException(nameof(value));

            var list = new List<TreeViewItemData<T>>();

            for (var i = 0; i < items; i++)
            {
                list.Add(new TreeViewItemData<T>(index++, value(index, 0)));
            }

            var queue = new Queue<(TreeViewItemData<T> Data, int Depth)>();

            foreach (var data in list)
            {
                queue.Enqueue((data, 0));
            }

            while (queue.Count > 0)
            {
                var (item, itemDepth) = queue.Dequeue();

                if (itemDepth >= depth)
                    continue;

                var childDepth = itemDepth + 1;

                for (var i = 0; i < items; i++)
                {
                    ((IList<TreeViewItemData<T>>)item.children).Add(new TreeViewItemData<T>(index++, value(index, childDepth)));
                }

                if (childDepth >= depth)
                    continue;

                foreach (var child in item.children)
                {
                    queue.Enqueue((child, childDepth));
                }
            }

            return list;
        }
    }
}
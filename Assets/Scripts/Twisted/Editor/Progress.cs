using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Twisted.Editor
{
    public sealed class Progress : IEnumerable<Progress>
    {
        public Progress(string header, int length = default)
        {
            if (string.IsNullOrWhiteSpace(header))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(header));

            Header = header;
            Length = length;
        }

        public Progress? Parent { get; private set; }

        private List<Progress> Children { get; } = new();

        private bool IsRoot => Parent == null;

        private bool IsLeaf => Parent != null && Children.Count == 0;

        private bool IsNode => Parent != null && Children.Count != 0;

        public string Header { get; }

        public int Index { get; private set; }

        public int Length { get; }

        public Progress Root
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

        public int Depth
        {
            get
            {
                var depth = 0;

                for (var node = Parent; node != null; node = node.Parent)
                {
                    depth++;
                }

                return depth;
            }
        }

        public IEnumerator<Progress> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Children).GetEnumerator();
        }

        public Progress Add(Progress progress)
        {
            if (progress == null)
                throw new ArgumentNullException(nameof(progress));

            Children.Add(progress);

            progress.Parent = this;

            return progress;
        }

        public event EventHandler<ProgressChangedEventArgs>? ProgressChanged;

        public bool CanProgress()
        {
            return IsLeaf && Index < Length;
        }

        public float GetProgress()
        {
            var nodes = Traverse().Where(s => s.IsLeaf).ToArray();
            var count = nodes.Sum(s => s.Length);
            var index = nodes.Sum(s => s.Index);

            return 1.0f / count * index;
        }

        public void SetProgress()
        {
            SetProgress(Index + 1);
        }

        public void SetProgress(int index)
        {
            if (index < 1 || index > Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (IsRoot || IsNode)
                throw new InvalidOperationException();

            Index = index;

            for (var node = Parent; node != null; node = node.Parent)
            {
                node.ProgressChanged?.Invoke(node, new ProgressChangedEventArgs(this, node.GetProgress()));
            }
        }

        public IEnumerable<Progress> Traverse()
        {
            var stack = new Stack<Progress>();

            stack.Push(this);

            while (stack.Count > 0)
            {
                var pop = stack.Pop();

                yield return pop;

                for (var i = pop.Children.Count - 1; i >= 0; i--)
                {
                    stack.Push(pop.Children[i]);
                }
            }
        }

        public override string ToString()
        {
            return $"{nameof(Header)}: {Header}";
        }
    }
}
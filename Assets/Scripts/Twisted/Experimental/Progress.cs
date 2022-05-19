using System;
using System.Collections.Generic;
using System.Linq;

namespace Twisted.Experimental
{
    /// <summary>
    ///     Hierarchical progress reporting.
    /// </summary>
    public sealed class Progress
    {
        public Progress(string? header = default, int length = default, Progress? parent = default)
        {
            Header = header;
            Length = length;
            Parent = parent;

            Parent?.Children.Add(this);
        }

        private Progress? Parent { get; }

        private int Length { get; }

        public string? Header { get; }

        private float Value { get; set; }

        private List<Progress> Children { get; } = new();

        private bool IsLeaf => Parent != null && Children.Count == 0;

        private bool IsNode => Parent != null && Children.Count != 0;

        private Progress Head
        {
            get
            {
                var head = this;

                while (head.Parent != null)
                {
                    head = head.Parent;
                }

                return head;
            }
        }

        public event EventHandler<ProgressChangedEventArgs>? ProgressChanged;

        public void Report(float value)
        {
            if (IsNode)
                throw new InvalidOperationException();

            if (value is < 0.0f or > 1.0f)
                throw new ArgumentOutOfRangeException(nameof(value));

            var value1 = (int)Math.Floor(value * 100.0f);
            var value2 = (int)Math.Floor(Value * 100.0f);

            Value = value;

            if (value1 <= value2)
                return;

            var head = Head;

            for (var node = Parent; node != null; node = node.Parent)
            {
                node.ProgressChanged?.Invoke(node, new ProgressChangedEventArgs(head, this));
            }
        }

        public float GetProgress()
        {
            var nodes = Traverse().Where(s => s.IsLeaf).ToArray();
            var count = nodes.Sum(s => s.Length);
            var value = nodes.Sum(node => (float)node.Length / count * node.Value);

            return value;
        }

        private IEnumerable<Progress> Traverse()
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
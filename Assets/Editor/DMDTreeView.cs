using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Twisted.Graphics;
using UnityEngine.UIElements;

namespace Editor
{
    internal sealed class DMDTreeView : GenericTreeView<DMDNode>
    {
        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        public static GenericTreeViewColumn<DMDNode>[] GetColumns()
        {
            return new[]
            {
                new GenericTreeViewColumn<DMDNode>
                {
                    Title          = "Node",
                    Name           = "Node",
                    ValueGetter    = s => s.GetType().Name,
                    ValueComparer  = s => Comparer<string>.Default,
                    ValueFormatter = s => $"{s}",
                    MinWidth       = 200.0f
                },
                new GenericTreeViewColumn<DMDNode>
                {
                    Title          = "Type 1",
                    Name           = "Type 1",
                    ValueGetter    = s => (s.NodeType >> 16) & 0xFFFF,
                    ValueComparer  = s => Comparer<uint>.Default,
                    ValueFormatter = s => s is uint u1 ? $"0x{u1:X4}" : null,
                    MinWidth       = 60.0f
                },
                new GenericTreeViewColumn<DMDNode>
                {
                    Title          = "Type 2",
                    Name           = "Type 2",
                    ValueGetter    = s => (s.NodeType >> 00) & 0xFFFF,
                    ValueComparer  = s => Comparer<uint>.Default,
                    ValueFormatter = s => s is uint u ? $"0x{u:X4}" : null,
                    MinWidth       = 60.0f
                },
                new GenericTreeViewColumn<DMDNode>
                {
                    Title          = "Position",
                    Name           = "Position",
                    ValueGetter    = s => s.Position,
                    ValueComparer  = s => Comparer<long>.Default,
                    ValueFormatter = s => $"{s}",
                    MinWidth       = 75.0f
                },
                new GenericTreeViewColumn<DMDNode>
                {
                    Title          = "Length",
                    Name           = "Length",
                    ValueGetter    = s => s.Length,
                    ValueComparer  = s => Comparer<long>.Default,
                    ValueFormatter = s => $"{s}",
                    MinWidth       = 75.0f
                },
                new GenericTreeViewColumn<DMDNode>
                {
                    Title          = "Polygons",
                    Name           = "Polygons",
                    ValueGetter    = s => s is DMDNode00FF ff ? ff.GetPolygonsString() : "N/A",
                    ValueComparer  = s => Comparer<string>.Default,
                    ValueFormatter = s => $"{s}",
                    MinWidth       = 200.0f
                }
            };
        }

        public new class UxmlFactory : UxmlFactory<DMDTreeView, UxmlTraits>
        {
        }
    }
}
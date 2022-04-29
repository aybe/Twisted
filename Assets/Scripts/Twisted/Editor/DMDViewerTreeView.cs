using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Twisted.Controls;
using Twisted.Formats.Database;
using UnityEngine.UIElements;

namespace Twisted.Editor
{
    internal sealed class DMDViewerTreeView : TreeView<DMDNode>
    {
        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        public static TreeViewColumn<DMDNode>[] GetColumns()
        {
            return new[]
            {
                new TreeViewColumn<DMDNode>
                {
                    Title          = "Node",
                    Name           = "Node",
                    ValueGetter    = s => s.GetType().Name,
                    ValueComparer  = s => Comparer<string>.Default,
                    ValueFormatter = s => $"{s}",
                    MinWidth       = 200.0f
                },
                new TreeViewColumn<DMDNode>
                {
                    Title          = "Type 1",
                    Name           = "Type 1",
                    MinWidth       = 60.0f
                    ValueGetter    = s => s.NodeKind,
                    ValueComparer  = s => Comparer<ushort>.Default,
                    ValueFormatter = s => s is ushort u ? $"0x{u:X4}" : null,
                },
                new TreeViewColumn<DMDNode>
                {
                    Title          = "Type 2",
                    Name           = "Type 2",
                    MinWidth       = 60.0f
                    ValueGetter    = s => s.NodeRole,
                    ValueComparer  = s => Comparer<ushort>.Default,
                    ValueFormatter = s => s is ushort u ? $"0x{u:X4}" : null,
                },
                new TreeViewColumn<DMDNode>
                {
                    Title          = "Position",
                    Name           = "Position",
                    ValueGetter    = s => s.Position,
                    ValueComparer  = s => Comparer<long>.Default,
                    ValueFormatter = s => $"{s}",
                    MinWidth       = 75.0f
                },
                new TreeViewColumn<DMDNode>
                {
                    Title          = "Length",
                    Name           = "Length",
                    ValueGetter    = s => s.Length,
                    ValueComparer  = s => Comparer<long>.Default,
                    ValueFormatter = s => $"{s}",
                    MinWidth       = 75.0f
                },
                new TreeViewColumn<DMDNode>
                {
                    Title          = "Polygons",
                    Name           = "Polygons",
                    ValueGetter    = s => s is DMDNode00FF ff ? ff.GetPolygonsString() : "N/A",
                    ValueComparer  = s => Comparer<string>.Default,
                    ValueFormatter = s => $"{s}",
                    MinWidth       = 150.0f
                }
            };
        }

        public new class UxmlFactory : UxmlFactory<DMDViewerTreeView, UxmlTraits>
        {
        }
    }
}
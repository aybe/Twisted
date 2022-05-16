using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Twisted.Controls;
using Twisted.Formats.Database;
using UnityEngine.UIElements;

namespace Twisted.Editor
{
    internal sealed class ViewerTreeView : TreeView<DMDNode>
    {
        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        public static TreeViewColumn<DMDNode>[] GetColumns()
        {
            return new[]
            {
                new TreeViewColumn<DMDNode>
                {
                    Title          = "Name",
                    Name           = "Name",
                    MinWidth       = 200.0f,
                    ValueGetter    = s => s.GetType().Name,
                    ValueComparer  = s => Comparer<string>.Default,
                    ValueFormatter = s => $"{s}"
                },
                new TreeViewColumn<DMDNode>
                {
                    Title          = "Kind",
                    Name           = "Kind",
                    MinWidth       = 75.0f,
                    ValueGetter    = s => s.NodeKind,
                    ValueComparer  = s => Comparer<ushort>.Default,
                    ValueFormatter = s => s is ushort u ? $"0x{u:X4}" : null
                },
                new TreeViewColumn<DMDNode>
                {
                    Title          = "Role",
                    Name           = "Role",
                    MinWidth       = 75.0f,
                    ValueGetter    = s => s.NodeRole,
                    ValueComparer  = s => Comparer<ushort>.Default,
                    ValueFormatter = s => s is ushort u ? $"0x{u:X4}" : null
                },
                new TreeViewColumn<DMDNode>
                {
                    Title          = "Position",
                    Name           = "Position",
                    MinWidth       = 75.0f,
                    ValueGetter    = s => s.Position,
                    ValueComparer  = s => Comparer<long>.Default,
                    ValueFormatter = s => $"{s}"
                },
                new TreeViewColumn<DMDNode>
                {
                    Title          = "Length",
                    Name           = "Length",
                    MinWidth       = 75.0f,
                    ValueGetter    = s => s.Length,
                    ValueComparer  = s => Comparer<long>.Default,
                    ValueFormatter = s => $"{s}"
                },
                new TreeViewColumn<DMDNode>
                {
                    Title          = "Description",
                    Name           = "Description",
                    MinWidth       = 150.0f,
                    ValueGetter    = s => s.TryGetNodeInfo(),
                    ValueComparer  = s => Comparer<string>.Default,
                    ValueFormatter = s => $"{s}"
                },
                new TreeViewColumn<DMDNode>
                {
                    Title          = "Polygons",
                    Name           = "Polygons",
                    MinWidth       = 150.0f,
                    ValueGetter    = s => s is DMDNode00FF ff ? ff.GetPolygonsString() : null,
                    ValueComparer  = s => Comparer<string>.Default,
                    ValueFormatter = s => $"{s}"
                }
            };
        }

        public new class UxmlFactory : UxmlFactory<ViewerTreeView, UxmlTraits>
        {
        }
    }
}
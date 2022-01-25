using System;
using System.Collections.Generic;
using System.Linq;
using Twisted.Extensions;
using Twisted.PS;
using Twisted.PS.Polygons;

namespace Twisted.Tests;

public class TreeNodeUtility
{
    public static Dictionary<Type, List<IBinaryObject>> GetDictionaryOfNodes(TreeNode treeNode)
    {
        if (treeNode == null)
            throw new ArgumentNullException(nameof(treeNode));

        var dictionary = new Dictionary<Type, List<IBinaryObject>>();

        foreach (var grouping in treeNode.TraverseDfsPreOrder().OfType<IBinaryObject>().GroupBy(s => s.GetType()).OrderBy(s => s.Key.Name))
        {
            var objects = grouping.OrderBy(s => s.GetObjectData(), ArrayComparer<byte>.Instance).ToList();

            dictionary.Add(grouping.Key, objects);
        }

        return dictionary;
    }

    public static Dictionary<Type, List<IBinaryObject>> GetDictionaryOfPolygons(TreeNode treeNode)
    {
        if (treeNode == null)
            throw new ArgumentNullException(nameof(treeNode));

        var comparer = new DelegateEqualityComparer<Polygon>((x, y) =>
        {
            if (x == null && y == null)
                return true;

            if (x == null || y == null)
                return false;

            var equals = EqualityComparer<long>.Default.Equals(x.Position, y.Position);

            return equals;
        }, s => s.Position.GetHashCode());

        var polygons1 = treeNode.TraverseDfsPreOrder().OfType<DMDNode00FF>().SelectMany(s => s.Polygons).ToArray();
        var polygons2 = polygons1.Distinct(comparer).ToArray();

        var dictionary = new Dictionary<Type, List<IBinaryObject>>();

        foreach (var grouping in polygons2.ToLookup(s => s.GetType()))
        {
            var polygons3 = grouping.OrderBy(s => s.GetObjectData(), ArrayComparer<byte>.Instance).Cast<IBinaryObject>().ToList();

            dictionary.Add(grouping.Key, polygons3);
        }

        return dictionary;
    }
}
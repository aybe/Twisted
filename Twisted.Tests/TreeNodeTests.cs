using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twisted.Tests;

[TestClass]
public class TreeNodeTests
{
    [TestMethod]
    public void TestDepth()
    {
        var node1 = new TreeNodeTest();
        var node2 = new TreeNodeTest(node1);

        Assert.AreEqual(0, node1.Depth);
        Assert.AreEqual(1, node2.Depth);

        node1.Remove(node2);

        Assert.AreEqual(0, node2.Depth);
    }

    [TestMethod]
    public void TestParent()
    {
        var node1 = new TreeNodeTest();
        var node2 = new TreeNodeTest(node1);

        Assert.AreEqual(node1, node2.Parent);

        node1.Remove(node2);

        Assert.IsNull(node2.Parent);
    }

    [TestMethod]
    public void TestRoot()
    {
        var node1 = new TreeNodeTest();
        var node2 = new TreeNodeTest(node1);

        Assert.AreEqual(node1, node1.Root);
        Assert.AreEqual(node1, node2.Root);

        node1.Remove(node2);

        Assert.AreEqual(node2, node2.Root);
    }

    [TestMethod]
    public void TestCount()
    {
        var node1 = new TreeNodeTest();

        Assert.AreEqual(0, node1.Count);

        var node2 = new TreeNodeTest();

        node1.Add(node2);

        Assert.AreEqual(1, node1.Count);
    }

    [TestMethod]
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
    public void TestIsReadOnly()
    {
        var node1 = new TreeNodeTest();

        Assert.IsFalse(node1.IsReadOnly);
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void TestIndexer()
    {
        var node1 = new TreeNodeTest();
        var node2 = new TreeNodeTest(node1);

        Assert.AreEqual(node2, node1[0]);

        node1[0] = node2;
    }

    [TestMethod]
    [SuppressMessage("ReSharper", "CollectionNeverQueried.Local")]
    public void TestAddItem()
    {
        var node1 = new TreeNodeTest();
        var node2 = new TreeNodeTest();
        node1.Add(node2);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    [SuppressMessage("ReSharper", "CollectionNeverQueried.Local")]
    public void TestAddTwice()
    {
        var node1 = new TreeNodeTest();
        var node2 = new TreeNodeTest();

        node1.Add(node2);
        node1.Add(node2);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    [SuppressMessage("ReSharper", "CollectionNeverQueried.Local")]
    public void TestAddTwiceDifferentParents()
    {
        var node1 = new TreeNodeTest();
        var node2 = new TreeNodeTest();
        var node3 = new TreeNodeTest();

        node1.Add(node3);
        node2.Add(node3);
    }

    [TestMethod]
    public void TestClear()
    {
        var node1 = new TreeNodeTest();
        var node2 = new TreeNodeTest(node1);

        Assert.IsTrue(node1.Contains(node2));
        Assert.AreEqual(node2.Parent, node1);

        node1.Clear();

        Assert.IsFalse(node1.Contains(node2));
        Assert.IsNull(node2.Parent);
    }

    [TestMethod]
    public void TestContains()
    {
        var node1 = new TreeNodeTest();
        var node2 = new TreeNodeTest();

        Assert.IsFalse(node1.Contains(node2));

        node1.Add(node2);

        Assert.IsTrue(node1.Contains(node2));
    }

    [TestMethod]
    public void TestCopyTo()
    {
        var node1 = new TreeNodeTest();
        var node2 = new TreeNodeTest(node1);

        var array = new TreeNode[1];

        node1.CopyTo(array, 0);

        Assert.IsTrue(array.Contains(node2));
    }

    [TestMethod]
    public void TestGetEnumerator()
    {
        var node1 = new TreeNodeTest();
        var node2 = new TreeNodeTest(node1);

        var enumerable = node1 as IEnumerable;
        var enumerator = enumerable.GetEnumerator();

        while (enumerator.MoveNext())
        {
            var node = enumerator.Current as TreeNodeTest ?? throw new InvalidOperationException();

            Assert.AreEqual(node, node2);
        }
    }

    [TestMethod]
    public void TestGetEnumeratorT()
    {
        var node1 = new TreeNodeTest();
        var node2 = new TreeNodeTest(node1);

        foreach (var node in node1)
        {
            Assert.AreEqual(node, node2);
        }
    }

    [TestMethod]
    [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void TestIndexOf()
    {
        var node1 = new TreeNodeTest();
        var node2 = new TreeNodeTest();

        Assert.AreEqual(-1, node1.IndexOf(node2));

        node1.Add(node2);

        Assert.AreNotEqual(-1, node1.IndexOf(node2));
    }

    [TestMethod]
    public void TestInsert()
    {
        var node1 = new TreeNodeTest();
        var node2 = new TreeNodeTest();

        node1.Insert(0, node2);

        Assert.IsTrue(node1.Contains(node2));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    [SuppressMessage("ReSharper", "CollectionNeverQueried.Local")]
    public void TestInsertSame()
    {
        var node1 = new TreeNodeTest();
        var node2 = new TreeNodeTest();

        node1.Insert(0, node2);
        node1.Insert(0, node2);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    [SuppressMessage("ReSharper", "CollectionNeverQueried.Local")]
    public void TestInsertOther()
    {
        var node1 = new TreeNodeTest();
        var node2 = new TreeNodeTest();
        var node3 = new TreeNodeTest();

        node1.Insert(0, node3);
        node2.Insert(0, node3);
    }

    [TestMethod]
    public void TestRemove()
    {
        var node1 = new TreeNodeTest();
        var node2 = new TreeNodeTest();

        var remove1 = node1.Remove(node2);
        Assert.IsFalse(remove1);

        node1.Add(node2);

        var remove2 = node1.Remove(node2);
        Assert.IsTrue(remove2);
    }

    [TestMethod]
    [SuppressMessage("ReSharper", "UnusedVariable")]
    public void TestRemoveAt()
    {
        var node1 = new TreeNodeTest();
        var node2 = new TreeNodeTest(node1);

        node1.RemoveAt(0);
    }

    [TestMethod]
    public void TestTraverseBFS()
    {
        var node1 = new TreeNodeTest();
        var node2 = new TreeNodeTest();
        var node3 = new TreeNodeTest();
        var node4 = new TreeNodeTest();
        var node5 = new TreeNodeTest();
        var node6 = new TreeNodeTest();
        var node7 = new TreeNodeTest();

        node1.Add(node2);
        node1.Add(node3);
        node2.Add(node4);
        node2.Add(node5);
        node3.Add(node6);
        node3.Add(node7);

        var nodes = node1.TraverseBfs().ToList();

        CollectionAssert.AreEqual(
            new[]
            {
                node1,
                node2,
                node3,
                node4,
                node5,
                node6,
                node7
            },
            nodes
        );
    }

    [TestMethod]
    public void TestTraverseDFS()
    {
        var node1 = new TreeNodeTest();
        var node2 = new TreeNodeTest();
        var node3 = new TreeNodeTest();
        var node4 = new TreeNodeTest();
        var node5 = new TreeNodeTest();
        var node6 = new TreeNodeTest();
        var node7 = new TreeNodeTest();

        node1.Add(node2);
        node1.Add(node3);
        node2.Add(node4);
        node2.Add(node5);
        node3.Add(node6);
        node3.Add(node7);

        var nodes = node1.TraverseDfsPreOrder().ToList();

        CollectionAssert.AreEqual(
            new[]
            {
                node1,
                node2,
                node4,
                node5,
                node3,
                node6,
                node7
            },
            nodes
        );
    }
}
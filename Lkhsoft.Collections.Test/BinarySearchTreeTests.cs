using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Xml;
using Lkhsoft.Collections.Trees.Bst;

namespace Lkhsoft.Collections.Test;

[TestFixture]
public class BinarySearchTreeTests
{
    [Test]
    public void Add_ShouldWork()
    {
        var tree = new BinarySearchTree<int>
        {
            1,
            2,
            3
        };

        Assert.That(tree.Count, Is.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree.Contains(1), Is.True);
            Assert.That(tree.Contains(2), Is.True);
            Assert.That(tree.Contains(3), Is.True);
        }
    }

    [Test]
    public void Add_DuplicateItem_ShouldNotWork()
    {
        var tree = new BinarySearchTree<int>
        {
            1,
        };

        Assert.Throws<InvalidOperationException>(() => tree.Add(1));
        Assert.That(tree.Count, Is.EqualTo(1));
    }

    [Test]
    public void Remove_ShouldWork()
    {
        var tree = new BinarySearchTree<int>
        {
            1,
            2,
            3
        };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree.Remove(2), Is.True);
            Assert.That(tree.Count, Is.EqualTo(2));
        }
        Assert.That(tree.Contains(2), Is.False);
    }

    [Test]
    public void Remove_NonExistentItem_ShouldNotWork()
    {
        var tree = new BinarySearchTree<int> {1};

        Assert.That(tree.Remove(2), Is.False);
    }

    [Test]
    public void Contains_ShouldWork()
    {
        var tree = new BinarySearchTree<int> {1};

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree.Contains(1), Is.True);
            Assert.That(tree.Contains(2), Is.False);
        }
    }

    [Test]
    public void Clear_ShouldWork()
    {
        var tree = new BinarySearchTree<int>
        {
            1,
            2
        };

        tree.Clear();

        Assert.That(tree.Count, Is.EqualTo(0));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree.Contains(1), Is.False);
            Assert.That(tree.Contains(2), Is.False);
        }
    }

    [Test]
    public void CopyTo_ShouldWork()
    {
        var tree = new BinarySearchTree<int>
        {
            1,
            2
        };

        var array = new int[2];
        tree.CopyTo(array, 0);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(array[0], Is.EqualTo(1));
            Assert.That(array[1], Is.EqualTo(2));
        }
    }

    [Test]
    public void CopyTo_ArrayTooSmall_ShouldNotWork()
    {
        var tree = new BinarySearchTree<int>
        {
            1,
            2
        };

        var array = new int[1];

        Assert.Throws<ArgumentException>(() => tree.CopyTo(array, 0));
    }

    [Test]
    public void GetEnumerator_ShouldWork()
    {
        var tree = new BinarySearchTree<int>
        {
            2,
            1,
            3
        };

        using var enumerator = tree.GetEnumerator();
        var list = new List<int>();

        while (enumerator.MoveNext())
        {
            list.Add(enumerator.Current);
        }

        Assert.That(list, Is.EqualTo(new List<int> { 1, 2, 3 }));
    }
    
    [Test]
    public async Task GetAsyncEnumerator_ShouldWork()
    {
        var tree = new BinarySearchTree<int>
        {
            {1},
            {2}
        };

        var list = new List<int>();

        await foreach (var item in tree)
        {
            list.Add(item);
        }

        Assert.That(list, Is.EqualTo(new List<int> { 1, 2 }));
    }
    
    [Test]
    public void JsonSerialization_ShouldWork()
    {
        var tree = new BinarySearchTree<int>
        {
            {1},
            {2}
        };

        var json = JsonSerializer.Serialize(tree);
        var deserializedTree = JsonSerializer.Deserialize<BinarySearchTree<int>>(json);

        Assert.That(deserializedTree.Count, Is.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(deserializedTree.Contains(1), Is.True);
            Assert.That(deserializedTree.Contains(2), Is.True);
        }
    }

    [Test]
    public void XmlSerialization_ShouldWork()
    {
        var tree = new AvlTree<int>() {10, 11, 9, 3, 5};
        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter);
        tree.WriteXml(xmlWriter);
        var xml = stringWriter.ToString();

        using (var reader = XmlReader.Create(new StringReader(xml)))
        {
            tree.ReadXml(reader);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree, Does.Contain(10));
            Assert.That(tree, Does.Contain(11));
            Assert.That(tree, Does.Contain(9));
            Assert.That(tree, Does.Contain(3));
            Assert.That(tree, Does.Contain(5));
            Assert.That(tree, Has.Count.EqualTo(5));
        }
    }
}

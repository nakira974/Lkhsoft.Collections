using NUnit.Framework;
using System;
using System.Collections.Generic;
using Lkhsoft.Collections.Bst;

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
    public void ReadXml_ShouldWork()
    {
        var tree = new BinarySearchTree<int>();
        var xml = "<ArrayOfInt><int>1</int><int>2</int><int>3</int></ArrayOfInt>";
        var reader = new System.Xml.XmlTextReader(new System.IO.StringReader(xml));

        tree.ReadXml(reader);

        Assert.That(tree.Count, Is.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree.Contains(1), Is.True);
            Assert.That(tree.Contains(2), Is.True);
            Assert.That(tree.Contains(3), Is.True);
        }
    }

    [Test]
    public void WriteXml_ShouldWork()
    {
        var tree = new BinarySearchTree<int>
        {
            1,
            2,
            3
        };

        var writer = new System.Xml.XmlTextWriter(new System.IO.StringWriter());
        tree.WriteXml(writer);

        var xml = writer.ToString();
        Assert.That(xml, Does.Contain("<int>1</int>"));
        Assert.That(xml, Does.Contain("<int>2</int>"));
        Assert.That(xml, Does.Contain("<int>3</int>"));
    }
}

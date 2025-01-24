#region

using System.Xml;
using Lkhsoft.Collections.Bst;

#endregion

namespace Lkhsoft.Collections.Test;

[TestFixture]
public class AvlTreeTests
{
    [Test]
    public void Add_ShouldWork()
    {
        var tree = new AvlTree<int>
        {
            10,
            20,
            15
        };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree, Does.Contain(10));
            Assert.That(tree, Does.Contain(20));
            Assert.That(tree, Does.Contain(15));
            Assert.That(tree, Has.Count.EqualTo(3));
        }
    }

    [Test]
    public void Add_DuplicateItem_ShouldNotWork()
    {
        var tree = new AvlTree<int> {10};

        Assert.Throws<InvalidOperationException>(() => tree.Add(10));
    }

    [Test]
    public void Remove_ShouldWork()
    {
        var tree = new AvlTree<int>
        {
            10,
            20,
            15
        };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree.Remove(15), Is.True);
            Assert.That(tree, Does.Not.Contain(15));
            Assert.That(tree, Has.Count.EqualTo(2));
        }
    }

    [Test]
    public void Remove_NonExistentItem_ShouldNotWork()
    {
        var tree = new AvlTree<int>
        {
            10,
            20
        };

        Assert.That(tree.Remove(15), Is.False);
    }

    [Test]
    public void Contains_ShouldWork()
    {
        var tree = new AvlTree<int>
        {
            10,
            20,
            15
        };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree, Does.Contain(10));
            Assert.That(tree, Does.Contain(20));
            Assert.That(tree, Does.Contain(15));
            Assert.That(tree, Does.Not.Contain(5));
        }
    }

    [Test]
    public void Clear_ShouldWork()
    {
        var tree = new AvlTree<int>
        {
            10,
            20,
            15
        };

        tree.Clear();

        Assert.That(tree, Is.Empty);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree, Does.Not.Contain(10));
            Assert.That(tree, Does.Not.Contain(20));
            Assert.That(tree, Does.Not.Contain(15));
        }
    }

    [Test]
    public void CopyTo_ShouldWork()
    {
        var tree = new AvlTree<int>
        {
            10,
            20,
            15
        };

        var array = new int[3];
        tree.CopyTo(array, 0);

        Assert.That(array, Is.EqualTo(new[] {10, 15, 20}));
    }

    private static readonly int[] Expected = [10, 15, 20];

    [Test]
    public void GetEnumerator_ShouldWork()
    {
        var tree = new AvlTree<int>
        {
            10,
            20,
            15
        };

        using var enumerator = tree.GetEnumerator();
        var list = new List<int>();

        while (enumerator.MoveNext()) list.Add(enumerator.Current);

        Assert.That(list, Is.EqualTo(Expected));
    }

    [Test]
    public void ReadXml_ShouldWork()
    {
        var tree = new AvlTree<int>() {10, 11, 9, 3, 5};
        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter);
        tree.WriteXml(xmlWriter);
        var xml = stringWriter.ToString();

        tree.Clear();
        using (var reader = XmlReader.Create(new StringReader(xml)))
        {
            tree.ReadXml(reader);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree, Does.Contain(10));
            Assert.That(tree, Does.Contain(20));
            Assert.That(tree, Does.Contain(15));
            Assert.That(tree, Has.Count.EqualTo(3));
        }
    }

    [Test]
    public void WriteXml_ShouldWork()
    {
        var tree = new AvlTree<int>
        {
            10,
            20,
            15
        };

        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter);
        tree.WriteXml(xmlWriter);
        var xml = stringWriter.ToString();
        Assert.That(xml, Does.Contain("<int>10</int>"));
        Assert.That(xml, Does.Contain("<int>20</int>"));
        Assert.That(xml, Does.Contain("<int>15</int>"));
    }
}
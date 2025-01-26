#region

using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;
using Lkhsoft.Collections.Trees.Bst;

#endregion

namespace Lkhsoft.Collections.Test;

[TestFixture]
public class RedBlackTreeTests
{
    [Test]
    public void AddAndGet_ShouldWork()
    {
        var tree = new RedBlackTree<string>
        {
            "one", "two", "three"
        };

        Assert.That(tree.Count, Is.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree.Contains("one"), Is.True);
            Assert.That(tree.Contains("two"), Is.True);
            Assert.That(tree.Contains("three"), Is.True);
        }
    }

    [Test]
    public void Remove_ShouldWork()
    {
        var tree = new RedBlackTree<string>
        {
            "one", "two", "three"
        };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree.Remove("two"), Is.True);
            Assert.That(tree.Contains("two"), Is.False);
            Assert.That(tree.Count, Is.EqualTo(2));
        }
    }

    [Test]
    public void Contains_ShouldWork()
    {
        var tree = new RedBlackTree<string>
        {
            "one", "two"
        };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree.Contains("one"), Is.True);
            Assert.That(tree.Contains("two"), Is.True);
            Assert.That(tree.Contains("three"), Is.False);
        }
    }

    [Test]
    public void Clear_ShouldWork()
    {
        var tree = new RedBlackTree<string>
        {
            "one", "two"
        };

        tree.Clear();
        Assert.That(tree.Count, Is.EqualTo(0));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree.Contains("one"), Is.False);
            Assert.That(tree.Contains("two"), Is.False);
        }
    }

    [Test]
    public void JsonSerialization_ShouldWork()
    {
        var tree = new RedBlackTree<string>
        {
            "one", "two"
        };

        var json = JsonSerializer.Serialize(tree);
        var deserializedTree = JsonSerializer.Deserialize<RedBlackTree<string>>(json);

        Assert.That(deserializedTree.Count, Is.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(deserializedTree.Contains("one"), Is.True);
            Assert.That(deserializedTree.Contains("two"), Is.True);
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
    
    private static readonly int[] Expected = [10, 15, 20];

    [Test]
    public void GetEnumerator_ShouldWork()
    {
        var tree = new RedBlackTree<int>
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
    public async Task GetAsyncEnumerator_ShouldWork()
    {
        var tree = new RedBlackTree<int>
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
}
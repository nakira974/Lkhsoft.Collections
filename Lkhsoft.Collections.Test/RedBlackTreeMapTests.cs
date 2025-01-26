#region

using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;
using Lkhsoft.Collections.Trees.Bst;

#endregion

namespace Lkhsoft.Collections.Test;

[TestFixture]
public class RedBlackTreeMapTests
{
    [Test]
    public void AddAndGet_ShouldWork()
    {
        var tree = new RedBlackTree<string, int>
        {
            {"one", 1},
            {"two", 2},
            {"three", 3}
        };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree["one"], Is.EqualTo(1));
            Assert.That(tree["two"], Is.EqualTo(2));
            Assert.That(tree["three"], Is.EqualTo(3));
        }
    }

    [Test]
    public void Remove_ShouldWork()
    {
        var tree = new RedBlackTree<string, int>
        {
            {"one", 1},
            {"two", 2},
            {"three", 3}
        };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree.Remove("two"), Is.True);
            Assert.That(tree.ContainsKey("two"), Is.False);
            Assert.That(tree.Count, Is.EqualTo(2));
        }
    }

    [Test]
    public void ContainsKey_ShouldWork()
    {
        var tree = new RedBlackTree<string, int>
        {
            {"one", 1},
            {"two", 2}
        };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree.ContainsKey("one"), Is.True);
            Assert.That(tree.ContainsKey("two"), Is.True);
            Assert.That(tree.ContainsKey("three"), Is.False);
        }
    }

    [Test]
    public void TryGetValue_ShouldWork()
    {
        var tree = new RedBlackTree<string, int>
        {
            {"one", 1},
            {"two", 2}
        };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree.TryGetValue("one", out var value), Is.True);
            Assert.That(value, Is.EqualTo(1));

            Assert.That(tree.TryGetValue("three", out value), Is.False);
            Assert.That(value, Is.EqualTo(0));
        }
    }

    [Test]
    public void Clear_ShouldWork()
    {
        var tree = new RedBlackTree<string, int>
        {
            {"one", 1},
            {"two", 2}
        };

        tree.Clear();
        Assert.That(tree.Count, Is.EqualTo(0));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree.ContainsKey("one"), Is.False);
            Assert.That(tree.ContainsKey("two"), Is.False);
        }
    }

    [Test]
    public void JsonSerialization_ShouldWork()
    {
        var tree = new RedBlackTree<string, int>
        {
            {"one", 1},
            {"two", 2}
        };

        var json = JsonSerializer.Serialize(tree);
        var deserializedTree = JsonSerializer.Deserialize<RedBlackTree<string, int>>(json);

        Assert.That(deserializedTree.Count, Is.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(deserializedTree.ContainsKey("one"), Is.True);
            Assert.That(deserializedTree.ContainsKey("two"), Is.True);
            Assert.That(deserializedTree["one"], Is.EqualTo(1));
            Assert.That(deserializedTree["two"], Is.EqualTo(2));
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
    
    private static readonly KeyValuePair<int, string>[] Expected =
    [
        new(1, "one"),
        new(2, "two"),
        new(3, "three")
    ];
    
    [Test]
    public void GetEnumerator_ShouldWork()
    {
        var tree = new RedBlackTree<int, string>
        {
            {1, "one"},
            {2, "two"},
            {3, "three"}
        };

        using var enumerator = tree.GetEnumerator();
        var list = new List<KeyValuePair<int, string>>();

        while (enumerator.MoveNext()) list.Add(enumerator.Current);

        Assert.That(list, Is.EqualTo(Expected));
    }
    
    [Test]
    public async Task GetAsyncEnumerator_ShouldWork()
    {
        var tree = new RedBlackTree<int, string>
        {
            {1, "one"},
            {2, "two"}
        };

        var list = new List<KeyValuePair<int, string>>();

        await foreach (var item in tree)
        {
            list.Add(item);
        }

        Assert.That(list, Is.EqualTo(new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(1, "one"), new KeyValuePair<int, string>(2, "two") }));
    }
}
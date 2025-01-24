#region

using System.Text.Json;
using System.Xml.Serialization;
using Lkhsoft.Collections.Bst;

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
        var tree = new RedBlackTree<string>
        {
            "one", "two"
        };

        var xmlSerializer = new XmlSerializer(typeof(RedBlackTree<string>));

        using var stringWriter = new StringWriter();
        xmlSerializer.Serialize(stringWriter, tree);
        var xml = stringWriter.ToString();
        Assert.That(xml, Does.Contain("<RedBlackTree>"));
        Assert.That(xml, Does.Contain("<Item>one</Item>"));
        Assert.That(xml, Does.Contain("<Item>two</Item>"));

        using var stringReader = new StringReader(xml);
        var deserializedTree = (RedBlackTree<string>) xmlSerializer.Deserialize(stringReader)!;
        Assert.That(deserializedTree.Count, Is.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(deserializedTree.Contains("one"), Is.True);
            Assert.That(deserializedTree.Contains("two"), Is.True);
        }
    }
}
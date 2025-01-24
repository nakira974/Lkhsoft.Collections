using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Threading.Tasks;
using Lkhsoft.Collections.Bst;

namespace Lkhsoft.Collections.Test;

[TestFixture]
public class BinarySearchTreeMapTests
{
    [Test]
    public void Add_ShouldWork()
    {
        var tree = new BinarySearchTree<int, string>
        {
            {1, "one"},
            {2, "two"},
            {3, "three"}
        };

        Assert.That(tree.Count, Is.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree.ContainsKey(1), Is.True);
            Assert.That(tree.ContainsKey(2), Is.True);
            Assert.That(tree.ContainsKey(3), Is.True);
        }
    }

    [Test]
    public void Add_DuplicateKey_ShouldUpdateValue()
    {
        var tree = new BinarySearchTree<int, string> {{1, "one"}};
        Assert.Throws<InvalidOperationException>(() => tree.Add(1, "uno"));

        Assert.That(tree.Count, Is.EqualTo(1));
        Assert.That(tree[1], Is.EqualTo("one"));
    }

    [Test]
    public void Remove_ShouldWork()
    {
        var tree = new BinarySearchTree<int, string>
        {
            {1, "one"},
            {2, "two"},
            {3, "three"}
        };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree.Remove(2), Is.True);
            Assert.That(tree.Count, Is.EqualTo(2));
        }
        Assert.That(tree.ContainsKey(2), Is.False);
    }

    [Test]
    public void Remove_NonExistentKey_ShouldNotWork()
    {
        var tree = new BinarySearchTree<int, string> {{1, "one"}};

        Assert.That(tree.Remove(2), Is.False);
    }

    [Test]
    public void ContainsKey_ShouldWork()
    {
        var tree = new BinarySearchTree<int, string> {{1, "one"}};

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree.ContainsKey(1), Is.True);
            Assert.That(tree.ContainsKey(2), Is.False);
        }
    }

    [Test]
    public void Indexer_Get_ShouldWork()
    {
        var tree = new BinarySearchTree<int, string>
        {
            {1, "one"},
            {2, "two"}
        };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree[1], Is.EqualTo("one"));
            Assert.That(tree[2], Is.EqualTo("two"));
        }
    }

    [Test]
    public void Indexer_Get_NonExistentKey_ShouldNotWork()
    {
        var tree = new BinarySearchTree<int, string> {{1, "one"}};

        Assert.Throws<KeyNotFoundException>(() => { var value = tree[2]; });
    }

    [Test]
    public void Indexer_Set_ShouldWork()
    {
        var tree = new BinarySearchTree<int, string> {{1, "one"}};

        tree[1] = "uno";

        Assert.That(tree[1], Is.EqualTo("uno"));
    }

    [Test]
    public void Clear_ShouldWork()
    {
        var tree = new BinarySearchTree<int, string>
        {
            {1, "one"},
            {2, "two"}
        };

        tree.Clear();

        Assert.That(tree.Count, Is.EqualTo(0));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree.ContainsKey(1), Is.False);
            Assert.That(tree.ContainsKey(2), Is.False);
        }
    }

    [Test]
    public void Keys_ShouldWork()
    {
        var tree = new BinarySearchTree<int, string>
        {
            {1, "one"},
            {2, "two"}
        };

        var keys = tree.Keys;

        Assert.That(keys, Does.Contain(1));
        Assert.That(keys, Does.Contain(2));
    }

    [Test]
    public void Values_ShouldWork()
    {
        var tree = new BinarySearchTree<int, string>
        {
            {1, "one"},
            {2, "two"}
        };

        var values = tree.Values;

        Assert.That(values, Does.Contain("one"));
        Assert.That(values, Does.Contain("two"));
    }

    [Test]
    public void TryGetValue_ShouldWork()
    {
        var tree = new BinarySearchTree<int, string> {{1, "one"}};

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree.TryGetValue(1, out var value), Is.True);
            Assert.That(value, Is.EqualTo("one"));
        }
    }

    [Test]
    public void TryGetValue_NonExistentKey_ShouldNotWork()
    {
        var tree = new BinarySearchTree<int, string> {{1, "one"}};

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree.TryGetValue(2, out var value), Is.False);
            Assert.That(value, Is.Null);
        }
    }

    [Test]
    public void CopyTo_ShouldWork()
    {
        var tree = new BinarySearchTree<int, string>
        {
            {1, "one"},
            {2, "two"}
        };

        var array = new KeyValuePair<int, string>[2];
        tree.CopyTo(array, 0);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(array[0], Is.EqualTo(new KeyValuePair<int, string>(1, "one")));
            Assert.That(array[1], Is.EqualTo(new KeyValuePair<int, string>(2, "two")));
        }
    }

    [Test]
    public void CopyTo_ArrayTooSmall_ShouldNotWork()
    {
        var tree = new BinarySearchTree<int, string>
        {
            {1, "one"},
            {2, "two"}
        };

        var array = new KeyValuePair<int, string>[1];

        Assert.Throws<ArgumentException>(() => tree.CopyTo(array, 0));
    }

    [Test]
    public void Contains_ShouldWork()
    {
        var tree = new BinarySearchTree<int, string> {{1, "one"}};

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree.Contains(new KeyValuePair<int, string>(1, "one")), Is.True);
            Assert.That(tree.Contains(new KeyValuePair<int, string>(1, "two")), Is.False);
        }
    }

    [Test]
    public void Remove_KeyValuePair_ShouldWork()
    {
        var tree = new BinarySearchTree<int, string> {{1, "one"}};

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree.Remove(new KeyValuePair<int, string>(1, "one")), Is.True);
            Assert.That(tree.Count, Is.EqualTo(0));
        }
    }

    [Test]
    public void Remove_KeyValuePair_NonExistent_ShouldNotWork()
    {
        var tree = new BinarySearchTree<int, string> {{1, "one"}};

        Assert.That(tree.Remove(new KeyValuePair<int, string>(2, "two")), Is.False);
    }

    [Test]
    public void ReadXml_ShouldWork()
    {
        var tree = new BinarySearchTree<int, string>();
        var xml = "<ArrayOfKeyValuePairOfInt32String><KeyValuePairOfInt32String><Key>1</Key><Value>one</Value></KeyValuePairOfInt32String><KeyValuePairOfInt32String><Key>2</Key><Value>two</Value></KeyValuePairOfInt32String></ArrayOfKeyValuePairOfInt32String>";
        var reader = new XmlTextReader(new System.IO.StringReader(xml));

        tree.ReadXml(reader);

        Assert.That(tree.Count, Is.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree.ContainsKey(1), Is.True);
            Assert.That(tree.ContainsKey(2), Is.True);
        }
    }

    [Test]
    public void WriteXml_ShouldWork()
    {
        var tree = new BinarySearchTree<int, string>
        {
            {1, "one"},
            {2, "two"}
        };

        var writer = new XmlTextWriter(new System.IO.StringWriter());
        tree.WriteXml(writer);

        var xml = writer.ToString();
        Assert.That(xml, Does.Contain("<int>1</int>"));
        Assert.That(xml, Does.Contain("<string>one</string>"));
        Assert.That(xml, Does.Contain("<int>2</int>"));
        Assert.That(xml, Does.Contain("<string>two</string>"));
    }

    [Test]
    public async Task GetAsyncEnumerator_ShouldWork()
    {
        var tree = new BinarySearchTree<int, string>
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

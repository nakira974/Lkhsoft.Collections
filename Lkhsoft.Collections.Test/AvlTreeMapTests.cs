using Lkhsoft.Collections.Bst;

namespace Lkhsoft.Collections.Test;

using NUnit.Framework;
using System;
using System.Collections.Generic;

[TestFixture]
public class AvlTreeMapTests
{
    [Test]
    public void Add_ShouldWork()
    {
        var tree = new AvlTree<int, string>
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
    public void Add_DuplicateKey_ShouldNotWork()
    {
        var tree = new AvlTree<int, string> {{1, "one"}};

        Assert.Throws<InvalidOperationException>(() => tree.Add(1, "one"));
    }

    [Test]
    public void Remove_ShouldWork()
    {
        var tree = new AvlTree<int, string>
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
        var tree = new AvlTree<int, string> {{1, "one"}};

        Assert.That(tree.Remove(2), Is.False);
    }

    [Test]
    public void ContainsKey_ShouldWork()
    {
        var tree = new AvlTree<int, string> {{1, "one"}};

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree.ContainsKey(1), Is.True);
            Assert.That(tree.ContainsKey(2), Is.False);
        }
    }

    [Test]
    public void Indexer_Get_ShouldWork()
    {
        var tree = new AvlTree<int, string>
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
        var tree = new AvlTree<int, string> {{1, "one"}};

        Assert.Throws<KeyNotFoundException>(() => { var value = tree[2]; });
    }

    [Test]
    public void Indexer_Set_ShouldWork()
    {
        var tree = new AvlTree<int, string> {{1, "one"}};

        tree[1] = "uno";

        Assert.That(tree[1], Is.EqualTo("uno"));
    }

    [Test]
    public void Clear_ShouldWork()
    {
        var tree = new AvlTree<int, string>
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
        var tree = new AvlTree<int, string>
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
        var tree = new AvlTree<int, string>
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
        var tree = new AvlTree<int, string> {{1, "one"}};

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree.TryGetValue(1, out var value), Is.True);
            Assert.That(value, Is.EqualTo("one"));
        }
    }

    [Test]
    public void TryGetValue_NonExistentKey_ShouldNotWork()
    {
        var tree = new AvlTree<int, string> {{1, "one"}};

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree.TryGetValue(2, out var value), Is.False);
            Assert.That(value, Is.Null);
        }
    }

    [Test]
    public void CopyTo_ShouldWork()
    {
        var tree = new AvlTree<int, string>
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
    public void Contains_ShouldWork()
    {
        var tree = new AvlTree<int, string> {{1, "one"}};

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree.Contains(new KeyValuePair<int, string>(1, "one")), Is.True);
            Assert.That(tree.Contains(new KeyValuePair<int, string>(1, "two")), Is.False);
        }
    }

    [Test]
    public void Remove_KeyValuePair_ShouldWork()
    {
        var tree = new AvlTree<int, string> {{1, "one"}};

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree.Remove(new KeyValuePair<int, string>(1, "one")), Is.True);
            Assert.That(tree.Count, Is.EqualTo(0));
        }
    }

    [Test]
    public void Remove_KeyValuePair_NonExistent_ShouldNotWork()
    {
        var tree = new AvlTree<int, string> {{1, "one"}};

        Assert.That(tree.Remove(new KeyValuePair<int, string>(2, "two")), Is.False);
    }
}

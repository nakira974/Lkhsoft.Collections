#region

using Lkhsoft.Collections.Trees.BTrees;

#endregion

namespace Lkhsoft.Collections.Test;

[TestFixture]
public class BTreeTests
{
    [Test]
    public void Insert_ShouldWork()
    {
        var tree = new BTree<int>(3)
        {
            10,
            20,
            30,
            40,
            50
        };

        Assert.That(tree.Count, Is.EqualTo(5));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree.Contains(10), Is.True);
            Assert.That(tree.Contains(20), Is.True);
            Assert.That(tree.Contains(30), Is.True);
            Assert.That(tree.Contains(40), Is.True);
            Assert.That(tree.Contains(50), Is.True);
        }
    }

    [Test]
    public void Remove_ShouldWork()
    {
        var tree = new BTree<int>(3)
        {
            10,
            20,
            30,
            40
        };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree.Remove(30), Is.True);
            Assert.That(tree.Count, Is.EqualTo(3));
        }

        Assert.That(tree.Contains(30), Is.False);
    }

    [Test]
    public void Remove_ShouldNotWork_WhenKeyDoesNotExist()
    {
        var tree = new BTree<int>(3)
        {
            10,
            20,
            30
        };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree.Remove(40), Is.False);
            Assert.That(tree.Count, Is.EqualTo(3));
        }
    }

    [Test]
    public void Clear_ShouldWork()
    {
        var tree = new BTree<int>(3)
        {
            10,
            20,
            30
        };

        tree.Clear();

        Assert.That(tree.Count, Is.EqualTo(0));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(tree.Contains(10), Is.False);
            Assert.That(tree.Contains(20), Is.False);
            Assert.That(tree.Contains(30), Is.False);
        }
    }

    [Test]
    public void GetEnumerator_ShouldWork()
    {
        var tree = new BTree<int>(3);

        for (var i = 0; i < 100; i++) tree.Add(i);

        var expected = new List<int>();
        for (var i = 0; i < 100; i++) expected.Add(i);
        var actual = tree.ToList();

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public async Task GetEnumeratorAsync_ShouldWork()
    {
        var tree = new BTree<int>(3);

        for (var i = 0; i < 100; i++) tree.Add(i);

        var expected = new List<int>(100);
        for (var i = 0; i < 100; i++) expected.Add(i);
        var actual = new List<int>(100);

        await foreach (var value in tree) actual.Add(value);

        Assert.That(actual, Is.EqualTo(expected));
    }
}
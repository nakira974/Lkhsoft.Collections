namespace Lkhsoft.Collections.Test;

[TestFixture]
public class TrieTests
{
    private Trie _trie;

    [SetUp]
    public void Setup()
    {
        _trie = new Trie();
    }

    [Test]
    public void Add_SingleWord_WordIsContained()
    {
        _trie.Add("hello");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(_trie.Contains("hello"), Is.True);
            Assert.That(_trie.Count, Is.EqualTo(1));
        }
    }

    [Test]
    public void Add_MultipleWords_AllWordsAreContained()
    {
        _trie.Add("hello");
        _trie.Add("world");
        _trie.Add("trie");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(_trie.Contains("hello"), Is.True);
            Assert.That(_trie.Contains("world"), Is.True);
            Assert.That(_trie.Contains("trie"), Is.True);
            Assert.That(_trie.Count, Is.EqualTo(3));
        }
    }

    [Test]
    public void Remove_ExistingWord_WordIsRemoved()
    {
        _trie.Add("hello");
        _trie.Remove("hello");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(_trie.Contains("hello"), Is.False);
            Assert.That(_trie.Count, Is.EqualTo(0));
        }
    }

    [Test]
    public void Remove_NonExistingWord_TrieUnchanged()
    {
        _trie.Add("hello");

        _trie.Remove("world");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(_trie.Contains("hello"), Is.True);
            Assert.That(_trie.Count, Is.EqualTo(1));
        }
    }

    [Test]
    public void Clear_AllWordsRemoved()
    {
        _trie.Add("hello");
        _trie.Add("world");

        _trie.Clear();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(_trie.Contains("hello"), Is.False);
            Assert.That(_trie.Contains("world"), Is.False);
            Assert.That(_trie.Count, Is.EqualTo(0));
        }
    }

    [Test]
    public void CopyTo_CopiesElementsCorrectly()
    {
        _trie.Add("hello");
        _trie.Add("world");

        var array = new string[_trie.Count];
        _trie.CopyTo(array, 0);

        Assert.That(array, Is.EquivalentTo(["hello", "world"]));
    }

    [Test]
    public void Enumerator_IteratesAllWords()
    {
        _trie.Add("hello");
        _trie.Add("world");

        var words = _trie.ToList();

        Assert.That(words, Is.EquivalentTo(["hello", "world"]));
    }

    [Test]
    public void Contains_EmptyTrie_ReturnsFalse()
    {
        Assert.That(_trie.Contains("hello"), Is.False);
    }

    [Test]
    public void Count_InitiallyZero()
    {
        Assert.That(_trie.Count, Is.EqualTo(0));
    }
}
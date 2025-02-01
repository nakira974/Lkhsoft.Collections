using System.Diagnostics;
using Lkhsoft.Collections.Maps;

namespace Lkhsoft.Collections.Test;

[TestFixture]
public class SortedMultiDictionaryTests
{
    private SortedMultiDictionary<int, string> _sortedMultiDictionary;

    [SetUp]
    public void SetUp()
    {
        _sortedMultiDictionary = new SortedMultiDictionary<int, string>();
    }
    
    [Test]
    public void PerformanceTest_InsertAndRetrieve_OneBillionValues()
    {
        const int numberOfKeys = 0x64; // 100 clés
        const int valuesPerKey = 0xF4240; // 1 millions de valeurs par clé
        const int totalValues = numberOfKeys * valuesPerKey; // 1 milliard de valeurs

        var stopwatch = new Stopwatch();

        stopwatch.Start();
        for (var key = 0; key < numberOfKeys; key++)
        {
            for (var valueIndex = 0; valueIndex < valuesPerKey; valueIndex++)
            {
                _sortedMultiDictionary.Add(key, $"Value_{valueIndex}");
            }
        }
        stopwatch.Stop();

        //  Vérification du temps d'insertion
        var insertionTime = stopwatch.Elapsed;
        TestContext.Out.WriteLine($"Temps d'insertion de {totalValues} valeurs : {insertionTime}");

        // Recherche des valeurs
        stopwatch.Restart();
        for (var key = 0; key < numberOfKeys; key++)
        {
            var values = _sortedMultiDictionary[key];
            Assert.That(values.Count, Is.EqualTo(valuesPerKey));
        }
        stopwatch.Stop();

        // Vérification du temps de recherche
        var searchTime = stopwatch.Elapsed;
        TestContext.Out.WriteLine($"Temps de recherche pour {numberOfKeys} clés : {searchTime}");

        // Itération sur toutes les valeurs
        stopwatch.Restart();
        var totalValuesCount = 0;
        foreach (var kvp in _sortedMultiDictionary)
        {
            totalValuesCount += kvp.Value.Count;
        }
        stopwatch.Stop();

        // Vérification du temps d'itération
        var iterationTime = stopwatch.Elapsed;
        TestContext.Out.WriteLine($"Temps d'itération sur {totalValues} valeurs : {iterationTime}");

        // Vérification du nombre total de valeurs
        Assert.That(totalValuesCount, Is.EqualTo(totalValues));
    }

    [Test]
    public void Add_KeyValuePair_ShouldWork()
    {
        // Arrange
        var keyValuePair = new KeyValuePair<int, ICollection<string>>(1, new List<string> { "A", "B" });

        // Act
        _sortedMultiDictionary.Add(keyValuePair);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(_sortedMultiDictionary.ContainsKey(1), Is.True);
            Assert.That(_sortedMultiDictionary[1], Contains.Item("A"));
        }
        Assert.That(_sortedMultiDictionary[1], Contains.Item("B"));
    }

    [Test]
    public void Add_DuplicateKey_ShouldWork()
    {
        // Arrange
        _sortedMultiDictionary.Add(1, "A");
        _sortedMultiDictionary.Add(1, "B");

        // Assert
        Assert.That(_sortedMultiDictionary[1], Contains.Item("A"));
        Assert.That(_sortedMultiDictionary[1], Contains.Item("B"));
    }

    [Test]
    public void Add_NullValue_ShouldNotWork()
    {
        var keyValuePair = new KeyValuePair<int, ICollection<string>>(1, null);

        Assert.Throws<ArgumentNullException>(() => _sortedMultiDictionary.Add(keyValuePair));
    }

    [Test]
    public void Remove_ExistingKey_ShouldWork()
    {
        _sortedMultiDictionary.Add(1, "A");
        _sortedMultiDictionary.Add(1, "B");

        var result = _sortedMultiDictionary.Remove(1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(_sortedMultiDictionary.ContainsKey(1), Is.False);
        }
    }

    [Test]
    public void Remove_NonExistingKey_ShouldNotWork()
    {
        _sortedMultiDictionary.Add(1, "A");

        var result = _sortedMultiDictionary.Remove(2);

        Assert.That(result, Is.False);
    }

    [Test]
    public void Indexer_GetExistingKey_ShouldWork()
    {
        _sortedMultiDictionary.Add(1, "A");

        var values = _sortedMultiDictionary[1];

        Assert.That(values, Contains.Item("A"));
    }

    [Test]
    public void Indexer_GetNonExistingKey_ShouldNotWork()
    {
        Assert.Throws<KeyNotFoundException>(() =>
        {
            var values = _sortedMultiDictionary[Byte.MaxValue];
        });
    }

    [Test]
    public void ContainsKey_ExistingKey_ShouldWork()
    {
        _sortedMultiDictionary.Add(1, "A");

        var result = _sortedMultiDictionary.ContainsKey(1);

        Assert.That(result, Is.True);
    }

    [Test]
    public void ContainsKey_NonExistingKey_ShouldNotWork()
    {
        var result = _sortedMultiDictionary.ContainsKey(1);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task GetAsyncEnumerator_ShouldWork()
    {
        _sortedMultiDictionary.Add(1, "A");
        _sortedMultiDictionary.Add(2, "B");

        var asyncEnumerator = _sortedMultiDictionary.GetAsyncEnumerator();
        var results = new List<KeyValuePair<int, ICollection<string>>>();

        while (await asyncEnumerator.MoveNextAsync())
        {
            results.Add(asyncEnumerator.Current);
        }

        Assert.That(results.Count, Is.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(results[0].Key, Is.EqualTo(1));
            Assert.That(results[0].Value, Contains.Item("A"));
            Assert.That(results[1].Key, Is.EqualTo(2));
            Assert.That(results[1].Value, Contains.Item("B"));
        }
    }

    [Test]
    public void Clear_ShouldWork()
    {
        _sortedMultiDictionary.Add(1, "A");
        _sortedMultiDictionary.Add(2, "B");

        _sortedMultiDictionary.Clear();

        Assert.That(_sortedMultiDictionary.Count, Is.EqualTo(0));
    }

    [Test]
    public void TryGetValue_ExistingKey_ShouldWork()
    {
        _sortedMultiDictionary.Add(1, "A");

        var result = _sortedMultiDictionary.TryGetValue(1, out var values);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(values, Contains.Item("A"));
        }
    }

    [Test]
    public void TryGetValue_NonExistingKey_ShouldNotWork()
    {
        var result = _sortedMultiDictionary.TryGetValue(1, out var values);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.False);
            Assert.That(values, Is.Null);
        }
    }
}
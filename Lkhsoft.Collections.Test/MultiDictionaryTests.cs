using System.Diagnostics;
using Lkhsoft.Collections.Maps;

namespace Lkhsoft.Collections.Test;

public class MultiDictionaryTests
{
    private MultiDictionary<int, string> _multiDictionary;

    [SetUp]
    public void SetUp()
    {
        _multiDictionary = new MultiDictionary<int, string>();
    }
    
    [Test]
    public void PerformanceTest_InsertAndRetrieve_OneBillionValues()
    {
        const int numberOfKeys = 0x64; // 100 clés
        const int valuesPerKey = 0xF4240; // 1 millions de valeurs par clé
        const int totalValues = numberOfKeys * valuesPerKey; // 1 milliard de valeurs

        var stopwatch = new Stopwatch();

        // Insertion des données
        stopwatch.Start();
        for (var key = 0; key < numberOfKeys; key++)
        {
            for (var valueIndex = 0; valueIndex < valuesPerKey; valueIndex++)
            {
                _multiDictionary.Add(key, $"Value_{valueIndex}");
            }
        }
        stopwatch.Stop();

        // Vérification du temps d'insertion
        var insertionTime = stopwatch.Elapsed;
        TestContext.Out.WriteLine($"Temps d'insertion de {totalValues} valeurs : {insertionTime}");

        // Recherche des valeurs
        stopwatch.Restart();
        for (var key = 0; key < numberOfKeys; key++)
        {
            var values = _multiDictionary[key];
            Assert.That(values.Count, Is.EqualTo(valuesPerKey));
        }
        stopwatch.Stop();

        // Vérification du temps de recherche
        var searchTime = stopwatch.Elapsed;
        TestContext.Out.WriteLine($"Temps de recherche pour {numberOfKeys} clés : {searchTime}");

        // Itération sur toutes les valeurs
        stopwatch.Restart();
        var totalValuesCount = 0;
        foreach (var kvp in _multiDictionary)
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
        
        var keyValuePair = new KeyValuePair<int, ICollection<string>>(1, new List<string> { "A", "B" });

        
        _multiDictionary.Add(keyValuePair);

        
        Assert.That(_multiDictionary.ContainsKey(1), Is.True);
        Assert.That(_multiDictionary[1], Contains.Item("A"));
        Assert.That(_multiDictionary[1], Contains.Item("B"));
    }

    [Test]
    public void Add_DuplicateKey_ShouldWork()
    {
        
        _multiDictionary.Add(1, "A");
        _multiDictionary.Add(1, "B");

        
        Assert.That(_multiDictionary[1], Contains.Item("A"));
        Assert.That(_multiDictionary[1], Contains.Item("B"));
    }

    [Test]
    public void Add_NullValue_ShouldNotWork()
    {
        
        var keyValuePair = new KeyValuePair<int, ICollection<string>>(1, null);

        Assert.Throws<ArgumentNullException>(() => _multiDictionary.Add(keyValuePair));
    }

    [Test]
    public void Remove_ExistingKey_ShouldWork()
    {
        
        _multiDictionary.Add(1, "A");
        _multiDictionary.Add(1, "B");

        
        var result = _multiDictionary.Remove(1);

        
        Assert.That(result, Is.True);
        Assert.That(_multiDictionary.ContainsKey(1), Is.False);
    }

    [Test]
    public void Remove_NonExistingKey_ShouldNotWork()
    {
        
        _multiDictionary.Add(1, "A");

        
        var result = _multiDictionary.Remove(2);

        
        Assert.That(result, Is.False);
    }

    [Test]
    public void Indexer_GetExistingKey_ShouldWork()
    {
        
        _multiDictionary.Add(1, "A");

        
        var values = _multiDictionary[1];
        
        Assert.That(values, Contains.Item("A"));
    }

    [Test]
    public void Indexer_GetNonExistingKey_ShouldNotWork()
    {
        Assert.Throws<KeyNotFoundException>(() =>
        {
            var values = _multiDictionary[1];
        });
    }

    [Test]
    public void ContainsKey_ExistingKey_ShouldWork()
    {
        
        _multiDictionary.Add(1, "A");

        
        var result = _multiDictionary.ContainsKey(1);

        
        Assert.That(result, Is.True);
    }

    [Test]
    public void ContainsKey_NonExistingKey_ShouldNotWork()
    {
        
        var result = _multiDictionary.ContainsKey(1);

        
        Assert.That(result, Is.False);
    }

    [Test]
    public void Clear_ShouldWork()
    {
        
        _multiDictionary.Add(1, "A");
        _multiDictionary.Add(2, "B");
        
        _multiDictionary.Clear();

        
        Assert.That(_multiDictionary.Count, Is.EqualTo(0));
    }

    [Test]
    public void TryGetValue_ExistingKey_ShouldWork()
    {

        _multiDictionary.Add(1, "A");

        var result = _multiDictionary.TryGetValue(1, out var values);

        Assert.That(result, Is.True);
        Assert.That(values, Contains.Item("A"));
    }

    [Test]
    public void TryGetValue_NonExistingKey_ShouldNotWork()
    {
        
        var result = _multiDictionary.TryGetValue(1, out var values);

        
        Assert.That(result, Is.False);
        Assert.That(values, Is.Null);
    }

    [Test]
    public void Enumerator_ShouldWork()
    {
        
        _multiDictionary.Add(1, "A");
        _multiDictionary.Add(2, "B");

        
        var results = new List<KeyValuePair<int, ICollection<string>>>();
        foreach (var kvp in _multiDictionary)
        {
            results.Add(kvp);
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
}
#region

using System.Diagnostics;
using Lkhsoft.Collections.Bst;

#endregion

namespace Lkhsoft.Collections.Test;

[TestFixture]
public class BTreeTests
{
    [Test]
    public void TestSearchPerformance_ShouldWork()
    {
        const int numberOfKeys = 0x64;
        const int valuesPerKey = 0x2710;
        var random = new Random();

        // Ajout des clés avec leurs valeurs
        var addStopWatch = Stopwatch.StartNew();
        var i = 1;
        GC.Collect();
        var btree = new BTree<int, string>(128);
        var memoryBefore = GC.GetTotalMemory(true);
        while (i <= numberOfKeys)
        {
            var addValuesStopwatch = Stopwatch.StartNew();
            TestContext.Out.WriteLine($"Ajout des valeurs pour la clé {i}");
            var j = 1;
            while (j <= valuesPerKey)
            {
                var valueAddStopwatch = Stopwatch.StartNew();
                btree.Add(i, $"Value-{i}-{j}");
                valueAddStopwatch.Stop();
                TestContext.Out.WriteLine($"Temps d'exécution de l'ajout de la valeur Value-{i}-{j} : {valueAddStopwatch.ElapsedMilliseconds} ms");
                j++;
            }
            addValuesStopwatch.Stop();
            TestContext.Out.WriteLine($"Temps d'exécution de l'ajout des valeurs pour la clé {i}: {addValuesStopwatch.ElapsedMilliseconds} ms");
            i++;
        }
        addStopWatch.Stop();
        TestContext.Out.WriteLine($"Temps d'exécution de l'ajout : {addStopWatch.ElapsedMilliseconds} ms");
        GC.Collect();
        var memoryAfter = GC.GetTotalMemory(true);

        var totalMemory = memoryAfter - memoryBefore;
        Console.WriteLine($"Poids du BTree en mémoire : {totalMemory} octets");
        
        // Choisir une clé aléatoire pour effectuer une recherche
        var keyToSearch = random.Next(1, numberOfKeys + 1);

        // Valeurs à rechercher (toutes les valeurs de cette clé)
        var valuesToSearch = new List<string>();
        for (var j = 1; j <= valuesPerKey; j++)
        {
            valuesToSearch.Add($"Value-{keyToSearch}-{j}");
        }

        // Mesurer le temps d'exécution de la recherche
        var stopwatch = Stopwatch.StartNew();
        var indexes  = btree.Search(keyToSearch, valuesToSearch, out var foundValues);
        stopwatch.Stop();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(foundValues is not null, Is.True);
            Assert.That(indexes is not null, Is.True);
        }

        if (foundValues is null || indexes is null) return;
        
        using (Assert.EnterMultipleScope())
        {
            // Vérifications
            Assert.That(indexes.Any(), Is.True, "La clé recherchée doit être trouvée dans l'arbre.");
            Assert.That(foundValues, Is.Not.Null, "Les valeurs trouvées ne doivent pas être nulles.");
        }
        Assert.That(foundValues, Is.EquivalentTo(valuesToSearch), "Les valeurs trouvées doivent correspondre exactement aux valeurs insérées.");

        // Afficher le temps d'exécution
        TestContext.Out.WriteLine($"Temps d'exécution de la recherche : {stopwatch.ElapsedMilliseconds} ms");

        // Ajouter une assertion pour vérifier que le temps est raisonnable
        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(2000), "La recherche ne doit pas dépasser 2 secondes.");

    }
    
    [Test]
    public void Insert_ShouldWork()
    {
        var bTree = new BTree<int, string>
        {
            {1, "value1"},
            {2, "value2"}
        };

        Assert.That(bTree.Count, Is.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(bTree.ContainsKey(1), Is.True);
            Assert.That(bTree.ContainsKey(2), Is.True);
        }
    }

    [Test]
    public void InsertDuplicate_ShouldNotWork()
    {
        var bTree = new BTree<int, string> {{1, "value1"}};

        Assert.Throws<InvalidOperationException>(() => bTree.Add(1, "value1"));
    }

    [Test]
    public void Remove_ShouldWork()
    {
        var bTree = new BTree<int, string>();

        bTree.Add(1, "value1");
        bTree.Add(1, "value2");
        bTree.Add(1, "value3");
        bTree.Add(1, "value5");
        bTree.Add(1, "value6");
        bTree.Add(1, "value7");
        bTree.Add(2, "value1");
        bTree.Remove(1);

        Assert.That(bTree.Count, Is.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(bTree.ContainsKey(1), Is.False);
            Assert.That(bTree.ContainsKey(2), Is.True);
        }
    }
    
    [Test]
    public void DuplicatedKey_ShoulNotdWork()
    {
        var bTree = new BTree<int, string>
        {
            {1, "value1"},
            {1, "value2"},
            {1, "value3"},
            {1, "value5"},
            {1, "value6"},
            {1, "value7"},
            {2, "value1"}
        };

        Assert.Throws<InvalidOperationException>(() => bTree.Add(1, "value1"));
    }

    [Test]
    public void RemoveNonExistentKey_ShouldNotWork()
    {
        var bTree = new BTree<int, string> {{1, "value1"}};

        Assert.That(bTree.Remove(2), Is.False);
    }

    [Test]
    public void Search_ShouldWork()
    {
        var bTree = new BTree<int, string>
        {
            {1, "value1"},
            {2, "value2"}
        };

        var result = bTree.Search(1, "value1");
        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public void SearchNonExistentValue_ShouldNotWork()
    {
        var bTree = new BTree<int, string> {{1, "value1"}};

        var result = bTree.Search(1, "value2");
        Assert.That(result, Is.EqualTo(-2));
    }

    [Test]
    public void GetValues_ShouldWork()
    {
        var bTree = new BTree<int, string>
        {
            {1, "value1"},
            {1, "value2"}
        };

        var values = bTree[1];
        Assert.That(values, Is.EqualTo(new[] {"value1", "value2"}));
    }

    [Test]
    public void GetValuesNonExistentKey_ShouldNotWork()
    {
        var bTree = new BTree<int, string> {{1, "value1"}};

        var values = bTree[2];
        Assert.That(values, Is.Empty);
    }

    [Test]
    public void Clear_ShouldWork()
    {
        var bTree = new BTree<int, string>
        {
            {1, "value1"},
            {2, "value2"}
        };

        bTree.Clear();

        Assert.That(bTree.Count, Is.EqualTo(0));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(bTree.ContainsKey(1), Is.False);
            Assert.That(bTree.ContainsKey(2), Is.False);
        }
    }

    [Test]
    public void ContainsKey_ShouldWork()
    {
        var bTree = new BTree<int, string> {{1, "value1"}};

        Assert.That(bTree.ContainsKey(1), Is.True);
    }

    [Test]
    public void ContainsKeyNonExistentKey_ShouldNotWork()
    {
        var bTree = new BTree<int, string> {{1, "value1"}};

        Assert.That(bTree.ContainsKey(2), Is.False);
    }

    [Test]
    public void TryGetValue_ShouldWork()
    {
        var bTree = new BTree<int, string> {{1, "value1"}};

        var result = bTree.TryGetValue(1, out var values);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(values, Is.EqualTo(new[] { "value1" }));
        }
    }

    [Test]
    public void TryGetValueNonExistentKey_ShouldNotWork()
    {
        var bTree = new BTree<int, string> {{1, "value1"}};

        var result = bTree.TryGetValue(2, out var values);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.False);
            Assert.That(values, Is.Empty);
        }
    }
}
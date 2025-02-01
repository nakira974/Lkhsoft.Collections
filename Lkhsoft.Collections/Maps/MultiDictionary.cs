using Lkhsoft.Collections.Trees.Bst;

namespace Lkhsoft.Collections.Maps;

/// <summary>
/// Multimap implementation
/// </summary>
public class MultiDictionary<TKey, TValue> : RedBlackTree<TKey, ICollection<TValue>>
    where TKey : IComparable<TKey>
{
    ///<inheritdoc/>
    public override ICollection<TValue> this[TKey key]
    {
        get => !TryGetValue(key, out var values) ? throw new ArgumentNullException(): values;
        set
        {
            if (!TryGetValue(key, out var values))
                throw new KeyNotFoundException();
            ((List<TValue>)base[key]).AddRange(value ?? throw new ArgumentNullException());
        }
    }
    
    /// <summary>
    /// Adds a value to the collection of values associated with the specified key
    /// </summary>
    public void Add(TKey key, TValue value)
    {
        if(!TryGetValue(key, out var values))
        {
            base.Add(key, new List<TValue>());
            ((List<TValue>)base[key]).Add(value);
            return;
        }
        ((List<TValue>)base[key]).Add(value);
    }
    
    /// <inheritdoc/>
    public override void Add(KeyValuePair<TKey, ICollection<TValue>> item)
    {
        Add(item.Key, item.Value ?? throw new ArgumentNullException());
    }
}
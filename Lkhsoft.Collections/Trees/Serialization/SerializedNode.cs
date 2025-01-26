using System.Xml.Serialization;

namespace Lkhsoft.Collections.Trees.Serialization;

/// <summary>
/// Node of a simple tree for serialization purposes only
/// </summary>
[XmlRoot("Node")]
public class SerializedNode<T>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public SerializedNode()
    {
        
    }
    
    /// <summary>
    /// Base constructor
    /// </summary>
    public SerializedNode(T value)
    {
        Value = value;
    }

    /// <summary>
    /// Value of the node
    /// </summary>
    [XmlAttribute(AttributeName = "Value")]
    public T Value { get; set; }
}

/// <summary>
/// Node of a key-value tree for serialization purposes only
/// </summary>
[XmlRoot("Node")]
public class SerializedNode<TKey, T> :  SerializedNode<T>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public SerializedNode()
    {
        
    }
    
    /// <summary>
    /// Base constructor
    /// </summary>
    public SerializedNode(TKey key, T value) : base(value)
    {
        Key = key;
    }
    
    /// <summary>
    /// Key of the node
    /// </summary>
    [XmlAttribute(AttributeName = "Key")]
    public TKey Key { get; set; }
}

[XmlRoot("Tree")]
public class SerializedNodes<T>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public SerializedNodes()
    {
        
    }
    
    /// <summary>
    /// Base constructor
    /// </summary>
    public SerializedNodes(IEnumerable<SerializedNode<T>> nodes)
    {
        Nodes = nodes.ToList();
        Count = (uint)Nodes.Count;
    }
    
    /// <summary>
    /// Number of nodes
    /// </summary>
    [XmlElement(ElementName = "Count")]
    public uint Count { get; set; }

    /// <summary>
    /// List of nodes
    /// </summary>
    [XmlArrayItem("Node")]
    public List<SerializedNode<T>> Nodes { get; set; }
}

[XmlRoot("Tree")]
public class SerializedNodes<TKey, TValue>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public SerializedNodes()
    {
        
    }
    
    /// <summary>
    /// Number of nodes
    /// </summary>
    [XmlElement(ElementName = "Count")]
    public uint Count { get; set; }

    /// <summary>
    /// Base constructor
    /// </summary>
    public SerializedNodes(IEnumerable<SerializedNode<TKey, TValue>> nodes)
    {
        Nodes = nodes.ToList();
        Count = (uint)Nodes.Count;
    }

    /// <summary>
    /// List of nodes
    /// </summary>
    [XmlArrayItem("Node")]
    public List<SerializedNode<TKey, TValue>> Nodes { get; set; }
}
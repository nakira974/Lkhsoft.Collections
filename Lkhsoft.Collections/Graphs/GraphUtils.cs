namespace Lkhsoft.Collections.Graphs;

public static class GraphUtils
{
    /// <summary>
    ///  Breadth-first search algorithm
    /// </summary>
    public static IEnumerable<TValue> Bfs<TValue>(Graph<TValue> graph, TValue startNode)
        where TValue : class, IComparable<TValue>
    {
        if (graph is null || startNode is null)
        {
            throw new ArgumentNullException();
        }

        var visited = new HashSet<TValue>();
        var queue = new Queue<TValue>();
        var result = new List<TValue>();

        queue.Enqueue(startNode);
        visited.Add(startNode);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            result.Add(node);

            foreach (var neighbor in graph.GetOutgoingEdges(node))
            {
                if (!visited.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Depth-first search algorithm
    /// </summary>
    public static IEnumerable<TValue> Dfs<TValue>(Graph<TValue> graph, TValue startNode)
        where TValue : class, IComparable<TValue>
    {
        if (graph == null || startNode == null)
        {
            throw new ArgumentNullException();
        }

        var visited = new HashSet<TValue>();
        var stack = new Stack<TValue>();
        var result = new List<TValue>();

        stack.Push(startNode);

        while (stack.Count > 0)
        {
            var node = stack.Pop();
            if (!visited.Add(node)) continue;
            result.Add(node);

            foreach (var neighbor in graph.GetOutgoingEdges(node))
            {
                if (!visited.Contains(neighbor))
                {
                    stack.Push(neighbor);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Calculates the shortest path from a source node to all other nodes in the graph using Dijkstra's algorithm.
    /// </summary>
    /// <typeparam name="TWeight">The type of the edge weight, must be comparable and support arithmetic operations.</typeparam>
    /// <typeparam name="TValue">The type of the node value, must be comparable.</typeparam>
    /// <param name="graph">The weighted graph to search.</param>
    /// <param name="source">The source node.</param>
    /// <param name="destination">The optional destination node. If specified, stops when the destination is reached.</param>
    /// <returns>An enumerable containing the nodes in the shortest path from source to destination, or from source to all nodes if no destination is specified.</returns>
public static Dictionary<TValue, TWeight> Dijkstra<TWeight, TValue>(
        WeightedGraph<TWeight, TValue> graph,
        TValue source)
        where TWeight : struct, IComparable<TWeight>
        where TValue : class, IComparable<TValue>
    {
        if (!graph.Contains(source))
        {
            throw new ArgumentException("Source node is not in the graph.");
        }

        var distances = new Dictionary<TValue, TWeight>();
        var previous = new Dictionary<TValue, TValue?>(); // To reconstruct the shortest path
        var priorityQueue = new SortedSet<(TWeight, TValue)>(Comparer<(TWeight, TValue)>.Create((a, b) =>
        {
            var compare = a.Item1.CompareTo(b.Item1); // Compare distances
            return compare == 0 ? Comparer<TValue>.Default.Compare(a.Item2, b.Item2) : compare;
        }));

        // Initialize distances and priority queue
        foreach (var edge in graph)  // Iterate over the dictionary's KeyValuePair<TWeight, ISet<TValue>>
        {
            foreach (var node in edge.Value)  // Iterate over the ISet<TValue> for each weight
            {
                distances[node] = (dynamic)default(TWeight)! + (dynamic)int.MaxValue; // Initialize to a large value
                previous[node] = null;
            }
        }

        distances[source] = (dynamic)0; // Distance to the source is 0
        priorityQueue.Add((distances[source], source));

        // Main algorithm loop
        while (priorityQueue.Count > 0)
        {
            var (currentDistance, currentNode) = priorityQueue.Min;
            priorityQueue.Remove(priorityQueue.Min);

            // Iterate over neighbors and update distances
            foreach (var neighbor in graph.GetOutgoingEdges(currentNode))
            {
                var edgeWeight = graph.GetEdgeWeight(currentNode, neighbor);
                var newDistance = (dynamic)currentDistance + (dynamic)edgeWeight;

                if (distances.ContainsKey(neighbor) && newDistance.CompareTo(distances[neighbor]) >= 0) continue;
                priorityQueue.Remove((distances[neighbor], neighbor)); // Remove old distance
                distances[neighbor] = newDistance;
                previous[neighbor] = currentNode;
                priorityQueue.Add((newDistance, neighbor));
            }
        }

        // Return the dictionary with the shortest distances from the source
        return distances;
    }
}
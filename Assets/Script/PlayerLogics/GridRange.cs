using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridRange 
{
    public static HashSet<Node> GetReachableNodes(Node start ,int range)
    {
        Queue<Node> frontier = new Queue<Node>();
        Dictionary<Node, int> costSoFar = new Dictionary<Node, int>();

        frontier.Enqueue(start);
        costSoFar[start] = 0;

        while (frontier.Count > 0)
        {
            Node current = frontier.Dequeue();

            foreach (Node next in GridManager.Instance.GetNeighbours(current))
            {
                int newCost = costSoFar[current] + 1;

                if (newCost > range) continue;
                if (!costSoFar.ContainsKey(next))
                {
                    costSoFar[next] = newCost;
                    frontier.Enqueue(next);
                }
            }
        }
        return new HashSet<Node>(costSoFar.Keys);
    }
}

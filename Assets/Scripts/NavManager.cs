using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavManager : MonoBehaviour {

    /// <summary>
    /// Singleton for the NavManager class.
    /// </summary>
    private static NavManager instance;
    public static NavManager Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<NavManager>();
                if (instance == null) {
                    GameObject navigationManager = 
                        new GameObject("Navigation Manager", typeof(NavManager));
                    instance = navigationManager
                        .GetComponent<NavManager>();
                }
            }
            return instance;
        }
    }

    public bool isInitialized = false;
    public List<NavNode> nodes = new List<NavNode>();

    /// <summary>
    /// Returns the sortest path to the destination node from the from node.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="goal"></param>
    /// <returns></returns>
    public NavNode[] GetPathToNode(NavNode start, NavNode goal) {
        List<NavNode> path = new List<NavNode>();

        PriorityQueue<NodeEdge> frontier = new PriorityQueue<NodeEdge>(Compare);
        frontier.Push(new NodeEdge(start, 0));

        Dictionary<NavNode, NavNode> cameFrom = new Dictionary<NavNode, NavNode>();
        cameFrom.Add(start, null);

        while (frontier.Count > 0) {
            NodeEdge item = frontier.Pop();
            NavNode current = item.node;

            if (current == goal) {
                break;
            }

            for (int childIndex = 0; childIndex < current.neighbours.Count; childIndex++) {
                NavNode next = current.neighbours[childIndex];
                if (!cameFrom.ContainsKey(next)) {
                    float distance = Heuristic(goal, next);
                    frontier.Push(new NodeEdge(next, distance));
                    cameFrom.Add(next, current);
                }
            }
        }

        // Back trace
        int a = 0;
        NavNode key = goal;
        NavNode value;
        while (key != null && cameFrom.TryGetValue(key, out value)) {
            path.Add(key);
            key = value;
        }

        path.Reverse();
        return path.ToArray();
    }

    public float Heuristic(NavNode a, NavNode b) {
        // Manhattan distance on a square grid
        return Mathf.Abs(a.transform.position.x - b.transform.position.x) + 
            Mathf.Abs(a.transform.position.y - b.transform.position.y);
    }

    public NodeEdge FindInSet(PriorityQueue<NodeEdge> set, NavNode target) {
        for (int stackIndex = 0; stackIndex < set.Count; stackIndex++) {
            NodeEdge stackItem = set.stack[stackIndex];
            if (stackItem.node == target) {
                return stackItem;
            }
        }
        return null;
    }

    /// <summary>
    /// Loops through all nodes to find the closest one to the given point.
    /// </summary>
    /// <param name="from"></param>
    /// <returns></returns>
    public NavNode FindClosestNode(Vector3 from) {
        float closestDistance = float.MaxValue;
        NavNode closestNode = null;
        foreach(NavNode node in nodes) {
            float distance = Vector3.Distance(from, node.transform.position);
            if (distance < closestDistance) {
                closestNode = node;
                closestDistance = distance;
            }
        }
        return closestNode;
    }

    public int Compare(NodeEdge a, NodeEdge b) {
        if (a.distance < b.distance) return -1;
        else if (a.distance > b.distance) return 1;
        else return 0;
    }
}

public class NodeEdge {
    public NavNode node;
    public float distance;
    public NodeEdge(NavNode key, float value) {
        this.node = key;
        this.distance = value;
    }
}


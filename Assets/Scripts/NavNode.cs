using System;
using System.Collections.Generic;
using UnityEngine;

public class NavNode {
    public string name;
    public Transform transform;
    public List<NavNode> neighbours;
    public List<float> distances;

    public NavNode(Transform transform) {
        neighbours = new List<NavNode>();
        distances = new List<float>();
        this.name = transform.name;
        this.transform = transform;
        NavManager.Instance.nodes.Add(this);
    }

    public void AddNeighbour(NavNode node) {
        float distance = Vector3.Distance(node.transform.position, 
                                          transform.position);
        if (!node.neighbours.Contains(this)) {
            node.neighbours.Add(this);
            node.distances.Add(distance);
        }
        if (!node.neighbours.Contains(node)) {
            neighbours.Add(node);
            distances.Add(distance);
        }
    }

    public bool Equals(NavNode obj) {
        return (name == obj.name && 
                distances == obj.distances &&
                transform == obj.transform);
    }

    public static bool operator ==(NavNode a, NavNode b) {
        // If both are null, or both are same instance, return true.
        if (ReferenceEquals(a, b)) {
            return true;
        }
        // If one is null, but not both, return false.
        if (((object)a == null) || ((object)b == null)){
            return false;
        }
        // Return true if the fields match.
        return (a.name == b.name && 
                a.distances == b.distances &&
                a.transform == b.transform);
    }

    public static bool operator !=(NavNode a, NavNode b) {
        return !(a == b);
    }
}
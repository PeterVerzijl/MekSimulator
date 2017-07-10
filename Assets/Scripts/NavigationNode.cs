using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationNode : MonoBehaviour {

    private NavNode node;
    public NavNode Node {
        get {
            if (node == null) {
                node = new NavNode(transform);
                foreach(Transform child in childNodes) {
                    NavNode childNode = child.GetComponent<NavigationNode>().Node;
                    if (childNode != null) {
                        node.AddNeighbour(childNode);
                    } else {
                        Debug.LogErrorFormat("Obj {0} tried to register {1} as subnode.", transform.name, child.name);
                    }
                }
            }
            return node;
        }
    }

    public Transform[] childNodes;

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        float sphereSize = 0.1f;
        Gizmos.DrawSphere(transform.position, sphereSize);
        if (Node != null) {
            foreach(NavNode child in Node.neighbours) {
                Gizmos.DrawLine(transform.position, child.transform.position);
                Gizmos.DrawSphere(child.transform.position, sphereSize);
            }
        } else if (childNodes.Length > 0) {
            foreach(Transform child in childNodes) {
                Gizmos.DrawLine(transform.position, child.position);
                Gizmos.DrawSphere(child.position, sphereSize);
            }
        }
    }
}

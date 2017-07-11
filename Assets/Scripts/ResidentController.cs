using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResidentController : MonoBehaviour {

    public Resident resident;
    
    private bool hasTarget = false;
    public float walkSpeed = 5;
    public Vector3 target;

    public SpriteRenderer renderer;

	// Use this for initialization
	void Start () {
        renderer = GetComponentInChildren<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		/*if (Input.GetMouseButtonDown(0)) {
            // TODO: Make this a lot less ugly than just checking for all distances!
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Find closest navpoint
            NavNode destNode = NavManager.Instance.FindClosestNode(mousePos);
            NavNode curNode = NavManager.Instance.FindClosestNode(transform.position);
            NavNode[] path = NavManager.Instance.GetPathToNode(curNode, destNode);

            StartCoroutine(MoveAlongPath(path));
        }*/
	}

    IEnumerator MoveAlongPath(NavNode[] nodes) {
        // Print route
        string buffer = "Path: ";
        foreach (NavNode node in nodes)
            buffer += node.name + " -> ";
        print(buffer);

        NavNode current = null;
        foreach (NavNode next in nodes) {
            // Jump instantly if we are moving from stairs to stairs or from a door to stairs etc.
            if (current != null && (
                (current.name.Contains("Door") && next.name.Contains("Stairs")) || 
                (current.name.Contains("Stairs") && next.name.Contains("Door")) ||
                (current.name.Contains("Stairs") && next.name.Contains("Stairs")))) {
                if (current.name.Contains("Door") && next.name.Contains("Stairs")) {
                    // If going into a door
                    current.transform.GetComponent<Animator>().SetBool("Open", true);
                    yield return new WaitForSeconds(1f);
                    renderer.sortingOrder = -1;
                    current.transform.GetComponent<Animator>().SetBool("Open", false);
                    yield return new WaitForSeconds(1f);
                    transform.position = next.transform.position;
                    renderer.sortingOrder = 5;
                } else if (current.name.Contains("Stairs") && 
                           next.name.Contains("Door")) {
                    // If going from stairs to dore
                    transform.position = next.transform.position;
                    renderer.sortingOrder = -1;
                    next.transform.GetComponent<Animator>().SetBool("Open", true);
                    yield return new WaitForSeconds(1f);
                    renderer.sortingOrder = 5;
                    next.transform.GetComponent<Animator>().SetBool("Open", false);
                    yield return new WaitForSeconds(1f);
                } else if (current.name.Contains("Stairs") && next.name.Contains("Stairs")) {
                    // Going up stairs
                    yield return new WaitForSeconds(0.2f);
                    transform.position = next.transform.position;
                }
            } else {
                // Move graduately
                while (Vector3.Distance(transform.position, next.transform.position) > 0.5f) {
                    transform.position = Vector3.MoveTowards(transform.position, 
                                                             next.transform.position, 
                                                             walkSpeed * Time.deltaTime);
                    yield return new WaitForEndOfFrame();
                }
            }
            current = next;
        }
    }

    private void OnDrawGizmos() {
        if (hasTarget) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(target, 0.1f);
            Gizmos.DrawLine(transform.position, target);
        }
    }
}
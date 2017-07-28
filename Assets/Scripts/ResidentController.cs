using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResidentController : MonoBehaviour {

    public Resident resident;
    
    private bool hasTarget = false;
    public float walkSpeed = 5;
    public Vector3 target;

    public Sprite maleSprite;
    public Sprite femaleSprite;

    public new SpriteRenderer renderer;

    [Header("Activity")]
    public double activityStartTime;
    public float[] activityProgress;
    public Activity currentActivity;

	// Use this for initialization
	void Start () {
        renderer = GetComponentInChildren<SpriteRenderer>();

        if (resident != null) {
            renderer.sprite = (resident.sex == Sex.Male) ? 
                maleSprite : femaleSprite;
        }
	}
	
	// Update is called once per frame
	void Update () {
		if (currentActivity != null) {
            float elapsedTime = (float)(GameManager.Instance.CurrentTime - activityStartTime);
            for(int attributeIndex = 0; 
                attributeIndex < currentActivity.changingAtributes.Length; 
                attributeIndex++) 
            {
                StatChange stateChange = currentActivity.changingAtributes[attributeIndex];
                activityProgress[attributeIndex] = 
                    elapsedTime / stateChange.hoursToChange;
                while (activityProgress[attributeIndex] > 1.0f) {
                    activityProgress[attributeIndex] -= 1.0f;
                    resident.UpdateResidentStats(stateChange);
                }
            }

        }
	}

    IEnumerator MoveAlongPath(NavNode[] nodes, Action<Transform> onDestinationReached = null) {
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
        if (current != null && onDestinationReached != null) {
            onDestinationReached.Invoke(current.transform);
        }
    }

    internal void UpdateSex() {
        renderer.sprite = (resident.sex == Sex.Male) ? 
                maleSprite : femaleSprite;
    }

    /// <summary>
    /// Sets the taget for the resident to walk towards.
    /// </summary>
    /// <param name="targetNode">The target node to walk towards.</param>
    internal void SetTarget(NavNode targetNode) {
        NavNode curNode = NavManager.Instance.FindClosestNode(transform.position);
        NavNode[] path = NavManager.Instance.GetPathToNode(curNode, targetNode);
        StartCoroutine(MoveAlongPath(path, (Transform t)=> {
            RoomManager rm = t.GetComponent<RoomManager>();
            if (rm != null) {
                rm.AddResident(this);
            }
        }));
    }

    public void SetActivity(Activity activity) {
        currentActivity = activity;
        activityProgress = new float[activity.changingAtributes.Length];
        activityStartTime = GameManager.Instance.CurrentTime;  
    }

    public void RemoveCurrentActivity() {
        currentActivity = null;
    }

    private void OnDrawGizmos() {
        if (hasTarget) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(target, 0.1f);
            Gizmos.DrawLine(transform.position, target);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseManager : MonoBehaviour {

    public int rooms = 3;
    public int floors { get { return Mathf.CeilToInt(rooms / 3.0f); } }

    public GameObject floorPrefab;
    List<Transform> floorObjects = new List<Transform>();
    List<Transform> roomObjects = new List<Transform>();

	// Use this for initialization
	void Start () {
        Transform roof = transform.Find("Roof");
        roof.transform.position += Vector3.up * (2 * floors);

        int roomsToActivate = rooms;
        NavNode curNode = GetComponentInChildren<NavigationNode>().Node;
		for (int i = 0; i < floors; i++) {
            Transform floor = Instantiate(floorPrefab, transform).transform;
            NavNode navNode = floor.Find("Stairs").GetComponent<NavigationNode>().Node;
            curNode.AddNeighbour(navNode);
            curNode = navNode;

            // De-Activate rooms
            foreach (Transform child in floor) {
                if (child.name.Contains("Room")) {
                    roomObjects.Add(child);
                    if (roomsToActivate > 0 ) {
                        roomsToActivate--;
                    } else {
                        child.gameObject.SetActive(false);
                    }
                }
            }

            
            floor.localPosition = Vector3.up * (2 + i * 2);
            floorObjects.Add(floor);
        }
        // TODO: The roof should really be a place where characters 
        // should be able to go to kiss and enjoy the summer.
        curNode.AddNeighbour(roof.GetComponent<NavigationNode>().Node);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

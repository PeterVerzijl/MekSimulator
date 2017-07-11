using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HouseManager : MonoBehaviour {

    public enum RoomType {
        Bedroom,
        Bathroom,
        Loundryroom,
        Floor,
        Base
    }

    public int rooms = 3;
    public int floors { get { return Mathf.CeilToInt(rooms / 3.0f); } }

    public GameObject bedroomPrefab;
    public GameObject bathroomPrefab;
    public GameObject loundryroomPrefab;
    public GameObject floorPrefab;
    public GameObject basePrefab;
    
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
                if (child.name.Contains("SlotPoint")) {
                    roomObjects.Add(child);
                    if (roomsToActivate > 0 ) {
                        roomsToActivate--;
                        AddRoom(child, floor, RoomType.Bedroom);
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

    public void StartSelectRoomTarget(RoomType type) {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("RoomSlot");
        for (int targetIndex = 0; targetIndex < targets.Length; targetIndex++) {
            Transform target = targets[targetIndex].transform;
            Transform floor = target.parent;
            target.GetChild(0).gameObject.SetActive(true);
            target.GetComponentInChildren<Button>().onClick.AddListener(()=> {
                AddRoom(target, floor, type);
            });
        }
    }

    public void AddRoom(Transform slot, Transform floor, RoomType type) {
        Transform goTransform = null;
        switch (type) {
            case RoomType.Bedroom: {
                goTransform = Instantiate(bedroomPrefab, floor).transform;
            } break;

            case RoomType.Bathroom: {
                goTransform = Instantiate(bathroomPrefab, floor).transform;
            } break;

            case RoomType.Loundryroom: {
                goTransform = Instantiate(loundryroomPrefab, floor).transform;
            } break;
        }
        goTransform.position = slot.position;
        NavNode navNode = floor.Find("Stairs")
            .GetComponent<NavigationNode>().Node;
        navNode.AddNeighbour(
            goTransform.GetComponent<NavigationNode>().Node);
        Destroy(slot.gameObject);
    }
}

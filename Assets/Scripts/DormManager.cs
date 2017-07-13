using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DormManager : MonoBehaviour {

    public GameObject bedroomPrefab;
    public GameObject bathroomPrefab;
    public GameObject loundryroomPrefab;
    public GameObject floorPrefab;
    public GameObject basePrefab;

    Transform dormBase;
    Transform roof;

    Dorm dorm { get { return GameManager.Instance.Dorm; } }
    
    List<Transform> floorTransforms = new List<Transform>();

	// Use this for initialization
	void Start () {
        dormBase = transform.Find("house_base");
        roof = transform.Find("Roof");

        foreach (Floor floor in dorm.floors) {
            Transform floorTransform = AddFloor();
            int childIndex = 0;
            foreach (Transform child in floorTransform) {
                if (child.name.Contains("SlotPoint")) {
                    AddRoom(child, floorTransform, floor.rooms[childIndex]);
                    childIndex++;
                }
            }
        }
#if false
        NavNode curNode = GetComponentInChildren<NavigationNode>().Node;
		for (int i = 0; i < floors; i++) {
            Transform floor = Instantiate(floorPrefab, transform).transform;
            NavNode navNode = floor.Find("Stairs").GetComponent<NavigationNode>().Node;
            curNode.AddNeighbour(navNode);
            curNode = navNode;

            // De-Activate rooms
            foreach (Transform child in floor) {
                if (child.name.Contains("SlotPoint")) {
                    if (roomsToActivate > 0 ) {
                        roomsToActivate--;
                        AddRoom(child, floor, RoomType.Bedroom);
                    }
                }
            }

            
            floor.localPosition = Vector3.up * (2 + i * 2);
            floorTransforms.Add(floor);
        }
        // TODO: The roof should really be a place where characters 
        // should be able to go to kiss and enjoy the summer.
        curNode.AddNeighbour(roof.GetComponent<NavigationNode>().Node);
#endif
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Adds a floor at the top of the stack of floors, just below the roof.
    /// This function also adds the floor to the floors array and 
    /// reattaches the navigation nodes.
    /// </summary>
    /// <returns>A transform of the created floor object.</returns>
    public Transform AddFloor() {
        Transform result = Instantiate(floorPrefab, this.transform).transform;
        result.position = Vector3.zero + Vector3.up * (2 + 2 * floorTransforms.Count);
        roof.transform.position += Vector3.up * 2;
        NavNode roofNode = roof.GetComponent<NavigationNode>().Node;

        // Add to dorm
        dorm.floors.Add(new Floor());

        // Now fix the node graphs
        Transform prevFloor;
        NavNode prevFloorNode;
        if (floorTransforms.Count > 0) {
            prevFloor = floorTransforms[floorTransforms.Count - 1];
            prevFloorNode = prevFloor.Find("Stairs")
                .GetComponent<NavigationNode>().Node;
        } else {
            prevFloor = dormBase;
            prevFloorNode = dormBase.Find("Door").GetComponent<NavigationNode>().Node;
        }
        prevFloorNode.RemoveNeighbour(roofNode);
        NavNode floorNode = result.Find("Stairs").GetComponent<NavigationNode>().Node;
        roofNode.neighbours.Clear();
        prevFloorNode.AddNeighbour(floorNode);
        floorNode.AddNeighbour(roofNode);
        
        // Add our floor to our list of floors
        floorTransforms.Add(result);
        return result;
    }

    public void StartSelectRoomTarget(RoomType type) {
        if (type != RoomType.Floor) {
            GameObject[] targets = GameObject.FindGameObjectsWithTag("RoomSlot");
            for (int targetIndex = 0; targetIndex < targets.Length; targetIndex++) {
                Transform target = targets[targetIndex].transform;
                Transform floor = target.parent;
                target.GetChild(0).gameObject.SetActive(true);
                target.GetComponentInChildren<Button>().onClick.AddListener(()=> {
                    AddRoom(target, floor, type);
                });
            }
        } else {
            AddFloor();
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

        // Find the slot index and floor index such that we can update the dorm obj.
        int slotIndex = 0;
        int floorIndex = floorTransforms.IndexOf(floor);
        foreach(Transform child in floor) {
            if (child.name.Contains("SlotPoint")) {
                if (child == slot) {
                    break;
                }
                slotIndex++;
            }
        }
        dorm.floors[floorIndex].rooms[slotIndex] = type;

        goTransform.position = slot.position;
        NavNode navNode = floor.Find("Stairs")
            .GetComponent<NavigationNode>().Node;
        navNode.AddNeighbour(
            goTransform.GetComponent<NavigationNode>().Node);
        Destroy(slot.gameObject);
    }
}

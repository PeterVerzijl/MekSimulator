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
            Transform floorTransform = InstantiateFloor();
            int childIndex = 0;
            foreach (Transform child in floorTransform) {
                if (child.name.Contains("SlotPoint")) {
                    InstantiateRoom(child, floorTransform, floor.rooms[childIndex]);
                    childIndex++;
                }
            }
        }
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
    public Transform InstantiateFloor() {
        Transform result = Instantiate(floorPrefab, this.transform).transform;
        result.position = Vector3.zero + Vector3.up * (2 + 2 * floorTransforms.Count);
        roof.transform.position += Vector3.up * 2;
        NavNode roofNode = roof.GetComponent<NavigationNode>().Node;

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

    public Transform AddFloor() {
        // Add to dorm
        dorm.floors.Add(new Floor());
        return InstantiateFloor();
    }

    private RoomType lastSelectedRoomType;
    public void StartSelectRoomTarget(RoomType type) {
        if (type != RoomType.Floor) {
            lastSelectedRoomType = type;
            GameObject[] targets = GameObject.FindGameObjectsWithTag("RoomSlot");
            for (int targetIndex = 0; targetIndex < targets.Length; targetIndex++) {
                Transform target = targets[targetIndex].transform;
                Transform floor = target.parent;
                target.GetChild(0).gameObject.SetActive(true);
                target.GetComponentInChildren<Button>().onClick.AddListener(
                    delegate { OnSelectRoomTarget(target); });
            }
        } else {
            InstantiateFloor();
        }
    }

    private void OnSelectRoomTarget(Transform target) {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("RoomSlot");
        AddRoom(target, target.parent, lastSelectedRoomType);
        // Now turn all targets off.
        foreach(GameObject go in targets) {
            Transform t = go.transform;
            t.GetChild(0).gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Instantiates a room in the scene.
    /// </summary>
    /// <param name="slot">The slot where the room is build</param>
    /// <param name="floor">The foor on which the room is build</param>
    /// <param name="type">The type of room</param>
    public void InstantiateRoom(Transform slot, Transform floor, RoomType type) {
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

            case RoomType.None: return;
        }

        goTransform.position = slot.position;
        NavNode navNode = floor.Find("Stairs")
            .GetComponent<NavigationNode>().Node;
        navNode.AddNeighbour(
            goTransform.GetComponent<NavigationNode>().Node);
        Destroy(slot.gameObject);
    }

    /// <summary>
    /// Adds a new room to the dorm and instantiates a room object in the slot.
    /// </summary>
    /// <param name="slot">The slot where the room is build</param>
    /// <param name="floor">The foor on which the room is build</param>
    /// <param name="type">The type of room</param>
    public void AddRoom(Transform slot, Transform floor, RoomType type) {
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

        // Now instantiate the room
        InstantiateRoom(slot, floor, type);
    }
}

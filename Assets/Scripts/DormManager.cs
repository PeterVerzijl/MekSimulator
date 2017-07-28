using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DormManager : MonoBehaviour {

    public Transform outside;
    public Transform dormBase;
    public Transform roof;

    [Header("Prefabs")]
    public GameObject bedroomPrefab;
    public GameObject bathroomPrefab;
    public GameObject loundryroomPrefab;
    public GameObject floorPrefab;
    public GameObject basePrefab;
    [Space]
    public GameObject residentPrefab;

    Dorm dorm { get { return GameManager.Instance.Dorm; } }

    FloorManager baseFloorManager;
    List<FloorManager> floorManagers = new List<FloorManager>();

	// Use this for initialization
	void Start () {
        baseFloorManager = dormBase.GetComponent<FloorManager>();

        // Initialize floors
        for(int floorIndex = 0; floorIndex < dorm.floors.Count; floorIndex++) {
            Floor floor = dorm.floors[floorIndex];
            Transform floorTransform = InstantiateFloor();
            int childIndex = 0;
            FloorManager floorManager = floorTransform.GetComponent<FloorManager>();
            for (int slotIndex = 0; slotIndex < floorManager.roomSlots.Length; slotIndex++) {
                Transform roomT = InstantiateRoom(slotIndex, floorManager, 
                                                  floor.rooms[childIndex]);
                if (roomT != null) {
                    RoomManager roomManager = roomT.GetComponent<RoomManager>();
                    if (roomManager) {
                        roomManager.floorIndex = floorIndex;
                        roomManager.roomIndex = slotIndex;
                        floorManager.roomManagers[slotIndex] = roomManager;
                    }
                }
                childIndex++;
            }
        }
        // Initialize residents
        foreach (Resident resident in dorm.residents) {
            Transform residentObj = Instantiate(residentPrefab).transform;
            Transform room = null;
            if (resident.floorIndex >= 0) {
                room = floorManagers[resident.floorIndex]
                    .roomManagers[resident.roomIndex].transform;
            } else if (resident.floorIndex == -1) {
                room = baseFloorManager.roomManagers[resident.roomIndex].transform;
            }
            if (room != null) {
                residentObj.position = room.position;
            } else {
                residentObj.position = outside.position;
            }
            residentObj.GetComponent<ResidentController>().resident = resident;
        }
	}

    // Update is called once per frame
    void Update () {
		
	}

    internal void InstantiateResident(Resident resident) {
        GameObject residentObj = Instantiate(residentPrefab);
        ResidentController controller = residentObj.GetComponent<ResidentController>();
        controller.resident = resident;
        controller.UpdateSex();

        residentObj.transform.position = outside.position;
    }

    /// <summary>
    /// Adds a floor at the top of the stack of floors, just below the roof.
    /// This function also adds the floor to the floors array and 
    /// reattaches the navigation nodes.
    /// </summary>
    /// <returns>A transform of the created floor object.</returns>
    public Transform InstantiateFloor() {
        Transform result = Instantiate(floorPrefab, this.transform).transform;
        FloorManager floorManager = result.GetComponent<FloorManager>();
        floorManager.Initialize();

        result.position = Vector3.zero + Vector3.up * (2 + 2 * floorManagers.Count);
        roof.transform.position += Vector3.up * 2;
        NavNode roofNode = roof.GetComponent<NavigationNode>().Node;

        // Now fix the node graphs
        NavNode prevFloorNode;
        if (floorManagers.Count > 0) {
            prevFloorNode = floorManagers[floorManagers.Count - 1].stairs
                .GetComponent<NavigationNode>().Node;
        } else {
            prevFloorNode = dormBase.Find("Door").GetComponent<NavigationNode>().Node;
        }
        prevFloorNode.RemoveNeighbour(roofNode);
        NavNode floorNode = result.Find("Stairs").GetComponent<NavigationNode>().Node;
        roofNode.neighbours.Clear();
        prevFloorNode.AddNeighbour(floorNode);
        floorNode.AddNeighbour(roofNode);
        
        // Add our floor to our list of floors
        floorManagers.Add(floorManager);
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

                target.GetComponentInChildren<Button>().onClick.AddListener(()=>{
                    GameObject[] targets2 = GameObject.FindGameObjectsWithTag("RoomSlot");
                    foreach(GameObject go in targets2) {
                        Transform t = go.transform;
                        t.GetChild(0).gameObject.SetActive(false);
                    }
                });
            }
        } else {
            AddFloor();
        }
    }

    /// <summary>
    /// Instantiates a room in the scene.
    /// </summary>
    /// <param name="slot">The slot where the room is build</param>
    /// <param name="floor">The foor on which the room is build</param>
    /// <param name="type">The type of room</param>
    public Transform InstantiateRoom(int slotIndex, FloorManager floor, RoomType type) {
        Transform resultRoom = null;
        switch (type) {
            case RoomType.Bedroom: {
                resultRoom = Instantiate(bedroomPrefab, floor.transform).transform;
            } break;

            case RoomType.Bathroom: {
                resultRoom = Instantiate(bathroomPrefab, floor.transform).transform;
            } break;

            case RoomType.Loundryroom: {
                resultRoom = Instantiate(loundryroomPrefab, floor.transform).transform;
            } break;

            case RoomType.None: return null;
        }

        // Get the old slot, kill that, and set it to the spawned room.
        Transform slot = floor.roomSlots[slotIndex];
        floor.roomSlots[slotIndex] = resultRoom;
        resultRoom.position = slot.position;

        // Setup navigation
        NavNode navNode = floor.transform.Find("Stairs")
            .GetComponent<NavigationNode>().Node;
        navNode.AddNeighbour(
            resultRoom.GetComponent<NavigationNode>().Node);

        Destroy(slot.gameObject);

        return resultRoom;
    }

    /// <summary>
    /// Adds a new room to the dorm and instantiates a room object in the slot. The 
    /// room type is based on the lastSelectedRoomType
    /// </summary>
    /// <param name="roomIndex">The index of the slot where the room is build</param>
    /// <param name="slot">The slot in which the room is build</param>
    /// <param name="floor">The type of room</param>
    public void AddRoom(int roomIndex, Transform slot, FloorManager floor) {
        int floorIndex = floorManagers.IndexOf(floor);
        dorm.floors[floorIndex].rooms[roomIndex] = lastSelectedRoomType;
        InstantiateRoom(roomIndex, floor, lastSelectedRoomType);
    }
}

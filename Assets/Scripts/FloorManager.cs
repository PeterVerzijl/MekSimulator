using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorManager : MonoBehaviour {

    public NavigationNode stairs;

    public Transform[] roomSlots = new Transform[3];
    private Button[] roomSlotButtons = new Button[3];
    public RoomManager[] roomManagers = new RoomManager[3];

    public Transform smallRoomSlot;

    public void Initialize() {
        DormManager dormManager = FindObjectOfType<DormManager>();
        for (int slotIndex = 0; slotIndex < roomSlots.Length; slotIndex++) {
            Transform slot = roomSlots[slotIndex];
            // NOTE(Unity): Why?! Unity WHY?! Why the hell would GetComponentInChildren 
            // not look through inactive children and do you ask me to do this bullshit?!
            Button slotButton = slot.GetChild(0).GetChild(0).GetComponent<Button>();
            roomSlotButtons[slotIndex] = slotButton;     
            
            int slotIndexCopy = slotIndex;           
            slotButton.onClick.AddListener(()=> {
                dormManager.AddRoom(slotIndexCopy, slot, this);
            });
        }
    }

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

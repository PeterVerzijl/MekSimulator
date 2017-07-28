using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ResidentEnterEvent : UnityEvent<Resident> { }

public class RoomManager : MonoBehaviour {

    public bool lights = false;
    private new SpriteRenderer renderer;

    public int floorIndex;
    public int roomIndex;

    public Activity activity;

    [SerializeField]
    public ResidentEnterEvent residentEnterEvent;

    public List<ResidentController> residentsInRoom = new List<ResidentController>();

	// Use this for initialization
	void Start () {
		renderer = GetComponent<SpriteRenderer>();
        SwitchLights(false);

        if (residentEnterEvent == null) {
            residentEnterEvent = new ResidentEnterEvent();
        }
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void SwitchLights(bool turnOn) {
        lights = turnOn;
        float darkning = (lights)? 1.0f : 0.5f;
        renderer.color = new Color(darkning, darkning, darkning, 1.0f);
    }

    public void AddResident(ResidentController controller) {
        if (activity) {
            controller.SetActivity(activity);
        }

        residentEnterEvent.Invoke(controller.resident);
        controller.resident.floorIndex = floorIndex;
        controller.resident.roomIndex = roomIndex;

        if (!residentsInRoom.Contains(controller)) {
            residentsInRoom.Add(controller);
        }

        if (residentsInRoom.Count == 1) {
            SwitchLights(true);
        }
    }

    public void RemoveResident(ResidentController resident) {
        residentsInRoom.Remove(resident);
        if (residentsInRoom.Count == 0) {
            SwitchLights(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.tag == Tags.RESIDENT) {
            ResidentController residentController = 
                collision.GetComponent<ResidentController>();
            if (residentController) {
                RemoveResident(residentController);
                residentController.RemoveCurrentActivity();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == Tags.RESIDENT) {
            ResidentController residentController = 
                collision.GetComponent<ResidentController>();
            if (residentController) {
                AddResident(residentController);
            }
        }
    }
}

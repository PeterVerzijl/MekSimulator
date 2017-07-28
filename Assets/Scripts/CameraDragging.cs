using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraDragging : MonoBehaviour {

    public Transform dorm;
    public Transform roof;
    public float scrollSpeed = 2.0f;

    bool isPanning = false;

    public GameObject roomSelectionIndicatorPrefab;
    SpriteRenderer roomSelectionIndicator;

    public GameObject draggableCharacterPrefab;
    GameObject draggableResident;
    Collider2D draggableResidentCollider;
    Transform draggingResident;

    public ResidentPanelController residentPanel;

    LayerMask residentMask;
    Vector3 prevMousePos;

    DormManager dormManager;

    Collider2D closestRoomCollider = null;

    // Touch related stuff
    bool hasTouchMoved = false;

	// Use this for initialization
	void Start () {
        prevMousePos = Input.mousePosition;
        residentMask = 1 << Layers.RESIDENT_INT;

        dormManager = FindObjectOfType<DormManager>();
	}

	// Update is called once per frame
	void Update () {
        bool onGUI = EventSystem.current.IsPointerOverGameObject();

        if (!onGUI && Input.touchCount == 1) {
            Touch touch = Input.GetTouch(0);
            
            switch (touch.phase) {
                case TouchPhase.Began: {
                    hasTouchMoved = false;
                    isPanning = false;
                } break;

                case TouchPhase.Moved: {
                    if (hasTouchMoved == false) {
                        Transform residentTransform;
                        if (IsResidentBelowTouch(out residentTransform)) {
                            draggingResident = residentTransform;
                            draggableResident = Instantiate(draggableCharacterPrefab);
                            draggableResidentCollider = draggableResident.GetComponent<BoxCollider2D>();
                            roomSelectionIndicator = Instantiate(roomSelectionIndicatorPrefab)
                                .GetComponent<SpriteRenderer>();
                        } else {
                            prevMousePos = Input.mousePosition;
                            isPanning = true;
                        }
                    }
                    hasTouchMoved = true;
                    // Do dragging stuff here

                    if (!onGUI && isPanning) {
                        // Check if we are not pressing on something else
                        Vector3 mouseDelta = Input.mousePosition - prevMousePos;
                        Vector3 newPos = transform.position + Vector3.up * -mouseDelta.y * 
                            scrollSpeed * Time.deltaTime;
                        newPos.y = Mathf.Clamp(newPos.y, dorm.position.y, roof.position.y);
                        transform.position = newPos;

                        prevMousePos = Input.mousePosition;
                    } else if (draggingResident != null) {
                        Vector2 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        draggableResident.transform.position = mousePoint - 0.5f * Vector2.up;

                        // Get closest room collider
                        Collider2D newClosestRoomCollider = GetClosestRoomCollider(draggableResidentCollider);
                        if (newClosestRoomCollider != null && newClosestRoomCollider != closestRoomCollider) {
                            closestRoomCollider = newClosestRoomCollider;
                            Vector3 colliderBoundsSize = closestRoomCollider.bounds.size;
                            roomSelectionIndicator.size = colliderBoundsSize * 1.1f;
                            roomSelectionIndicator.transform.position = 
                                closestRoomCollider.transform.position
                                + Vector3.up * closestRoomCollider.bounds.size.y/2;
                        }
                    }
                } break;

                case TouchPhase.Ended: {
                    if (!hasTouchMoved) {
                        Transform residentTransform;
                        if (IsResidentBelowTouch(out residentTransform)) {
                            ResidentController residentController = 
                                residentTransform.GetComponent<ResidentController>();
                            if (residentController != null) {
                                residentPanel.gameObject.SetActive(true);
                                residentPanel.UpdateResident(residentController.resident);
                            }
                        }
                    }
                    if (draggableResident) {
                        // Now check for target room

                        NavigationNode navNode = closestRoomCollider.GetComponent<NavigationNode>();
                        NavNode targetNode = navNode.Node;
                
                        // Set target
                        draggingResident.GetComponent<ResidentController>().SetTarget(targetNode);
            
                        Destroy(draggableResident.gameObject);
                        draggingResident = null;
                        draggableResident = null;
                        Destroy(roomSelectionIndicator.gameObject);
                    }
                } break;
            }
        } 

        if (!onGUI && Input.GetMouseButtonDown(0)) {
            
        }
        if (!onGUI && Input.GetMouseButtonUp(0) && draggableResident) {
            
        }
        if (!onGUI && Input.GetMouseButton(0)) {
            
        }
	}

    private Collider2D GetClosestRoomCollider(Collider2D draggableCollider) {
        Collider2D result = null;

        // Find the room on which we are floting.
        Collider2D[] contacts = new Collider2D[32];
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;
        filter.SetLayerMask((1 << Layers.NAV_TARGET_INT));
        int collidingCount = draggableCollider.OverlapCollider(filter, contacts);

        // Find closest if more than one
        if (collidingCount > 1) {
            float closestDistance = float.MaxValue;
            for(int colliderIndex = 0; colliderIndex < collidingCount; colliderIndex++) {
                Collider2D targetCollider = contacts[colliderIndex];
                ColliderDistance2D colliderDistance = targetCollider.Distance(draggableCollider);
                if (colliderDistance.distance < closestDistance) {
                    closestDistance = colliderDistance.distance;
                    result = targetCollider;
                }
            }
        } else {
            result = contacts[0];
        }
        return result;
    }

    private bool IsResidentBelowTouch(out Transform resident) {
        bool foundResident = false;
        resident = null;

        Vector2 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePoint, Vector2.zero, 0, residentMask);
        if(hits.Length > 0) {
            resident = hits[0].transform;
            foundResident = true;
        }
        return foundResident;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDragging : MonoBehaviour {

    public Transform dorm;
    public Transform roof;
    public float scrollSpeed = 2.0f;


    Vector3 prevMousePos;

	// Use this for initialization
	void Start () {
        prevMousePos = Input.mousePosition;
	}
	
	// Update is called once per frame
	void Update () {
		
        if (Input.GetMouseButtonDown(0)) {
            prevMousePos = Input.mousePosition;
        }
        if (Input.GetMouseButton(0)) {
            // Check if we are not pressing on something else
            Vector3 mouseDelta = Input.mousePosition - prevMousePos;
            Vector3 newPos = transform.position + Vector3.up * -mouseDelta.y * 
                scrollSpeed * Time.deltaTime;
            newPos.y = Mathf.Clamp(newPos.y, dorm.position.y, roof.position.y);
            transform.position = newPos;

            prevMousePos = Input.mousePosition;
        }
	}
}

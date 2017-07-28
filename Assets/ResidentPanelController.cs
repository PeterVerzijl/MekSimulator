using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ResidentPanelController : MonoBehaviour {

    public Text nameField;
    public Text happyField;
    public Text sleepField;
    public Slider[] statSliders = new Slider[6];

    public Resident resident;

	// Use this for initialization
	void Start () {
		if (resident != null) {
            UpdateResident(resident);
        }
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
            bool onUI = EventSystem.current.IsPointerOverGameObject();
            if (!onUI || EventSystem.current.currentSelectedGameObject != this.gameObject) {
                gameObject.SetActive(false);
            }
        }
	}

    public void UpdateResident(Resident resident) {
        this.resident = resident;

        nameField.text = resident.name;
        happyField.text = "Happyness: " + resident.happyness + "%";
        sleepField.text = "Sleepyness: " + (100 - resident.sleep) + "%";
        
        statSliders[0].value = resident.stats.intelligence;
        statSliders[1].value = resident.stats.social;
        statSliders[2].value = resident.stats.ego;
        statSliders[3].value = resident.stats.extrovert;
        statSliders[4].value = resident.stats.introvert;
        statSliders[5].value = resident.stats.venturousness;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    public GameObject mainMenu;
    public GameObject levelMenu;
    public GameObject settingsMenu;

	// Use this for initialization
	void Start () {
		SetMenu(mainMenu);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetMenu(GameObject menu) {
        mainMenu.SetActive(false);
        levelMenu.SetActive(false);
        settingsMenu.SetActive(false);

        menu.SetActive(true);
    }

    public void Exit() {
        // Exit based
    }
}

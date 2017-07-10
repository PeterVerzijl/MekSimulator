using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleBuyMenu : MonoBehaviour {

    public bool confirmActive = false;
    public GameObject[] buyLayout;
    public GameObject confirmLayout;

    void Start() {
        foreach (GameObject item in buyLayout)
            item.SetActive(!confirmActive);
        confirmLayout.SetActive(confirmActive);
    }

    public void Toggle() {
        confirmActive = !confirmActive;

        foreach (GameObject item in buyLayout)
            item.SetActive(!confirmActive);
        confirmLayout.SetActive(confirmActive);
    }
}

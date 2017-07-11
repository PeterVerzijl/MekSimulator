using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBuyButton : MonoBehaviour {

    public bool confirmActive = false;
    public GameObject[] buyLayout;
    public GameObject confirmLayout;

    public HouseManager.RoomType type;
    public Button buyButton;
    public HouseManager houseManager;

    void Start() {
        SetActive(confirmActive);

        GetComponent<Button>().onClick.AddListener(()=> {
            SetActive(true);
        });
        buyButton.onClick.AddListener(()=> {
            houseManager.StartSelectRoomTarget(type);
        });
    }

    private void OnEnable() {
        SetActive(false);
    }

    public void Toggle() {
        SetActive(!confirmActive);
    }

    public void SetActive(bool active) {
        confirmActive = active;
        foreach (GameObject item in buyLayout)
            item.SetActive(!active);
        confirmLayout.SetActive(active);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBuyButton : MonoBehaviour {

    public bool confirmActive = false;
    public GameObject[] buyLayout;
    public GameObject confirmLayout;

    public RoomType type;
    public DormManager houseManager;

    public static List<UIBuyButton> buyButtons = new List<UIBuyButton>();

    void Start() {
        if (!buyButtons.Contains(this)) {
            buyButtons.Add(this);
        }

        SetActive(confirmActive);

        Button buildButton = transform.Find("Buy Container/Button (Buy)")
            .GetComponent<Button>();

        GetComponent<Button>().onClick.AddListener(()=> {
            foreach(UIBuyButton button in buyButtons) {
                button.SetActive(false);
            }
            SetActive(true);
        });

        buildButton.onClick.AddListener(()=> {
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

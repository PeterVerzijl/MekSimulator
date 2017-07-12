using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class LevelLoader : MonoBehaviour {

    public GameObject houseButtonPrefab;
    public Button addDormButton;
    public Button[] dormButtons;

    [Header("Input Panel")]
    public GameObject dormNameInputPanel;
    public Button nameSubmit;

    // Use this for initialization
    void Start () {
        // Load stuff
        Dorm d = CreateDorm("B. S. V. Moedt Ende Kraght");
        GameObject houseButton = Instantiate(houseButtonPrefab, this.transform);
        houseButton.GetComponentInChildren<Text>().text =  d.name;    
        
        dormButtons = new Button[1];
        dormButtons[0] = houseButton.GetComponent<Button>();
        foreach (Button dormButton in dormButtons) {
            dormButton.onClick.AddListener(()=> {
                LoadDorm(d);
            });
        }
	}

    private void OnEnable() {
        dormNameInputPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void LoadDorm(Dorm dorm) {
        GameManager.Instance.dorm = dorm;
        SceneManager.LoadScene("main", LoadSceneMode.Single);
    }

    /// <summary>
    /// Loads a saved dorm by name in the ..... settings??.
    /// </summary>
    /// <param name="name">The name of the dorm.</param>
    /// <returns></returns>
    public static Dorm LoadSavedDorm(string name) {
        Dorm result = new Dorm("B. S. V. Moedt Ende Kraght");
        RoomType[] floor1 = new RoomType[3];
        floor1[0] = RoomType.Bedroom;
        result.floors.Add(floor1);

        return result;
    }

    public void StartNewDormSequence() {
        dormNameInputPanel.SetActive(true);
        InputField textField = dormNameInputPanel.GetComponentInChildren<InputField>();
        nameSubmit.onClick.AddListener(()=>{
            Dorm d = CreateDorm(textField.text);
            SaveDorm(d);
            LoadDorm(d);
        });
    }

    private void SaveDorm(Dorm d) {
        // Save the dorm somewhrere!
    }

    /// <summary>
    /// Creates and saves a new dorm based on the given name.
    /// </summary>
    /// <param name="name"></param>
    public Dorm CreateDorm(string name) {
        Dorm result = new Dorm(name);
        return result;
    }


}

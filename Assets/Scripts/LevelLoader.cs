using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {

    public GameObject houseButtonPrefab;
    public Button addDormButton;
    public List<Button> dormButtons = new List<Button>();

    public Button deleteDormButton;

    [Header("Input Panel")]
    public GameObject dormNameInputPanel;
    public Button nameSubmit;

    static Regex filenameRegex = new Regex(@"[^a-zA-Z0-9]");

    Dorm[] dorms;

    // Use this for initialization
    void Start () {
        string levelFolder = Application.persistentDataPath + @"/dorms/";

        // Load all dorm files
        if (!Directory.Exists(levelFolder)) {
            Directory.CreateDirectory(levelFolder);
        }
        string[] files = Directory.GetFiles(levelFolder, @"*.lvl", 
                                            SearchOption.TopDirectoryOnly);
        int dormIndex = 0;
        dorms = new Dorm[files.Length];
        foreach (string filePath in files) {
            Dorm dorm = JsonUtility.FromJson<Dorm>(File.ReadAllText(filePath));
            dorms[dormIndex++] = dorm;
            GameObject dormButton = Instantiate(houseButtonPrefab, this.transform);
            dormButton.GetComponentInChildren<Text>().text = dorm.name;
            dormButton.GetComponent<Button>().onClick.AddListener(()=> {
                if (!isInDeletionMode) {
                    LoadDorm(dorm);
                } else {
                    DeleteDorm(dorm, dormButton);
                }
            });
            dormButtons.Add(dormButton.GetComponent<Button>());
        }

        // Attach button to function
        deleteDormButton.onClick.AddListener(ToggleDeletionSequence);
	}

    private void OnEnable() {
        dormNameInputPanel.SetActive(false);
        isInDeletionMode = false;
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void LoadDorm(Dorm dorm) {
        GameManager.Instance.Dorm = dorm;
        SceneManager.LoadScene("main", LoadSceneMode.Single);
    }

    /// <summary>
    /// Loads a saved dorm by name in the ..... settings??.
    /// </summary>
    /// <param name="name">The name of the dorm.</param>
    /// <returns></returns>
    public static Dorm LoadSavedDorm(string filename) {
        if (!filename.Contains(".lvl")) { filename += ".lvl"; }
        string fileContents = File.ReadAllText(
            Application.persistentDataPath + @"/dorms/" + filename);
        Dorm result = JsonUtility.FromJson<Dorm>(fileContents);
        return result;
    }

    public void StartNewDormSequence() {
        dormNameInputPanel.SetActive(true);
        InputField textField = dormNameInputPanel.GetComponentInChildren<InputField>();
        nameSubmit.onClick.AddListener(()=>{
            if (!String.IsNullOrEmpty(textField.text)) {
                Dorm d = CreateAndSaveDorm(textField.text);
                if (d != null) {
                    LoadDorm(d);
                } else {
                    // TODO(Peter): Make this more robust, name was already taken!!
                }
            } else {
                // TODO(Peter): Make this more robust!
            }
        });
    }

    /// <summary>
    /// Saves and possibly overwrites the new dorm object.
    /// </summary>
    /// <param name="dorm">The dorm to save.</param>
    public static void SaveDorm(Dorm dorm) {
        string filename = filenameRegex.Replace(dorm.name, "");
        string filePath = Application.persistentDataPath + 
            @"/dorms/" + filename + ".lvl";
        string serializedObject = JsonUtility.ToJson(dorm);
        if (File.Exists(filePath)) {
            File.WriteAllText(filePath, string.Empty);
        }
        StreamWriter outputFile = new StreamWriter(filePath);
        outputFile.Write(serializedObject);
        outputFile.Close();
    }

    /// <summary>
    /// Creates and saves a new dorm based on the given name.
    /// </summary>
    /// <param name="name"></param>
    public Dorm CreateAndSaveDorm(string name) {
        Dorm result = new Dorm(name);

        string filename = filenameRegex.Replace(name, "");
        string filePath = Application.persistentDataPath + 
            @"/dorms/" + filename + ".lvl";
        if (!File.Exists(filePath)) {
            SaveDorm(result);
        }

        return result;
    }

    private bool isInDeletionMode;
    private ColorBlock oldColors;
    public void ToggleDeletionSequence() {
        isInDeletionMode = !isInDeletionMode;
        if (isInDeletionMode) {
            oldColors = dormButtons[0].colors;
            foreach(Button dormButton in dormButtons) {
                ColorBlock tempColors = oldColors;
                tempColors.normalColor = Color.red;
                dormButton.colors = tempColors;
            }
        } else {
            foreach(Button dormButton in dormButtons) {
                dormButton.colors = oldColors;
            }
        }
    }

    public void DeleteDorm(Dorm dorm, GameObject button) {
        // Find file with the same name.
        string filename = filenameRegex.Replace(dorm.name, "");
        string filePath = Application.persistentDataPath + 
            @"/dorms/" + filename + ".lvl";
        if (File.Exists(filePath)) {
            File.Delete(filePath);
            dormButtons.Remove(button.GetComponent<Button>());
            Destroy(button);
        } else {
            // TODO(Peter): This should NEVER EVER happen.
        }
        ToggleDeletionSequence();
    }
}

using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {

    public GameObject houseButtonPrefab;
    public Button addDormButton;
    public Button[] dormButtons;

    [Header("Input Panel")]
    public GameObject dormNameInputPanel;
    public Button nameSubmit;

    static Regex filenameRegex = new Regex(@"[^a-zA-Z0-9]");
    static string levelFolder;

    Dorm[] dorms;

    // Use this for initialization
    void Start () {
        levelFolder = Application.persistentDataPath + @"\dorms\";

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
            GameObject houseButton = Instantiate(houseButtonPrefab, this.transform);
            houseButton.GetComponentInChildren<Text>().text = dorm.name;
            houseButton.GetComponent<Button>().onClick.AddListener(()=> {
                LoadDorm(dorm);
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
        string filePath = levelFolder + filename + ".lvl";
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
        string filePath = levelFolder + filename + ".lvl";
        if (!File.Exists(filePath)) {
            SaveDorm(result);
        }

        return result;
    }


}

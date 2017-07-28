using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    /// <summary>
    /// Singleton for the GameManager class.
    /// </summary>
    private static GameManager instance;
    public static GameManager Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<GameManager>();
                if (instance == null) {
                    GameObject navigationManager = 
                        new GameObject("Game Manager", typeof(GameManager));
                    instance = navigationManager
                        .GetComponent<GameManager>();
                    DontDestroyOnLoad(instance);
                }
            }
            return instance;
        }
    }
    
    private DateTime epoch = new DateTime(1970, 1, 1);

    [SerializeField]
    private Dorm dorm = null;
    public Dorm Dorm {
        get {
            if (dorm == null) {
                dorm = LevelLoader.LoadSavedDorm("BSVMoedtEndeKragt");
            }
            return dorm;
        }
        set { dorm = value; }
    }

    public double CurrentTime {
        get { return DateTime.Now.Subtract(epoch).TotalSeconds * simulationDaysInRealDays; }
    }

    [Header("Day Simulation")]
    public int simulationDaysInRealDays = 24 * 8/*24*/;
    internal static DateTime date;

    // Use this for initialization
    void Start () {
        DormManager dormManager = FindObjectOfType<DormManager>();
        // Check if the dorm has no residents, if so generate two.
        if (Dorm.residents.Count == 0) {
            for (int i = 0; i < 2; i++) {
                Sex sex = (UnityEngine.Random.Range(0.0f, 1.0f) > 0.7f) ? Sex.Male : Sex.Female;
                Resident resident = new Resident(RandomNameGenerator.Generate(sex), 
                                                 sex, new DateTime(1993, 5, 30));
                Dorm.residents.Add(resident);
                dormManager.InstantiateResident(resident);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    void OnApplicationPause(bool pause) {
        // If the applicaiton is paused, we should probably save the game.
        if (pause) {
            LevelLoader.SaveDorm(Dorm);
        }
    }

    void OnApplicationQuit() {
        // Save the game!!
        LevelLoader.SaveDorm(Dorm);
    }
}

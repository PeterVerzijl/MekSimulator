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
    
    private Dorm dorm;
    public Dorm Dorm {
        get {
            if (dorm == null) {
                dorm = LevelLoader.LoadSavedDorm("BSVMoedtEndeKragt");
            }
            return dorm;
        }
        set { dorm = value; }
    }

    internal static DateTime date;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (Dorm == null) {
        }		
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
        Debug.Log("Application was exited.");   
    }
}

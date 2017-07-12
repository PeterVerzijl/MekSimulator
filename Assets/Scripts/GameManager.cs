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
    
    public Dorm dorm;

    internal static DateTime date;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaylightTimeManager : MonoBehaviour {
    
    double secondsToHours = 0.000277778;
    public double hour = 0.0f;

    public Gradient daylightGradient;
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        DateTime now = DateTime.Now;
        double totalSeconds = now.Subtract(new DateTime(now.Year, now.Month, now.Day, 0, 0, 0)).TotalSeconds;
        double hoursPassed = totalSeconds * secondsToHours;
	    hour = (hoursPassed * GameManager.Instance.simulationDaysInRealDays) % 24;
        float fraction = (float)hour / 24.0f;
        RenderSettings.ambientLight = daylightGradient.Evaluate(fraction);	
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHeaderManager : MonoBehaviour {

    string happynessFormat = "{0}% Happy";
    public Text happynessText;

    string feutFormat = "{0} FP";
    public Text feutText;

    string studyFormat = "{0} ECTS";
    public Text studyText;

    string pandaFormat = "{0} PP";
    public Text pandaText;

    string nestorFormat = "{0} NP";
    public Text nestorText;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		happynessText.text = string.Format(happynessFormat, 0);
		feutText.text = string.Format(feutFormat, 0);
		studyText.text = string.Format(studyFormat, 0);
		pandaText.text = string.Format(pandaFormat, 0);
		nestorText.text = string.Format(nestorFormat, 0);
	}
}

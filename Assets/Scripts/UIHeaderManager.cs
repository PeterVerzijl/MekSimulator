using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHeaderManager : MonoBehaviour {

    public Text dormNameText;

    [Header("Stats")]
    public string happynessFormat = "{0}% Happy";
    public Text happynessText;

    public string feutFormat = "{0} FP";
    public Text feutText;

    public string studyFormat = "{0} ECTS";
    public Text studyText;

    public string pandaFormat = "{0} PP";
    public Text pandaText;

    public string nestorFormat = "{0} NP";
    public Text nestorText;

	// Use this for initialization
	void Start () {
        dormNameText.text = GameManager.Instance.dorm.name;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewManager : MonoBehaviour {

    public GameObject activityPanelPrefab;
    public RectTransform container;

	// Use this for initialization
	void Start () {
		Activity[] activities = Resources.LoadAll<Activity>("Activities");
        foreach(Activity activity in activities) {
            Transform panel = Instantiate(activityPanelPrefab, container).transform;
            panel.Find("Name Text").GetComponent<Text>().text = activity.header;
            panel.Find("Image").GetComponent<Image>().sprite = activity.icon;
            panel.Find("Description Text").GetComponent<Text>().text = activity.description;

            string buffer = "";
            for(int statIndex = 0; statIndex < activity.changingAtributes.Length; statIndex++) {
                StatChange statChange = activity.changingAtributes[statIndex];
                if (statChange.changeAmount > 0) {
                    buffer += "+" + statChange.type.ToString();
                } else if (statChange.changeAmount < 0) {
                    buffer += "-" + statChange.type.ToString();
                }
                if (statIndex < activity.changingAtributes.Length - 1) { buffer += " "; }
            }
            panel.Find("Cost Text").GetComponent<Text>().text = buffer;
        }
    }


}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Activity", menuName = "Create Activity", order = 1)]
public class Activity : ScriptableObject {
    public string header;
    public Sprite icon;
    [TextArea] public string description;
    public StatChange[] changingAtributes;
}

[System.Serializable]
public struct StatChange {
    public ResidentAttributeType type;
    public float hoursToChange;
    public int changeAmount;
}
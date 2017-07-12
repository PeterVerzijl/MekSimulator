using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType {
    None,
    Bedroom,
    Bathroom,
    Loundryroom,
    LivingRoom,
    Floor,
}

[System.Serializable]
public class Dorm {
    /// <summary>
    /// Dorm name
    /// </summary>
    [SerializeField]
    public string name;

    /// <summary>
    /// This contains a list of floors with each three room types.
    /// </summary>
    [SerializeField]
    public List<RoomType[]> floors;

    /// <summary>
    /// List of residents who live in the dorm.
    /// </summary>
    [SerializeField]
    public List<Resident> residents;

    public Dorm(string name) {
        this.name = name;
        floors = new List<RoomType[]>();
        residents = new List<Resident>();
    }
}
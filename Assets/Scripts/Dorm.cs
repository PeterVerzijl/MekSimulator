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

public enum SmallRoomType {
    None,
    Storage,
    Toilet,
    ServerRoom
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
    public List<Floor> floors = new List<Floor>();

    /// <summary>
    /// List of residents who live in the dorm.
    /// </summary>
    [SerializeField]
    public List<Resident> residents = new List<Resident>();

    public Dorm(string name) {
        this.name = name;
    }
}

[System.Serializable]
public class Floor {
    public RoomType[] rooms = new RoomType[3];
    public SmallRoomType sideRoom;
}
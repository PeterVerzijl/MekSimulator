using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[System.Serializable]
public class Resident : ScriptableObject {

    [System.Serializable]
    public enum Sex {
        Male,
        Female,
        Transexual,
        Other,
        Length,
    }

    public string name;
    public Sex sex;
    public List<Sex> interrest = new List<Sex>();

    private int age;
    public int Age {
        get {
            return GetBirthdate();
        }
        private set {
            age = value;
        }
    }

    public DateTime birthdate { get; private set; }
    
    public Resident(string name, Sex sex, DateTime birthdate) {
        this.name = name;
        this.sex = sex;
        this.birthdate = birthdate;
        interrest.Add(GetOppositeSex(sex));
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Resident")]
    public static Resident  Create()
    {
        Resident asset = CreateInstance<Resident>();
        AssetDatabase.CreateAsset(asset, AssetDatabase.GetAssetPath(Selection.activeObject) + "/Resident.asset");
        AssetDatabase.SaveAssets();
        return asset;
    }
#endif

    public Sex GetOppositeSex(Sex sex) {
        if (sex == Sex.Male) return Sex.Female;
        if (sex == Sex.Female) return Sex.Male;
        return sex;
    }

	public int GetBirthdate() {
        int nowYear = GameManager.date.Year;
        int birthYear = birthdate.Year;
        int result = nowYear - birthYear;
        if (GameManager.date.Month < birthdate.Month) {
            result -= 1;
        }
        return result;
    }
}

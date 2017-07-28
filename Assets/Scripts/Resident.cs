using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[System.Serializable]
public enum Sex {
    Male,
    Female,
}

[System.Serializable]
public enum ResidentAttributeType {
    Intelligence,
    Sociality,
    Ego,
    Extraversion,
    Introversion,
    Venturousness,
}

[System.Serializable]
public class ResidentStats {
    // NOTE(All): These should range between 0 and 7.
    [SerializeField] public int intelligence;
    [SerializeField] public int social;
    [SerializeField] public int ego;
    [SerializeField] public int extrovert;
    [SerializeField] public int introvert;
    [SerializeField] public int venturousness;

    /// <summary>
    /// Gets a new set of random stats between 0 and 7.
    /// </summary>
    /// <returns>The random stats.</returns>
    public static ResidentStats GetRandomStats() {
        int totalSum = 14;
        ResidentStats result = new ResidentStats();
        int[] randomNumbers = new int[6];
        float sum = 0;
        for (int i = 0; i < randomNumbers.Length; i++) {
            randomNumbers[i] = UnityEngine.Random.Range(0, 7);
            sum += randomNumbers[i];
        }

        result.intelligence  = (int)((randomNumbers[0] / sum) * totalSum);
        result.social        = (int)((randomNumbers[1] / sum) * totalSum);
        result.ego           = (int)((randomNumbers[2] / sum) * totalSum);
        result.extrovert     = (int)((randomNumbers[3] / sum) * totalSum);
        result.introvert     = (int)((randomNumbers[4] / sum) * totalSum);
        result.venturousness = (int)((randomNumbers[5] / sum) * totalSum);

        return result;
    }
}

[System.Serializable]
public class Resident {

    public string name;
    public Sex sex;
    public List<Sex> interrest = new List<Sex>();
    public int age;
    public System.DateTime birthdate { get; private set; }

    public float sleep = 75.0f;
    public float happyness = 50.0f;
    public ResidentStats stats;
    
    public int floorIndex;
    public int roomIndex;

    public Resident(string name, Sex sex, System.DateTime birthdate) {
        this.name = name;
        this.sex = sex;
        this.birthdate = birthdate;
        interrest.Add(GetOppositeSex(sex));

        stats = ResidentStats.GetRandomStats();
    }

    public static Sex GetOppositeSex(Sex sex) {
        if (sex == Sex.Male) return Sex.Female;
        if (sex == Sex.Female) return Sex.Male;
        return sex;
    }

	public int GetBirthday() {
        int nowYear = GameManager.date.Year;
        int birthYear = birthdate.Year;
        int result = nowYear - birthYear;
        if (GameManager.date.Month < birthdate.Month) {
            result -= 1;
        }
        return result;
    }

    internal void UpdateResidentStats(StatChange stateChange) {
        switch (stateChange.type) {
        
        case ResidentAttributeType.Ego:
                stats.ego += stateChange.changeAmount;
                break;
            case ResidentAttributeType.Extraversion:
                stats.extrovert += stateChange.changeAmount;
                break;
            case ResidentAttributeType.Intelligence:
                stats.intelligence += stateChange.changeAmount;
                break;
            case ResidentAttributeType.Introversion:
                stats.introvert += stateChange.changeAmount;
                break;
            case ResidentAttributeType.Sociality:
                stats.social += stateChange.changeAmount;
                break;
            case ResidentAttributeType.Venturousness:
                stats.venturousness += stateChange.changeAmount;
                break;

            // NOTE(All): This case should NEVER happen. If it does, it means one of the
            // ResidentAttributeTypes does not have it's own case!!!!
            default: throw new Exception();
        }
    }
}

public class RandomNameGenerator {
    public static string[] maleNames = {
		"Donnie",
		"Franklyn",
		"Leonel",
		"Kip",
		"Tracey",
		"Denver",
		"Yong",
		"Glen",
		"Vance",
		"Murray",
		"Emmanuel",
		"Darrell",
		"Jan",
		"Elroy",
		"Rene",
		"Pat",
		"Harvey",
		"Refugio",
		"Kris",
		"Elvin",
		"Clair",
		"Scott",
		"Maximo",
		"Dusty",
		"Federico",
		"Arden",
		"Bart",
		"Quintin",
		"Herbert",
		"Lorenzo",
		"Octavio",
		"Deandre",
		"Scottie",
		"Jorge",
		"Carlton",
		"Kenton",
		"Nathaniel",
		"Rocky",
		"Santos",
		"Rex",
		"Monty",
		"Stanley",
		"Ezequiel",
		"Delbert",
		"Bryce",
		"Vito",
		"Jesus",
		"Cletus",
		"Keneth",
		"Carson",
    };
    public static string[] femaleNames = {
        "Agueda",
		"Althea",
		"Margot",
		"Kareen",
		"Sona",
		"Almeda",
		"Serena",
		"Teena",
		"Kenia",
		"Jeneva",
		"Evangeline",
		"Anjanette",
		"Odessa",
		"Renay",
		"Claudette",
		"Nell",
		"Adena",
		"Kendra",
		"Mana",
		"Waneta",
		"Roxanna",
		"Ashton",
		"Dorcas",
		"Janessa",
		"Elidia",
		"Isabelle",
		"Mina",
		"Caren",
		"Stephania",
		"Priscila",
		"Kathie",
		"Denise",
		"Gwendolyn",
		"Ressie",
		"Cleora",
		"Candelaria",
		"Zulma",
		"Cornelia",
		"Zenobia",
		"Song",
		"Dawne",
		"Mae",
		"Breann",
		"Chantell",
		"Vella",
		"Taisha",
		"Deborah",
		"Kristal",
		"Janita",
		"Kelsi",
    };

    public static string Generate(Sex sex) {
        if (sex == Sex.Male) {
            return maleNames[UnityEngine.Random.Range(0, maleNames.Length - 1)];
        } else if (sex == Sex.Female) {
            return femaleNames[UnityEngine.Random.Range(0, femaleNames.Length - 1)];
        } else {
            return (UnityEngine.Random.Range(0, 1) == 1) ? 
                maleNames[UnityEngine.Random.Range(0, maleNames.Length - 1)] :
                femaleNames[UnityEngine.Random.Range(0, femaleNames.Length - 1)];
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JSONCardArray
{
    public JSONCardData[] data;
}

[System.Serializable]
public class JSONCardData
{
    public string impl;
    public int set;
    public string name;
    public string color;
    public string type;
    public string rarity;
    public int level;

    // Affinity Data
    public int red;
    public int blue;
    public int green;
    public int gold;
    public int violet;
    public int indigo;

    // Stats
    public int stat1;
    public int stat2;
    public int stat3;

    // Damage Type
    public string dmg_type;

    // Keys
    public string key1;
    public string key2;
    public string key3;

    // Ability Keys
    public string ka1;
    public string ka2;
    public string ka3;

    // Text
    public string text;

    // Ability Values
    public int val1;
    public int val2;
    public int val3;
}

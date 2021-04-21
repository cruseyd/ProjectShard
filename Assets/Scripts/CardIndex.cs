using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class CardIndex : MonoBehaviour
{
    public static CardIndex instance;

    private static List<CardData> _allCardData;

    private static List<CardData> _redCardData;
    private static List<CardData> _blueCardData;
    private static List<CardData> _greenCardData;

    private static List<CardData> _allSpells;
    private static List<CardData> _allTechniques;
    private static List<CardData> _allThralls;

    private static List<CardData> _commonData;
    private static List<CardData> _scarceData;
    private static List<CardData> _rareData;
    private static List<CardData> _mythicData;

    private bool _loaded = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Load();
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static void Load()
    {
        if (instance._loaded) { return; }
        _allCardData = new List<CardData>();
        _redCardData = new List<CardData>();
        _blueCardData = new List<CardData>();
        _greenCardData = new List<CardData>();
        _allSpells = new List<CardData>();
        _allTechniques = new List<CardData>();
        _allThralls = new List<CardData>();
        _commonData = new List<CardData>();
        _scarceData = new List<CardData>();
        _rareData = new List<CardData>();
        _mythicData = new List<CardData>();

        //CardData[] cards = Resources.LoadAll<CardData>("Cards/Set_1");

        string path = Application.dataPath + "/Resources/Cards";
        string json = File.ReadAllText(path + "/cards.json");
        json = "{ \"data\":" + json + "}";

        JSONCardArray cardArray = JsonUtility.FromJson<JSONCardArray>(json);
        Array.Sort(cardArray.data, (JSONCardData d1, JSONCardData d2) => { return d1.name.CompareTo(d2.name); });
        List<CardData> cards = new List<CardData>();

        for (int ii = 0; ii < cardArray.data.Length; ii++)
        {
            if (cardArray.data[ii].impl == "X" && cardArray.data[ii].set <= 1)
            {
                cards.Add(new CardData(cardArray.data[ii]));
            } else
            {
                //Debug.Log(cardArray.data[ii].name + " not loaded (not implemented)");
            }
        }
        foreach (CardData data in cards)
        {
            _allCardData.Add(data);
            switch (data.color)
            {
                case Card.Color.RAIZ: _redCardData.Add(data); break;
                case Card.Color.IRI: _blueCardData.Add(data); break;
                case Card.Color.FEN: _greenCardData.Add(data); break;
                default: break;
            }
            switch (data.type)
            {
                case Card.Type.SPELL: _allSpells.Add(data); break;
                case Card.Type.TECHNIQUE: _allTechniques.Add(data); break;
                case Card.Type.THRALL: _allThralls.Add(data); break;
            }
            switch (data.rarity)
            {
                case Card.Rarity.COMMON: _commonData.Add(data); break;
                case Card.Rarity.SCARCE: _scarceData.Add(data); break;
                case Card.Rarity.RARE: _rareData.Add(data); break;
                case Card.Rarity.MYTHIC: _mythicData.Add(data); break;
            }
        }
        instance._loaded = true;
    }
    public static CardData Rand(List<CardData> _list = null)
    {
        Load();
        while (true)
        {
            CardData data;
            if (_list == null)
            {
                Debug.Log("Choosing a random card out of " + _allCardData.Count);
                data = _allCardData[UnityEngine.Random.Range(0, _allCardData.Count)];
            }
            else
            {
                data = _list[UnityEngine.Random.Range(0, _list.Count)];
            }

            if (data.type != Card.Type.AFFLICTION && !data.abilityKeywords.Contains(KeywordAbility.Key.EPHEMERAL))
            {
                return data;
            }
        }
    }
    public static CardData Rand(TargetTemplate template, List<CardData> _list = null)
    {
        Load();
        List<CardData> matches = new List<CardData>();
        if (_list == null)
        {
            foreach (CardData data in _allCardData)
            {
                if (data.Compare(template))
                {
                    matches.Add(data);
                }
            }
        }
        else
        {
            foreach (CardData data in _list)
            {
                if (data.Compare(template))
                {
                    matches.Add(data);
                }
            }
        }
        while (true)
        {
            CardData data = matches[UnityEngine.Random.Range(0, matches.Count)];
            if (data.type != Card.Type.AFFLICTION && !data.abilityKeywords.Contains(KeywordAbility.Key.EPHEMERAL))
            {
                return data;
            }
        }
    }

    public static CardData Rand(Card.Rarity rarity)
    {
        Load();
        switch (rarity)
        {
            case Card.Rarity.COMMON: return Rand(_commonData);
            case Card.Rarity.SCARCE: return Rand(_scarceData);
            case Card.Rarity.RARE: return Rand(_rareData);
            case Card.Rarity.MYTHIC: return Rand(_mythicData);
            default: return null;
        }
    }

    public static CardData Get(string id)
    {
        Load();
        foreach (CardData data in _allCardData)
        {
            if (data.id == id) { return data; }
        }
        Debug.Log("Could not find Card ID: " + id);
        return null;
    }
}

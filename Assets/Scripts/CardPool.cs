using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CardPoolEntry
{
    public List<CardRate> rates;
    public int quantity;
}

[System.Serializable]
public struct CardRate
{
    public CardData card;
    public float rate;
}

[System.Serializable]
public class CardPool
{
    public List<CardPoolEntry> pool;
}

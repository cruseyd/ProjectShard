using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class DecklistItem
{
    public string id;
    public int qty;
}

[System.Serializable]
public class DecklistArray
{
    public List<Decklist> decks;
}

[System.Serializable]
public class Decklist : IComparable
{
    public string name;
    public List<DecklistItem> list;
    
    public int CompareTo(object obj)
    {
        Decklist dlist = obj as Decklist;
        if (dlist != null)
        {
            return name.CompareTo(dlist.name);
        } else
        {
            throw new ArgumentException("Tried to compare a Decklist with a non-Decklist");
        }
    }
    
}

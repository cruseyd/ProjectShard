using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public struct DecklistItem
{
    public CardData card;
    public int quantity;
}

[CreateAssetMenu(fileName = "NewDecklist", menuName = "Decklist")]
public class Decklist : ScriptableObject, IComparable
{
    public new string name;
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

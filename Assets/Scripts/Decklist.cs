using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DecklistItem
{
    public CardData card;
    public int quantity;
}

[CreateAssetMenu(fileName = "NewDecklist", menuName = "Decklist")]
public class Decklist : ScriptableObject
{
    public List<DecklistItem> list;
}

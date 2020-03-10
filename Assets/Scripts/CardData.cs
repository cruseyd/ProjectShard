using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "NewCardData", menuName = "CardData")]
public class CardData : ScriptableObject
{
    public new string name;
    public string id;
    public int level;

    public Card.Color color;
    public int violetAffinity;
    public int redAffinity;
    public int goldAffinity;
    public int greenAffinity;
    public int blueAffinity;
    public int indigoAffinity;

    public Card.Type type;
    public Keyword[] keywords;

    public int strength;
    public int perception;
    public int finesse;

    [Header("Thrall Specific Parameters")]
    public int power;
    public int health;
    public int upkeep;
    public Keyword damageType;

}

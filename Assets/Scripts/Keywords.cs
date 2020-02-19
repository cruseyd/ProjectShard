using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Keyword
{
    DEFAULT = 0,
    //==============
    SLASHING = 100,
    PIERCING,
    CRUSHING,
    //==============
    GRACEFUL = 200,
    BRUTAL,
    CUNNING,
    HEROIC,
    //==============
    FIRE = 300,
    WATER,
    EARTH,
    WIND,
    LIGHT,
    DARK,
    LIGHTNING,
    ICE,
    POISON,
    //==============
    ARCANE = 400,
    MYSTIC,
    ELDER,
    //===============

}

public static class Keywords
{
    public static string Parse(Keyword word)
    {
        switch (word)
        {
            case Keyword.SLASHING:  return "Slashing";
            case Keyword.PIERCING:  return "Piercing";
            case Keyword.CRUSHING:  return "Crushing";
            case Keyword.BRUTAL:    return "Brutal";
            case Keyword.GRACEFUL:  return "Graceful";
            case Keyword.CUNNING:   return "Cunning";
            case Keyword.HEROIC:    return "Heroic";
            case Keyword.FIRE:      return "Fire";
            case Keyword.WATER:     return "Water";
            case Keyword.EARTH:     return "Earth";
            case Keyword.WIND:      return "Wind";
            case Keyword.ICE:       return "Ice";
            case Keyword.LIGHTNING: return "Lightning";
            case Keyword.LIGHT:     return "Light";
            case Keyword.DARK:      return "Dark";
            case Keyword.POISON:    return "Poison";
            case Keyword.ARCANE:    return "Arcane";
            case Keyword.MYSTIC:    return "Mystic";
            case Keyword.ELDER:     return "Elder";
            case Keyword.DEFAULT:   return "";
            default:
                Debug.Log("Unrecognized Keyword: " + word);
                return "";
        }
    }
    public static string Parse(Card.Type type)
    {
        switch(type)
        {
            case Card.Type.SPELL:       return "Spell";
            case Card.Type.CANTRIP:     return "Cantrip";
            case Card.Type.ABILITY:     return "Ability";
            case Card.Type.STRATEGY:    return "Strategy";
            case Card.Type.ITEM:        return "Item";
            case Card.Type.THRALL:      return "Thrall";
            case Card.Type.DEFAULT:     return "";
            default:
                Debug.Log("Unrecognized Keyword: " + type);
                return "";
        }
    }
    public static string Parse(Equipment.Type type)
    {
        switch(type)
        {
            case Equipment.Type.ARMOR:  return "Armor";
            case Equipment.Type.RELIC:  return "Relic";
            case Equipment.Type.WEAPON: return "Weapon";
            default:
                Debug.Log("Unrecognized Keyword: " + type);
                return "";
        }
    }
}

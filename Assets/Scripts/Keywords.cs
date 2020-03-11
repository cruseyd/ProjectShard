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
    MELEE = 500,
    RANGED, 
    //===============
    WARRIOR=600,
    CLERIC,
    MAGE,
    ROGUE,
    RANGER,
    SORCERER,
    //===============
    BEAST=700,
    SCALE,
    SHELL,
    PLANT,
    SPIRIT,
    UNDEAD
}

public enum AbilityKeyword
{
    DEFAULT,
    SWIFT,
    NIMBLE,
    PASSIVE
}

public static class Keywords
{
    public static string Parse(Keyword word)
    {
        return word.ToString("g");
    }
    public static string Parse(AbilityKeyword word)
    {
        return word.ToString("g");
    }
    public static string Parse(Card.Type type)
    {
        return type.ToString("g");
        
    }
    public static string Parse(Equipment.Type type)
    {
        return type.ToString("g");
    }

}

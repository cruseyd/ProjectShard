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

public static class Keywords
{
    public static string Parse(Keyword word)
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

public abstract class KeywordAbility
{
    private static Dictionary<Key, KeywordAbility> _index;
    public enum Key
    {
        DEFAULT = 0,
        SWIFT,
        NIMBLE,
        BLOODLUST_1 = 100,
        BLOODLUST_2,
        BLOODLUST_3,
        BLOODLUST_4,
        BLOODLUST_5,
        RAGE_1 = 110,
        RAGE_2,
        RAGE_3,
        RAGE_4,
        RAGE_5
    }
    public static void Parse(Key key, Card card)
    {
        if (_index == null)
        {
            _index = new Dictionary<Key, KeywordAbility>();
            _index[Key.SWIFT] = new KA_Swift();
        }
        if (_index.ContainsKey(key))
        {
            _index[key].Set(card);
        }
    }
    protected abstract void Set(Card card, int level = 0);
}

public class KA_Swift : KeywordAbility
{
    protected override void Set(Card card, int level)
    {
        card.cardEvents.onEnterPlay += EnterPlayHandler;
    }
    private void EnterPlayHandler(Card card)
    {
        card.attackAvailable = true;
    }
}

public class KA_Bloodlust : KeywordAbility
{
    private int _level;
    public KA_Bloodlust(int level)
    {
        _level = level;
    }

    protected override void Set(Card card, int level = 0)
    {
        Debug.Assert(card.type == Card.Type.THRALL);
        card.controller.actorEvents.onStartTurn += StartTurnHandler;

    }

    private void StartTurnHandler(Actor actor)
    {

    }
}

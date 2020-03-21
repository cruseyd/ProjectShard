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

    protected Card _user;
    protected int _level;

    public static KeywordAbility Get(Key key, int level, Card user)
    {
        switch (key)
        {
            default: return null;
        }
    }
    public KeywordAbility(int level, Card card)
    {
        _user = card;
        _level = level;
        Set();
    }
}

public class KA_Swift : KeywordAbility
{
    public KA_Swift(int level, Card card) : base(level, card)
    {
        _user.cardEvents.onEnterPlay += EnterPlayHandler;
    }

    private void EnterPlayHandler(Card card)
    {
        card.attackAvailable = true;
    }
}

public class KA_Bloodlust : KeywordAbility
{
    public KA_Bloodlust(int level, Card card) : base(level, card)
    {
        Debug.Assert(_user.type == Card.Type.THRALL);
        _user.controller.actorEvents.onStartTurn += StartTurnHandler;
    }

    private void StartTurnHandler(Actor actor)
    {
        _user.controller.actorEvents.onPlayCard += PlayCardHandler;
    }

    private void PlayCardHandler(Card card)
    {
        if (card.type == Card.Type.ABILITY)
        {
            _user.power.baseValue += 1;
            _user.controller.actorEvents.onEndTurn += EndTurnHandler;
            _user.controller.

        }
    }
    private void EndTurnHandler(Actor actor)
    {
    }
}

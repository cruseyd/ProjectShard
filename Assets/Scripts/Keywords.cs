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
    UNDEAD,
    ELEMENTAL
}

public static class Keywords
{
    public static string Parse(Keyword word)
    {
        return "<b>" + word.ToString("g").ToLower() + "</b>";
    }
    public static string Parse(Card.Type type)
    {
        return "<b>" + type.ToString("g").ToLower() + "</b>";
    }
    public static string Parse(Equipment.Type type)
    {
        return "<b>" + type.ToString("g").ToLower() + "</b>";
    }

    public static string Parse(KeywordAbility.Key word)
    {
        return "<b>" + word.ToString("g").ToLower() + "</b>";
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
        GUARDIAN,
        ELUSIVE,
        EPHEMERAL,
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
    protected Key _key;
    public Key key { get { return _key; } }

    public static KeywordAbility Get(Key key, Card user)
    {
        switch (key)
        {
            case Key.SWIFT: return new KA_Swift(0, user);
            case Key.EPHEMERAL: return new KA_Ephemeral(0, user);
            case Key.BLOODLUST_1: return new KA_Bloodlust(1, user);
            case Key.BLOODLUST_2: return new KA_Bloodlust(2, user);
            case Key.BLOODLUST_3: return new KA_Bloodlust(3, user);
            case Key.BLOODLUST_4: return new KA_Bloodlust(4, user);
            case Key.BLOODLUST_5: return new KA_Bloodlust(5, user);
            case Key.NIMBLE:
            case Key.GUARDIAN:
            case Key.ELUSIVE:
            default: return null;
        }
    }
    public KeywordAbility(int level, Card card)
    {
        _user = card;
        _level = level;
    }
}

public class KA_Ephemeral : KeywordAbility
{
    public KA_Ephemeral(int level, Card card) : base(level, card)
    {
        _user.cardEvents.onEnterDiscard += EnterDiscardHandler;
        _key = Key.EPHEMERAL;
    }

    private void EnterDiscardHandler(Card card)
    {
        card.StartCoroutine(doDiscard());
    }

    private IEnumerator doDiscard()
    {
        Card user = _user as Card;
        user.StartCoroutine(user.Zoom(true, 0.1f));
        yield return user.Translate(Vector2.zero);
        user.Delete();
    }
}
public class KA_Swift : KeywordAbility
{
    public KA_Swift(int level, Card card) : base(level, card)
    {
        _key = Key.SWIFT;
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
        if (card.type == Card.Type.TECHNIQUE && _user.inPlay)
        {
            _user.controller.actorEvents.onPlayCard -= PlayCardHandler;
            _user.AddModifier(new StatModifier(_level, Stat.Name.POWER, null, StatModifier.Duration.START_OF_TURN));
        }
    }
}

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
    public static string Parse(Card.Color color)
    {
        switch (color)
        {
            case Card.Color.RED: return "RAIZ";
            case Card.Color.BLUE: return "IRI";
            case Card.Color.GREEN: return "FEN";
            case Card.Color.VIOLET: return "LIS";
            case Card.Color.GOLD: return "ORA";
            case Card.Color.INDIGO: return "VAEL";
            case Card.Color.TAN: return "NEUTRAL";
            default: return "";
        }
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
        WARDEN,
        PASSIVE,
        EVASIVE,
        ENIGMATIC,
        OVERWHELM,
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
            case Key.GUARDIAN: return new KA_Guardian(0, user);
            case Key.WARDEN: return new KA_Warden(0, user);
            case Key.NIMBLE: return new KA_Nimble(0, user);
            case Key.ELUSIVE: return new KA_Elusive(0, user);
            case Key.EVASIVE: return new KA_Evasive(0, user);
            case Key.ENIGMATIC: return new KA_Enigmatic(0, user);
            case Key.OVERWHELM: return new KA_Overwhelm(0, user);
            case Key.PASSIVE:
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

public class KA_Elusive : KeywordAbility
{
    public KA_Elusive(int level, Card card) : base(level, card)
    {
        _key = Key.ELUSIVE;
        _user.opponent.actorEvents.onTryMarkTarget += TryMarkTargetHandler;
    }

    private void TryMarkTargetHandler(ITargetable source, ITargetable target, Attempt attempt)
    {
        if (_user.inPlay)
        {
            if (target is Card && ((Card)target) == _user)
            {
                if (source is Card && ((Card)source).type == Card.Type.THRALL && !((Card)source).HasKeyword(Key.NIMBLE))
                {
                    foreach (Card card in _user.controller.active)
                    {
                        if (card.type == Card.Type.THRALL && !card.HasKeyword(Key.ELUSIVE))
                        {
                            attempt.success = false;
                        }
                    }
                }
            }
        }
    }
}

public class KA_Guardian : KeywordAbility
{
    public KA_Guardian(int level, Card card) : base(level, card)
    {
        _key = Key.GUARDIAN;
        _user.opponent.actorEvents.onTryMarkTarget += TryMarkTargetHandler;
    }

    private void TryMarkTargetHandler(ITargetable source, ITargetable target, Attempt attempt)
    {
        if (_user.inPlay)
        {
            if (target.controller == _user.controller && source is Card)
            {
                Card src = source as Card;
                if (src.type == Card.Type.TECHNIQUE || src.type == Card.Type.THRALL)
                {
                    if (src.HasKeyword(Key.NIMBLE))
                    {
                        attempt.success = true; return;
                    }
                    else if (target is Card)
                    {
                        Card trg = target as Card;
                        if (trg.HasKeyword(Key.WARDEN) || trg.HasKeyword(Key.GUARDIAN))
                        {
                            attempt.success = true;
                        }
                        else
                        {
                            attempt.success = false;
                        }
                    }
                    else
                    {
                        attempt.success = false;
                    }
                }
            }
        }
    }
}
public class KA_Warden : KeywordAbility
{
    public KA_Warden(int level, Card card) : base(level, card)
    {
        _key = Key.WARDEN;
        _user.opponent.actorEvents.onTryMarkTarget += TryMarkTargetHandler;
    }

    private void TryMarkTargetHandler(ITargetable source, ITargetable target, Attempt attempt)
    {
        if (_user.inPlay)
        {
            if (target.controller == _user.controller && source is Card)
            {
                Card src = source as Card;
                if (src.HasKeyword(Key.NIMBLE))
                {
                    attempt.success = true; return;
                }
                if (src.type == Card.Type.SPELL || src.type == Card.Type.THRALL)
                {
                    if (target is Card)
                    {
                        Card trg = target as Card;
                        if (trg.HasKeyword(Key.WARDEN) || trg.HasKeyword(Key.GUARDIAN))
                        {
                            attempt.success = true;
                        } else
                        {
                            attempt.success = false;
                        }
                    } else
                    {
                        attempt.success = false;
                    }
                }
            }
        }
    }
}

public class KA_Nimble : KeywordAbility
{
    public KA_Nimble(int level, Card card) : base(level, card)
    {
        _key = Key.NIMBLE;
        _user.controller.actorEvents.onTryMarkTarget += TryMarkTargetHandler;
    }

    private void TryMarkTargetHandler(ITargetable source, ITargetable target, Attempt attempt)
    {
        if (_user.inPlay)
        {
            if (source is Card && ((Card)source) == _user && target is Actor && ((Actor)target) == _user.opponent)
            {
                if (_user.opponent.nimble.Count > 0)
                {
                    attempt.success = false;
                } else
                {
                    attempt.success = true;
                }
            }
        }
    }
}

public class KA_Enigmatic : KeywordAbility
{
    public KA_Enigmatic(int level, Card card) : base(level, card)
    {
        _key = Key.ENIGMATIC;
        _user.cardEvents.onTryMarkTarget += TryMarkTargetHandler;
    }

    private void TryMarkTargetHandler(ITargetable source, Card target, Attempt attempt)
    {
        if (_user.inPlay)
        {
            if (source is Card && ((Card)source).type == Card.Type.SPELL)
            {
                attempt.success = false;
            } else
            {
                attempt.success = true;
            }
        }
    }
}
public class KA_Evasive : KeywordAbility
{
    public KA_Evasive(int level, Card card) : base(level, card)
    {
        _key = Key.EVASIVE;
        _user.cardEvents.onTryMarkTarget += TryMarkTargetHandler;
    }

    private void TryMarkTargetHandler(ITargetable source, Card target, Attempt attempt)
    {
        if (_user.inPlay)
        {
            if (source is Card && ((Card)source).type == Card.Type.TECHNIQUE)
            {
                attempt.success = false;
            }
            else
            {
                attempt.success = true;
            }
        }
    }
}

public class KA_Overwhelm : KeywordAbility
{
    public KA_Overwhelm(int level, Card card) : base(level, card)
    {
        _user.targetEvents.onDealOverflowDamage += DealOverflowDamageHandler;

    }

    private void DealOverflowDamageHandler(DamageData data)
    {
        if (data.isAttackDamage)
        {
            DamageData overwhelmDamage = new DamageData(data.damage, data.type, data.source, data.target.controller, data.isAttackDamage);
            data.target.controller.Damage(overwhelmDamage);
        }
    }
}


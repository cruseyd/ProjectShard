using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability
{
    public enum Mode
    {
        ACTIVATE,
        PLAY,
        PASSIVE,
        ATTACK,
        INFUSE,
        CREATE
    }
    private static Dictionary<string, Ability> _index;
    public static Dictionary<string, Ability> index
    {
        get
        {
            if (_index == null)
            {
                _index = new Dictionary<string, Ability>();
                _index["NULL"] = new A_Null();
                _index["CINDER"] = new A_Cinder();
                _index["SINGE"] = new A_Singe();
                _index["BLITZ"] = new A_Blitz();
                _index["SLASH"] = new A_Slash();
                _index["FLASHBLADE_SKIRMISHER"] = new A_FlashbladeSkirmisher();
                _index["INFERNO_DJINN"] = new A_InfernoDjinn();
                _index["CONTINUITY"] = new A_Continuity();
                _index["PASSION"] = new A_Resource();
                _index["REASON"] = new A_Resource();
                _index["ELIXIR_OF_PASSION"] = new A_Elixir();
                _index["ELIXIR_OF_REASON"] = new A_Elixir();
                _index["CHILL"] = new A_Chill();
                _index["DRIFTING_VOIDLING"] = new A_DriftingVoidling();

            }
            return _index;
        }
    }

    public List<TargetTemplate> activateTargets;
    public List<TargetTemplate> playTargets;

    protected virtual void Activate(Card source, List<ITargetable> targets, bool undo = false, GameState state = null) { }
    protected virtual void Play(Card source, List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        // at the bare minimum, all thralls are put into play, changing the boardstate
        if (state != null)
        {
            if (source.type == Card.Type.THRALL)
            {
                if (undo) { state.RemoveCard(source); }
                else { state.AddCard(source); }
            }
        }
    }
    protected virtual void Passive(Card source, List<ITargetable> targets, bool undo = false, GameState state = null) { }
    protected virtual void Create(Card source, List<ITargetable> targets, bool undo = false, GameState state = null) { }
    protected static void Attack(Card source, IDamageable target, bool undo = false, GameState state = null)
    {
        Ability.Damage(new DamageData(source.power.value, source.damageType, source, target), undo, state);
        if (target is Card)
        {
            Card targetCard = (Card)target;
            Ability.Damage(new DamageData(targetCard.power.value, targetCard.damageType, targetCard, source), undo, state);
        }
        if (state != null)
        {
            if (undo) { source.attackAvailable = true; }
            else { source.attackAvailable = false; }
        }
        else { source.attackAvailable = false; }
    }
    public static void Infuse(Player player, Card target, bool undo = false, GameState state = null)
    {
        if (target == null) { return; }

        if (target.type == Card.Type.THRALL && player.focus.value > 0)
        {
            player.focus.baseValue -= 1;
            target.allegiance.baseValue += 1;
        }
    }

    public TargetTemplate GetQuery(Mode mode, int n)
    {
        if (n > NumTargets(mode)) { return null; }
        switch (mode)
        {
            case Mode.ACTIVATE:
                if (activateTargets != null && activateTargets.Count > n)
                {
                    return activateTargets[n];
                }
                return null;
            case Mode.PLAY: 
                if (playTargets != null && playTargets.Count > n)
                {
                    return playTargets[n];
                }
                return null;
            case Mode.ATTACK:
                TargetTemplate t = new TargetTemplate();
                t.isOpposing = true;
                t.isAttackable = true;
                t.inPlay = true;
                return t; 
            default: return null;
        }

    }
    public void Use(Mode mode, ITargetable source, List<ITargetable> targets)
    {
        switch (mode)
        {
            case Mode.PLAY: Play((Card)source, targets); break;
            case Mode.ACTIVATE: Activate((Card)source, targets); break;
            case Mode.ATTACK: Attack((Card)source, (IDamageable)targets[0]); break;
            case Mode.PASSIVE: Passive((Card)source, targets); break;
            case Mode.CREATE: Create((Card)source, targets); break;
            default: break;
        }
    }
    public void Try(Mode mode, ITargetable source, List<ITargetable> targets, GameState state)
    {
        switch (mode)
        {
            case Mode.PLAY: Play((Card)source, targets, false, state); break;
            case Mode.ACTIVATE: Activate((Card)source, targets, false, state); break;
            case Mode.ATTACK: Attack((Card)source, (IDamageable)targets[0], false, state); break;
            case Mode.PASSIVE: Passive((Card)source, targets, false, state); break;
            default: break;
        }
    }
    public void Undo(Mode mode, ITargetable source, List<ITargetable> targets, GameState state)
    {
        switch (mode)
        {
            case Mode.PLAY: Play((Card)source, targets, true, state); break;
            case Mode.ACTIVATE: Activate((Card)source, targets, true, state); break;
            case Mode.ATTACK: Attack((Card)source, (IDamageable)targets[0], true, state); break;
            case Mode.PASSIVE: Passive((Card)source, targets, true, state); break;
            default: break;
        }
    }
    public void Show(Mode mode, ITargetable source, List<ITargetable> targets)
    {
        Targeter.Clear();
        switch (mode)
        {
            case Mode.PLAY:
                Dungeon.MoveCard((Card)source, CardZone.MAGNIFY);
                ((Card)source).FaceUp(true, true);
                if (targets!= null && targets.Count > 0)
                {
                    Targeter.ShowTarget(source, targets[0]);
                }
                break;
            case Mode.ATTACK:
                Targeter.ShowTarget(source, targets[0]);
                break;
            case Mode.ACTIVATE:
                if (source is Card)
                {
                    ((Card)source).particles.Glow(true);
                }
                if (targets != null && targets.Count > 0)
                {
                    Targeter.ShowTarget(source, targets[0]);
                }
                break;
            default: return;
        }
    }
    public int NumTargets(Mode type)
    {
        switch (type)
        {
            case Mode.PLAY:
                if (playTargets == null) { return 0; }
                else { return playTargets.Count; }
            case Mode.ACTIVATE:
                if (activateTargets == null) { return 0; }
                else { return activateTargets.Count; }
            case Mode.ATTACK: return 1;
            case Mode.PASSIVE: return 0;
            default: return 0;
        }
    }
    public abstract string Text(Card source);
    public static void Damage(DamageData data, bool undo, GameState state)
    {
        if (state != null)
        {
            state.ApplyDamage(data, undo);
        } else
        {
            data.target.Damage(data);
        }
    }

}

public class A_Null : Ability
{
    public override string Text(Card source)
    {
        return "EMPTY ABILITY";
    }
}
public class A_Elixir : Ability
{
    protected override void Play(Card source, List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(source, targets, undo, state);
        if (state != null)
        {
            if (undo)
            {
            }
            else
            {
            }
        }
        else
        {
            if (source.owner is Player)
            {
                ((Player)source.owner).focus.baseValue += 2;
                ((Player)source.owner).maxFocus.baseValue += 1;
            }
        }
    }
    public override string Text(Card source)
    {
        return "Gain 2/1 Focus.";
    }
}
public class A_Resource : Ability
{
    protected override void Play(Card source, List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(source, targets, undo, state);
        if (state != null)
        {
            if (undo)
            {
                state.DrawCards(source.owner, -1);
            }
            else
            {
                state.DrawCards(source.owner, 1);
            }
        }
        else
        {
            source.owner.Draw();
            if (source.owner is Player)
            {
                ((Player)source.owner).focus.baseValue += 1;
            }
        }
    }
    public override string Text(Card source)
    {
        return "Draw a card. Gain 1/0 Focus.";
    }
}

// ============================================ RED CARDS ============================================
public class A_Cinder : Ability
{
    public A_Cinder()
    {
        playTargets = new List<TargetTemplate>();
        TargetTemplate t = new TargetTemplate();
        t.isDamageable = true;
        t.isOpposing = true;
        t.inPlay = true;
        playTargets.Add(t);
    }
    protected override void Play(Card source, List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(source, targets, undo, state);
        int damage = 2;
        IDamageable target = (IDamageable)targets[0];
        DamageData data = new DamageData(damage, Keyword.FIRE, source, target);
        Ability.Damage(data, undo, state);
    }

    public override string Text(Card source)
    {
        return "Cinder inflicts 2 Fire damage to any target.";
    }
}
public class A_Singe : Ability
{
    protected override void Play(Card source, List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(source, targets, undo, state);
        int stacks = 2;
        if (state != null)
        {

        } else
        {
            source.opponent.AddStatus(StatusName.BURN, stacks);
        }
    }

    public override string Text(Card source)
    {
        return "Singe adds 2 stacks of Burn to your opponent.";
    }
}
public class A_Blitz : Ability
{
    protected override void Play(Card source, List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(source, targets, undo, state);
        if (state != null)
        {
            if (undo)
            {
                state.DrawCards(source.owner, -1);
            } else
            {
                state.DrawCards(source.owner, 1);
            }
        } else
        {
            int n = 0;
            source.owner.Draw();
            if (source.playerCard)
            {
                foreach (Card card in source.owner.hand)
                {
                    if (card.type == Card.Type.ABILITY) { n += 1; }
                }
                ((Player)source.owner).focus.baseValue += n;
            }
        }
        
    }
    protected override void Passive(Card source, List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        if (source.playerCard)
        {
            ((Player)source.owner).focus.baseValue += 1;
        }
    }
    public override string Text(Card source)
    {
        string txt = "<b>Play:</b> Draw a card.";
        if (source.playerCard)
        {
            txt += " Gain 1/0 Focus for each Ability in your hand.";
            txt += "\n<b>Passive:</b> Gain 1/0 Focus.";
        }
        return txt;
    }
}
public class A_Slash : Ability
{
    public A_Slash()
    {
        playTargets = new List<TargetTemplate>();
        TargetTemplate t = new TargetTemplate();
        t.isDamageable = true;
        t.isOpposing = true;
        t.inPlay = true;
        playTargets.Add(t);
    }
    protected override void Play(Card source, List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(source, targets, undo, state);
        int damage = 2;
        IDamageable target = (IDamageable)targets[0];
        DamageData data = new DamageData(damage, Keyword.SLASHING, source, target);
        Ability.Damage(data, undo, state);
    }

    public override string Text(Card source)
    {
        return "Slash inflicts 2 Slashing damage to any target.";
    }
}
public class A_FlashbladeSkirmisher : Ability
{
    /*
    protected override void Play(Card source, List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(source, targets, undo, state);
    }
    */
    public override string Text(Card source)
    {
        return "<i>Flavor text</i>";
    }
}
public class A_InfernoDjinn : Ability
{
    /*
    protected override void Play(Card source, List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(source, targets, undo, state);
    }
    */
    public override string Text(Card source)
    {
        return "<i>Flavor text</i>";
    }
}

// ============================================ BLUE CARDS ============================================
public class A_Continuity : Ability
{
    protected override void Play(Card source, List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(source, targets, undo, state);
        if (state != null)
        {
            if (undo)
            {
                state.DrawCards(source.owner, -1);
            }
            else
            {
                state.DrawCards(source.owner, 1);
            }
        }
        else
        {
            int n = 0;
            source.owner.Draw();
            if (source.playerCard)
            {
                foreach (Card card in source.owner.hand)
                {
                    if (card.type == Card.Type.SPELL) { n += 1; }
                }
                ((Player)source.owner).focus.baseValue += n;
            }
        }

    }
    protected override void Passive(Card source, List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        if (source.playerCard)
        {
            ((Player)source.owner).focus.baseValue += 1;
        }
    }
    public override string Text(Card source)
    {
        string txt = "<b>Play:</b> Draw a card.";
        if (source.playerCard)
        {
            txt += " Gain 1/0 Focus for each Spell in your hand.";
            txt += "\n<b>Passive:</b> Gain 1/0 Focus.";
        }
        return txt;
    }
}
public class A_Chill : Ability
{
    public A_Chill()
    {
        playTargets = new List<TargetTemplate>();
        TargetTemplate t = new TargetTemplate();
        t.isDamageable = true;
        t.isOpposing = true;
        t.inPlay = true;
        playTargets.Add(t);
    }
    protected override void Play(Card source, List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(source, targets, undo, state);
        int damage = 1;
        Keyword damageType = Keyword.ICE;
        IDamageable target = (IDamageable)targets[0];
        DamageData data = new DamageData(damage, damageType, source, target);
        Ability.Damage(data, undo, state);

        if (target is Card)
        {
            ((Card)target).AddStatus(StatusName.STUN);
        }
    }

    public override string Text(Card source)
    {
        return "Chill inflicts 1 damage to any target. If the target is a thrall, stun it.";
    }
}

public class A_DriftingVoidling : Ability
{
    public override string Text(Card source)
    {
        return "When Drifting Voidling is destroyed, draw a card.";
    }

    protected override void Play(Card source, List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(source, targets, undo, state);
    }

    protected override void Create(Card source, List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Create(source, targets, undo, state);
        Debug.Log("Called create ability");
        source.events.onDestroy += OnDestroy;
    }

    private void OnDestroy(Card card)
    {
        card.owner.Draw();
        Debug.Log("Called destroy ability");
    }
}
public class A_IceElemental : Ability
{
    protected override void Play(Card source, List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(source, targets, undo, state);
        if (source.playerCard)
        {

        }
    }

    public override string Text(Card source)
    {
        if (source.playerCard)
        {
            return "While Ice Elemental is in play, ice Spells and Cantrips have Cost - 1";
        } else
        {
            return "";
        }
        
    }
}


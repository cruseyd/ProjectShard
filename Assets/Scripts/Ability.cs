using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Factory class
public static class AbilityIndex
{
    public static Ability Get(string id, ITargetable user)
    {
        switch (id)
        {
            // resources
            case "PASSION":
            case "REASON":
            case "PATIENCE":
                return new A_Ideal(user);
            case "ELIXIR_OF_PASSION":
            case "ELIXIR_OF_REASON":
            case "ELIXIR_OF_PATIENCE":
                return new A_Elixir(user);

            // red cards
            case "CINDER": return new A_Cinder(user);
            case "SINGE": return new A_Singe(user);
            case "BLITZ": return new A_Blitz(user);
            case "SLASH": return new A_Slash(user);
            case "SHARPEN": return new A_Sharpen(user);
            case "WILD_SWING": return new A_WildSwing(user);
            case "FLASHBLADE_SKIRMISHER": return new A_FlashbladeSkirmisher(user);
            case "INFERNO_DJINN": return new A_InfernoDjinn(user);

            // blue cards
            case "CONTINUITY": return new A_Continuity(user);
            case "CHILL":   return new A_Chill(user);
            case "DRIFTING_VOIDLING": return new A_DriftingVoidling(user);
            case "ICE_ELEMENTAL": return new A_IceElemental(user);
            case "FROST_LATTICE": return new A_FrostLattice(user);
            case "RIME_SPRITE": return new A_RimeSprite(user);

            // multicolor cards
            case "STATIC": return new A_Static(user);
            case "REND": return new A_Rend(user);
            case "FERAL_WARCAT": return new A_FeralWarcat(user);
            // null ability
            default: return new A_Null(user);
        }
    }
}

public abstract class Ability
{
    public enum Mode
    {
        ACTIVATE,
        PLAY,
        PASSIVE,
        ATTACK,
        INFUSE
    }
    
    public List<TargetTemplate> activateTargets;
    public List<TargetTemplate> playTargets;
    protected ITargetable _user;

    public Ability(ITargetable user) { _user = user; }
    protected virtual void Activate(List<ITargetable> targets, bool undo = false, GameState state = null) { }
    protected virtual void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        // at the bare minimum, all thralls are put into play, changing the boardstate
        Debug.Assert(_user is Card);
        Card user = (Card)_user;
        if (state != null)
        {
            if (user.type == Card.Type.THRALL)
            {
                if (undo) { state.RemoveCard(user); }
                else { state.AddCard(user); }
            }
        }
    }
    protected virtual void Passive(List<ITargetable> targets, bool undo = false, GameState state = null) { }
    protected void Attack(ITargetable target, bool undo = false, GameState state = null)
    {
        Debug.Assert(_user is Card);
        Card user = (Card)_user;
        DamageData _userDamage = new DamageData(user.power.value, user.damageType, _user, target);
        DamageData targetDamage = null;
        if (target is Card)
        {
            Card targetCard = (Card)target;
            targetDamage = new DamageData(targetCard.power.value, targetCard.damageType, targetCard, user);
        }

        target.Damage(_userDamage);
        user.Damage(targetDamage);
        target.ResolveDamage(_userDamage);
        user.ResolveDamage(targetDamage);

        if (state != null)
        {
            if (undo) { user.attackAvailable = true; }
            else { user.attackAvailable = false; }
        }
        else { user.attackAvailable = false; }
    }
    public static void Infuse(Card target, bool undo = false, GameState state = null)
    {
        if (target == null) { return; }

        if (target.type == Card.Type.THRALL && Player.instance.focus.value > 0)
        {
            Player.instance.focus.baseValue -= 1;
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
    public void Use(Mode mode,List<ITargetable> targets)
    {
        
        switch (mode)
        {
            case Mode.PLAY: Play(targets); break;
            case Mode.ACTIVATE: Activate(targets); break;
            case Mode.ATTACK: Attack((ITargetable)targets[0]); break;
            case Mode.PASSIVE: Passive(targets); break;
            default: break;
        }
    }
    public void Try(Mode mode,List<ITargetable> targets, GameState state)
    {
        switch (mode)
        {
            case Mode.PLAY: Play(targets, false, state); break;
            case Mode.ACTIVATE: Activate(targets, false, state); break;
            case Mode.ATTACK: Attack((ITargetable)targets[0], false, state); break;
            case Mode.PASSIVE: Passive(targets, false, state); break;
            default: break;
        }
    }
    public void Undo(Mode mode,List<ITargetable> targets, GameState state)
    {
        switch (mode)
        {
            case Mode.PLAY: Play(targets, true, state); break;
            case Mode.ACTIVATE: Activate(targets, true, state); break;
            case Mode.ATTACK: Attack((ITargetable)targets[0], true, state); break;
            case Mode.PASSIVE: Passive(targets, true, state); break;
            default: break;
        }
    }
    public void Show(Mode mode,List<ITargetable> targets)
    {
        Targeter.Clear();
        switch (mode)
        {
            case Mode.PLAY:
                Dungeon.MoveCard((Card)_user, CardZone.MAGNIFY);
                ((Card)_user).FaceUp(true, true);
                if (targets!= null && targets.Count > 0)
                {
                    Targeter.ShowTarget(Dungeon.GetZone(CardZone.MAGNIFY).transform.position, targets[0].transform.position);
                }
                break;
            case Mode.ATTACK:
                Targeter.ShowTarget(_user, targets[0]);
                break;
            case Mode.ACTIVATE:
                if (_user is Card)
                {
                    ((Card)_user).particles.Glow(true);
                }
                if (targets != null && targets.Count > 0)
                {
                    Targeter.ShowTarget(_user, targets[0]);
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
    public abstract string Text();
    public static void Damage(DamageData data, bool undo, GameState state)
    {
        if (state != null)
        {
            state.ApplyDamage(data, undo);
        } else
        {
            data.target.Damage(data);
            data.target.ResolveDamage(data);
        }
    }
    public void TargetAnyOpposing()
    {
        if (playTargets == null)
        {
            playTargets = new List<TargetTemplate>();
        }
        TargetTemplate t = new TargetTemplate();
        t.isDamageable = true;
        t.isOpposing = true;
        t.inPlay = true;
        playTargets.Add(t);
    }
    public void TargetOpposingThrall()
    {
        if (playTargets == null)
        {
            playTargets = new List<TargetTemplate>();
        }
        TargetTemplate t = new TargetTemplate();
        t.isOpposing = true;
        t.inPlay = true;
        t.cardType = Card.Type.THRALL;
        playTargets.Add(t);
    }
    public static TargetTemplate RandomOpposingTarget()
    {
        TargetTemplate t = new TargetTemplate();
        t.isDamageable = true;
        t.isOpposing = true;
        t.inPlay = true;
        return t;
    }
    public static List<ITargetable> ValidTargets(Card _user, TargetTemplate template)
    {
        List<ITargetable> validTargets = new List<ITargetable>();
        validTargets.Add(_user.opponent);
        foreach (Card card in _user.opponent.active)
        {
            if (card.Compare(template, _user.controller))
            {
                validTargets.Add(card);
            }
        }
        return validTargets;
    }
    public static ITargetable RandomValidTarget(Card _user, TargetTemplate template)
    {
        List < ITargetable > valid = ValidTargets(_user, template);
        return valid[Random.Range(0, valid.Count)];
    }

    public static ITargetable RandomOtherTarget(Card _user, ITargetable ignore, TargetTemplate template)
    {
        List<ITargetable> valid = ValidTargets(_user, template);
        if (valid.Count > 1)
        {
            ITargetable target = null;
            while (target == null)
            {
                ITargetable temp = valid[Random.Range(0, valid.Count)];
                if (temp != ignore) { target = temp; }
            }
            return target;
        }
        return null;
    }
}

public class A_Null : Ability
{
    public A_Null(ITargetable user) : base(user) { }
    public override string Text()
    {
        return "EMPTY ABILITY";
    }
}
public class A_Elixir : Ability
{
    public A_Elixir(ITargetable user) : base(user) { }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        Debug.Assert(_user is Card);
        Card user = (Card)_user;
        base.Play(targets, undo, state);
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
            if (user.controller is Player)
            {
                ((Player)user.controller).focus.baseValue += 1;
                ((Player)user.controller).maxFocus.baseValue += 1;
            }
        }
    }
    public override string Text()
    {
        return "Gain 1/1 Focus.";
    }
}
public class A_Ideal : Ability
{
    public A_Ideal(ITargetable user) : base(user) { }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        Debug.Assert(_user is Card);
        Card user = (Card)_user;
        base.Play(targets, undo, state);
        if (state != null)
        {
            if (undo)
            {
                state.DrawCards(user.controller, -1);
            }
            else
            {
                state.DrawCards(user.controller, 1);
            }
        }
        else
        {
            user.controller.Draw();
            if (user.controller is Player)
            {
                ((Player)user.controller).maxFocus.baseValue += 1;
            }
        }
    }
    public override string Text()
    {
        return "Gain 0/1 Focus. Draw a card. ";
    }
}

// ============================================ RED CARDS ============================================
public class A_Cinder : Ability
{
    public A_Cinder(ITargetable user) : base(user)
    {
        playTargets = new List<TargetTemplate>();
        TargetTemplate t = new TargetTemplate();
        t.isDamageable = true;
        t.isOpposing = true;
        t.inPlay = true;
        playTargets.Add(t);
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        Debug.Assert(_user is Card);
        Card user = (Card)_user;
        base.Play(targets, undo, state);
        int damage = 2;
        ITargetable target = (ITargetable)targets[0];
        DamageData data = new DamageData(damage, Keyword.FIRE, _user, target);
        Ability.Damage(data, undo, state);
    }

    public override string Text()
    {
        return "Cinder inflicts 2 Fire damage to any target.";
    }
    }
}
public class A_Singe : Ability
{
    public A_Singe(ITargetable user) : base(user) { }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        Debug.Assert(_user is Card);
        Card user = (Card)_user;
        if (state != null)
        {

        } else
        {
            int n = user.opponent.GetStatus(StatusName.BURN);
            if (n > 0) { user.opponent.AddStatus(StatusName.BURN, 1); }
            else { user.opponent.AddStatus(StatusName.BURN, 2); }
        }
    }

    public override string Text()
    {
        return "Your oppenent gains 1 stack of Burn. If they had no stacks of Burn, add an additional stack.";
    }
}
public class A_Blitz : Ability
{
    public A_Blitz(ITargetable user) : base(user) { }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        Debug.Assert(_user is Card);
        Card user = (Card)_user;
        base.Play(targets, undo, state);
        if (state != null)
        {
            if (undo)
            {
                state.DrawCards(user.controller, -1);
            } else
            {
                state.DrawCards(user.controller, 1);
            }
        } else
        {
            int n = 0;
            user.controller.Draw();
            if (user.playerControlled)
            {
                foreach (Card card in user.controller.hand)
                {
                    if (card.type == Card.Type.ABILITY) { n += 1; }
                }
                ((Player)user.controller).focus.baseValue += n;
            }
        }
        
    }
    protected override void Passive(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        if (((Card)_user).playerControlled)
        {
            Player.instance.focus.baseValue += 1;
        }
    }
    public override string Text()
    {
        string txt = "<b>Play:</b> Draw a card.";
        if (((Card)_user).playerControlled)
        {
            txt += " Gain 1/0 Focus for each Ability in your hand.";
            txt += "\n<b>Passive:</b> Gain 1/0 Focus.";
        }
        return txt;
    }
}
public class A_Slash : Ability
{
    public A_Slash(ITargetable user) : base(user)
    {
        TargetAnyOpposing();
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        Debug.Assert(_user is Card);
        Card user = (Card)_user;
        base.Play(targets, undo, state);
        int damage = 1;
        Equipment weapon = _user.controller.weapon;
        if (weapon != null && weapon.HasKeyword(Keyword.SLASHING) && weapon.durability > 0)
        {
            damage += 1;
            weapon.durability -= 1;
        }
        
        ITargetable target = (ITargetable)targets[0];
        DamageData data = new DamageData(damage, Keyword.SLASHING, _user, target);
        Ability.Damage(data, undo, state);
    }

    public override string Text()
    {
        string txt = "Slash inflicts 1 Slashing damage to any target.";
        txt += "<b> \nSlashing Weapon: </b> +1 damage. -1 durability.";
        return txt;
    }
}
public class A_WildSwing : Ability
{
    public A_WildSwing(ITargetable user) : base(user)
    {
        TargetAnyOpposing();
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        Debug.Assert(_user is Card);
        Card user = (Card)_user;
        base.Play(targets, undo, state);
        int damage = 2;
        ITargetable t1 = (ITargetable)targets[0];
        ITargetable t2 = (ITargetable)RandomOtherTarget(user, targets[0], playTargets[0]);
        
        DamageData d1 = new DamageData(damage, Keyword.SLASHING, _user, t1);
        Ability.Damage(d1, undo, state);
        if (t2 != null)
        {
            DamageData d2 = new DamageData(damage, Keyword.SLASHING, _user, t2);
            Ability.Damage(d2, undo, state);
        }
    }

    public override string Text()
    {
        return "Wild Swing inflicts 2 Slashing damage to any target and to another random opposing target.";
    }
}
public class A_FlashbladeSkirmisher : Ability
{
    public A_FlashbladeSkirmisher(ITargetable user) : base(user) { }
    public override string Text()
    {
        return "<i>Flavor text</i>";
    }
}
public class A_InfernoDjinn : Ability
{
    public A_InfernoDjinn(ITargetable user) : base(user) { }
    public override string Text()
    {
        return "<i>Flavor text</i>";
    }
}

public class A_Sharpen : Ability
{
    public A_Sharpen(ITargetable user) : base(user) { }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        _user.controller.targetEvents.onDealRawDamage += RawDamageHandler;
        _user.controller.actorEvents.onEndTurn += EndTurnHandler;
        _user.controller.Draw();
    }
    public override string Text()
    {
        return "Increase all Slashing damage you deal this turn by 1. Draw a card.";
    }

    void RawDamageHandler(DamageData data)
    {
        Debug.Log("Triggering Sharpen ability");
        if (data.type == Keyword.SLASHING) { data.damage += 1; }
    }

    void EndTurnHandler(Actor actor)
    {
        Debug.Log("Unregistering for Sharpen's events");
        actor.targetEvents.onDealRawDamage -= RawDamageHandler;
        actor.actorEvents.onEndTurn -= EndTurnHandler;
    }
}
// ============================================ GREEN CARDS ===========================================

public class A_TerritorialBriar : Ability
{
    public A_TerritorialBriar(ITargetable user) : base(user) { }
    public override string Text()
    {
        return "IMPLEMENT ME";
    }

    private void OnLeavePlayHandler(Card _user)
    {

    }

    private void OnEnterPlayHandler(Card _user)
    {
        
    }

}

// ============================================ BLUE CARDS ============================================

public class A_Continuity : Ability
{
    public A_Continuity(ITargetable user) : base(user) { }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        Debug.Assert(_user is Card);
        Card user = (Card)_user;
        base.Play(targets, undo, state);
        if (state != null)
        {
            if (undo)
            {
                state.DrawCards(user.controller, -1);
            }
            else
            {
                state.DrawCards(user.controller, 1);
            }
        }
        else
        {
            int n = 0;
            user.controller.Draw();
            if (user.playerControlled)
            {
                foreach (Card card in user.controller.hand)
                {
                    if (card.type == Card.Type.SPELL) { n += 1; }
                }
                ((Player)user.controller).focus.baseValue += n;
            }
        }

    }
    protected override void Passive(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        if (_user.playerControlled)
        {
            ((Player)_user.controller).focus.baseValue += 1;
        }
    }
    public override string Text()
    {
        string txt = "<b>Play:</b> Draw a card.";
        if (((Card)_user).playerControlled)
        {
            txt += " Gain 1/0 Focus for each Spell in your hand.";
            txt += "\n<b>Passive:</b> Gain 1/0 Focus.";
        }
        return txt;
    }
}
public class A_Chill : Ability
{
    public A_Chill(ITargetable user) : base(user)
    {
        playTargets = new List<TargetTemplate>();
        TargetTemplate t = new TargetTemplate();
        t.isDamageable = true;
        t.isOpposing = true;
        t.inPlay = true;
        playTargets.Add(t);
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        int damage = 1;
        Keyword damageType = Keyword.ICE;
        ITargetable target = (ITargetable)targets[0];
        DamageData data = new DamageData(damage, damageType, _user, target);
        Ability.Damage(data, undo, state);

        ((ITargetable)target).AddStatus(StatusName.CHILL,1);
    }

    public override string Text()
    {
        return "Chill inflicts 1 damage to any target. If the target is a thrall, stun it.";
    }
}

public class A_DriftingVoidling : Ability
{
    public A_DriftingVoidling(ITargetable user) : base(user)
    {
        ((Card)_user).cardEvents.onLeavePlay += LeavePlayHandler;
    }
    public override string Text()
    {
        return "When Drifting Voidling is destroyed, its controller gains a stack of Elder Knowledge.";
    }

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
    }

    private void LeavePlayHandler(Card card)
    {
        card.controller.AddStatus(StatusName.ELDER_KNOWLEDGE);
    }
}
public class A_IceElemental : Ability
{
    public A_IceElemental(ITargetable user) : base(user) { }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        if (_user.playerControlled)
        {
            TargetTemplate t = new TargetTemplate();
            t.inHand = true;
            t.isSelf = true;
            t.keyword = Keyword.ICE;
            t.cardType = Card.Type.SPELL;
            TemplateModifier mod = new TemplateModifier(-1, Stat.Name.COST, t, (Card)_user);
            Dungeon.AddModifier(mod);
        }
    }

    public override string Text()
    {
        if (_user.playerControlled)
        {
            return "While Ice Elemental is in play, ice Spells have Cost - 1";
        } else
        {
            return "";
        }
        
    }
}

public class A_FrostLattice : Ability
{
    public A_FrostLattice(ITargetable user) : base(user) { }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        TargetTemplate t = new TargetTemplate();
        t.isDamageable = true;
        t.isOpposing = true;
        t.inPlay = true;
        List < ITargetable > validTargets = new List<ITargetable>();
        validTargets.Add(_user.opponent);
        foreach (Card card in _user.opponent.active)
        {
            if (card.Compare(t, _user.controller))
            {
                validTargets.Add(card);
            }
        }
        Debug.Log("Frost Lattice has " + validTargets.Count + " valid targets");

        ITargetable target = validTargets[Random.Range(0, validTargets.Count)];
        target.AddStatus(StatusName.CHILL, 1);
        target = validTargets[Random.Range(0, validTargets.Count)];
        target.AddStatus(StatusName.CHILL, 1);
        _user.controller.Draw();
    }

    public override string Text()
    {
        return "Frost Lattice adds 1 stack of Chill to 2 random opposing targets. Draw a card.";
    }
}

public class A_RimeSprite : Ability
{
    public A_RimeSprite(ITargetable user) : base(user)
    {
        _user.targetEvents.onDealDamage += DealDamageHandler;
    }
    public override string Text()
    {
        return "When Rime Sprite deals damage to a target, add a Chill counter to that target.";
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
    }

    public void DealDamageHandler(DamageData data)
    {
        ((ITargetable)data.target).AddStatus(StatusName.CHILL, 1);
        data.source.controller.Discard(((Card)data.source));
    }
}
// ============================================ MULTICOLOR CARDS ============================================
public class A_Static : Ability
{
    public A_Static(ITargetable user) : base(user) { }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        TargetTemplate t = new TargetTemplate();
        t.isDamageable = true;
        t.isOpposing = true;
        t.inPlay = true;
        List<ITargetable> validTargets = new List<ITargetable>();
        validTargets.Add(_user.opponent);
        foreach (Card card in _user.opponent.active)
        {
            if (card.Compare(t, _user.controller))
            {
                validTargets.Add(card);
            }
        }

        ITargetable target = validTargets[Random.Range(0, validTargets.Count)];
        Damage(new DamageData(1, Keyword.LIGHTNING, _user, (ITargetable)target), undo, state);
        target = validTargets[Random.Range(0, validTargets.Count)];
        Damage(new DamageData(1, Keyword.LIGHTNING, _user, (ITargetable)target), undo, state);
    }

    public override string Text()
    {
        return "Static deals 1 Lightning damage to 2 random targets.";
    }
}

public class A_FeralWarcat : Ability
{
    public A_FeralWarcat(ITargetable user) : base(user)
    {
        user.controller.targetEvents.onDeclareTarget += DeclareTargetHandler;
    }
    public override string Text()
    {
        return "When you target an enemy with an Ability, Feral Warcat deals 1 Slashing damage to that target.";
    }

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
    }

    private void DeclareTargetHandler(ITargetable source, ITargetable target)
    {
        if (source is Card && ((Card)_user).inPlay)
        {
            Card src = (Card)source;
            if (src.type == Card.Type.ABILITY)
            {
                Ability.Damage(new DamageData(1, Keyword.SLASHING, _user, target), false, null);
            }
        }
    }

}

public class A_Rend : Ability
{
    public A_Rend(ITargetable user) : base(user)
    {
        TargetOpposingThrall();
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        int damage = 2;
        Equipment weapon = _user.controller.weapon;
        if (weapon != null && weapon.HasKeyword(Keyword.CRUSHING) && weapon.durability > 0)
        {
            targets[0].AddStatus(StatusName.STUN);
            weapon.durability -= 1;
        }

        ITargetable target = (ITargetable)targets[0];
        DamageData data = new DamageData(damage, Keyword.CRUSHING, _user, target);
        Ability.Damage(data, undo, state);
    }

    public override string Text()
    {
        string txt = "Rend deals 2 damage to target opposing thrall.";
        txt += "\n<b>Crushing Weapon:</b> The target is also Dazed.";
        return txt;
    }
}

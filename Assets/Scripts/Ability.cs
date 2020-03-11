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
            case "INFERNO_DJINN": return new A_InfernoDjinn(user);

            // green cards
            case "TERRITORIAL_BRIAR": return new A_TerritorialBriar(user);
            case "CARNIVOROUS_PITFALL": return new A_CarnivorousPitfall(user);
            case "PITFALL_VINE": return new A_PitfallVine(user);
            case "BLOSSOMING_IVYPRONG": return new A_BlossomingIvyProng(user);
            case "RAMPAGING_SWORDTUSK": return new A_RampagingSwordtusk(user);
            case "KYRNANOS": return new A_KyrnanosLordOfTheWild(user);

            // blue cards
            case "CONTINUITY": return new A_Continuity(user);
            case "CHILL":   return new A_Chill(user);
            case "DRIFTING_VOIDLING": return new A_DriftingVoidling(user);
            case "ICE_ELEMENTAL": return new A_IceElemental(user);
            case "FROST_LATTICE": return new A_FrostLattice(user);
            case "RIME_SPRITE": return new A_RimeSprite(user);

            // multicolor cards
            case "RIDE_THE_LIGHTNING": return new A_RideTheLightning(user);
            case "CHAIN_LIGHTNING": return new A_ChainLightning(user);
            case "STATIC": return new A_Static(user);
            case "REND": return new A_Rend(user);
            case "FERAL_WARCAT": return new A_FeralWarcat(user);
            case "CRYOFALL": return new A_Cryofall(user);
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

    protected ITargetable _user;
    protected List<TargetTemplate> _activateTargets;
    protected List<TargetTemplate> _playTargets;
    public List<TargetTemplate> activateTargets
    {
        get
        {
            if (_activateTargets == null)
            {
                _activateTargets = new List<TargetTemplate>();
            }
            return _activateTargets;
        }
    }
    public List<TargetTemplate> playTargets
    {
        get
        {
            if (_playTargets == null)
            {
                _playTargets = new List<TargetTemplate>();
            }
            return _playTargets;
        }
    }
    public Ability(ITargetable user) { _user = user; }
    public virtual bool ActivationAvailable() { return false; }
    protected virtual void Activate(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        Debug.Assert(_user is Card);
        ((Card)_user).activationAvailable = false;
        ((Card)_user).attackAvailable = false;
    }
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
        user.controller.targetEvents.DeclareAttack(user, target);
        user.targetEvents.DeclareAttack(user, target);

        if (!user.inPlay || user.health.value <= 0) { return; }

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

    protected void Fight(Card target, bool undo = false, GameState state = null)
    {
        Debug.Assert(_user is Card);
        Card user = (Card)_user;
        //user.controller.targetEvents.DeclareAttack(user, target);
        //user.targetEvents.DeclareAttack(user, target);

        if (!user.inPlay || user.health.value <= 0) { return; }

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
    }

    public static void Infuse(Card target, bool undo = false, GameState state = null)
    {
        if (target == null) { return; }

        if (target.type == Card.Type.THRALL && Player.instance.focus.value > 0)
        {
            Player.instance.focus.baseValue -= 1;
            target.health.baseValue += 1;
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
    public TargetTemplate TargetAnyOpposing(ITargetable ignore = null)
    {
        TargetTemplate t = new TargetTemplate();
        t.isDamageable = true;
        t.isOpposing = true;
        t.inPlay = true;
        if (ignore != null)
        {
            t.isNot = ignore;
        }
        return t;
    }
    public TargetTemplate TargetOpposingThrall()
    {
        TargetTemplate t = new TargetTemplate();
        t.isOpposing = true;
        t.inPlay = true;
        t.cardType = Card.Type.THRALL;
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
}

public class A_Null : Ability
{
    public A_Null(ITargetable user) : base(user) { }
    public override string Text()
    {
        return "";
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
                TargetTemplate t = new TargetTemplate();
                t.cardType = Card.Type.IDEAL;
                if (((Player)user.controller).NumPlayedThisTurn(t) == 0)
                {
                    ((Player)user.controller).maxFocus.baseValue += 1;
                }
            }
        }
    }
    public override string Text()
    {
        return "If you have not played an Ideal this turn, gain 0/1 Focus. \nDraw a card. ";
    }
}

// ============================================ RED CARDS ============================================
public class A_Cinder : Ability
{
    public A_Cinder(ITargetable user) : base(user)
    {
        playTargets.Add(TargetAnyOpposing());
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
        playTargets.Add(TargetAnyOpposing());
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
        playTargets.Add(TargetAnyOpposing());
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        Debug.Assert(_user is Card);
        Card user = (Card)_user;
        base.Play(targets, undo, state);
        int damage = 2;
        ITargetable t1 = (ITargetable)targets[0];
        ITargetable t2 = RandomValidTarget(user, TargetAnyOpposing(t1));
        
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

public class A_Spinetail : Ability
{
    public A_Spinetail(ITargetable user) : base(user)
    {
        activateTargets.Add(TargetOpposingThrall());
    }
    public override string Text()
    {
        return "<b>Activate: </b> Impale target opposing thrall.";
    }

    public override bool ActivationAvailable()
    {
        return true;
    }

    protected override void Activate(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Activate(targets, undo, state);
        targets[0].AddStatus(StatusName.IMPALE, 1);
    }
}
// ============================================ GREEN CARDS ===========================================

public class A_TerritorialBriar : Ability
{
    public A_TerritorialBriar(ITargetable user) : base(user)
    {
        user.opponent.actorEvents.onPlayCard += OnPlayCardHandler;
        user.opponent.targetEvents.onDeclareAttack += OnDeclareAttackHandler;
    }
    public override string Text()
    {
        string txt = "";
        txt += "When an opposing thrall attacks, it first takes 1 Piercing damage.";
        txt += "When your opponent uses an Ability, they first take 1 Piercing damage.";
        return txt;
    }

    private void OnDeclareAttackHandler(Card source, ITargetable target)
    {
        if (_user.inPlay)
        {
            Ability.Damage(new DamageData(1, Keyword.PIERCING, _user, source), false, null);
        }
    }

    private void OnPlayCardHandler(Card source)
    {
        if (_user.inPlay && source.type == Card.Type.ABILITY)
        {
            source.controller.Damage(new DamageData(1, Keyword.PIERCING, _user, source.controller));
        }
    }
}

public class A_CarnivorousPitfall : Ability
{
    public A_CarnivorousPitfall(ITargetable user) : base(user) { }

    public override string Text()
    {
        return "<b>Activate: </b> Add 2 <b>Pitfall Vine </b> to opponent's deck.";
    }

    public override bool ActivationAvailable()
    {
        return true;
    }
    protected override void Activate(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Activate(targets, undo, state);
        CardData data = Resources.Load("Cards/Fen/PitfallVine") as CardData;
        _user.opponent.deck.Shuffle(data);
        _user.opponent.deck.Shuffle(data);
    }
}

public class A_PitfallVine : Ability
{
    public override string Text()
    {
        return "When you draw this, take 2 Crushing damage. Then, put this into play under your opponent's control. Draw a card.";
    }
    public A_PitfallVine(ITargetable user) : base(user)
    {
        ((Card)user).cardEvents.onDraw += OnDrawHandler;
    }

    private void OnDrawHandler(Card card)
    {
        Actor owner = card.controller;
        Ability.Damage(new DamageData(2, Keyword.CRUSHING, card, card.controller), false, null);
        owner.PutInPlay(card, false);
        owner.Draw();
    }
}

public class A_BlossomingIvyProng : Ability
{
    public A_BlossomingIvyProng(ITargetable user) : base(user)
    {
        ((Card)user).cardEvents.onEnterPlay += EnterPlayHandler;
    }
    public override string Text()
    {
        return "When this enters play, gain 1 health.";
    }

    void EnterPlayHandler(Card card)
    {
        _user.controller.IncrementHealth(1);
    }
}

public class A_RampagingSwordtusk : Ability
{
    public A_RampagingSwordtusk(ITargetable user) : base(user)
    {
        activateTargets.Add(TargetOpposingThrall());
    }

    public override string Text()
    {
        return "<b>Activate:</b> Rampaging Swordtusk fights target opposing thrall.";
    }

    public override bool ActivationAvailable()
    {
        return true;
    }
    protected override void Activate(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Activate(targets, undo, state);
        Fight((Card)targets[0], undo, state);
    }
}

public class A_KyrnanosLordOfTheWild : Ability
{
    public A_KyrnanosLordOfTheWild(ITargetable user) : base(user)
    {
        ((Card)_user).cardEvents.onEnterPlay += EnterPlayHandler;
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
    }

    public override string Text()
    {
        if (_user.playerControlled)
        {
            return "While Kyrnanos Lord of the Wild is in play, beasts you control have -1 upkeep";
        }
        else
        {
            return "";
        }
    }

    public void EnterPlayHandler(Card self)
    {
        TargetTemplate t = new TargetTemplate();
        t.inPlay = true;
        t.isSelf = true;
        t.keyword = Keyword.BEAST;
        t.cardType = Card.Type.THRALL;
        TemplateModifier mod = new TemplateModifier(-1, Stat.Name.UPKEEP, t, (Card)_user);
        Dungeon.AddModifier(mod);
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
        playTargets.Add(TargetAnyOpposing());
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
    TargetTemplate _template;
    public A_IceElemental(ITargetable user) : base(user)
    {
        user.controller.actorEvents.onPlayCard += PlayCardHandler;
        TargetTemplate t = new TargetTemplate();
        t.isSelf = true;
        t.keyword = Keyword.ICE;
        t.cardType = Card.Type.SPELL;
        _template = t;
    }
    public override string Text()
    {
        return "When you play an Ice Spell, Ice Elemental gains 1 health.";
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
    }

    public void PlayCardHandler(Card card)
    {
        if (_user.inPlay && card.Compare(_template, _user.controller))
        {
            _user.IncrementHealth(1);
        }
    }
    
}

public class A_FrostLattice : Ability
{
    public A_FrostLattice(ITargetable user) : base(user) { }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        TargetTemplate t = TargetAnyOpposing();
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

public class A_Cryofall : Ability
{
    private TargetTemplate _template;
    public A_Cryofall(ITargetable user) : base(user)
    {
        user.controller.actorEvents.onPlayCard += PlayCardHandler;
        TargetTemplate t = new TargetTemplate();
        t.isSelf = true;
        t.keyword = Keyword.ICE;
        t.cardType = Card.Type.SPELL;
        _template = t;
    }

    public override string Text()
    {
        return "When you play an Ice Spell, a random opposing target is Impaled.";
    }
    private void PlayCardHandler(Card card)
    {
        if (_user.inPlay && card.Compare(_template, _user.controller))
        {
            ITargetable target = RandomValidTarget((Card)_user, TargetAnyOpposing());
            target.AddStatus(StatusName.IMPALE, 1);
        }
    }
}
// ============================================ MULTICOLOR CARDS ============================================

public class A_RideTheLightning : Ability
{
    private TargetTemplate _template;
    public A_RideTheLightning(ITargetable user) : base(user)
    {
        TargetTemplate t = new TargetTemplate();
        t.isSelf = true;
        t.cardType = Card.Type.SPELL;
        _template = t;
    }

    public override string Text()
    {
        return "When you play a Spell this turn, draw a card, gain 1 focus, and take 1 damage.";
    }

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        _user.controller.actorEvents.onPlayCard += PlayCardHandler;
        _user.controller.actorEvents.onEndTurn += EndTurnHandler;
    }

    private void PlayCardHandler(Card card)
    {
        if (card.Compare(_template, _user.controller))
        {
            _user.controller.Draw();
            if (_user.controller is Player)
            {
                ((Player)_user.controller).focus.baseValue += 1;
            }
            Damage(new DamageData(1, Keyword.LIGHTNING, _user, _user.controller), false, null);
        }
    }

    private void EndTurnHandler(Actor actor)
    {
        _user.controller.actorEvents.onPlayCard -= PlayCardHandler;
        _user.controller.actorEvents.onEndTurn -= EndTurnHandler;
    }
}

public class A_ChainLightning : Ability
{
    public A_ChainLightning(ITargetable user) : base(user)
    {
        playTargets.Add(TargetAnyOpposing());
    }

    public override string Text()
    {
        return "Chain Lightning deals 3 Lightning damage to any opposing target," +
            " 2 damage to a another random opposing target," +
            " and 1 damage to another random opposing target";
    }

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        Damage(new DamageData(3, Keyword.LIGHTNING, _user, targets[0]), undo, state);
        TargetTemplate t = TargetAnyOpposing(targets[0]);
        List<ITargetable> valid = ValidTargets((Card)_user, t);
        foreach (ITargetable option in valid)
        {
            Debug.Log("Valid Chain Target: " + option.name);
        }
        if (valid.Count == 0) { Debug.Log("Nothing to chain to."); return; }
        else
        {
            ITargetable t2 = RandomValidTarget((Card)_user, t);
            ITargetable t3 = RandomValidTarget((Card)_user, TargetAnyOpposing(t2));
            Damage(new DamageData(2, Keyword.LIGHTNING, _user, t2), undo, state);
            Damage(new DamageData(1, Keyword.LIGHTNING, _user, t3), undo, state);
        }
        
    }
}

public class A_Static : Ability
{
    public A_Static(ITargetable user) : base(user) { }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        TargetTemplate t = TargetAnyOpposing();
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Factory class
public static class AbilityIndex
{
    public static CardAbility Get(string id, Card user)
    {
        switch (id)
        {
            // colorless
            case "FATIGUE": return new A_Fatigue(user);

            // resources
            case "PASSION":
            case "REASON":
            case "PATIENCE":
                return new A_Ideal(user);
            case "ELIXIR_OF_PASSION":
            case "ELIXIR_OF_REASON":
            case "ELIXIR_OF_PATIENCE":
                return new A_Elixir(user);

            // tokens
            case "SLIMELING": return new A_TokenThrall(user);

            // red cards
            case "CINDER": return new A_Cinder(user);
            case "SINGE": return new A_Singe(user);
            case "BLITZ": return new A_Blitz(user);
            case "SLASH": return new A_Slash(user);
            case "VICIOUS_REND": return new A_ViciousRend(user);
            case "MIGHTY_CLEAVE": return new A_MightyCleave(user);
            case "FLURRY": return new A_Flurry(user);
            case "WILD_SWING": return new A_WildSwing(user);
            case "SEEING_RED": return new A_SeeingRed(user);
            case "INTO_THE_FRAY": return new A_IntoTheFray(user);
            case "IREBLADE": return new A_Ireblade(user);
            case "SHARPEN": return new A_Sharpen(user);
            case "EMBERFALL": return new A_Emberfall(user);
            case "EMBERSTRIKE": return new A_Emberstrike(user);
            case "IGNITE": return new A_Ignite(user);
            case "FLAMING_FLOURISH": return new A_FlamingFlourish(user);
            case "INFERNO_DJINN": return new A_InfernoDjinn(user);
            case "BLAZING_VORTEX": return new A_BlazingVortex(user);
            case "HYROC_CHAMPION": return new A_HyrocChampion(user);
            case "FERAL_WARCAT": return new A_FeralWarcat(user);

            // green cards
            case "TERRITORIAL_BRIAR": return new A_TerritorialBriar(user);
            case "CARNIVOROUS_PITFALL": return new A_CarnivorousPitfall(user);
            case "PITFALL_VINE": return new A_PitfallVine(user);
            case "BLOSSOMING_IVYPRONG": return new A_BlossomingIvyProng(user);
            case "RAMPAGING_SWORDTUSK": return new A_RampagingSwordtusk(user);
            case "KYRNANOS": return new A_KyrnanosLordOfTheWild(user);
            case "EQUANIMITY": return new A_Equanimity(user);
            case "GENESIS_SPRING": return new A_GenesisSpring(user);
            case "RICOCHET": return new A_Ricochet(user);
            case "CONCUSSIVE_SHOT": return new A_ConcussiveShot(user);
            case "RANGERS_JUDGEMENT": return new A_RangersJudgement(user);
            case "PACK_WOLF_ALPHA": return new A_PackWolfAlpha(user);
            case "CONSUMING_BLOB": return new A_ConsumingBlob(user);
            case "FENS_BLESSING": return new A_FensBlessing(user);
            case "MITOTIC_SLIME": return new A_MitoticSlime(user);
            case "STONESUNDER": return new A_Stonesunder(user);
            case "FISSURE": return new A_Fissure(user);
            case "REJUVENATE": return new A_Rejuvenate(user);
            case "CONSUME_ADRENALINE": return new A_ConsumeAdrenaline(user);
            case "HOWL_OF_THE_PACK": return new A_HowlOfThePack(user);

            // blue cards
            case "CONTINUITY": return new A_Continuity(user);
            case "CHILL":   return new A_Chill(user);
            case "RIMESHIELD": return new A_Rimeshield(user);
            case "CRYSTAL_LANCE": return new A_CrystalLance(user);
            case "DRIFTING_VOIDLING": return new A_DriftingVoidling(user);
            case "ICE_ELEMENTAL": return new A_IceElemental(user);
            case "MANAPLASM": return new A_Manaplasm(user);
            case "PRIMORDIAL_SLIME": return new A_PrimordialSlime(user);
            case "FROST_LATTICE": return new A_FrostLattice(user);
            case "RIME_SPRITE": return new A_RimeSprite(user);
            case "TIME_DILATION": return new A_TimeDilation(user);
            case "LEGERDEMAIN": return new A_Legerdemain(user);
            case "MNEMONIC_RECITATION": return new A_MnemonicRecitation(user);
            case "SHARDFALL": return new A_Shardfall(user);
            case "SHARDSTRIKE": return new A_Shardstrike(user);
            case "HAIL_OF_SPLINTERS": return new A_HailOfSplinters(user);
            case "RIDE_THE_LIGHTNING": return new A_RideTheLightning(user);
            case "CHAIN_LIGHTNING": return new A_ChainLightning(user);
            case "STATIC": return new A_Static(user);
            case "KATABATIC_SQUALL": return new A_KatabaticSquall(user);
            case "EPIPHANY": return new A_Epiphany(user);

            
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
        CHANNEL
    }

    protected ITargetable _user;
    protected int _channelCost = 0;
    protected bool _react = false;

    private List<TargetTemplate> _activateTargets;
    private List<TargetTemplate> _playTargets;
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
    public bool react { get { return _react; } }
    public Ability(ITargetable user)
    {
        _user = user;
        _react = false;
    }
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

    protected virtual void Channel() { }

    protected void Attack(ITargetable target, bool undo = false, GameState state = null)
    {
        Debug.Assert(_user is Card);
        Card user = (Card)_user;
        user.controller.targetEvents.DeclareAttack(user, target);
        user.targetEvents.DeclareAttack(user, target);

        if (!user.inPlay || user.endurance.value <= 0) { return; }

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
        else
        {
            user.attackAvailable = false;
            user.activationAvailable = false;
        }
    }

    protected void Fight(Card target, bool undo = false, GameState state = null)
    {
        Debug.Assert(_user is Card);
        Card user = (Card)_user;
        //user.controller.targetEvents.DeclareAttack(user, target);
        //user.targetEvents.DeclareAttack(user, target);

        if (!user.inPlay || user.endurance.value <= 0) { return; }

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
                if (!((Card)_user).HasKeyword(KeywordAbility.Key.NIMBLE))
                {
                    t.isAttackable = true;
                }
                t.isDamageable = true;
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
            case Mode.CHANNEL: Channel(); break;
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
        Dungeon.ClearTargeter();
        switch (mode)
        {
            case Mode.PLAY:
                Card user = _user as Card;
                user.Move(Dungeon.GetZone(CardZone.Type.MAGNIFY));
                user.FaceUp(true, true);
                user.particles.ClearShimmer();
                if (targets!= null && targets.Count > 0)
                {
                    Dungeon.ShowTargeter(Dungeon.GetZone(CardZone.Type.MAGNIFY).transform, targets[0].transform);
                }
                break;
            case Mode.ATTACK:
                Dungeon.ShowTargeter(_user, targets[0]);
                break;
            case Mode.ACTIVATE:
                if (_user is Card)
                {
                }
                if (targets != null && targets.Count > 0)
                {
                    Dungeon.ShowTargeter(_user, targets[0]);
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

    public void StartChannel()
    {
        _user.controller.targetEvents.onTakeDamage += ChannelDamageHandler;
        _channelCost = 0;
    }

    private void ChannelDamageHandler(DamageData data)
    {
        Debug.Log("bonus cost: " + _channelCost);
        Card user = _user as Card;
        _channelCost++;
        user.upkeep.RemoveModifiersFromSource(user as Object);
        user.AddModifier(new StatModifier(_channelCost, Stat.Name.UPKEEP, user));
        if (!_user.playerControlled && _channelCost >= 5)
        {
            ((Card)_user).Destroy();
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
            //data.target.ResolveDamage(data);
        }
    }
    public TargetTemplate TargetAnyOpposing(ITargetable ignore = null)
    {
        TargetTemplate t = new TargetTemplate();
        t.isDamageable = true;
        t.isOpposing = true;
        t.inPlay = true;
        t.isNot = ignore;
        return t;
    }
    public TargetTemplate TargetOpposingThrall()
    {
        TargetTemplate t = new TargetTemplate();
        t.isOpposing = true;
        t.inPlay = true;
        t.cardType.Add(Card.Type.THRALL);
        return t;
    }
    public static List<ITargetable> ValidOpposingTargets(Card _user, TargetTemplate template)
    {
        List<ITargetable> validTargets = new List<ITargetable>();
        
        if (_user.opponent.Compare(template, _user.controller)) { validTargets.Add(_user.opponent); }
        foreach (Card card in _user.opponent.active)
        {
            if (card.Compare(template, _user.controller))
            {
                validTargets.Add(card);
            }
        }
        return validTargets;
    }
    public static ITargetable RandomOpposing(Card _user, TargetTemplate template)
    {
        List < ITargetable > valid = ValidOpposingTargets(_user, template);
        if (valid.Count == 0) { return null; }
        return valid[Random.Range(0, valid.Count)];
    }
}

public abstract class CardAbility : Ability
{
    protected Card user { get { return _user as Card; } }
    public CardAbility(Card user) : base(user) { }
}

public abstract class ActorAbility : Ability
{
    protected Actor user { get { return _user as Actor; } }
    public ActorAbility(Actor user) : base(user) { }
}

public class A_Null : CardAbility
{
    public A_Null(Card user) : base(user) { }
    public override string Text()
    {
        return "";
    }
}
public class A_Elixir : CardAbility
{
    public A_Elixir(Card user) : base(user) { }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        
    }
    public override string Text()
    {
        return "Gain 1/1 Focus.";
    }
}
public class A_Ideal : CardAbility
{
    TargetTemplate _template;
    public A_Ideal(Card user) : base(user)
    {
        _template = new TargetTemplate();
        _template.cardType.Add(Card.Type.IDEAL);
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
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
                if (((Player)user.controller).NumPlayedThisTurn(_template) == 0)
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

public class A_Fatigue : CardAbility
{
    public A_Fatigue(Card user) : base(user)
    { }

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);

        if (user.controller is Player)
        {
            user.controller.Draw();
        }
    }

    public override string Text()
    {
        string txt = "";
        if (_user.controller is Player) { txt += "Draw a card."; }
        txt += "\n<i>This is what I get for skipping leg day.</i>";
        return txt;
    }
}

public class A_TokenThrall : CardAbility
{
    public A_TokenThrall(Card user) : base(user)
    {
        user.targetEvents.onDealDamage += DealDamageHandler;
    }

    public override string Text()
    {
        string txt = "When this deals damage, destroy this.";
        return txt;
    }

    private void DealDamageHandler(DamageData data)
    {
        user.Destroy();
    }
}


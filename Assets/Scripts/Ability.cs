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
                return new A_Ideal_0(user);
            case "ZEAL":
            case "LOGIC":
            case "PERSEVERANCE":
                return new A_Ideal_1(user);
            case "OBSESSION":
            case "ACUITY":
            case "FORTITUDE":
                return new A_Ideal_2(user);
            case "ELIXIR_OF_PASSION":
            case "ELIXIR_OF_REASON":
            case "ELIXIR_OF_PATIENCE":
                return new A_Elixir(user);

            // red cards

            case "BLITZ": return new A_Blitz(user);
            case "SLASH": return new A_Slash(user);
            case "VICIOUS_REND": return new A_ViciousRend(user);
            case "MIGHTY_CLEAVE": return new A_MightyCleave(user);
            case "FLURRY": return new A_Flurry(user);
            case "WILD_SWING": return new A_WildSwing(user);
            case "SEEING_RED": return new A_SeeingRed(user);
            case "INTO_THE_FRAY": return new A_IntoTheFray(user);
            case "FERAL_THRASH": return new A_FeralThrash(user);
            case "SEVER": return new A_Sever(user);
            case "SKY_SUNDER": return new A_SkySunder(user);
            case "SHARPEN": return new A_Sharpen(user);
            case "HYROC_CHAMPION": return new A_HyrocChampion(user);
            case "RELENTLESS": return new A_Relentless(user);
            case "HYROC_SCOUT": return new A_HyrocScout(user);
            case "SKYSWORN_ADEPT": return new A_SkyswornAdept(user);
            case "HYROC_STORMTAMER": return new A_HyrocStormtamer(user);
            case "HORIX": return new A_Horix(user);
            case "SPINETAIL": return new A_Spinetail(user);
            case "FEATHER_PHALANX": return new A_FeatherPhalanx(user);
            case "ARCING_BOLT": return new A_ArcingBolt(user);
            case "FULMINATE": return new A_Fulminate(user);
            case "CHAIN_LIGHTNING": return new A_ChainLightning(user);

            case "CINDER": return new A_Cinder(user);
            case "SINGE": return new A_Singe(user);
            case "INFERNO_DJINN": return new A_InfernoDjinn(user);
            case "BLAZING_VORTEX": return new A_BlazingVortex(user);
            case "FERAL_WARCAT": return new A_FeralWarcat(user);
            case "IGNITE": return new A_Ignite(user);
            case "FLAMING_FLOURISH": return new A_FlamingFlourish(user);

            // green cards
            case "TERRITORIAL_BRIAR": return new A_TerritorialBriar(user);
            case "BLOSSOMING_IVYPRONG": return new A_BlossomingIvyProng(user);
            case "RAMPAGING_SWORDTUSK": return new A_RampagingSwordtusk(user);
            
            case "PACK_MENTALITY": return new A_PackMentality(user);
            
            case "SHADOW_FANG_ALPHA": return new A_ShadowFangAlpha(user);
            case "SHADOW_FANG_AMBUSHER": return new A_ShadowFangAmbusher(user);
            case "LOYAL_SHADOW_FANG": return new A_LoyalShadowFang(user);
            case "CONSUMING_BLOB": return new A_ConsumingBlob(user);
            case "MITOTIC_SLIME": return new A_MitoticSlime(user);
            case "AMORPHOUS_DEVOURER": return new A_AmorphousDevourer(user);
            case "EMPOWER": return new A_Empower(user);
            case "VERDURE": return new A_Verdure(user);
            case "REJUVENATE": return new A_Rejuvenate(user);
            case "ADRENALINE_RUSH": return new A_AdrenalineRush(user);
            case "ECHOING_HOWL": return new A_EchoingHowl(user);
            case "IVYPRONG_SPIRITCALLER": return new A_IvyprongSpiritcaller(user);
            case "WILL_OF_THE_WILD": return new A_WillOfTheWild(user);
            case "PRIMAL_STRENGTH": return new A_PrimalStrength(user);
            case "FIRST_AID": return new A_FirstAid(user);
            case "SURVIVALISM": return new A_Survivalism(user);
            case "OBSTINATE_OAKENWOLD": return new A_ObstinateOakenwold(user);
            case "WEATHERED_WILLOWOLD": return new A_WeatheredWillowold(user);
            case "LUMBERING_BANEBOUGH": return new A_LumberingBanebough(user);
            case "TITAN_REDWOLD": return new A_TitanRedwold(user);
            case "ERIDEA": return new A_Eridea(user);
            case "DRYAD_WOODWEAVER": return new A_DryadWoodweaver(user);
            case "DRYAD_MENDER": return new A_DryadMender(user);

            case "KYRNANOS": return new A_Kyrnanos(user);
            case "EQUANIMITY": return new A_Equanimity(user);
            case "STONESUNDER": return new A_Stonesunder(user);
            case "FISSURE": return new A_Fissure(user);
            case "RICOCHET": return new A_Ricochet(user);
            case "PIERCING_BOLT": return new A_PiercingBolt(user);
            case "CONCUSSIVE_ARROW": return new A_ConcussiveArrow(user);
            case "MASTERFUL_SHOT": return new A_MasterfulShot(user);


            // blue cards
            case "CONTINUITY": return new A_Continuity(user);
            case "CHILL":   return new A_Chill(user);
            case "RIMESHIELD": return new A_Rimeshield(user);
            case "CRYSTAL_LANCE": return new A_CrystalLance(user);
            case "DRIFTING_VOIDLING": return new A_DriftingVoidling(user);
            case "GLACIER_COLOSSUS": return new A_GlacierColossus(user);
            case "MANAPLASM": return new A_Manaplasm(user);
            case "PRIMORDIAL_SLIME": return new A_PrimordialSlime(user);
            case "FROST_LATTICE": return new A_FrostLattice(user);
            case "RIME_SPRITE": return new A_RimeSprite(user);
            case "TIME_DILATION": return new A_TimeDilation(user);
            case "LEGERDEMAIN": return new A_Legerdemain(user);
            case "MNEMONIC_RECITATION": return new A_MnemonicRecitation(user);
            case "STORMCALL": return new A_Stormcall(user);
            case "PIERCING_HAIL": return new A_PiercingHail(user);
            case "HAIL_OF_SPLINTERS": return new A_HailOfSplinters(user);
            case "RIDE_THE_LIGHTNING": return new A_RideTheLightning(user);
            case "KATABATIC_SQUALL": return new A_KatabaticSquall(user);
            case "EPIPHANY": return new A_Epiphany(user);
            case "MOMENT_OF_CLARITY": return new A_MomentOfClarity(user);
            case "WIND_SCREAMER": return new A_WindScreamer(user);
            case "REPLICATE": return new A_Replicate(user);
            case "PRISMATIC_GOLEM": return new A_PrismaticGolem(user);
            
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
        ATTACK
    }

    protected ITargetable _user;
    protected int _channelCost = 0;

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
    public Ability(ITargetable user)
    {
        _user = user;
    }
    public virtual bool ActivationAvailable() { return false; }
    protected virtual void Activate(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        Debug.Assert(_user is Card);
        if (state != null)
        {

        } else
        {
            ((Card)_user).activationAvailable = false;
            ((Card)_user).attackAvailable = false;
        }
    }
    protected virtual void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        Debug.Assert(_user is Card);
        Card user = (Card)_user;
        
        if (state != null)
        {
            if (user.type == Card.Type.THRALL || user.type == Card.Type.CONSTANT)
            {
                state.PutCardInPlay(user, undo);
            }
        }
    }
    protected void Attack(ITargetable target, bool undo = false, GameState state = null)
    {
        Debug.Assert(_user is Card);
        Card user = (Card)_user;

        DamageData _userDamage = new DamageData(user.power.value, user.damageType, _user, target, true);
        DamageData targetDamage = null;

        if (state == null)
        {
            user.controller.targetEvents.DeclareAttack(user, target);
            user.targetEvents.DeclareAttack(user, target);
        }
        if (!user.inPlay || user.endurance.value <= 0) { return; }
        if (target is Card)
        {
            Card targetCard = (Card)target;
            if (!targetCard.inPlay || targetCard.endurance.value <= 0) { return; }
            targetDamage = new DamageData(targetCard.power.value, targetCard.damageType, targetCard, user);
        }
        Ability.Damage(_userDamage, undo, state);
        Ability.Damage(targetDamage, undo, state);
        if (state == null)
        {
            user.attackAvailable = false;
            user.activationAvailable = false;
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
                    ((Card)_user).particles.GlowGold();
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
            default: return 0;
        }
    }

    public static void Fight(Card aggressor, Card target, bool undo = false, GameState state = null)
    {
        if (!aggressor.inPlay || aggressor.endurance.value <= 0) { return; }

        DamageData _userDamage = new DamageData(aggressor.power.value, aggressor.damageType, aggressor, target);
        DamageData targetDamage = null;
        if (target is Card)
        {
            Card targetCard = (Card)target;
            targetDamage = new DamageData(targetCard.power.value, targetCard.damageType, targetCard, aggressor);
        }

        Ability.Damage(_userDamage, undo, state);
        Ability.Damage(targetDamage, undo, state);
        //target.Damage(_userDamage);
        //user.Damage(targetDamage);
        if (state == null)
        {
            target.ResolveDamage(_userDamage);
            aggressor.ResolveDamage(targetDamage);
        }
    }
    public static void AddFocus(Actor actor, int focus, int maxFocus, bool undo = false, GameState state = null)
    {
        if (state != null)
        {

        } else
        {
            actor.AddFocus(focus, maxFocus);
        }
    }
    public static void Damage(DamageData data, bool undo = false, GameState state = null)
    {
        if (data == null) { return; }
        if (state != null)
        {
            state.Damage(data, undo);
        } else
        {
            data.target.Damage(data);
        }
    }
    public static void Heal(ITargetable target, int value, bool undo = false, GameState state = null)
    {
        Debug.Assert(value >= 0);

        if (state != null)
        {
            if (target is Actor) { state.Heal((Actor)target, value, undo); }
            else if (target is Card) { state.Heal((Card)target, value, undo); }
        } else
        {
            target.IncrementHealth(value);
        }
    }
    public static void Cleanse(ITargetable target, bool undo = false, GameState state = null)
    {
        foreach (StatusEffect.ID status in target.GetAllStatus())
        {
            if (StatusData.Get(status).isDebuff)
            {
                Ability.Status(target, status, -9999, undo, state);
            }
        }
    }
    public static void Status(ITargetable target, StatusEffect.ID id, int stacks, bool undo = false, GameState state = null)
    {
        if (target is null) { return; }
        if (state != null)
        {
            state.Status(target, id, stacks, undo);
        } else
        {
            target.AddStatus(id, stacks);
        }
    }
    public static void Draw(Actor actor, int n, bool undo = false, GameState state = null)
    {
        if (state != null)
        {
            state.Draw(actor, n, undo);
        } else
        {
            actor.Draw(n);
        }
        
    }
    public static Card CreateCard(Actor controller, CardData data, Vector3 spawn, CardZone.Type zone, bool undo = false, GameState state = null)
    {
        if (state != null)
        {
            switch(zone)
            {
                case CardZone.Type.HAND: state.AddCardToHand(controller, undo); break;
                default: break;
            }
            return null;
        } else
        {
            Card card = Card.Spawn(data, controller.playerControlled, spawn);
            switch (zone)
            {
                case CardZone.Type.HAND: controller.AddToHand(card); break;
                case CardZone.Type.ACTIVE: controller.PutInPlay(card); break;
            }
            return card;
        }
    }
    public static void DestroyCard(Card card, bool undo = false, GameState state = null)
    {
        if (state != null)
        {
            state.RemoveCardFromPlay(card, undo);
        } else
        {
            card.Destroy();
        }
    }
    public static void ShuffleIntoDeck(Card card, bool undo = false, GameState state = null)
    {
        if (state != null)
        {
            if (card.inPlay)
            {
                state.RemoveCardFromPlay(card, undo);
            }
        }
        else
        {
            card.controller.deck.Shuffle(card);
        }
    }
    public static void AddCardToHand(Card card, bool undo = false, GameState state = null)
    {
        if (state != null)
        {
            state.RemoveCardFromPlay(card, undo);
            state.AddCardToHand(card.controller, undo);
        } else
        {
            card.controller.AddToHand(card);
        }
    }
    public static void AddStatModifier(Card card, StatModifier mod, bool undo = false, GameState state = null)
    {
        if (state != null)
        {
            state.AddStatModifier(card, mod, undo);
        } else
        {
            Debug.Log("Adding Stat Modifier");
            mod.SetTarget(card);
        }
    }
    public static void RemoveStatModifier(Card card, StatModifier mod, bool undo = false, GameState state = null)
    {
        if (state != null)
        {
            state.RemoveStatModifier(card, mod, undo);
        }
        else
        {
            mod.RemoveTarget(card);
        }
    }
    public static void AddTemplateModifier(TemplateModifier mod, bool undo = false, GameState state = null)
    {
        if (state != null)
        {
            state.AddTemplateModifier(mod, undo);
        } else
        {
            Dungeon.AddModifier(mod);
        }
    }
    public static void RemoveTemplateModifier(TemplateModifier mod, bool undo = false, GameState state = null)
    {
        if (state != null)
        {
            state.RemoveTemplateModifier(mod, undo);
        }
        else
        {
            Dungeon.RemoveModifier(mod);
        }
    }
    public static void AddAffinity(Actor target, Card.Color color, int amount, bool undo = false, GameState state = null)
    {
        if (target is Player)
        {
            Player player = target as Player;
            if (state != null)
            {

            }
            else
            {
                player.addAffinity(color, amount);
            }

        }
    }
    public static void Harmonize(Actor target, Card.Color color, bool undo = false, GameState state = null)
    {
        if (state != null)
        {

        } else
        {
            if (!(target is Player)) { return; }
            foreach (Card card in target.hand)
            {
                if (card.data.color != color) { return; }
            }
            Player.instance.addAffinity(color, 1);
        }
    }
    public static void Attune(Actor target, Card.Color color, bool undo = false, GameState state = null)
    {
        if (state != null)
        {

        } else
        {
            if (!(target is Player)) { return; }
            foreach (Card card in Player.instance.hand)
            {
                if (card.Affinity(color) > Player.instance.Affinity(color))
                {
                    Player.instance.addAffinity(color, 1);
                    break;
                }
            }
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
    public TargetTemplate TargetAnyFriendly(ITargetable ignore = null)
    {
        TargetTemplate t = new TargetTemplate();
        t.isDamageable = true;
        t.isSelf = true;
        t.inPlay = true;
        t.isNot = ignore;
        return t;
    }
    public TargetTemplate TargetAnyThrall(ITargetable ignore = null)
    {
        TargetTemplate t = new TargetTemplate();
        t.cardType.Add(Card.Type.THRALL);
        t.inPlay = true;
        t.isNot = ignore;
        return t;
    }
    public TargetTemplate TargetOpposingThrall(ITargetable ignore = null)
    {
        TargetTemplate t = new TargetTemplate();
        t.isOpposing = true;
        t.inPlay = true;
        t.cardType.Add(Card.Type.THRALL);
        t.isNot = ignore;
        return t;
    }
    public TargetTemplate TargetFriendlyThrall(ITargetable ignore = null)
    {
        TargetTemplate t = new TargetTemplate();
        t.isSelf = true;
        t.inPlay = true;
        t.cardType.Add(Card.Type.THRALL);
        t.isNot = ignore;
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
}
public class A_Elixir : CardAbility
{
    public A_Elixir(Card user) : base(user) { }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        
    }
}
public class A_Ideal_0 : CardAbility
{
    public A_Ideal_0(Card user) : base(user)
    {
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        user.controller.Draw();
        if (user.controller is Player)
        {
            ((Player)user.controller).maxFocus.baseValue += 1;
        }
    }
}
public class A_Ideal_1 : CardAbility
{
    public A_Ideal_1(Card user) : base(user)
    {
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        Ability.Draw(user.controller, 1, undo, state);
        if (user.playerControlled)
        {
            Player.instance.addAffinity(user.data.color, 1);
            Ability.Harmonize(user.controller, user.data.color, undo, state);
        }
    }
}
public class A_Ideal_2 : CardAbility
{
    public A_Ideal_2(Card user) : base(user)
    {
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        if (user.playerControlled)
        {
            Player.instance.addAffinity(user.data.color, 1);
            user.controller.Draw();
        }
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
}


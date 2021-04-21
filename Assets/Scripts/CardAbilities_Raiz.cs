using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_Slash : CardAbility
{
    public A_Slash(Card user) : base(user)
    {
        playTargets.Add(TargetAnyOpposing());
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        int damage = user.values[0].value;
        DamageData data = new DamageData(damage, Keyword.SLASHING, _user, targets[0]);
        Ability.Damage(data, undo, state);
    }
}
public class A_ViciousRend : CardAbility
{
    public A_ViciousRend(Card user) : base(user)
    {
        playTargets.Add(TargetAnyOpposing());
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        int damage = user.values[0].value;
        DamageData data = new DamageData(damage, Keyword.SLASHING, _user, targets[0]);
        Ability.Damage(data, undo, state);
    }
}
public class A_WildSwing : CardAbility
{
    public A_WildSwing(Card user) : base(user)
    {
        playTargets.Add(TargetAnyOpposing());
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        int damage_1 = user.values[0].value;
        int damage_2 = user.values[1].value;
        ITargetable t1 = (ITargetable)targets[0];
        ITargetable t2 = RandomOpposing(user, TargetAnyOpposing(t1));

        DamageData d1 = new DamageData(damage_1, Keyword.SLASHING, _user, t1);
        Ability.Damage(d1, undo, state);
        if (t2 != null)
        {
            DamageData d2 = new DamageData(damage_2, Keyword.SLASHING, _user, t2);
            Ability.Damage(d2, undo, state);
        }
    }

}
public class A_MightyCleave : CardAbility
{
    public A_MightyCleave(Card user) : base(user)
    {
        playTargets.Add(TargetAnyOpposing());
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        int damage = user.values[0].value;
        ITargetable target = (ITargetable)targets[0];
        DamageData data = new DamageData(damage, Keyword.SLASHING, _user, target);
        Ability.Damage(data, undo, state);
    }
}
public class A_Sever : CardAbility
{
    public A_Sever(Card user) : base(user)
    {
        playTargets.Add(TargetAnyOpposing());
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        int damage = ((Player)user.controller).Affinity(Card.Color.RAIZ);
        ITargetable target = (ITargetable)targets[0];
        DamageData data = new DamageData(damage, Keyword.SLASHING, _user, target);
        Ability.Damage(data, undo, state);
    }
}
public class A_SkySunder : CardAbility
{
    public A_SkySunder(Card user) : base(user)
    {
        playTargets.Add(TargetAnyOpposing());
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        int damage = user.values[0].value;
        int splash = user.values[1].value;
        ITargetable target = targets[0];
        DamageData data = new DamageData(damage, Keyword.SLASHING, _user, target);
        Ability.Damage(data, undo, state);
        List < Card > opposing = user.opponent.active;
        foreach (Card card in opposing)
        {
            if (card == (Object)target) { continue; }
            if (card.type == Card.Type.THRALL)
            {
                DamageData splashDamage = new DamageData(splash, Keyword.SLASHING, _user, card);
                Ability.Damage(splashDamage, undo, state);
            }
        }
        if (user.opponent != (Object)target)
        {
            DamageData splashDamage = new DamageData(splash, Keyword.SLASHING, _user, user.opponent);
            Ability.Damage(splashDamage, undo, state);
        }
    }
}
public class A_Flurry : CardAbility
{
    private TargetTemplate _template;
    public A_Flurry(Card user) : base(user)
    {
        playTargets.Add(TargetAnyOpposing());
        _template = new TargetTemplate();
        _template.cardType.Add(Card.Type.TECHNIQUE);

        user.cardEvents.onDraw += DrawHandler;

    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        int damage = user.values[0].value;
        DamageData data = new DamageData(damage, Keyword.SLASHING, _user, targets[0]);
        Ability.Damage(data, undo, state);
    }
    private void DrawHandler(Card card)
    {
        _user.controller.actorEvents.onPlayCard += PlayedCardHandler;
    }
    private void PlayedCardHandler(Card card)
    {
        if (user.playedThisTurn && card.Compare(_template, _user.controller))
        {
            Ability.AddCardToHand(user);
        }
    }
}
public class A_Blitz : CardAbility
{
    public A_Blitz(Card user) : base(user)
    {
        user.cardEvents.onCycle += CycleHandler;
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        int focusPerCard = user.values[0].value;
        Ability.Draw(user.controller, 1, undo, state);
        int n = 0;
        foreach (Card card in user.controller.hand)
        {
            if (card.type == Card.Type.TECHNIQUE) { n += 1; }
        }
        Ability.AddFocus(user.controller, n * focusPerCard, 0, undo, state);
    }
    private void CycleHandler(Card card)
    {
        int passiveFocus = user.values[1].value;
        Ability.AddFocus(user.controller, passiveFocus, 0);
    }
}
public class A_Sharpen : CardAbility
{
    public A_Sharpen(Card user) : base(user) { }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        _user.controller.targetEvents.onDealRawDamage += RawDamageHandler;
        _user.opponent.actorEvents.onEndTurn += EndTurnHandler;
        Ability.Draw(user.controller, 1, undo, state);
        Ability.Status(user.controller, StatusEffect.ID.SHARPEN, 1, undo, state);
    }

    void RawDamageHandler(DamageData data)
    {
        int bonusDamage = user.values[0].value;
        if (data.type == Keyword.SLASHING) { data.damage += bonusDamage; }
    }

    void EndTurnHandler(Actor actor)
    {
        _user.controller.targetEvents.onDealRawDamage -= RawDamageHandler;
        _user.opponent.actorEvents.onEndTurn -= EndTurnHandler;
        _user.controller.RemoveStatus(StatusEffect.ID.SHARPEN, 1);
    }
}
public class A_Relentless : CardAbility
{
    public A_Relentless(Card user) : base(user)
    {

    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        Ability.Draw(user.controller, 1, undo, state);
        int alacrityStacks = user.values[0].value;
        foreach (Card card in user.controller.hand)
        {
            if (card.type == Card.Type.TECHNIQUE)
            {
                Ability.Status(user.controller, StatusEffect.ID.ALACRITY, alacrityStacks, undo, state);
                return;
            }
        }
    }
}
public class A_SeeingRed : CardAbility
{
    public A_SeeingRed(Card user) : base(user)
    {
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        int frenzyStacks = user.values[0].value;
        Ability.Status(user.controller, StatusEffect.ID.FRENZY, frenzyStacks, undo, state);
        Ability.Draw(user.controller, 1, undo, state);
    }
}
public class A_IntoTheFray : CardAbility
{
    public A_IntoTheFray(Card user) : base(user)
    {
        user.cardEvents.onCycle += CycleHandler;
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        int frenzyStacks = user.values[0].value;
        Ability.Status(user.controller, StatusEffect.ID.FRENZY, frenzyStacks, undo, state);
    }
    private void CycleHandler(Card card)
    {
        int passiveFrenzy = user.values[1].value;
        Ability.Status(user.controller, StatusEffect.ID.FRENZY, passiveFrenzy);
    }
}
public class A_FeralThrash : CardAbility
{
    private StatModifier _mod;
    public A_FeralThrash(Card user) : base(user)
    {
        playTargets.Add(TargetAnyOpposing());
        user.targetEvents.onRefresh += RefreshHandler;
        user.cardEvents.onCycle += CycleHandler;
        _mod = new StatModifier(0, Stat.Name.COST, user);
        user.AddModifier(_mod);
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        int damage = user.values[0].value;
        DamageData data = new DamageData(damage, Keyword.SLASHING, _user, targets[0]);
        Ability.Damage(data, undo, state);
    }

    private void CycleHandler(Card card)
    {
        Ability.Status(user.controller, StatusEffect.ID.FRENZY, 1);
    }

    private void RefreshHandler()
    {
        _mod.value = -user.controller.GetStatus(StatusEffect.ID.FRENZY);
    }
}
public class A_Spinetail : CardAbility
{
    public A_Spinetail(Card user) : base(user)
    {
        activateTargets.Add(TargetOpposingThrall());
    }

    public override bool ActivationAvailable()
    {
        return true;
    }

    protected override void Activate(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Activate(targets, undo, state);
        Ability.Status(targets[0], StatusEffect.ID.IMPALE, 1, undo, state);
    }
}
public class A_HyrocChampion : CardAbility
{
    private StatModifier _mod;
    public A_HyrocChampion(Card user) : base(user)
    {
        user.controller.actorEvents.onPlayCard += PlayCardHandler;
        user.controller.actorEvents.onEndTurn += EndTurnHandler;
        user.cardEvents.onEnterPlay += EnterPlayHandler;
        _mod = new StatModifier(0, Stat.Name.UPKEEP, user);
        user.AddModifier(_mod);
    }
    private void EnterPlayHandler(Card card)
    {
        _mod.value = 0;
    }
    private void PlayCardHandler(Card card)
    {
        if (user.inPlay && card.type == Card.Type.TECHNIQUE)
        {
            _mod.value -= 1;
        }
    }
    private void EndTurnHandler(Actor actor)
    {
        _mod.value = 0;
    }
}
public class A_HyrocScout : CardAbility
{
    public A_HyrocScout(Card user) : base(user)
    {
        user.targetEvents.onDealDamage += DealDamageHandler;
    }
    private void DealDamageHandler(DamageData data)
    {
        if (data != null && ((data.target as Object) == user.controller.opponent))
        {
            user.controller.Draw();
            user.controller.deck.Shuffle(user);
        }
    }
}
public class A_SkyswornAdept : CardAbility
{
    public A_SkyswornAdept(Card user) : base(user)
    {
        activateTargets.Add(TargetAnyOpposing());
    }
    public override bool ActivationAvailable()
    {
        return true;
    }
    protected override void Activate(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        int damage = user.values[0].value;
        Ability.Damage(new DamageData(damage, Keyword.LIGHTNING, user, targets[0]));
    }
}
public class A_HyrocStormtamer : CardAbility
{
    TargetTemplate _template;
    public A_HyrocStormtamer(Card user) : base(user)
    {
        _template = new TargetTemplate();
        _template.cardType.Add(Card.Type.SPELL);
        _template.keywordOr.Add(Keyword.ICE);
        _template.keywordOr.Add(Keyword.LIGHTNING);
    }
    public override bool ActivationAvailable()
    {
        return true;
    }
    protected override void Activate(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Activate(targets, undo, state);
        CardData data = CardIndex.Rand(_template);
        Card card = Ability.CreateCard(user.controller, data, user.transform.position, CardZone.Type.HAND);
        Ability.Status(card, StatusEffect.ID.ALACRITY, 1);
        card.AddKeywordAbility(KeywordAbility.Key.EPHEMERAL);
    }
}
public class A_Horix : CardAbility
{
    public A_Horix(Card user) : base(user)
    {
        _user.controller.targetEvents.onDealRawDamage += RawDamageHandler;
    }

    void RawDamageHandler(DamageData data)
    {
        Debug.Log("RawDamageHandler | damageType = " + data.type);
        int bonusDamage = user.values[0].value;
        if (data.type == Keyword.LIGHTNING)
        {data.damage += bonusDamage; }
    }
}
public class A_ArcingBolt : CardAbility
{
    public A_ArcingBolt(Card user) : base(user) { }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        TargetTemplate t = TargetAnyOpposing();
        ITargetable t1 = RandomOpposing(user, t);
        ITargetable t2 = RandomOpposing(user, t);
        Ability.Damage(new DamageData(1, Keyword.LIGHTNING, _user, t1), undo, state);
        Ability.Damage(new DamageData(1, Keyword.LIGHTNING, _user, t2), undo, state);
    }
}
public class A_Fulminate : CardAbility
{
    public A_Fulminate(Card user) : base(user)
    {
        playTargets.Add(TargetAnyOpposing());
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        int damage = user.values[0].value;
        DamageData data = new DamageData(damage, Keyword.LIGHTNING, _user, targets[0]);
        Ability.Damage(data, undo, state);
    }
}
public class A_ChainLightning : CardAbility
{
    public A_ChainLightning(Card user) : base(user)
    {
        playTargets.Add(TargetAnyOpposing());
    }

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        int d1 = user.values[0].value;
        int d2 = user.values[1].value;
        int d3 = user.values[2].value;
        Ability.Damage(new DamageData(d1, Keyword.LIGHTNING, _user, targets[0]), undo, state);

        ITargetable t2 = RandomOpposing((Card)_user, TargetAnyOpposing(targets[0]));
        ITargetable t3 = RandomOpposing((Card)_user, TargetAnyOpposing(t2));
        if (t2 == null) { return; }
        Ability.Damage(new DamageData(d2, Keyword.LIGHTNING, _user, t2), undo, state);
        Ability.Damage(new DamageData(d3, Keyword.LIGHTNING, _user, t3), undo, state);
    }
}
public class A_FeatherPhalanx : CardAbility
{
    public A_FeatherPhalanx(Card user) : base(user) { }

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
    }
    protected override void Activate(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Activate(targets, undo, state);
        if (state == null)
        {
            int numCards = user.values[0].value;
            for (int ii = 0; ii < numCards; ii++)
            {
                Card card = user.controller.deck.Remove(0);
                if (card.HasKeyword(Keyword.HYROC) && card.type == Card.Type.THRALL)
                {
                    user.controller.PutInPlay(card);
                }
                else
                {
                    user.controller.Discard(card);
                }
            }
        }
    }
    public override bool ActivationAvailable() { return true; }
}


public class A_BlazingVortex : CardAbility
{
    public A_BlazingVortex(Card user) : base(user)
    {
    }

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        int damage = user.values[0].value;

        List<ITargetable> trg = new List<ITargetable>();
        trg.Add(Enemy.instance);
        foreach (Card card in Enemy.instance.active)
        {
            if (card.type == Card.Type.THRALL)
            {
                trg.Add(card);
            }
        }
        foreach (Card card in Player.instance.active)
        {
            if (card.type == Card.Type.THRALL)
            {
                trg.Add(card);
            }
        }
        trg.Add(Player.instance);

        foreach (ITargetable target in trg)
        {
            DamageData data = new DamageData(damage, Keyword.FIRE, _user, target);
            Ability.Damage(data, undo, state);
        }
    }
}
public class A_Cinder : CardAbility
{
    public A_Cinder(Card user) : base(user)
    {
        playTargets.Add(TargetAnyOpposing());
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        int damage = user.values[0].value;
        DamageData data = new DamageData(damage, Keyword.FIRE, _user, targets[0]);
        Ability.Damage(data, undo, state);
    }
}
public class A_Singe : CardAbility
{
    public A_Singe(Card user) : base(user) { }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        int n = user.opponent.GetStatus(StatusEffect.ID.BURN);
        int stacks = user.values[0].value;
        if (n > 0) { stacks = 1; }
        Ability.Status(user.opponent, StatusEffect.ID.BURN, stacks, undo, state);
    }

}
public class A_InfernoDjinn : CardAbility
{
    private TargetTemplate _template;
    public A_InfernoDjinn(Card user) : base(user)
    {
        _template = new TargetTemplate();
        _template.keywordAnd.Add(Keyword.FIRE);
        _template.cardType.Add(Card.Type.SPELL);
        
        ((Card)user).cardEvents.onDraw += DrawHandler;
    }
    private void DrawHandler(Card card)
    {
        _user.controller.actorEvents.onPlayCard += OnPlayCardHandler;
    }
    private void OnPlayCardHandler(Card card)
    {
        if (_user.inPlay && card.Compare(_template, _user.controller))
        {
            Card user = _user as Card;
            user.AddModifier(new StatModifier(1, Stat.Name.POWER, user, StatModifier.Duration.PERMANENT));
        }
    }
}
public class A_Ignite : CardAbility
{
    public A_Ignite(Card user) : base(user) { }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        int firebrandStacks = user.values[0].value;
        Ability.Status(user.controller, StatusEffect.ID.FIREBRAND, 1, undo, state);
        Ability.Draw(user.controller, firebrandStacks, undo, state);
    }
}
public class A_FlamingFlourish : CardAbility
{
    public A_FlamingFlourish(Card user) : base(user)
    {
        user.cardEvents.onCycle += CycleHandler;
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        Ability.Draw(user.controller, 1, undo, state);
        List<Card> cards = user.controller.hand;
        int n = 0;
        foreach (Card card in cards)
        {
            if (card.type == Card.Type.TECHNIQUE) { n++; }
        }
        Ability.Status(user.controller, StatusEffect.ID.FIREBRAND, n, undo, state);
    }
    
    private void CycleHandler(Card card)
    {
        Ability.Status(user.controller, StatusEffect.ID.FIREBRAND, 1);
    }
}
public class A_FeralWarcat : CardAbility
{
    public A_FeralWarcat(Card user) : base(user)
    {
        _user.controller.targetEvents.onDeclareTarget += DeclareTargetHandler;
    }

    private void DeclareTargetHandler(ITargetable source, ITargetable target)
    {
        if (source is Card && user.inPlay)
        {
            Card src = (Card)source;
            if (src.type == Card.Type.TECHNIQUE)
            {
                Ability.Damage(new DamageData(1, Keyword.SLASHING, _user, target), false, null);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_Continuity : CardAbility
{
    public A_Continuity(Card user) : base(user)
    {
        user.cardEvents.onCycle += CycleHandler;
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        Ability.Draw(user.controller, 1, undo, state);
        int n = 0;
        foreach (Card card in user.controller.hand)
        {
            if (card.type == Card.Type.SPELL) { n += 1; }
        }
        Ability.AddFocus(user.controller, n, 0, undo, state);
    }

    public override string Text()
    {
        string txt = "Draw a card.";
        if (user.playerControlled)
        {
            txt += " Gain 1/0 FOCUS for each Spell in your hand.";
            txt += "\n<b>Passive:</b> Gain 1/0 FOCUS.";
        }
        return txt;
    }
    private void CycleHandler(Card card)
    {
        Ability.AddFocus(user.controller, 1, 0);
    }
}
public class A_Chill : CardAbility
{
    public A_Chill(Card user) : base(user)
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
        Ability.Status(target, StatusEffect.ID.CHILL, 1, undo, state);
    }
    public override string Text()
    {
        return "Target enemy takes 1 ICE damage and gains 1 Chill.";
    }
}
public class A_CrystalLance : CardAbility
{
    public A_CrystalLance(Card user) : base(user)
    {
        playTargets.Add(TargetAnyOpposing());
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        int damage = 2;
        Keyword damageType = Keyword.ICE;
        ITargetable target = (ITargetable)targets[0];
        DamageData data = new DamageData(damage, damageType, _user, target);
        Ability.Damage(data, undo, state);
        Ability.Status(target, StatusEffect.ID.IMPALE, 1, undo, state);
    }

    public override string Text()
    {
        return "Target enemy takes 2 ICE damage and is Impaled.";
    }
}
public class A_DriftingVoidling : CardAbility
{
    public A_DriftingVoidling(Card user) : base(user)
    {
        ((Card)_user).cardEvents.onLeavePlay += LeavePlayHandler;
    }
    public override string Text()
    {
        return "When Drifting Voidling is destroyed, its controller gains 1 Insight.";
    }
    private void LeavePlayHandler(Card card)
    {
        card.controller.AddStatus(StatusEffect.ID.INSIGHT);
    }
}
public class A_IceElemental : CardAbility
{
    TargetTemplate _template;
    public A_IceElemental(Card user) : base(user)
    {
        
        _template = new TargetTemplate();
        _template.isSelf = true;
        _template.keyword.Add(Keyword.ICE);
        _template.cardType.Add(Card.Type.SPELL);
        ((Card)user).cardEvents.onDraw += DrawHandler;
    }
    public override string Text()
    {
        return "When you play an ICE Spell, Ice Elemental gains 1 ENDURANCE.";
    }

    private void DrawHandler(Card card)
    {
        _user.controller.actorEvents.onPlayCard += PlayCardHandler;
    }

    public void PlayCardHandler(Card card)
    {
        if (_user.inPlay && card.Compare(_template, _user.controller))
        {
            _user.IncrementHealth(1);
        }
    }

}
public class A_HailOfSplinters : CardAbility
{
    public A_HailOfSplinters(Card user) : base(user) { }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        _user.controller.targetEvents.onDealDamage += DealDamageHandler;
        _user.controller.actorEvents.onEndTurn += EndTurnHandler;
        Ability.Draw(user.controller, 1, undo, state);
        _user.controller.Draw();
    }
    public override string Text()
    {
        return "When an enemy takes ICE damage this turn, that target takes an additional 1 PIERCING damage. Draw a card.";
    }

    void DealDamageHandler(DamageData data)
    {
        if (data.type == Keyword.ICE && data.damage > 0 )
        {
            Damage(new DamageData(1, Keyword.PIERCING, _user, data.target), false, null);
        }
    }

    void EndTurnHandler(Actor actor)
    {
        Debug.Log("Deactivating Hail of Splinters");
        actor.targetEvents.onDealDamage -= DealDamageHandler;
        actor.actorEvents.onEndTurn -= EndTurnHandler;
    }
}
public class A_Rimeshield : CardAbility
{
    public A_Rimeshield(Card user) : base(user)
    {

    }
    public override string Text()
    {
        string txt = "Gain 2 Armor and 1 Chill.";
        return txt;
    }

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        Ability.Status(_user.controller, StatusEffect.ID.ARMOR, 2, undo, state);
        Ability.Status(_user.controller, StatusEffect.ID.CHILL, 1, undo, state);
    }
}
public class A_FrostLattice : CardAbility
{
    public A_FrostLattice(Card user) : base(user) { }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        TargetTemplate t = TargetAnyOpposing();
        ITargetable t1 = RandomOpposing((Card)_user, t);
        ITargetable t2 = RandomOpposing((Card)_user, t);
        t1?.AddStatus(StatusEffect.ID.CHILL, 1);
        t2?.AddStatus(StatusEffect.ID.CHILL, 1);
        Ability.Draw(_user.controller, 1, undo, state);
    }

    public override string Text()
    {
        return "Frost Lattice adds 1 stack of Chill to 2 random opposing targets. Draw a card.";
    }
}
public class A_RimeSprite : CardAbility
{
    public A_RimeSprite(Card user) : base(user)
    {
        _user.targetEvents.onDealDamage += DealDamageHandler;
    }
    public override string Text()
    {
        return "When Rime Sprite deals damage to a target, add a Chill counter to that target and destroy this.";
    }
    public void DealDamageHandler(DamageData data)
    {
        ((ITargetable)data.target).AddStatus(StatusEffect.ID.CHILL, 1);
        ((Card)_user).Destroy();
    }
}
public class A_TimeDilation : CardAbility
{
    public A_TimeDilation(Card user) : base(user) { }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        Ability.Draw(user.controller, 2, undo, state);
        Ability.AddFocus(user.controller, 3, 0, undo, state);
    }
    public override string Text()
    {
        string txt = "Draw 2 cards.";
        if (user.controller is Player)
        {
            txt += " Gain 3/0 FOCUS.";
        }
        return txt;
    }
}
public class A_Legerdemain : CardAbility
{
    TargetTemplate _template;
    Card _card;
    bool _cardPlayed = false;
    public A_Legerdemain(Card user) : base(user)
    {
        _template = new TargetTemplate();
        _template.inHand = true;
        _template.isSelf = true;
        _template.isNot = _user;
        _template.cardType.Add(Card.Type.SPELL);
        _template.cardType.Add(Card.Type.TECHNIQUE);
        playTargets.Add(_template);
    }

    public override string Text()
    {
        string txt = "Memorize target Spell or Technique in your hand. If you play that card this turn, put it back into your hand instead of discarding it.";
        return txt;
    }

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        targets[0].AddStatus(StatusEffect.ID.MEMORIZED);
        _card = (Card)targets[0];
        _cardPlayed = false;
        _user.controller.actorEvents.onPlayCard += CardPlayedHandler;
    }

    private void CardPlayedHandler(Card card)
    {
        if (card == _card)
        {
            _user.controller.actorEvents.onPlayCard -= CardPlayedHandler;
            _card.cardEvents.onDiscard += CardDiscardedHandler;
        }
    }

    private void CardDiscardedHandler(Card card)
    {
        _card.cardEvents.onDiscard -= CardDiscardedHandler;
        _user.controller.AddToHand(card);
    }
}
public class A_MnemonicRecitation : CardAbility
{
    private TargetTemplate _template;
    public A_MnemonicRecitation(Card user) : base(user)
    {
        _template = new TargetTemplate();
        _template.inHand = true;
        _template.isSelf = true;
        _template.isNot = _user;
        _template.cardType.Add(Card.Type.SPELL);
        playTargets.Add(_template);
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        targets[0].AddStatus(StatusEffect.ID.MEMORIZED);
        Ability.Draw(user.controller, 1, undo, state);
    }
    public override string Text()
    {
        return "Memorize target Spell. Draw a card.";
    }
}
public class A_Shardfall : CardAbility
{
    public A_Shardfall(Card user) : base(user)
    {
    }

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        CardData data = Resources.Load("Cards/Iri/Shardstrike") as CardData;
        Ability.CreateCard(user.controller, data, user.transform.position, CardZone.Type.HAND, undo, state);
        Ability.CreateCard(user.controller, data, user.transform.position, CardZone.Type.HAND, undo, state);
    }

    protected override void Activate(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Activate(targets, undo, state);
        CardData data = Resources.Load("Cards/Iri/Shardstrike") as CardData;
        Ability.CreateCard(user.controller, data, user.transform.position, CardZone.Type.HAND, undo, state);
        Ability.CreateCard(user.controller, data, user.transform.position, CardZone.Type.HAND, undo, state);
    }
    public override bool ActivationAvailable() { return true; }

    public override string Text()
    {
        string txt = "Add 2 <b>Shardstrike</b> to your hand.";
        txt += "\n<b>Channel: </b> Add 2 <b>Shardstrike</b> to your hand.";
        return txt;
    }
}
public class A_Shardstrike : CardAbility
{
    public A_Shardstrike(Card user) : base(user)
    {
        playTargets.Add(TargetAnyOpposing());
    }
    public override string Text()
    {
        return "Target enemy takes 1 ICE damage and is Impaled.";
    }

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        Debug.Assert(_user is Card);
        Card user = _user as Card;
        base.Play(targets, undo, state);
        int damage = 1;

        ITargetable target = (ITargetable)targets[0];
        DamageData data = new DamageData(damage, Keyword.ICE, _user, target);
        Ability.Damage(data, undo, state);
        target.AddStatus(StatusEffect.ID.IMPALE);
    }
}
public class A_Manaplasm : CardAbility
{
    private StatModifier _mod;
    private int _powerMod;
    public A_Manaplasm(Card user) : base(user)
    {
        _mod = new StatModifier(0, Stat.Name.POWER, user, Modifier.Duration.SOURCE);
        user.AddModifier(_mod);
        user.controller.actorEvents.onPlayCard += PlayCardHandler;
        user.controller.actorEvents.onEndTurn += EndTurnHandler;
    }

    public override string Text()
    {
        string txt = "When you play a Spell, this gets +1 POWER this turn.";
        return txt;
    }

    private void PlayCardHandler(Card card)
    {
        if (user.inPlay && card.type == Card.Type.SPELL)
        {
            _mod.value++;
        }
    }

    private void EndTurnHandler(Actor actor)
    {
        _mod.value = 0;
    }

}
public class A_PrimordialSlime : CardAbility
{
    private TargetTemplate _template;
    public A_PrimordialSlime(Card user) : base(user)
    {
        _template = new TargetTemplate();
        _template.cardType.Add(Card.Type.SPELL);
        user.controller.actorEvents.onEndTurn += EndTurnHandler;
    }
    public override string Text()
    {
        string txt = "At the end of your turn, if you played 2 or more Spells, create a Primordial Slime with Ephemeral.";
        return txt;
    }

    private void EndTurnHandler(Actor actor)
    {
        int numSpells = actor.NumPlayedThisTurn(_template);
        if (numSpells >= 2 && user.inPlay)
        {
            Card clone = Ability.CreateCard(user.controller, user.data, user.transform.position, CardZone.Type.ACTIVE);
            clone.AddKeywordAbility(KeywordAbility.Key.EPHEMERAL);
        }
    }
}
public class A_RideTheLightning : CardAbility
{
    private TargetTemplate _template;
    public A_RideTheLightning(Card user) : base(user)
    {
        _template = new TargetTemplate();
        _template.isSelf = true;
        _template.cardType.Add(Card.Type.SPELL);
    }

    public override string Text()
    {
        return "When you play a Spell this turn, draw a card, gain 1 FOCUS, and take 1 LIGHTNING damage.";
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
            Ability.Draw(user.controller, 1);
            Ability.AddFocus(user.controller, 1, 0);
            Ability.Damage(new DamageData(1, Keyword.LIGHTNING, _user, _user.controller));
        }
    }

    private void EndTurnHandler(Actor actor)
    {
        _user.controller.actorEvents.onPlayCard -= PlayCardHandler;
        _user.controller.actorEvents.onEndTurn -= EndTurnHandler;
    }
}
public class A_ChainLightning : CardAbility
{
    public A_ChainLightning(Card user) : base(user)
    {
        playTargets.Add(TargetAnyOpposing());
    }

    public override string Text()
    {
        return "Chain Lightning deals 3 LIGHTNING damage to any opposing target," +
            " 2 LIGHTNING damage to a another random opposing target," +
            " and 1 LIGHTNING damage to another random opposing target";
    }

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        Ability.Damage(new DamageData(3, Keyword.LIGHTNING, _user, targets[0]), undo, state);

        ITargetable t2 = RandomOpposing((Card)_user, TargetAnyOpposing(targets[0]));
        ITargetable t3 = RandomOpposing((Card)_user, TargetAnyOpposing(t2));
        if (t2 == null) { return; }
        Ability.Damage(new DamageData(2, Keyword.LIGHTNING, _user, t2), undo, state);
        Ability.Damage(new DamageData(1, Keyword.LIGHTNING, _user, t3), undo, state);
    }
}
public class A_Static : CardAbility
{
    public A_Static(Card user) : base(user) { }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        TargetTemplate t = TargetAnyOpposing();
        ITargetable t1 = RandomOpposing(user, t);
        ITargetable t2 = RandomOpposing(user, t);
        Ability.Damage(new DamageData(1, Keyword.LIGHTNING, _user, t1), undo, state);
        Ability.Damage(new DamageData(1, Keyword.LIGHTNING, _user, t2), undo, state);
    }

    public override string Text()
    {
        return "Static deals 1 LIGHTNING damage to 2 random targets.";
    }
}
public class A_KatabaticSquall : CardAbility
{
    public A_KatabaticSquall(Card user) : base(user) { }

    public override string Text()
    {
        string txt = "Destroy all enemy thralls that are Chilled or Frozen. Add Chilled to each remaining enemy Thrall.";
        return txt;
    }

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        List<Card> cards = _user.controller.opponent.active;
        foreach (Card card in cards)
        {
            if (card.type != Card.Type.THRALL) { continue; }
            if (card.GetStatus(StatusEffect.ID.FROZEN) > 0 || card.GetStatus(StatusEffect.ID.CHILL) > 0)
            {
                Ability.DestroyCard(card, undo, state);
            } else
            {
                Ability.Status(card, StatusEffect.ID.CHILL, 1, undo, state);
            }
        }
    }
}
public class A_Epiphany : CardAbility
{
    public A_Epiphany(Card user) : base(user)
    {
        ((Card)user).cardEvents.onCycle += CycleHandler;
    }

    public override string Text()
    {
        string txt = "Gain 2 Insight.";
        txt += "\n<b>Cycle: </b> Gain 1 Insight.";
        return txt;
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        Ability.Status(user.controller, StatusEffect.ID.INSIGHT, 2, undo, state);
    }
    private void CycleHandler(Card card)
    {
        ((Card)_user).cardEvents.onCycle -= CycleHandler;
        _user.controller.AddStatus(StatusEffect.ID.INSIGHT);
    }
}
public class A_MomentOfClarity : CardAbility
{
    public A_MomentOfClarity(Card user) : base(user)
    {
    }

    public override string Text()
    {
        return "Draw a card. Spells in your hand cost -1 FOCUS this turn.";
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        Ability.Draw(user.controller, 1, undo, state);
        foreach (Card card in user.controller.hand)
        {
            if (card.type == Card.Type.SPELL)
            {
                card.cost.AddModifier(new StatModifier(-1, Stat.Name.COST, user, Modifier.Duration.END_OF_TURN));
            }
        }
    }
}
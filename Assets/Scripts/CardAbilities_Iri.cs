using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_Continuity : CardAbility
{
    public A_Continuity(Card user) : base(user) { }
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
        string txt = "Draw a card.";
        if (((Card)_user).playerControlled)
        {
            txt += " Then, gain 1/0 FOCUS for each Spell in your hand.";
            txt += "\n<b>Passive:</b> Gain 1/0 FOCUS.";
        }
        return txt;
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

        ((ITargetable)target).AddStatus(StatusEffect.ID.CHILL, 1);
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

        ((ITargetable)target).AddStatus(StatusEffect.ID.IMPALE, 1);
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

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
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
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
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
        _user.controller.AddStatus(StatusEffect.ID.ARMOR, 2);
        _user.controller.AddStatus(StatusEffect.ID.CHILL, 1);
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
        _user.controller.Draw();
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
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
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
        _user.controller.Draw();
        _user.controller.Draw();
        if (_user.controller is Player)
        {
            ((Player)_user.controller).focus.baseValue += 3;
        }
    }

    public override string Text()
    {
        return "Frost Lattice adds 1 stack of Chill to 2 random opposing targets. Draw a card.";
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
        _user.controller.Draw();
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
        StartChannel();
        CardData data = Resources.Load("Cards/Iri/Shardstrike") as CardData;
        for (int ii = 0; ii < 1; ii++)
        {
            _user.controller.AddToHand(Card.Spawn(data, _user.playerControlled, _user.transform.position));
        }
    }

    protected override void Channel()
    {
        base.Channel();
        CardData data = Resources.Load("Cards/Iri/Shardstrike") as CardData;
        for (int ii = 0; ii < 1; ii++)
        {
            _user.controller.AddToHand(Card.Spawn(data, _user.playerControlled, _user.transform.position));
        }
    }

    public override string Text()
    {
        string txt = "Add 2 <b>Shardstrike</b> with to your hand.";
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
    private int _powerMod;
    public A_Manaplasm(Card user) : base(user)
    {
        ((Card)user).cardEvents.onDraw += DrawHandler;
    }

    public override string Text()
    {
        string txt = "When you play a Spell, this gets +1 POWER this turn.";
        return txt;
    }

    private void DrawHandler(Card card)
    {
        _user.controller.actorEvents.onPlayCard += PlayCardHandler;
        _user.controller.actorEvents.onEndTurn += EndTurnHandler;
        _powerMod = 0;
    }
    private void PlayCardHandler(Card card)
    {
        Card user = _user as Card;
        if (card.type == Card.Type.SPELL)
        {
            _powerMod++;
            user.power.RemoveModifiersFromSource(user);
            user.power.AddModifier(new StatModifier(_powerMod, Stat.Name.POWER, user));
        }
    }

    private void EndTurnHandler(Actor actor)
    {
        Card user = _user as Card;
        _powerMod = 0;
        user.power.RemoveModifiersFromSource(user);
    }

}

public class A_PrimordialSlime : CardAbility
{

    private TargetTemplate _template;

    public A_PrimordialSlime(Card user) : base(user)
    {
        _template = new TargetTemplate();
        _template.cardType.Add(Card.Type.SPELL);

        ((Card)user).cardEvents.onDraw += DrawHandler;
    }

    private void DrawHandler(Card card)
    {
        _user.controller.actorEvents.onEndTurn += EndTurnHandler;
    }
    public override string Text()
    {
        string txt = "At the end of your turn, if you played 2 or more Spells, create a Primordial Slime with Ephemeral.";
        return txt;
    }

    private void EndTurnHandler(Actor actor)
    {
        Card user = _user as Card;
        int numSpells = actor.NumPlayedThisTurn(_template);
        Debug.Log("Played " + numSpells + " spells.");
        if (numSpells >= 2 && user.inPlay)
        {
            Card clone = Card.Spawn(user.data, user.playerControlled, user.transform.position);
            clone.AddKeywordAbility(KeywordAbility.Key.EPHEMERAL);
            user.controller.PutInPlay(clone);
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
        Damage(new DamageData(3, Keyword.LIGHTNING, _user, targets[0]), undo, state);

        ITargetable t2 = RandomOpposing((Card)_user, TargetAnyOpposing(targets[0]));
        ITargetable t3 = RandomOpposing((Card)_user, TargetAnyOpposing(t2));
        if (t2 == null) { return; }
        Damage(new DamageData(2, Keyword.LIGHTNING, _user, t2), undo, state);
        Damage(new DamageData(1, Keyword.LIGHTNING, _user, t3), undo, state);
    }
}

public class A_Static : CardAbility
{
    public A_Static(Card user) : base(user) { }
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
        return "Static deals 1 LIGHTNING damage to 2 random targets.";
    }
}

public class A_KatabaticSquall : CardAbility
{
    public A_KatabaticSquall(Card user) : base(user) { }

    public override string Text()
    {
        string txt = "Destroy all enemy thralls that are Chilled or Frozen. Add 2 Chilled to each remaining enemy Thrall.";
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
                card.Destroy();
            } else
            {
                card.AddStatus(StatusEffect.ID.CHILL, 2);
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
        _user.controller.AddStatus(StatusEffect.ID.INSIGHT, 2);
    }
    private void CycleHandler(Card card)
    {
        ((Card)_user).cardEvents.onCycle -= CycleHandler;
        _user.controller.AddStatus(StatusEffect.ID.INSIGHT);
    }
}
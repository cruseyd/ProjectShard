using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_Chill : CardAbility
{
    public A_Chill(Card user) : base(user)
    {
        playTargets.Add(TargetAnyOpposing());
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        int damage = user.values[0].value;
        DamageData data = new DamageData(damage, Keyword.ICE, user, targets[0]);
        Ability.Damage(data, undo, state);
        Ability.Status(targets[0], StatusEffect.ID.CHILL, 1, undo, state);
    }
}
public class A_FrostLattice : CardAbility
{
    public A_FrostLattice(Card user) : base(user) { }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        int numTargets = user.values[0].value;
        TargetTemplate t = TargetAnyOpposing();
        for (int ii = 0; ii < numTargets; ii++)
        {
            ITargetable target = RandomOpposing(user, t);
            Ability.Status(target, StatusEffect.ID.CHILL, 1, undo, state);
        }
        Ability.Draw(_user.controller, 1, undo, state);
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
        int damage = user.values[0].value;
        DamageData data = new DamageData(damage, Keyword.ICE, _user, targets[0]);
        Ability.Damage(data, undo, state);
        Ability.Status(targets[0], StatusEffect.ID.IMPALE, 1, undo, state);
    }
}
public class A_KatabaticSquall : CardAbility
{
    public A_KatabaticSquall(Card user) : base(user) { }

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
            }
            else
            {
                Ability.Status(card, StatusEffect.ID.CHILL, 1, undo, state);
            }
        }
    }
}
public class A_Stormcall : CardAbility
{
    public A_Stormcall(Card user) : base(user)
    {
    }

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
    }

    protected override void Activate(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Activate(targets, undo, state);

        CardData data1 = CardIndex.Get("ARCING_BOLT");
        CardData data2 = CardIndex.Get("PIERCING_HAIL");
        Card card1 = Ability.CreateCard(user.controller, data1, user.transform.position, CardZone.Type.HAND, undo, state);
        Card card2 = Ability.CreateCard(user.controller, data2, user.transform.position, CardZone.Type.HAND, undo, state);
        card1.AddKeywordAbility(KeywordAbility.Key.EPHEMERAL);
        card2.AddKeywordAbility(KeywordAbility.Key.EPHEMERAL);
        Ability.Status(card1, StatusEffect.ID.ALACRITY, 1);
        Ability.Status(card2, StatusEffect.ID.ALACRITY, 1);
    }
    public override bool ActivationAvailable() { return true; }
}
public class A_PiercingHail : CardAbility
{
    public A_PiercingHail(Card user) : base(user)
    {
    }

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        TargetTemplate t = TargetAnyOpposing();
        ITargetable target = RandomOpposing(user, t);
        Ability.Status(target, StatusEffect.ID.IMPALE, 1, undo, state);
    }
}
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
        int focusPerCard = user.values[0].value;
        foreach (Card card in user.controller.hand)
        {
            if (card.type == Card.Type.SPELL) { n += 1; }
        }
        Ability.AddFocus(user.controller, n*focusPerCard, 0, undo, state);
    }

    private void CycleHandler(Card card)
    {
        int cycleFocus = user.values[1].value;
        Ability.AddFocus(user.controller, cycleFocus, 0);
    }
}
public class A_Rimeshield : CardAbility
{
    public A_Rimeshield(Card user) : base(user)
    {

    }

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        int armor = user.values[0].value;
        Ability.Status(_user.controller, StatusEffect.ID.ARMOR, armor, undo, state);
        Ability.Status(_user.controller, StatusEffect.ID.CHILL, 1, undo, state);
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
        Ability.Status(targets[0], StatusEffect.ID.MEMORIZED, 1);
        Ability.Draw(user.controller, 1, undo, state);
    }
}
public class A_MomentOfClarity : CardAbility
{
    public A_MomentOfClarity(Card user) : base(user)
    {

    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        Ability.Draw(user.controller, 1, undo, state);
        int alacrityStacks = user.values[0].value;
        foreach (Card card in user.controller.hand)
        {
            if (card.type == Card.Type.SPELL)
            {
                Ability.Status(user.controller, StatusEffect.ID.ALACRITY, alacrityStacks, undo, state);
                return;
            }
        }
    }
}
public class A_TimeDilation : CardAbility
{
    public A_TimeDilation(Card user) : base(user) { }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        int numCards = user.values[0].value;
        int numFocus = user.values[1].value;
        Ability.Draw(user.controller, numCards, undo, state);
        Ability.AddFocus(user.controller, numFocus, 0, undo, state);
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

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        Status(targets[0], StatusEffect.ID.MEMORIZED, 1);
        _card = (Card)targets[0];
        _cardPlayed = false;
        _user.controller.actorEvents.onPlayCard += CardPlayedHandler;
        _user.controller.actorEvents.onEndTurn += EndTurnHandler;
        Status(targets[0], StatusEffect.ID.LEGERDEMAIN, 1);
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
        _card.RemoveStatus(StatusEffect.ID.LEGERDEMAIN);
    }

    private void EndTurnHandler(Actor actor)
    {
        _user.controller.actorEvents.onPlayCard -= CardPlayedHandler;
        _card.cardEvents.onDiscard -= CardDiscardedHandler;
        _card.RemoveStatus(StatusEffect.ID.LEGERDEMAIN);
    }
}
public class A_Replicate : CardAbility
{
    public A_Replicate(Card user) : base(user)
    {
        playTargets.Add(TargetAnyThrall());
    }

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        Card target = targets[0] as Card;
        Card clone = Ability.CreateCard(user.controller, target.data, target.transform.position, CardZone.Type.ACTIVE, undo, state);
        clone?.AddKeywordAbility(KeywordAbility.Key.EPHEMERAL);
    }
}
public class A_RimeSprite : CardAbility
{
    public A_RimeSprite(Card user) : base(user)
    {
        _user.targetEvents.onDealDamage += DealDamageHandler;
    }
    public void DealDamageHandler(DamageData data)
    {
        Status(data.target, StatusEffect.ID.CHILL, 1);
    }
}
public class A_WindScreamer : CardAbility
{
    public A_WindScreamer(Card user) : base(user)
    {
        activateTargets.Add(TargetOpposingThrall());
    }

    public override bool ActivationAvailable()
    {
        return user.inPlay;
    }
    protected override void Activate(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Activate(targets, undo, state);
        Card target = targets[0] as Card;
        Ability.ShuffleIntoDeck(target);
        Ability.ShuffleIntoDeck(user);
        Ability.Draw(user.controller, 1, undo, state);
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
public class A_GlacierColossus : CardAbility
{
    TargetTemplate _template;
    public A_GlacierColossus(Card user) : base(user)
    {

        _template = new TargetTemplate();
        _template.isSelf = true;
        _template.keywordAnd.Add(Keyword.ICE);
        _template.cardType.Add(Card.Type.SPELL);
        user.cardEvents.onDraw += DrawHandler;
    }

    private void DrawHandler(Card card)
    {
        _user.controller.actorEvents.onPlayCard += PlayCardHandler;
    }

    public void PlayCardHandler(Card card)
    {
        if (_user.inPlay && card.Compare(_template, _user.controller))
        {
            int health = user.values[0].value;
            Ability.Heal(user, health);
        }
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

    private void EndTurnHandler(Actor actor)
    {
        int numSpells = actor.NumPlayedThisTurn(_template);
        int targetNum = user.values[0].value;
        if (numSpells >= targetNum && user.inPlay)
        {
            Card clone = Ability.CreateCard(user.controller, user.data, user.transform.position, CardZone.Type.ACTIVE);
            clone.AddKeywordAbility(KeywordAbility.Key.EPHEMERAL);
        }
    }
}
public class A_PrismaticGolem : CardAbility
{
    TargetTemplate _template;
    public A_PrismaticGolem(Card user) : base(user)
    {
        _template = new TargetTemplate();
        _template.cardType.Add(Card.Type.SPELL);
        _template.isSelf = true;
        _template.zone.Add(CardZone.Type.HAND);
        activateTargets.Add(_template);
    }
    public override bool ActivationAvailable()
    {
        return user.inPlay;
    }
    protected override void Activate(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Activate(targets, undo, state);
        Card target = targets[0] as Card;
        Card clone = Ability.CreateCard(user.controller, target.data, target.transform.position, CardZone.Type.HAND, undo, state);
        clone.AddKeywordAbility(KeywordAbility.Key.EPHEMERAL);
    }
}
public class A_DriftingVoidling : CardAbility
{
    public A_DriftingVoidling(Card user) : base(user)
    {
        ((Card)_user).cardEvents.onLeavePlay += LeavePlayHandler;
    }
    private void LeavePlayHandler(Card card)
    {
        int numInsight = user.values[0].value;
        Ability.Status(card.controller, StatusEffect.ID.INSIGHT, numInsight);
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



public class A_RideTheLightning : CardAbility
{
    private TargetTemplate _template;
    public A_RideTheLightning(Card user) : base(user)
    {
        _template = new TargetTemplate();
        _template.isSelf = true;
        _template.cardType.Add(Card.Type.SPELL);
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

public class A_Epiphany : CardAbility
{
    public A_Epiphany(Card user) : base(user)
    {
        ((Card)user).cardEvents.onCycle += CycleHandler;
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        Ability.Status(user.controller, StatusEffect.ID.INSIGHT, 2, undo, state);
    }
    private void CycleHandler(Card card)
    {
        ((Card)_user).cardEvents.onCycle -= CycleHandler;
        Ability.Status(user.controller, StatusEffect.ID.INSIGHT, 1);
    }
}

public class A_Flicker : CardAbility
{
    public A_Flicker(Card user) : base(user)
    {
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
    }
}
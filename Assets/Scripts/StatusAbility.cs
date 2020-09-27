using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusAbility
{
    protected StatusEffect.ID _id;
    public readonly ITargetable target;
    protected int _stacks;
    public int stacks
    {
        get { return _stacks; }
        set
        {
            int delta = value - _stacks;
            AddStacks(delta);
            _stacks = value;
            if (_stacks <= 0)
            {
                target.RemoveStatus(_id, 9999);
            }
        }
    }

    public static StatusAbility Get(StatusEffect.ID a_id, ITargetable a_target, int a_stacks)
    {
        switch(a_id)
        {
            case StatusEffect.ID.BURN: return new SA_Burn(a_id, a_target, a_stacks);
            case StatusEffect.ID.DAZE: return new SA_Daze(a_id, a_target, a_stacks);
            case StatusEffect.ID.STUN: return new SA_Stun(a_id, a_target, a_stacks);
            case StatusEffect.ID.CHILL: return new SA_Chill(a_id, a_target, a_stacks);
            case StatusEffect.ID.FROZEN: return new SA_Frozen(a_id, a_target, a_stacks);
            case StatusEffect.ID.FRENZY: return new SA_Frenzy(a_id, a_target, a_stacks);
            case StatusEffect.ID.FIREBRAND: return new SA_Firebrand(a_id, a_target, a_stacks);
            case StatusEffect.ID.IMPALE: return new SA_Impale(a_id, a_target, a_stacks);
            case StatusEffect.ID.MEMORIZED: return new SA_Memorized(a_id, a_target, a_stacks);
            case StatusEffect.ID.ARMOR: return new SA_Armor(a_id, a_target, a_stacks);
            case StatusEffect.ID.INSIGHT: return new SA_Insight(a_id, a_target, a_stacks);
            default: return null;
        }
    }

    public StatusAbility(StatusEffect.ID a_id, ITargetable a_target, int a_stacks)
    {
        _id = a_id;
        target = a_target;
        stacks = a_stacks;
    }

    protected virtual void AddStacks(int a_stacks) { }
    public abstract void Remove();

}

// Template
/*
public class SA_NAME : StatusAbility
{
    public SA_NAME(StatusEffect.ID a_id, ITargetable a_target, int a_stacks) : base(a_id, a_target, a_stacks)
    { }
    public override void Remove()
    { }
}
*/
public class SA_Burn : StatusAbility
{
    public SA_Burn(StatusEffect.ID a_id, ITargetable a_target, int a_stacks) : base(a_id, a_target, a_stacks)
    {
        target.controller.actorEvents.onStartTurn += StartTurnHandler;
    }

    private void StartTurnHandler(Actor actor)
    {
        target.Damage(new DamageData(stacks, Keyword.FIRE, null, target));
        stacks -= 1;
    }

    public override void Remove()
    {
        target.controller.actorEvents.onStartTurn -= StartTurnHandler;
    }
}

public class SA_Daze : StatusAbility
{
    private StatModifier _mod;
    public SA_Daze(StatusEffect.ID a_id, ITargetable a_target, int a_stacks) : base(a_id, a_target, a_stacks)
    {
        target.controller.actorEvents.onEndTurn += EndTurnHandler;
        target.targetEvents.onGainStatus += AddStatusHandler;
        if (target is Card)
        {
            Card card = target as Card;
            Debug.Assert(card.type == Card.Type.THRALL);
            _mod = new StatModifier(-1, Stat.Name.POWER, null);
            card.power.AddModifier(_mod);
        }
    }
    public override void Remove()
    {
        target.controller.actorEvents.onEndTurn -= EndTurnHandler;
        target.targetEvents.onGainStatus -= AddStatusHandler;
        if (target is Card)
        {
            Card card = target as Card;
            Debug.Assert(card.type == Card.Type.THRALL);
            card.power.RemoveModifier(_mod);
        }
    }

    private void AddStatusHandler(StatusEffect status, int numStacks)
    {
        if (stacks > 1)
        {
            target.RemoveStatus(StatusEffect.ID.DAZE);
            target.AddStatus(StatusEffect.ID.STUN);
        }
    }

    private void EndTurnHandler(Actor actor)
    {
        target.RemoveStatus(StatusEffect.ID.DAZE);
    }
}

public class SA_Stun : StatusAbility
{
    public SA_Stun(StatusEffect.ID a_id, ITargetable a_target, int a_stacks) : base(a_id, a_target, a_stacks)
    {
        if (a_target is Card)
        {
            Card trg = a_target as Card;
            Debug.Assert(trg.type == Card.Type.THRALL);
            trg.blockAvailable = false;
        }
        a_target.controller.actorEvents.onStartTurn += StartTurnHandler;
        a_target.targetEvents.onTryGainStatus += TryGainStatusHandler;
    }
    private void TryGainStatusHandler(StatusEffect.ID a_id, int a_stacks, Attempt a_attempt)
    {
        if (a_id == StatusEffect.ID.DAZE) { a_attempt.success = false; }
    }
    private void StartTurnHandler(Actor actor)
    {
        if (target is Card)
        {
            Card trg = target as Card;
            Debug.Assert(trg.type == Card.Type.THRALL);
            trg.activationAvailable = false;
            trg.attackAvailable = false;
            trg.controller.actorEvents.onEndTurn += EndTurnHandler;
        } else if (target is Actor)
        {
            Actor trg = target as Actor;
            trg.DiscardRandom();
            target.RemoveStatus(_id);
        }
    }
    private void EndTurnHandler(Actor actor)
    {
        Debug.Assert(target is Card);
        Card trg = target as Card;
        Debug.Assert(trg.type == Card.Type.THRALL);
        target.RemoveStatus(_id);
    }

    public override void Remove()
    {
        target.controller.actorEvents.onStartTurn -= StartTurnHandler;
        target.controller.actorEvents.onEndTurn -= EndTurnHandler;
        target.targetEvents.onTryGainStatus -= TryGainStatusHandler;
    }
}

public class SA_Impale : StatusAbility
{
    public SA_Impale(StatusEffect.ID a_id, ITargetable a_target, int a_stacks) : base(a_id, a_target, a_stacks)
    {
        if (a_target is Card)
        {
            Card trg = a_target as Card;
            Debug.Assert(trg.type == Card.Type.THRALL);
            trg.blockAvailable = false;
        }
        a_target.controller.actorEvents.onStartTurn += StartTurnHandler;
        target.targetEvents.onTakeRawDamage += TakeRawDamageHandler;
    }
    private void StartTurnHandler(Actor actor)
    {
        if (target is Card)
        {
            Card trg = target as Card;
            Debug.Assert(trg.type == Card.Type.THRALL);
            trg.activationAvailable = false;
            trg.attackAvailable = false;
            trg.controller.actorEvents.onEndTurn += EndTurnHandler;
        }
        else if (target is Actor)
        {
            Actor trg = target as Actor;
            trg.DiscardRandom();
            target.RemoveStatus(_id);
        }
    }
    private void EndTurnHandler(Actor actor)
    {
        Debug.Assert(target is Card);
        Card trg = target as Card;
        Debug.Assert(trg.type == Card.Type.THRALL);
        target.RemoveStatus(_id);
    }

    private void TakeRawDamageHandler(DamageData data)
    {
        if (data.type == Keyword.LIGHTNING)
        {
            data.damage += 1;
        }
    }
    public override void Remove()
    {
        target.controller.actorEvents.onStartTurn -= StartTurnHandler;
        target.controller.actorEvents.onEndTurn -= EndTurnHandler;
        target.targetEvents.onTakeRawDamage -= TakeRawDamageHandler;
    }
}

public class SA_Chill : StatusAbility
{
    private StatModifier _mod;
    public SA_Chill(StatusEffect.ID a_id, ITargetable a_target, int a_stacks) : base(a_id, a_target, a_stacks)
    {
        target.targetEvents.onGainStatus += AddStatusHandler;
        if (target is Card)
        {
            Card card = target as Card;
            Debug.Assert(card.type == Card.Type.THRALL);
            _mod = new StatModifier(-stacks, Stat.Name.POWER, null);
            card.power.AddModifier(_mod);
        }
    }
    public override void Remove()
    {
        Debug.Log("Removing Chill");
        target.targetEvents.onGainStatus -= AddStatusHandler;
        if (target is Card)
        {
            Card card = target as Card;
            Debug.Assert(card.type == Card.Type.THRALL);
            card.power.RemoveModifier(_mod);
        }
    }

    private void AddStatusHandler(StatusEffect status, int numStacks)
    {
        if (stacks > 1)
        {
            target.RemoveStatus(StatusEffect.ID.CHILL);
            target.AddStatus(StatusEffect.ID.FROZEN);
        }

    }
}

public class SA_Frozen : StatusAbility
{
    public SA_Frozen(StatusEffect.ID a_id, ITargetable a_target, int a_stacks) : base(a_id, a_target, a_stacks)
    {
        if (a_target is Card)
        {
            Card trg = a_target as Card;
            Debug.Assert(trg.type == Card.Type.THRALL);
            trg.blockAvailable = false;
        }
        a_target.controller.actorEvents.onStartTurn += StartTurnHandler;
        a_target.targetEvents.onTryGainStatus += TryGainStatusHandler;
        a_target.targetEvents.onTakeRawDamage += TakeRawDamageHandler;
    }
    private void TryGainStatusHandler(StatusEffect.ID a_id, int a_stacks, Attempt a_attempt)
    {
        if (a_id == StatusEffect.ID.CHILL) { a_attempt.success = false; }
    }
    private void StartTurnHandler(Actor actor)
    {
        if (target is Card)
        {
            Card trg = target as Card;
            Debug.Assert(trg.type == Card.Type.THRALL);
            trg.activationAvailable = false;
            trg.attackAvailable = false;
            trg.controller.actorEvents.onEndTurn += EndTurnHandler;
        }
        else if (target is Actor)
        {
            Actor trg = target as Actor;
            trg.DiscardRandom();
            target.RemoveStatus(_id);
        }
    }
    private void EndTurnHandler(Actor actor)
    {
        Debug.Assert(target is Card);
        Card trg = target as Card;
        Debug.Assert(trg.type == Card.Type.THRALL);
        target.RemoveStatus(_id);
    }

    private void TakeRawDamageHandler(DamageData data)
    {
        if (data.type == Keyword.CRUSHING)
        {
            data.damage += 1;
        }
    }
    public override void Remove()
    {
        target.controller.actorEvents.onStartTurn -= StartTurnHandler;
        target.controller.actorEvents.onEndTurn -= EndTurnHandler;
        target.targetEvents.onTakeRawDamage -= TakeRawDamageHandler;
        target.targetEvents.onTryGainStatus -= TryGainStatusHandler;
        if (target is Card)
        {
            Card card = target as Card;
            target.AddStatus(StatusEffect.ID.CHILL, card.cost.value / 2);
        }
    }
}

public class SA_Frenzy : StatusAbility
{
    public SA_Frenzy(StatusEffect.ID a_id, ITargetable a_target, int a_stacks) : base(a_id, a_target, a_stacks)
    {
        Debug.Assert(target is Actor);
        Actor actor = target as Actor;
        actor.actorEvents.onTryPlayCard += PlayCardHandler;
    }
    public override void Remove()
    {
        ((Actor)target).actorEvents.onTryPlayCard -= PlayCardHandler;
    }

    private void PlayCardHandler(Card card, Attempt attempt)
    {
        if (card.type == Card.Type.TECHNIQUE)
        {
            Actor actor = target as Actor;
            actor.Draw();
            if (actor is Player)
            {
                Player.instance.focus.baseValue += 1;
            }
            actor.Damage(new DamageData(1, Keyword.DEFAULT, null, actor));
            stacks--;
        }
    }
}
public class SA_Firebrand : StatusAbility
{
    public SA_Firebrand(StatusEffect.ID a_id, ITargetable a_target, int a_stacks) : base(a_id, a_target, a_stacks)
    {
        Debug.Assert(target is Actor);
        Actor actor = target as Actor;
        actor.targetEvents.onDealDamage += DealDamageHandler;
    }
    public override void Remove()
    {
        target.targetEvents.onDealDamage -= DealDamageHandler;
    }
    private void DealDamageHandler(DamageData data)
    {
        if (data.source is Card && ((Card)data.source).type == Card.Type.TECHNIQUE)
        {
            Debug.Log("Firebrand Damage trigger");
            Ability.Damage(new DamageData(1, Keyword.FIRE, null, data.target));
            stacks--;
        }
    }
}

public class SA_Memorized : StatusAbility
{
    public SA_Memorized(StatusEffect.ID a_id, ITargetable a_target, int a_stacks) : base(a_id, a_target, a_stacks)
    {
        Debug.Assert(target is Card);
        Card card = target as Card;
        card.cardEvents.onTryCycle += TryCycleHandler;
    }
    public override void Remove()
    {
        ((Card)target).cardEvents.onTryCycle -= TryCycleHandler;
    }

    private void TryCycleHandler(Card card, Attempt attempt)
    {
        attempt.success = false;
        target.RemoveStatus(_id);
    }
}

public class SA_Armor : StatusAbility
{
    public SA_Armor(StatusEffect.ID a_id, ITargetable a_target, int a_stacks) : base(a_id, a_target, a_stacks)
    {
        target.targetEvents.onTakeRawDamage += TakeRawDamageHandler;
    }
    public override void Remove()
    {
        target.targetEvents.onTakeRawDamage -= TakeRawDamageHandler;
    }
    private void TakeRawDamageHandler(DamageData data)
    {
        Debug.Assert(data.target == target);
        data.damage--;
        stacks--;
    }
}

public class SA_Insight : StatusAbility
{
    public SA_Insight(StatusEffect.ID a_id, ITargetable a_target, int a_stacks) : base(a_id, a_target, a_stacks)
    {
        Debug.Assert(target is Actor);
        Actor actor = target as Actor;
        actor.actorEvents.onDrawCard += DrawCardhandler;
    }
    public override void Remove()
    {
        ((Actor)target).actorEvents.onDrawCard -= DrawCardhandler;
    }

    private void DrawCardhandler(Card card)
    {
        int n = stacks;
        target.RemoveStatus(_id);
        ((Actor)target).Draw(n);
    }
}

public class SA_Alacrity : StatusAbility
{
    private StatModifier _mod;
    public SA_Alacrity(StatusEffect.ID a_id, ITargetable a_target, int a_stacks) : base(a_id, a_target, a_stacks)
    {
        Debug.Assert(target is Card);
        Card card = target as Card;
        _mod = new StatModifier(-a_stacks, Stat.Name.COST, null);
        card.AddModifier(_mod);
        target.targetEvents.onRefresh += RefreshHandler;
    }

    public override void Remove()
    {
        target.targetEvents.onRefresh -= RefreshHandler;
        ((Card)target).RemoveModifier(_mod);
    }

    private void RefreshHandler()
    {
        _mod.value = -stacks;
    }
}
public class SA_Might : StatusAbility
{
    private StatModifier _powerMod;
    private StatModifier _enduranceMod;
    public SA_Might(StatusEffect.ID a_id, ITargetable a_target, int a_stacks) : base(a_id, a_target, a_stacks)
    {
        Debug.Assert(target is Card);
        Card card = target as Card;
        Debug.Assert(card.type == Card.Type.THRALL);
        _powerMod = new StatModifier(a_stacks, Stat.Name.POWER, null);
        _enduranceMod = new StatModifier(a_stacks, Stat.Name.ENDURANCE, null);
        card.AddModifier(_powerMod);
        card.AddModifier(_enduranceMod);
        target.targetEvents.onRefresh += RefreshHandler;
    }

    public override void Remove()
    {
        target.targetEvents.onRefresh -= RefreshHandler;
        ((Card)target).RemoveModifier(_powerMod);
        ((Card)target).RemoveModifier(_enduranceMod);
    }

    private void RefreshHandler()
    {
        _powerMod.value = stacks;
        _enduranceMod.value = stacks;
    }
}
public class SA_Atrophy : StatusAbility
{
    private StatModifier _powerMod;
    private StatModifier _enduranceMod;
    public SA_Atrophy(StatusEffect.ID a_id, ITargetable a_target, int a_stacks) : base(a_id, a_target, a_stacks)
    {
        Debug.Assert(target is Card);
        Card card = target as Card;
        Debug.Assert(card.type == Card.Type.THRALL);
        _powerMod = new StatModifier(-a_stacks, Stat.Name.POWER, null);
        _enduranceMod = new StatModifier(-a_stacks, Stat.Name.ENDURANCE, null);
        card.AddModifier(_powerMod);
        card.AddModifier(_enduranceMod);
        target.targetEvents.onRefresh += RefreshHandler;
    }

    public override void Remove()
    {
        target.targetEvents.onRefresh -= RefreshHandler;
        ((Card)target).RemoveModifier(_powerMod);
        ((Card)target).RemoveModifier(_enduranceMod);
    }

    private void RefreshHandler()
    {
        _powerMod.value = -stacks;
        _enduranceMod.value = -stacks;
    }
}
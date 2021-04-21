using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusAbility
{
    protected StatusEffect _status;
    public ITargetable target { get { return _status.target; } }
    public int stacks
    {
        get { return _status.stacks; }
        set
        {
            _status.stacks = value;
        }
    }

    public static StatusAbility Get(StatusEffect a_status, ITargetable a_target, int a_stacks)
    {

        switch(a_status.id)
        {
            // General Status Effects
            case StatusEffect.ID.BURN: return new SA_Burn(a_status);
            case StatusEffect.ID.DAZE: return new SA_Daze(a_status);
            case StatusEffect.ID.STUN: return new SA_Stun(a_status);
            case StatusEffect.ID.CHILL: return new SA_Chill(a_status);
            case StatusEffect.ID.FROZEN: return new SA_Frozen(a_status);
            case StatusEffect.ID.FRENZY: return new SA_Frenzy(a_status);
            case StatusEffect.ID.FIREBRAND: return new SA_Firebrand(a_status);
            case StatusEffect.ID.IMPALE: return new SA_Impale(a_status);
            case StatusEffect.ID.MEMORIZED: return new SA_Memorized(a_status);
            case StatusEffect.ID.ARMOR: return new SA_Armor(a_status);
            case StatusEffect.ID.INSIGHT: return new SA_Insight(a_status);
            case StatusEffect.ID.ALACRITY: return new SA_Alacrity(a_status);
            case StatusEffect.ID.MIGHT: return new SA_Might(a_status);
            case StatusEffect.ID.ATROPHY: return new SA_Atrophy(a_status);

            // Card Specific Status Effects
            case StatusEffect.ID.SHARPEN: return new SA_Empty(a_status);
            default:
                Debug.Log("Could not find StatusAbility: " + a_status.id);
                return null;
        }
    }

    public StatusAbility(StatusEffect a_status)
    {
        _status = a_status;
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
public class SA_Empty : StatusAbility
{
    public SA_Empty(StatusEffect a_status) : base(a_status)
    {
    }
    public override void Remove()
    {
    }
}
public class SA_Burn : StatusAbility
{
    public SA_Burn(StatusEffect a_status) : base(a_status)
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
    public SA_Daze(StatusEffect a_status) : base(a_status)
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
    public SA_Stun(StatusEffect a_status) : base(a_status)
    {
        if (target is Card)
        {
            Card trg = target as Card;
            Debug.Assert(trg.type == Card.Type.THRALL);
            trg.blockAvailable = false;
        }
        target.controller.actorEvents.onStartTurn += StartTurnHandler;
        target.targetEvents.onTryGainStatus += TryGainStatusHandler;
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
            target.RemoveStatus(_status.id);
        }
    }
    private void EndTurnHandler(Actor actor)
    {
        Debug.Assert(target is Card);
        Card trg = target as Card;
        Debug.Assert(trg.type == Card.Type.THRALL);
        target.RemoveStatus(_status.id);
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
    public SA_Impale(StatusEffect a_status) : base(a_status)
    {
        if (target is Card)
        {
            Card trg = target as Card;
            Debug.Assert(trg.type == Card.Type.THRALL);
            trg.blockAvailable = false;
        }
        target.controller.actorEvents.onStartTurn += StartTurnHandler;
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
            target.RemoveStatus(_status.id);
        }
    }
    private void EndTurnHandler(Actor actor)
    {
        Debug.Assert(target is Card);
        Card trg = target as Card;
        Debug.Assert(trg.type == Card.Type.THRALL);
        target.RemoveStatus(_status.id);
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
    public SA_Chill(StatusEffect a_status) : base(a_status)
    {
        target.targetEvents.onGainStatus += AddStatusHandler;
        if (target is Card)
        {
            Card card = target as Card;
            Debug.Assert(card.type == Card.Type.THRALL);
            _mod = new StatModifier(-stacks, Stat.Name.POWER, _status);
            card.power.AddModifier(_mod);
        }
    }
    public override void Remove()
    {
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
    public SA_Frozen(StatusEffect a_status) : base(a_status)
    {
        if (target is Card)
        {
            Card trg = target as Card;
            Debug.Assert(trg.type == Card.Type.THRALL);
            trg.blockAvailable = false;
        }
        target.controller.actorEvents.onStartTurn += StartTurnHandler;
        target.targetEvents.onTryGainStatus += TryGainStatusHandler;
        target.targetEvents.onTakeRawDamage += TakeRawDamageHandler;
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
            target.RemoveStatus(_status.id);
        }
    }
    private void EndTurnHandler(Actor actor)
    {
        Debug.Assert(target is Card);
        Card trg = target as Card;
        Debug.Assert(trg.type == Card.Type.THRALL);
        target.RemoveStatus(_status.id);
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
    public SA_Frenzy(StatusEffect a_status) : base(a_status)
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
    public SA_Firebrand(StatusEffect a_status) : base(a_status)
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
    public SA_Memorized(StatusEffect a_status) : base(a_status)
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
        target.RemoveStatus(_status.id);
    }
}

public class SA_Armor : StatusAbility
{
    public SA_Armor(StatusEffect a_status) : base(a_status)
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
        data.damage-=1;
        stacks--;
    }
}

public class SA_Insight : StatusAbility
{
    public SA_Insight(StatusEffect a_status) : base(a_status)
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
        target.RemoveStatus(_status.id);
        ((Actor)target).Draw(n);
    }
}

public class SA_Alacrity : StatusAbility
{
    private TargetTemplate _template;
    private TemplateModifier _mod;
    private StatModifier _cardMod;
    public SA_Alacrity(StatusEffect a_status) : base(a_status)
    {
        if (target is Player)
        {
            Player player = target as Player;
            _template = new TargetTemplate();
            _template.cardType.Add(Card.Type.SPELL);
            _template.cardType.Add(Card.Type.TECHNIQUE);
            _template.inHand = true;
            _template.owner = player;
            _mod = new TemplateModifier(-1, Stat.Name.COST, _status, Modifier.Duration.SOURCE, _template);
            player.actorEvents.onPlayCard += PlayCardHandler;
            Ability.AddTemplateModifier(_mod);
        } else if (target is Card)
        {
            Card card = target as Card;
            _cardMod = new StatModifier(0, Stat.Name.COST, _status, Modifier.Duration.SOURCE);
            Ability.AddStatModifier(card, _cardMod);
            card.targetEvents.onRefresh += RefreshHandler;
        } 
    }

    public void PlayCardHandler(Card card)
    {
        if (card.type == Card.Type.SPELL || card.type == Card.Type.TECHNIQUE)
        {
            stacks--;
        }
    }

    public override void Remove()
    {
        if (target is Player)
        {
            ((Player)target).actorEvents.onPlayCard -= PlayCardHandler;
            Ability.RemoveTemplateModifier(_mod);
        } else if (target is Card)
        {
            ((Card)target).targetEvents.onRefresh -= RefreshHandler;
            Ability.RemoveStatModifier((Card)target, _cardMod);
        }
    }

    public void RefreshHandler()
    {
        _cardMod.value = -stacks;
    }
}
public class SA_Might : StatusAbility
{
    private StatModifier _powerMod;
    private StatModifier _enduranceMod;
    public SA_Might(StatusEffect a_status) : base(a_status)
    {
        Debug.Assert(target is Card);
        Card card = target as Card;
        Debug.Assert(card.type == Card.Type.THRALL);
        _powerMod = new StatModifier(stacks, Stat.Name.POWER, null);
        _enduranceMod = new StatModifier(stacks, Stat.Name.ENDURANCE, null);
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
    public SA_Atrophy(StatusEffect a_status) : base(a_status)
    {
        Debug.Assert(target is Card);
        Card card = target as Card;
        Debug.Assert(card.type == Card.Type.THRALL);
        _powerMod = new StatModifier(-stacks, Stat.Name.POWER, null);
        _enduranceMod = new StatModifier(-stacks, Stat.Name.ENDURANCE, null);
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


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect
{
    public readonly StatusDisplay display;
    public readonly ITargetable target;
    public readonly StatusData data;

    private int _stacks;
    public int stacks
    {
        get { return _stacks; }
        set
        {
            _stacks = Mathf.Max(value, 0);
            if (!data.stackable) { _stacks = Mathf.Min(value, 1); }
            display.Refresh();
            if (_stacks <= 0)
            {
                target.RemoveStatus(data.id);
            }
        }
    }

    public StatusEffect(StatusName id, ITargetable a_target, StatusDisplay a_display, int a_stacks = 1)
    {
        display = a_display;
        target = a_target;
        data = Resources.Load("StatusConditions/" + id.ToString()) as StatusData;
        stacks = a_stacks;
        switch (id)
        {
            case StatusName.POISON:
                target.Controller().events.onStartTurn += Poison; break;
            case StatusName.BURN:
                target.Controller().events.onStartTurn += Burn; break;
            case StatusName.STUN:
                target.Controller().events.onStartTurn += Stun; break;
            case StatusName.ELDER_KNOWLEDGE:
                target.Controller().events.onDrawCard += ElderKnowledge; break;
            case StatusName.CHILL:
                if (target is Card)
                {
                    ((Card)target).events.onGainStatus += Chill;
                }
                else if (target is Actor)
                {
                    ((Actor)target).events.onGainStatus += Chill;
                }
                break;
            default: break;
        }
        display.SetStatus(this);
    }

    public void Remove()
    {
        display.SetStatus(null);
        switch (data.id)
        {
            case StatusName.POISON:
                target.Controller().events.onStartTurn -= Poison; break;
            case StatusName.BURN:
                target.Controller().events.onStartTurn -= Burn; break;
            case StatusName.STUN:
                target.Controller().events.onStartTurn -= Stun; break;
            case StatusName.ELDER_KNOWLEDGE:
                target.Controller().events.onDrawCard -= ElderKnowledge; break;
            case StatusName.CHILL:
                if (target is Card)
                {
                    ((Card)target).events.onGainStatus -= Chill;
                }
                else if (target is Actor)
                {
                    ((Actor)target).events.onGainStatus -= Chill;
                }
                break;
            default: break;
        }
    }


    private void Poison(Actor actor)
    {
        Debug.Assert(target is IDamageable);
        ((IDamageable)target).Damage(new DamageData(1, Keyword.POISON, null, (IDamageable)target));
        stacks -= 1;
    }
    private void Burn(Actor actor)
    {
        Debug.Assert(target is IDamageable);
        ((IDamageable)target).Damage(new DamageData(stacks, Keyword.FIRE, null, (IDamageable)target));
        stacks -= 1;
    }

    private void Stun(Actor actor)
    {
        Debug.Assert(target is Card);
        Debug.Assert(((Card)target).type == Card.Type.THRALL);
        ((Card)target).attackAvailable = false;
        target.RemoveStatus(data.id);
    }

    private void ElderKnowledge(Card drawn)
    {
        Debug.Assert(target is Actor);
        int n = stacks;
        target.RemoveStatus(data.id);
        Actor actor = (Actor)target;
        actor.Draw(n);
    }

    private void Chill(StatusEffect status, int s)
    {
        if (status == null || status.data.id != StatusName.CHILL) { return; }
        if (target is Card && ((Card)target).cost.value <= status.stacks)
        {
            status.stacks -= ((Card)target).cost.value;
            target.AddStatus(StatusName.STUN, 1);
        }
        else if (target is Actor && status.stacks >= 3)
        {
            status.stacks -= 3;
            ((Actor)target).DiscardRandom();
        }
    }
}


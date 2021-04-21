using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Modifier
{
    public enum Duration
    {
        DEFAULT,
        PERMANENT,
        SOURCE,
        END_OF_TURN,
        START_OF_TURN
    }

    public readonly Duration duration;
    public readonly Object source;

    public Modifier(Object _source, Duration dur = Duration.PERMANENT)
    {
        source = _source;
        duration = dur;
        if (source is Card)
        {
            Card src = source as Card;
            switch (duration)
            {
                case Duration.SOURCE: src.cardEvents.onLeavePlay += LeavePlayHandler; break;
                case Duration.END_OF_TURN: src.controller.actorEvents.onEndTurn += EndTurnHandler; break;
                case Duration.START_OF_TURN: src.controller.actorEvents.onStartTurn += StartTurnHandler; break;
                case Duration.PERMANENT:
                default:
                    break;
            }
        } else if (source is StatusEffect)
        {
            StatusEffect src = source as StatusEffect;
            switch (duration)
            {
                case Duration.SOURCE: src.events.onRemove += RemoveStatusHandler; break;
                case Duration.END_OF_TURN: src.target.controller.actorEvents.onEndTurn += EndTurnHandler; break;
                case Duration.START_OF_TURN: src.target.controller.actorEvents.onStartTurn += StartTurnHandler; break;
                case Duration.PERMANENT:
                default:
                    break;
            }
        }
        
    }
    public abstract void Remove();
    private void LeavePlayHandler(Card source)
    {
        Remove();
    }
    private void RemoveStatusHandler(StatusEffect source)
    {
        Remove();
    }
    private void StartTurnHandler(Actor actor)
    {
        Remove();
    }
    private void EndTurnHandler(Actor actor)
    {
        Debug.Log("Removing an end of turn modifier");
        Remove();
    }
}

public class StatModifier : Modifier
{
    private int _value;
    private List<Card> _targets;

    public readonly Stat.Name statName;
    public int value
    {
        get { return _value; }
        set
        {
            _value = value;
        }
    }

    public StatModifier(int a_value, Stat.Name a_statName, Object a_source,
        Modifier.Duration dur = Modifier.Duration.PERMANENT) : base(a_source, dur)
    {
        value = a_value;
        statName = a_statName;
        _targets = new List<Card>();
    }

    public bool SetTarget(Card a_target)
    {
        if (a_target.AddModifier(this))
        {
            _targets.Add(a_target);
            return true;
        }
        return false;
    }
    public bool RemoveTarget(Card a_target)
    {
        if (a_target.RemoveModifier(this))
        {
            _targets.Remove(a_target);
            return true;
        }
        return false;
    }
    public override void Remove()
    {
        for (int ii = _targets.Count-1; ii >= 0; ii--)
        {
            RemoveTarget(_targets[ii]);
        }
    }
}

public class TemplateModifier : StatModifier
{
    public readonly TargetTemplate template;
    private Actor _pov;

    public TemplateModifier(int a_value, Stat.Name a_statName, Object a_source, Duration a_dur, TargetTemplate a_template)
        : base(a_value, a_statName, a_source, a_dur)
    {
        template = a_template;
        if (a_source is Card)
        {
            _pov = ((Card)a_source).controller;
        } else if (a_source is StatusEffect)
        {
            _pov = ((StatusEffect)a_source).target.controller;
        }
    }
    public bool Compare(Card card) { return card.Compare(template, _pov); }


    public override void Remove()
    {
        base.Remove();
        Dungeon.RemoveModifier(this);
    }
}

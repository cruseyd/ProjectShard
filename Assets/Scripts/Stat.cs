using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatModifier
{
    public enum Duration
    {
        DEFAULT,
        PERMANENT,
        SOURCE,
        END_OF_TURN,
        START_OF_TURN
    }

    public readonly float value;
    public readonly Card source;
    public readonly Duration duration;
    private List<Stat> _targets;

    public StatModifier(float _value, Card _source, Duration dur = Duration.PERMANENT)
    {
        value = _value;
        source = _source;
        duration = dur;
        switch (duration)
        {
            case Duration.SOURCE: source.cardEvents.onLeavePlay += LeavePlayHandler; break;
            case Duration.END_OF_TURN: source.controller.actorEvents.onEndTurn += EndTurnHandler; break;
            case Duration.START_OF_TURN: source.controller.actorEvents.onStartTurn += StartTurnHandler; break;
            case Duration.PERMANENT:
            default:
                break;
        }
        _targets = new List<Stat>();
    }

    public void AddTarget(Stat stat)
    {
        stat.AddModifier(this);
        _targets.Add(stat);
    }

    public void Remove()
    {
        foreach (Stat stat in _targets)
        {
            stat.RemoveModifier(this);
        }
    }

    private void LeavePlayHandler(Card source)
    {
        Remove();
    }
    private void StartTurnHandler(Actor actor)
    {
        Remove();
    }
    private void EndTurnHandler(Actor actor)
    {
        Remove();
    }
}

public class TemplateModifier
{
    public readonly StatModifier.Duration duration;
    public readonly Stat.Name statName;
    public readonly TargetTemplate template;
    public readonly int value;
    public StatModifier mod;
    private Card _source;


    public TemplateModifier(int _value, Stat.Name stat, TargetTemplate t, StatModifier.Duration _dur, Card source)
    {
        value = _value;
        duration = _dur;
        statName = stat;
        template = t;
        _source = source;
        mod = new StatModifier(_value, _source);
        switch (duration)
        {
            case StatModifier.Duration.SOURCE: source.cardEvents.onLeavePlay += LeavePlayHandler; break;
            case StatModifier.Duration.END_OF_TURN: source.controller.actorEvents.onEndTurn += EndTurnHandler; break;
            case StatModifier.Duration.START_OF_TURN: source.controller.actorEvents.onStartTurn += StartTurnHandler; break;
            case StatModifier.Duration.PERMANENT:
            default:
                break;
        }
    }

    public void Compare(Card target)
    {
        if (target.Compare(template, _source.controller))
        {
            target.AddModifier(mod, statName);
        }
    }

    private void LeavePlayHandler(Card source)
    {
        Dungeon.RemoveModifier(this);
    }
    private void StartTurnHandler(Actor actor)
    {
        Dungeon.RemoveModifier(this);
    }
    private void EndTurnHandler(Actor actor)
    {
        Dungeon.RemoveModifier(this);
    }
}


public class Stat
{
    public enum Name
    {
        DEFAULT = 0,

        HEALTH = 100,
        MAX_HEALTH,
        FOCUS,
        MAX_FOCUS,
        ENDURANCE,

        COST = 200,
        STRENGTH,
        FINESSE,
        PERCEPTION,
        POWER,
        UPKEEP
    }
    private float _baseValue;
    private readonly List<StatModifier> _modifiers;
    
    public int value
    {
        get
        {
            float modifiedValue = _baseValue;
            for (int ii = _modifiers.Count - 1; ii >= 0; ii--)
            {
                if (_modifiers[ii] == null) { _modifiers.RemoveAt(ii); }
                else { modifiedValue += _modifiers[ii].value; }
            }
            return (int)modifiedValue;
        }
    }
    public int baseValue
    {
        set { _baseValue = value; }
        get { return (int)_baseValue; }
    }

    public Stat(float a_baseValue)
    {
        _baseValue = a_baseValue;
        _modifiers = new List<StatModifier>();
    }

    public bool AddModifier(StatModifier mod)
    {
        if (_modifiers.Contains(mod)) { return false; }
        else
        {
            _modifiers.Add(mod);
            return true;
        }
    }
    public bool RemoveModifier(StatModifier mod)
    {
        if (_modifiers.Contains(mod))
        {
            _modifiers.Remove(mod);
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool RemoveModifiersFromSource(Object a_source)
    {
        bool didRemove = false;
        for (int ii = _modifiers.Count - 1; ii >= 0; ii--)
        {
            if (_modifiers[ii].source == a_source)
            {
                didRemove = true;
                _modifiers.RemoveAt(ii);
            }
        }
        return didRemove;
    }

}

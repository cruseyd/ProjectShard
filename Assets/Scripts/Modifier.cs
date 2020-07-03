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
    public readonly Card source;

    public Modifier(Card _source, Duration dur = Duration.PERMANENT)
    {
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
    }
    public abstract void Remove();
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

public class StatModifier : Modifier
{
    private int _value;
    private Stat _target;

    public readonly Stat.Name statName;
    public int value
    {
        get { return _value; }
        set
        {
            _value = value;
        }
    }

    public StatModifier(int a_value, Stat.Name a_statName, Card a_source,
        Modifier.Duration dur = Modifier.Duration.PERMANENT) : base(a_source, dur)
    {
        value = a_value;
        statName = a_statName;
    }

    public void SetTarget(Stat stat) { _target = stat; }

    public override void Remove()
    {
        _target.RemoveModifier(this);
    }
}

public class TemplateModifier : Modifier
{
    public readonly Stat.Name statName;
    public readonly TargetTemplate template;
    public readonly int value;
    public StatModifier mod;

    public TemplateModifier(int _value, Stat.Name stat, TargetTemplate t, Duration _dur, Card source) : base(source, _dur)
    {
        value = _value;
        statName = stat;
        template = t;
        mod = new StatModifier(_value, stat, source);
    }

    public override void Remove()
    {
        Dungeon.RemoveModifier(this);
    }

    public void Compare(Card target)
    {
        if (target.Compare(template, source.controller))
        {
            target.AddModifier(mod);
        }
    }

}

public class ProtectionModifier : Modifier
{
    public readonly TargetTemplate template;
    private ITargetable _target;

    public ProtectionModifier(TargetTemplate _template, Card _source, Duration dur = Duration.PERMANENT) : base(_source, dur)
    {
        template = _template;
    }

    public void SetTarget(ITargetable trg)
    {
        _target = trg;
    }

    public override void Remove()
    {

    }
}

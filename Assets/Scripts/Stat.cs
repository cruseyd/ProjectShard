using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatModifier
{
    public readonly float value;
    public readonly Object source;

    public StatModifier(float _value, Object _source)
    {
        value = _value;
        source = _source;
    }
}

public class TemplateModifier
{
    public readonly StatModifier modifier;
    public readonly Stat.Name statName;
    public readonly TargetTemplate template;
    private Card _source;

    public TemplateModifier(StatModifier mod, Stat.Name stat, TargetTemplate t, Card source)
    {
        modifier = mod;
        statName = stat;
        template = t;
        _source = source;
        //source.events.onLeavePlay += this.Destroy()
    }

    private void Destroy(Card source)
    {

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
        ALLEGIANCE
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

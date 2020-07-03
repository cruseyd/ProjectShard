using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Stat
{
    public enum Name
    {
        DEFAULT = 0,

        HEALTH = 100,
        MAX_HEALTH,
        FOCUS,
        MAX_FOCUS,

        COST = 200,
        STRENGTH,
        FINESSE,
        PERCEPTION,

        POWER,
        UPKEEP,
        ENDURANCE,

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
            mod.SetTarget(this);
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
        Debug.Log("num modifiers: " + _modifiers.Count);
        return didRemove;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardState
{
    public Card source;
    public int power;
    public int health;
    public bool active = true;
    public Dictionary<StatusEffect.ID, int> _statusEffects;

    public float threat
    {
        get
        {
            if (!active) { return 0; }
            float value = source.cost.value*1.5f + power;
            float modifier = 1.0f;
            foreach (StatusEffect.ID id in _statusEffects.Keys)
            {
                modifier += StatusEffect.Threat(id) * _statusEffects[id];
            }
            return Mathf.Max(value * modifier, 0);
        }
    }

    public CardState(Card card)
    {
        source = card;
        if (card.type == Card.Type.THRALL)
        {
            power = card.power.value;
        }
        health = card.endurance.value;
        active = true;
        _statusEffects = new Dictionary<StatusEffect.ID, int>();
        foreach (StatusEffect.ID id in card.GetAllStatus())
        {
            _statusEffects[id] = card.GetStatus(id);
        }
    }

    public CardState(CardData data)
    {
        source = null;
        power = data.power;
        health = data.endurance;
        active = true;
        _statusEffects = new Dictionary<StatusEffect.ID, int>();
    }

    public void Damage(int damage)
    {
        if (source.type == Card.Type.THRALL)
        {
            health -= damage;
            if (health <= 0) { active = false; }
            else { active = true; }
        }
    }

    public void Status(StatusEffect.ID id, int stacks)
    {
        StatusData data = Resources.Load<StatusData>("StatusData/" + id.ToString());
        if (_statusEffects.ContainsKey(id))
        {
            _statusEffects[id] += stacks;
            if (!data.stackable) { _statusEffects[id] = Mathf.Clamp(_statusEffects[id], 0, 1); }
        }
        else if (!_statusEffects.ContainsKey(id))
        {
            _statusEffects[id] = stacks;
            if (!data.stackable) { _statusEffects[id] = Mathf.Clamp(_statusEffects[id], 0, 1); }
        }
        if (_statusEffects[id] == 0) { _statusEffects.Remove(id); }
    }

}

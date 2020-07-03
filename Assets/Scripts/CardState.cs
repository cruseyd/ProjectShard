using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardState
{
    public Card source;
    public int power;
    public int health;
    public bool active = true;

    public CardState(Card card)
    {
        source = card;
        power = card.power.value;
        health = card.endurance.value;
        active = true;
    }
    public void Damage(int damage)
    {
        if (source.type == Card.Type.THRALL && active)
        {
            health -= damage;
            if (health <= 0) { active = false; }
            else { active = true; }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardState
{
    public Card source;
    public int power;
    public int allegiance;
    public bool active = true;

    public CardState(Card card)
    {
        source = card;
        power = card.power.value;
        allegiance = card.allegiance.value;
        active = true;
    }
    public void Damage(int damage)
    {
        if (source.type == Card.Type.THRALL && active)
        {
            allegiance -= damage;
            if (allegiance <= 0) { active = false; }
            else { active = true; }
        }
    }
}

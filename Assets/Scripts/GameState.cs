using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public List<CardState> friendlyCards;
    public List<CardState> opposingCards;

    public int numHandCards;
    public int opposingNumHandCards;

    public int health;
    public int opposingHealth;

    public Actor pov;

    public void ApplyDamage(DamageData damage, bool undo = false)
    {
        int scale = 1;
        if (undo) { scale = -1; }
        if ((Object)damage.target == pov) { health -= scale*damage.damage; }
        else if ((Object)damage.target == pov.opponent) { opposingHealth -= scale*damage.damage; }
        else
        {
            Card targetCard = ((Card)damage.target);
            GetCardState(targetCard).Damage(scale*damage.damage);
        }
    }
    public void DamageActor(Actor src, int dmg, Keyword damageType = Keyword.DEFAULT)
    {
        //TODO: simulate damage type
        if (src == pov) { health -= dmg; }
        else { opposingHealth -= dmg; }
    }

    public void DrawCards(Actor src, int n)
    {
        if (src == pov) { numHandCards += n; }
        else { opposingNumHandCards += n; }
    }

    public GameState(Actor current)
    {
        pov = current;
        numHandCards = current.hand.Count;
        opposingNumHandCards = current.opponent.hand.Count;
        friendlyCards = new List<CardState>();
        opposingCards = new List<CardState>();
        foreach (Card card in Enemy.instance.active)
        {
            if (current == Enemy.instance)
            {
                friendlyCards.Add(new CardState(card));
            } else
            {
                opposingCards.Add(new CardState(card));
            }
        }
        foreach (Card card in Player.instance.active)
        {
            if (current == Enemy.instance)
            {
                opposingCards.Add(new CardState(card));
            }
            else
            {
                friendlyCards.Add(new CardState(card));
            }
        }
        if (current == Enemy.instance)
        {
            health = Enemy.instance.health.value;
            opposingHealth = Player.instance.health.value;
        } else
        {
            health = Player.instance.health.value;
            opposingHealth = Enemy.instance.health.value;

        }
    }

    public CardState GetCardState(Card card)
    {
        foreach (CardState state in friendlyCards) { if (state.source == card) { return state; } }
        foreach (CardState state in opposingCards) { if (state.source == card) { return state; } }
        return null;
    }

    public void AddCard(Card card)
    {
        if (GetCardState(card) == null)
        {
            if (card.controller == pov)
            {
                friendlyCards.Add(new CardState(card));
            } else
            {
                opposingCards.Add(new CardState(card));
            }
        }
    }

    public void RemoveCard(Card card)
    {
        CardState state = GetCardState(card);
        if (state != null)
        {
            if (card.controller == pov) { friendlyCards.Remove(state); }
            else { opposingCards.Remove(state); }
        }
    }
}

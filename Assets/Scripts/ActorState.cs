using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorState
{
    private List<CardState> _cardsInPlay;
    //private List<CardState> _cardsInDiscard;
    private Dictionary<StatusEffect.ID, int> _statusEffects;

    public int numCardsPlayable;
    public int numCardsInHand;
    public int health;
    public Actor actor;
    public float threat
    {
        get
        {
            float value = 0f;
            float modifier = 1.0f;
            foreach (CardState state in _cardsInPlay)
            {
                if (!state.active) { continue; }
                value += state.threat;
                modifier += 0.2f;
            }
            return value / modifier;
        }
    }
    public List<CardState> cards { get { return _cardsInPlay; } }
    public ActorState(Actor a_actor)
    {
        actor = a_actor;
        numCardsInHand = a_actor.hand.Count;
        numCardsPlayable = numCardsInHand;
        health = a_actor.health.value;

        _cardsInPlay = new List<CardState>();
        foreach (Card card in a_actor.active)
        {
            _cardsInPlay.Add(new CardState(card));
        }
        _statusEffects = new Dictionary<StatusEffect.ID, int>();
        foreach (StatusEffect.ID id in a_actor.GetAllStatus())
        {
            _statusEffects[id] = a_actor.GetStatus(id);
        }
        /*
        _cardsInDiscard = new List<CardState>();
        foreach (Card card in a_actor.discard)
        {
            _cardsInDiscard.Add(new CardState(card));
        }
        */
    }

    public CardState GetCardState(Card card)
    {
        foreach (CardState state in _cardsInPlay) { if (state.source == card) { return state; } }
        //foreach (CardState state in _cardsInDiscard) { if (state.source == card) { return state; } }
        return null;
    }
    public void Draw(int n)
    {
        /*
        if (actor.deck.count == 0)
        {
            _cardsInDiscard.Clear();
        }
        */
        numCardsInHand += n;
        numCardsPlayable += n;
    }
    public void AddCard(CardState card, CardZone.Type zone, bool undo)
    {
        switch (zone)
        {
            case CardZone.Type.HAND:
                if (undo) { numCardsInHand--; }
                else { numCardsInHand++;}
                break;
            case CardZone.Type.ACTIVE:
                if (undo) { _cardsInPlay.Remove(card); }
                else { _cardsInPlay.Add(card); }
                break;
            //case CardZone.Type.DISCARD: _cardsInDiscard.Add(card); break;
            default: break;
        }
    }
    public void RemoveCard(Card card, CardZone.Type zone, bool undo)
    {
        CardState state = GetCardState(card);
        if (state == null) { return; }
        switch (zone)
        {
            case CardZone.Type.HAND:
                if (undo) { numCardsInHand++; }
                else { numCardsInHand--; }
                break;
            case CardZone.Type.ACTIVE:
                if (undo) { state.active = true; }
                else { state.active = false; }
                break;
            //case CardZone.Type.DISCARD: _cardsInDiscard.Remove(card); break;
            default: break;
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

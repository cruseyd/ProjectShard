using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActorEvents
{
    private Actor _source;

    public ActorEvents(Actor source) { _source = source; }

    public event Action<Actor> onStartTurn;
    public event Action<Actor> onEndTurn;
    public event Action<Card> onDrawCard;
    public event Action<Card> onPlayCard;

    public void StartTurn() { onStartTurn?.Invoke(_source); }
    public void EndTurn() { onEndTurn?.Invoke(_source); }
    public void DrawCard(Card card) { onDrawCard?.Invoke(card); }
    public void PlayCard(Card card) { onPlayCard?.Invoke(card); }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActorEvents
{
    private Actor _source;

    public ActorEvents(Actor source) { _source = source; }

    public event Action<Actor> onStartTurn;
    public event Action<Actor> onBeginTurn;
    public event Action<Actor> onEndTurn;
    public event Action<Actor> onPostTurn;
    public event Action<Card> onDrawCard;
    public event Action<Card> onPlayCard;
    public event Action<Card, Attempt> onTryPlayCard;
    public event Action<Card> onActivateCard;
    public event Action<DamageData> onCardDamaged;
    public event Action<StatusEffect, int> onCardGainedStatus;
    public event Action<ITargetable, ITargetable, Attempt> onTryMarkTarget;

    public void StartTurn() { onStartTurn?.Invoke(_source); }
    public void BeginTurn() { onBeginTurn?.Invoke(_source); }
    public void EndTurn() { onEndTurn?.Invoke(_source); }
    public void PostTurn() { onPostTurn?.Invoke(_source); }
    public void DrawCard(Card card) { onDrawCard?.Invoke(card); }
    public void TryPlayCard(Card card, Attempt attempt) { onTryPlayCard?.Invoke(card, attempt); }
    public void PlayCard(Card card) { onPlayCard?.Invoke(card); }
    public void ActivateCard(Card card) { onActivateCard?.Invoke(card); }
    public void CardDamaged(DamageData data) { onCardDamaged?.Invoke(data); }
    public void CardGainedStatus(StatusEffect status, int stacks) { onCardGainedStatus?.Invoke(status, stacks); }
    public void TryMarkTarget(ITargetable source, ITargetable target, Attempt attempt) { onTryMarkTarget?.Invoke(source, target, attempt); }
}

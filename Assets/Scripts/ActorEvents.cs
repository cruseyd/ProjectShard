using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActorEvents
{
    private Actor _source;

    public ActorEvents(Actor source) { _source = source; }

    public event Action onStartTurn;
    public event Action onEndTurn;
    public event Action<Card> onDrawCard;
    public event Action<Card> onPlayCard;
    public event Action<StatusEffect, int> onGainStatus;
    public event Action<StatusEffect, int> onRemoveStatus;
    public event Action<DamageData> onRawDamage;
    public event Action<DamageData> onModifiedDamage;
    public event Action<DamageData> onReceiveDamage;

    public void StartTurn() { onStartTurn?.Invoke(); }
    public void EndTurn() { onEndTurn?.Invoke(); }
    public void DrawCard(Card card) { onDrawCard?.Invoke(card); }
    public void PlayCard(Card card) { onPlayCard?.Invoke(card); }
    public void RawDamage(DamageData data) { onRawDamage?.Invoke(data); }
    public void ModifiedDamage(DamageData data) { onModifiedDamage?.Invoke(data); }
    public void ReceiveDamage(DamageData data) { onReceiveDamage?.Invoke(data); }

    public void GainStatus(StatusEffect status, int stacks) { onGainStatus?.Invoke(status, stacks); }
    public void RemoveStatus(StatusEffect status, int stacks) { onRemoveStatus?.Invoke(status, stacks); }

}

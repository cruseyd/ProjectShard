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
    public event Action<StatusEffect, int> onGainStatus;
    public event Action<StatusEffect, int> onRemoveStatus;
    public event Action<DamageData> onDealRawDamage;
    public event Action<DamageData> onDealModifiedDamage;
    public event Action<DamageData> onDealDamage;
    public event Action<DamageData> onTakeRawDamage;
    public event Action<DamageData> onTakeModifiedDamage;
    public event Action<DamageData> onTakeDamage;
    public event Action<ITargetable, ITargetable> onTarget;

    public void StartTurn() { onStartTurn?.Invoke(_source); }
    public void EndTurn() { onEndTurn?.Invoke(_source); }
    public void DrawCard(Card card) { onDrawCard?.Invoke(card); }
    public void PlayCard(Card card) { onPlayCard?.Invoke(card); }
    public void DealRawDamage(DamageData data) { onDealRawDamage?.Invoke(data); }
    public void DealModifiedDamage(DamageData data) { onDealModifiedDamage?.Invoke(data); }
    public void DealDamage(DamageData data) { onDealDamage?.Invoke(data); }
    public void TakeRawDamage(DamageData data) { onTakeRawDamage?.Invoke(data); }
    public void TakeModifiedDamage(DamageData data) { onTakeModifiedDamage?.Invoke(data); }
    public void TakeDamage(DamageData data) { onTakeDamage?.Invoke(data); }
    public void GainStatus(StatusEffect status, int stacks) { onGainStatus?.Invoke(status, stacks); }
    public void RemoveStatus(StatusEffect status, int stacks) { onRemoveStatus?.Invoke(status, stacks); }
    public void Target(ITargetable source, ITargetable target) { onTarget?.Invoke(source, target); }
}

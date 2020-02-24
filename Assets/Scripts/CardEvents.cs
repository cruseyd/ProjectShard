using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEvents
{
    private Card _source;
    public CardEvents(Card source)
    {
        _source = source;
    }

    public event Action<Card> onDraw;
    public event Action<Card> onEnterPlay;
    public event Action<Card> onDestroy;
    public event Action<StatusEffect, int> onGainStatus;
    public event Action<StatusEffect, int> onRemoveStatus;
    public event Action<DamageData> onDealRawDamage;
    public event Action<DamageData> onDealModifiedDamage;
    public event Action<DamageData> onDealDamage;
    public event Action<DamageData> onTakeRawDamage;
    public event Action<DamageData> onTakeModifiedDamage;
    public event Action<DamageData> onTakeDamage;


    public void Draw() { onDraw?.Invoke(_source); }
    public void EnterPlay() { onEnterPlay?.Invoke(_source); }
    public void Destroy() { onDestroy?.Invoke(_source); }
    public void GainStatus(StatusEffect status, int stacks) { onGainStatus?.Invoke(status, stacks); }
    public void RemoveStatus(StatusEffect status, int stacks) { onRemoveStatus?.Invoke(status, stacks); }
    public void DealRawDamage(DamageData data) { onDealRawDamage?.Invoke(data); }
    public void DealModifiedDamage(DamageData data) { onDealModifiedDamage?.Invoke(data); }
    public void DealDamage(DamageData data) { onDealDamage?.Invoke(data); }
    public void TakeRawDamage(DamageData data) { onTakeRawDamage?.Invoke(data); }
    public void TakeModifiedDamage(DamageData data) { onTakeModifiedDamage?.Invoke(data); }
    public void TakeDamage(DamageData data) { onTakeDamage?.Invoke(data); }
}

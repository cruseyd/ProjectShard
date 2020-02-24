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
    public event Action<DamageData> onDealDamage;
    public event Action<DamageData> onRawDamage;
    public event Action<DamageData> onModifiedDamage;
    public event Action<DamageData> onReceiveDamage;


    public void Draw() { onDraw?.Invoke(_source); }
    public void EnterPlay() { onEnterPlay?.Invoke(_source); }
    public void Destroy() { onDestroy?.Invoke(_source); }
    public void GainStatus(StatusEffect status, int stacks) { onGainStatus?.Invoke(status, stacks); }
    public void RemoveStatus(StatusEffect status, int stacks) { onRemoveStatus?.Invoke(status, stacks); }
    public void DealDamage(DamageData data) { onDealDamage?.Invoke(data); }
    public void RawDamage(DamageData data) { onRawDamage?.Invoke(data); }
    public void ModifiedDamage(DamageData data) { onModifiedDamage?.Invoke(data); }
    public void ReceiveDamage(DamageData data) { onReceiveDamage?.Invoke(data); }
}

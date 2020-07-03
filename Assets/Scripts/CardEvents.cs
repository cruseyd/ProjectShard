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
    public event Action<Card> onLeavePlay;
    public event Action<Card> onEnterDiscard;
    public event Action<Card> onDiscard;
    public event Action<Card, Attempt> onTryCycle; 
    public event Action<Card> onCycle;

    public void Draw() { onDraw?.Invoke(_source); }
    public void EnterPlay() { onEnterPlay?.Invoke(_source); }
    public void Destroy() { onDestroy?.Invoke(_source); }
    public void LeavePlay() { onLeavePlay?.Invoke(_source); }
    public void EnterDiscard() { onEnterDiscard?.Invoke(_source); }
    public void Discard() { onDiscard?.Invoke(_source); }

    public void TryCycle(Attempt attempt) { onTryCycle?.Invoke(_source, attempt); }
    public void Cycle() { onCycle?.Invoke(_source); }
}

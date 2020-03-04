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

    public void Draw() { onDraw?.Invoke(_source); }
    public void EnterPlay() { onEnterPlay?.Invoke(_source); }
    public void Destroy() { onDestroy?.Invoke(_source); }
    public void LeavePlay() { onLeavePlay?.Invoke(_source); }
}

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

    public Action<Card> onDraw;
    public Action<Card> onEnterPlay;
    public Action<Card> onDestroy;

    public void Draw() { onDraw?.Invoke(_source); }
    public void EnterPlay() { onEnterPlay?.Invoke(_source); }
    public void Destroy() { onDestroy?.Invoke(_source); }
}

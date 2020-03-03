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
    


    //public event Action<ITargetable> onTarget;
    //public event Action<Card, ITargetable, ITargetable> onOwnerTarget;
    public event Action<Card, ITargetable> onDeclareAttack;
    public event Action<Card, Card, ITargetable> onOwnerDeclareAttack;

    public void Draw() { onDraw?.Invoke(_source); }
    public void EnterPlay() { onEnterPlay?.Invoke(_source); }
    public void Destroy() { onDestroy?.Invoke(_source); }
    public void LeavePlay() { onLeavePlay?.Invoke(_source); }

    //public void Target(ITargetable target) { onTarget?.Invoke(target); }
    //public void OwnerTarget(ITargetable source, ITargetable target) { onOwnerTarget?.Invoke(_source, source, target); }
    public void DeclareAttack(ITargetable target) { onDeclareAttack?.Invoke(_source, target); }
    public void OwnerDeclareAttack(Card source, ITargetable target) { onOwnerDeclareAttack?.Invoke(_source, source, target); }
}

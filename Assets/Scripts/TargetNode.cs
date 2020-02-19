using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetNode : MonoBehaviour
{
    /*
    public Card card
    {
        get
        {
            return GetComponent<Card>();
        }
    }
    public Actor actor { get { return GetComponent<Actor>(); } }
    public Ability ability
    {
        get
        {
            return card?.ability;
        }
    }
    public Actor owner
    {
        get
        {
            if (actor != null) { return actor; }
            else if (card != null) { return card.owner; }
            else { return null; }
        }
        
    }
    public new string name
    {
        get
        {
            if (actor != null) { return actor.name; }
            else if (card != null) { return card.name; }
            else return "NAME UNKNOWN";
        }
    }
    public bool targetable
    {
        get
        {
            bool flag = true;
            flag &= (Dungeon.phase == GamePhase.targeting);
            TargetTemplate template = Targeter.currentQuery;
            flag &= template.Compare(this, Targeter.source.owner);
            return flag;
        }
    }

    public void Start()
    {
        GameEvents.current.ListenQueryTarget(MarkTarget);
    }
    public void OnDestroy()
    {
        GameEvents.current.ListenQueryTarget(MarkTarget, false);
    }
    public void MarkTarget(TargetTemplate query, TargetNode source)
    {
        card?.particles.Clear();
        actor?.particles.Clear();
        if (query.Compare(this, source.owner) && (this != source))
        {
            Debug.Log("Marking " + name + " as a target");
            card?.particles.MarkValidTarget(true);
            actor?.particles.MarkValidTarget(true);
        } else if (this == source)
        {
            card?.particles.MarkSource(true);
            actor?.particles.MarkSource(true);
        }
    }
    public void DoubleClick()
    {
        if (targetable)
        {
            Targeter.AddTarget(GetComponent<TargetNode>());
            return;
        }
    }
    public void Damage(int value, Actor source, Keyword key)
    {
        actor?.Damage(value, source, key);
        card?.Damage(value, source, key);
    }
    */
}

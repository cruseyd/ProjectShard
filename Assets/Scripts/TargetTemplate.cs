using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemplateParam
{
    public TargetTemplate.Param param;
    public TargetTemplate.Op op;
    public int value;
    public TemplateParam(TargetTemplate.Param _param, TargetTemplate.Op _op, int _value)
    {
        param = _param; op = _op; value = _value;
    }
}

public class TargetTemplate
{
    public enum Op
    {
        EQ, NEQ, LT, LTE, GT, GTE
    }
    public enum Param
    {
        DEFAULT,
        LEVEL,
        POWER,
        ALLEGIANCE,
    }

    public bool isDamageable;
    public bool isActor;
    public bool isOpposing;
    public bool inPlay;
    public bool isSelf;
    public bool isAttackable;
    public Actor actor;
    public Actor owner;
    public Card.Type cardType;
    public Card.Color cardColor;
    public List<TemplateParam> templateParams;


    public TargetTemplate()
    {
        isDamageable = false;
        isOpposing = false;
        isActor = false;
        isSelf = false;
        inPlay = false;
        actor = null;
        owner = null;
        cardType = Card.Type.DEFAULT;
        cardColor = Card.Color.DEFAULT;
        templateParams = new List<TemplateParam>();
    }

    public void AddParam(Param p, Op op, int val)
    {
        templateParams.Add(new TemplateParam(p, op, val));
    }

    /*
    public bool Compare(TargetNode node, Actor self)
    {
        //if (node.card != null) { Debug.Log("Comparing with " + node.card.name); }
       // if (node.actor != null) { Debug.Log("Comparing with " + node.actor.name); }

        if (node == null) { return false; }
        bool flag = true;
        if (inPlay)
        {
            if (node.card != null)
            {
                flag &= (node.card.zone == CardZone.DUNGEON_ACTIVE || node.card.zone == CardZone.PLAYER_ACTIVE);
            }
            //Debug.Log("InPlay: " + flag);
            // if it's not a card, this has no effect
        }
        if (isDamageable)
        {
            flag &= (node.card != null && node.card.type == Card.Type.THRALL) || (node.actor != null);
            //Debug.Log("Damageable: " + flag);
        }
        if (isOpposing)
        {
            flag &= (node.card != null && node.card.owner != self) || (node.actor != null && node.actor != self);
            //Debug.Log("Opposing: " + flag);
        }
        if (isSelf)
        {
            flag &= (node.card != null && node.card.owner == self) || (node.actor != null && node.actor == self);
            //Debug.Log("Self: " + flag);
        }
        if (isAttackable)
        {
            if (node.card != null)
            {
                flag &= (node.card.type == Card.Type.THRALL);
            } else if (node.actor != null)
            {
                Card[] activeCards = node.actor.active;
                foreach (Card card in activeCards)
                {
                    flag &= (card.type != Card.Type.THRALL);
                }
            }
        }
        if (actor != null)
        {
            // looking for an actor node
            if (node.actor == null) { return false; }
            flag &= (node.actor == actor);
            //Debug.Log("Actor: " + flag);
        }
        if (node.card != null)
        {
            if (cardType != Card.Type.DEFAULT)
            {
                flag &= (node.card.type == cardType);
               // Debug.Log("Card Type: " + flag);
            }
            if (cardColor != Card.Color.DEFAULT)
            {
                flag &= (node.card.data.color == cardColor);
                //Debug.Log("Card Color: " + flag);
            }
            foreach (TemplateParam _param in templateParams)
            {
                switch (_param.param)
                {
                    case Param.LEVEL:
                        flag &= EvalOp(_param.op, node.card.focus, _param.value); break;
                    case Param.POWER:
                        flag &= EvalOp(_param.op, node.card.power, _param.value); break;
                    case Param.ALLEGIANCE:
                        flag &= EvalOp(_param.op, node.card.allegiance, _param.value); break;
                }
                //Debug.Log("Param " + _param.param + ": " + flag);
            }
        }
        return flag;
    }
    */
    public static bool EvalOp(Op op, int left, int right)
    {
        switch (op)
        {
            case Op.EQ: return (left == right);
            case Op.NEQ: return (left != right);
            case Op.LT: return (left < right);
            case Op.LTE: return (left <= right);
            case Op.GT: return (left > right);
            case Op.GTE: return (left >= right);
            default: return false;
        }
    }
    
}

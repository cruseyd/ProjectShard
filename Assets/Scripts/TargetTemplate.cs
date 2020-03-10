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
    public bool inHand;
    public bool inPlay;
    public bool isSelf;
    public bool isAttackable;
    public ITargetable isNot;
    public Keyword keyword;
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
        inHand = false;
        actor = null;
        owner = null;
        keyword = Keyword.DEFAULT;
        cardType = Card.Type.DEFAULT;
        cardColor = Card.Color.DEFAULT;
        templateParams = new List<TemplateParam>();
        isNot = null;
    }

    public void AddParam(Param p, Op op, int val)
    {
        templateParams.Add(new TemplateParam(p, op, val));
    }

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

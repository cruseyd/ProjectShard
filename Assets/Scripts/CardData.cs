using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "NewCardData", menuName = "CardData")]
public class CardData : ScriptableObject
{
    public new string name;
    public string id;
    public int level;

    public Card.Color color;
    public int violetAffinity;
    public int redAffinity;
    public int goldAffinity;
    public int greenAffinity;
    public int blueAffinity;
    public int indigoAffinity;

    public Card.Rarity rarity;
    public Card.Type type;
    public List<Keyword> keywords;
    public List<KeywordAbility.Key> abilityKeywords;

    public int strength;
    public int perception;
    public int finesse;

    [TextArea(minLines: 5, maxLines: 10)]
    public string flavorText;

    [Header("Thrall and Incantation Specific Parameters")]
    public int power;
    public int endurance;
    public int upkeep;
    public Keyword damageType;

    public bool Compare(TargetTemplate query)
    {
        bool flag = true;
        if (query.isNot != null) { return false; }
        if (query.isActor) { return false; }
        if (query.inHand) { return false; }
        if (query.inPlay) { return false; }
        if (query.isDamageable) { return false; }
        if (query.isAttackable) { return false; }
        if (query.isOpposing) { return false; }
        if (query.isSelf) { return false; }
        if (query.cardType.Count > 0)
        {
            bool success = false;
            foreach (Card.Type t in query.cardType)
            {
                if (t == this.type) { success = true; break; }
}
            if (!success) { return false; }
        }
        if (query.cardColor.Count > 0)
        {
            bool success = false;
            foreach (Card.Color c in query.cardColor)
            {
                if (c == color) { success = true; break; }
            }
            if (!success) { return false; }
        }
        if (query.keyword.Count > 0)
        {
            bool success = false;
            foreach (Keyword key in query.keyword)
            {
                if (keywords.Contains(key))
                {
                    success = true; break;
                }
            }
            if (!success) { return false; }
        }
        if (query.rarity.Count > 0)
        {
            bool success = false;
            foreach (Card.Rarity item in query.rarity)
            {
                if (item == rarity) { success = true; break; }
            }
            if (!success) { return false; }
        }
        foreach (TemplateParam _param in query.templateParams)
        {
            switch (_param.param)
            {
                case TargetTemplate.Param.LEVEL:
                    flag &= TargetTemplate.EvalOp(_param.op, level, _param.value); break;
                case TargetTemplate.Param.POWER:
                    flag &= TargetTemplate.EvalOp(_param.op, power, _param.value); break;
                case TargetTemplate.Param.HEALTH:
                    flag &= TargetTemplate.EvalOp(_param.op, endurance, _param.value); break;
            }
        }
        return flag;
    }

}

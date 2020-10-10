using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//[CreateAssetMenu(fileName = "NewCardData", menuName = "CardData")]
public class CardData : IComparable// : ScriptableObject
{
    public string name;
    public string id;
    public int level;
    public Card.Rarity rarity;

    public Card.Color color;
    public int violetAffinity;
    public int redAffinity;
    public int goldAffinity;
    public int greenAffinity;
    public int blueAffinity;
    public int indigoAffinity;

    
    public Card.Type type;
    public List<Keyword> keywords;
    public List<KeywordAbility.Key> abilityKeywords;

    public int strength;
    public int perception;
    public int finesse;

    //[TextArea(minLines: 5, maxLines: 10)]
    //public string flavorText;

    //[Header("Thrall and Incantation Specific Parameters")]
    public int power;
    public int endurance;
    public int upkeep;
    public Keyword damageType;

    public string text;

    public CardData(JSONCardData json)
    {
        Debug.Assert(json.name != null);
        Debug.Assert(json.rarity != null);
        Debug.Assert(json.color != null);
        name = json.name;
        id = name.ToUpper().Replace(" ", "_");
        level = json.level;
        rarity = (Card.Rarity)System.Enum.Parse(typeof(Card.Rarity), json.rarity);
        color = (Card.Color)System.Enum.Parse(typeof(Card.Color), json.color);

        violetAffinity = json.violet;
        redAffinity = json.red;
        greenAffinity = json.green;
        blueAffinity = json.blue;
        goldAffinity = json.gold;
        indigoAffinity = json.indigo;

        Debug.Assert(json.type != null);
        type = (Card.Type)System.Enum.Parse(typeof(Card.Type), json.type);

        keywords = new List<Keyword>();
        if (json.key1 != null) { keywords.Add((Keyword)System.Enum.Parse(typeof(Keyword), json.key1)); }
        if (json.key2 != null) { keywords.Add((Keyword)System.Enum.Parse(typeof(Keyword), json.key2)); }
        if (json.key3 != null) { keywords.Add((Keyword)System.Enum.Parse(typeof(Keyword), json.key3)); }

        abilityKeywords = new List<KeywordAbility.Key>();
        if (json.ka1 != null) { abilityKeywords.Add((KeywordAbility.Key)System.Enum.Parse(typeof(KeywordAbility.Key), json.ka1)); }
        if (json.ka2 != null) { abilityKeywords.Add((KeywordAbility.Key)System.Enum.Parse(typeof(KeywordAbility.Key), json.ka2)); }
        if (json.ka3 != null) { abilityKeywords.Add((KeywordAbility.Key)System.Enum.Parse(typeof(KeywordAbility.Key), json.ka3)); }

        switch (type)
        {
            case Card.Type.THRALL:
            case Card.Type.CONSTANT:
                power = json.stat1;
                endurance = json.stat2;
                upkeep = json.stat3;
                break;
            case Card.Type.TECHNIQUE:
            case Card.Type.SPELL:
            case Card.Type.IDEAL:
                strength = json.stat1;
                perception = json.stat2;
                finesse = json.stat3;
                break;
            default: break;
        }

        if (json.dmg_type != null) { damageType = (Keyword)System.Enum.Parse(typeof(Keyword), json.dmg_type); }
        if (json.text != null) { text = Icons.Parse(json.text); }
        else { text = ""; }
    }

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

    public int CompareTo(object obj)
    {
        return id.CompareTo(((CardData)obj).id);
    }
}

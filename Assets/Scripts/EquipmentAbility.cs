using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EquipmentAbility
{
    private static Dictionary<string, EquipmentAbility> _index;
    public static Dictionary<string, EquipmentAbility> index
    {
        get
        {
            if (_index == null)
            {
                _index = new Dictionary<string, EquipmentAbility>();
                _index["NULL"] = new EA_Null();
                _index["CHAINMAIL"] = new EA_Chainmail();
            }
            return _index;
        }
    }
    
    public abstract string text(Equipment source);
    public virtual void weapon() { }
    public virtual void armor(DamageData damage)
    {
    }
    public virtual void relic() { }
}

public class EA_Null : EquipmentAbility
{
    public override string text(Equipment source)
    {
        return "EMPTY ABILITY";
    }
}
public class EA_Chainmail : EquipmentAbility
{
    public override void armor(DamageData damage)
    {
        if (damage.target is Actor)
        {
            if (damage.type == Keyword.SLASHING && ((Actor)damage.target).armor.durability > 0)
            {
                damage.damage -= 1;
                ((Actor)damage.target).armor.durability -= 1;
            }
        }
        
    }

    public override string text(Equipment source)
    {
        return "1 Protection from Slashing damage.";
    }
}

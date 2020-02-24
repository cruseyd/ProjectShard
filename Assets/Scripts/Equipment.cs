using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Tooltip))]
public class Equipment : MonoBehaviour
{
    public enum Type
    {
        WEAPON,
        ARMOR,
        RELIC
    }

    [SerializeField] private ValueDisplay _durabilityDisplay;
    private Actor _owner;
    private EquipmentData _data;
    private int _durability;
    private EquipmentAbility _ability;

    public Actor owner { get { return _owner; } }
    public new string name
    { 
        get 
        { 
            if (_data == null) { return "NO EQUIPMENT FOUND"; }
            return _data.name;
        }
    }
    public int durability
    {
        get { return _durability; }
        set {
            _durability = Mathf.Clamp(value, 0, _data.durability);
            _durabilityDisplay.value = durability;
        }
    }

    public bool HasKeyword(Keyword key)
    {
        if (_data == null) { return false; }
        foreach (Keyword word in _data.keywords)
        {
            if (key == word) { return true; }
        }
        return false;
    }

    public void Unequip()
    {
        this.gameObject.SetActive(false);
        if (_data == null) { return; }
        switch (_data.type)
        {
            case Type.ARMOR: owner.events.onTakeModifiedDamage -= _ability.armor; break;
            case Type.WEAPON: break;
            case Type.RELIC: break;
            default: return;
        }
        _data = null;
        _durability = -1;
        
    }
    public void Equip(EquipmentData data, Actor actor)
    {
        Unequip();
        if (data == null) {return; }
        this.gameObject.SetActive(true);
        _data = data;
        _owner = actor;
        durability = data.durability;
        _durabilityDisplay.baseValue = data.durability;
        if (EquipmentAbility.index.ContainsKey(data.id))
        {
            _ability = EquipmentAbility.index[data.id];
        }
        GetComponent<Tooltip>().header = data.name;
        GetComponent<Tooltip>().content = Keywords.Parse(data.type);
        GetComponent<Tooltip>().content += ("\n\n" + _ability.text(this));
        switch (_data.type)
        {
            case Type.ARMOR: owner.events.onTakeModifiedDamage += _ability.armor; break;
            case Type.WEAPON: break;
            case Type.RELIC: break;
            default: return;
        }
    }
}

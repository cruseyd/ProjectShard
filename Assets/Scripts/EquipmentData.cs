using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEquipmentData", menuName = "EquipmentData")]
public class EquipmentData : ScriptableObject
{
    public new string name;
    public string id;

    public Equipment.Type type;
    public List<Keyword> keywords;
    public int durability;
}

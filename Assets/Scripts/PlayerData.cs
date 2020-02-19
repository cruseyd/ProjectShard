using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "PlayerData")]
public class PlayerData : ScriptableObject
{
    public Decklist decklist;

    public EquipmentData weapon;
    public EquipmentData armor;
    public EquipmentData relic;
}

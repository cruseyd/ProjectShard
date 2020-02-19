using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "EnemyData")]
public class EnemyData : ScriptableObject
{
    public new string name;
    public int maxHealth;

    public EquipmentData weapon;
    public EquipmentData armor;

    public CardPool cardPool;
    
}

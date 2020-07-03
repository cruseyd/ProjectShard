using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "EnemyData")]
public class EnemyData : ScriptableObject, IComparable
{
    public new string name;
    public int maxHealth;

    public EquipmentData weapon;
    public EquipmentData armor;

    public CardPool cardPool;
    public Decklist decklist;

    public int CompareTo(object obj)
    {
        EnemyData rhs = obj as EnemyData;

        if (rhs != null)
        {
            return name.CompareTo(rhs.name);
        }
        else
        {
            throw new ArgumentException("Tried to compare an EnemyData to a non-EnemyData");
        }

        throw new NotImplementedException();
    }
}

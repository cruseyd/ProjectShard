using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class EnemyDataArray
{
    public List<EnemyData> enemies;
}

[System.Serializable]
public class EnemyData : IComparable
{
    public string name;
    public int maxHealth;
    //public CardPool cardPool;
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

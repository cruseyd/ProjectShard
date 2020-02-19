using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DamageData
{
    private int _damage;
    public int damage
    {
        get { return _damage; }
        set
        {
            _damage = value;
        }
    }
    public Keyword type;
    public ITargetable source;
    public IDamageable target;

    public DamageData(int value, Keyword key, ITargetable src, IDamageable trg)
    {
        _damage = value;
        type = key;
        source = src;
        target = trg;
    }
}
public interface IDamageable
{
    void Damage(DamageData data);
}

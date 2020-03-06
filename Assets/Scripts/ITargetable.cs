using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IMonoBehaviour
{
    string name { get; }
    Transform transform { get; }
    GameObject gameObject { get; }


}

public class TargetEvents
{
    private ITargetable _source;
    public TargetEvents(ITargetable source)
    {
        _source = source;
    }

    public event Action<StatusEffect, int> onGainStatus;
    public event Action<StatusEffect, int> onRemoveStatus;

    public event Action<DamageData> onDealRawDamage;
    public event Action<DamageData> onDealModifiedDamage;
    public event Action<DamageData> onDealDamage;

    public event Action<DamageData> onTakeRawDamage;
    public event Action<DamageData> onTakeModifiedDamage;
    public event Action<DamageData> onTakeDamage;

    public event Action<int> onGainHealth;
    public event Action<int> onLoseHealth;

    public event Action<ITargetable, ITargetable> onDeclareTarget;
    public event Action<Card, ITargetable> onDeclareAttack;

    public void GainStatus(StatusEffect status, int stacks) { onGainStatus?.Invoke(status, stacks); }
    public void RemoveStatus(StatusEffect status, int stacks) { onRemoveStatus?.Invoke(status, stacks); }

    public void DealRawDamage(DamageData data) { onDealRawDamage?.Invoke(data); }
    public void DealModifiedDamage(DamageData data) { onDealModifiedDamage?.Invoke(data); }
    public void DealDamage(DamageData data) { onDealDamage?.Invoke(data); }

    public void TakeRawDamage(DamageData data) { onTakeRawDamage?.Invoke(data); }
    public void TakeModifiedDamage(DamageData data) { onTakeModifiedDamage?.Invoke(data); }
    public void TakeDamage(DamageData data) { onTakeDamage?.Invoke(data); }

    public void GainHealth(int value) { onGainHealth?.Invoke(value); }
    public void LoseHealth(int value) { onLoseHealth?.Invoke(value); }

    public void DeclareTarget(ITargetable source, ITargetable target) { onDeclareTarget?.Invoke(source, target); }
    public void DeclareAttack(Card source, ITargetable target) { onDeclareAttack?.Invoke(source, target); }

}

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
    public ITargetable target;

    public DamageData(int value, Keyword key, ITargetable src, ITargetable trg)
    {
        _damage = value;
        type = key;
        source = src;
        target = trg;
    }
}

public interface ITargetable : IMonoBehaviour
{
    bool playerControlled { get; }
    Actor controller { get; }
    Actor opponent { get; }
    TargetEvents targetEvents { get; }

    bool inPlay { get; }
    void AddTarget(ITargetable target);
    void FindTargets(Ability.Mode mode, int n, bool show = false);
    List<ICommand> FindMoves();
    void MarkTarget(TargetTemplate query, ITargetable source, bool show);
    bool Compare(TargetTemplate query, Actor self);
    TargetTemplate GetQuery(Ability.Mode mode, int n);
    bool Resolve(Ability.Mode mode, List<ITargetable> targets);

    void AddStatus(StatusName id, int stacks = 1);
    void RemoveStatus(StatusName id, int stacks = 1);
    int GetStatus(StatusName id);

    void Damage(DamageData data);
    void ResolveDamage(DamageData data);

    void IncrementHealth(int value);
}

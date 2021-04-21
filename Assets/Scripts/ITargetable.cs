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

public class Attempt
{
    public Attempt() { success = true; }
    public bool success;
}

public class TargetEvents
{
    private ITargetable _source;
    public TargetEvents(ITargetable source)
    {
        _source = source;
    }

    public event Action<StatusEffect.ID, int, Attempt> onTryGainStatus;
    public event Action<StatusEffect, int> onGainStatus;
    public event Action<StatusEffect, int> onRemoveStatus;

    public event Action<DamageData> onDealRawDamage;
    public event Action<DamageData> onDealModifiedDamage;
    public event Action<DamageData> onDealDamage;
    public event Action<DamageData> onDealOverflowDamage;

    public event Action<DamageData> onTakeRawDamage;
    public event Action<DamageData> onTakeModifiedDamage;
    public event Action<DamageData> onTakeDamage;

    public event Action<int> onGainHealth;
    public event Action<int> onLoseHealth;

    public event Action<ITargetable, ITargetable> onDeclareTarget;
    public event Action<Card, ITargetable> onDeclareAttack;

    public event Action onRefresh;

    public void TryGainStatus(StatusEffect.ID status, int stacks, Attempt attempt) { onTryGainStatus?.Invoke(status, stacks, attempt); }
    public void GainStatus(StatusEffect status, int stacks) { onGainStatus?.Invoke(status, stacks); }
    public void RemoveStatus(StatusEffect status, int stacks) { onRemoveStatus?.Invoke(status, stacks); }

    public void DealRawDamage(DamageData data)
    {
        onDealRawDamage?.Invoke(data); }
    public void DealModifiedDamage(DamageData data) { onDealModifiedDamage?.Invoke(data); }
    public void DealDamage(DamageData data) { onDealDamage?.Invoke(data); }
    public void DealOverFlowDamage(DamageData data) { onDealOverflowDamage?.Invoke(data); }
    public void TakeRawDamage(DamageData data)
    {
        onTakeRawDamage?.Invoke(data);
    }
    public void TakeModifiedDamage(DamageData data) { onTakeModifiedDamage?.Invoke(data); }
    public void TakeDamage(DamageData data) { onTakeDamage?.Invoke(data); }

    public void GainHealth(int value) { onGainHealth?.Invoke(value); }
    public void LoseHealth(int value) { onLoseHealth?.Invoke(value); }

    public void DeclareTarget(ITargetable source, ITargetable target) { onDeclareTarget?.Invoke(source, target); }
    public void DeclareAttack(Card source, ITargetable target) { onDeclareAttack?.Invoke(source, target); }

    public void Refresh() { onRefresh?.Invoke(); }

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
    public bool isAttackDamage;

    public DamageData(int value, Keyword key, ITargetable src, ITargetable trg, bool isAttack = false)
    {
        _damage = value;
        type = key;
        source = src;
        target = trg;
        isAttackDamage = isAttack;
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
    bool IsTargeting(ITargetable target);
    bool Compare(TargetTemplate query, Actor self);
    TargetTemplate GetQuery(Ability.Mode mode, int n);
    bool Resolve(Ability.Mode mode, List<ITargetable> targets);

    void AddStatus(StatusEffect.ID id, int stacks = 1);
    void RemoveStatus(StatusEffect.ID id, int stacks = 9999);
    void RemoveAllStatus();

    int GetStatus(StatusEffect.ID id);

    List<StatusEffect.ID> GetAllStatus();
    void Damage(DamageData data);
    void ResolveDamage(DamageData data);

    void IncrementHealth(int value);
}

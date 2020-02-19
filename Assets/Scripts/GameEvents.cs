using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class GameEvents : MonoBehaviour
{
    public static GameEvents current;

    private void Awake()
    {
        if (current == null)
        {
            current = this;
        } else
        {
            Destroy(this.gameObject);
        }
    }

    // Game Phase Events

    public event Action onRefresh;
    public event Action<Actor> onStartTurn;
    public event Action<TargetTemplate, ITargetable, bool> onQueryTarget;
    public void Refresh() { onRefresh?.Invoke(); }
    public void StartTurn(Actor actor) { onStartTurn?.Invoke(actor); }
    public void QueryTarget(TargetTemplate query, ITargetable source, bool show)
    {
        onQueryTarget?.Invoke(query, source, show);
    }

    /*
    private event Action<DamageData> onPlayerRawDamage;
    private event Action<DamageData> onEnemyRawDamage;
    public void ListenRawDamage(Actor actor, Action<DamageData> handler, bool listen = true)
    {
        if (actor.isPlayer)
        {
            if (listen) { onPlayerRawDamage += handler; }
            else { onPlayerRawDamage -= handler; }
        }
        else
        {
            if (listen) { onEnemyRawDamage += handler; }
            else { onEnemyRawDamage -= handler; }
        }
    }
    public void RawDamage(Actor actor, DamageData data)
    {
        if (actor.isPlayer) { onPlayerRawDamage?.Invoke(data); }
        else { onEnemyRawDamage?.Invoke(data); }
    }

    private event Action<DamageData> onPlayerModifiedDamage;
    private event Action<DamageData> onEnemyModifiedDamage;
    public void ListenModifiedDamage(Actor actor, Action<DamageData> handler, bool listen = true)
    {
        if (actor.isPlayer)
        {
            if (listen) { onPlayerModifiedDamage += handler; }
            else { onPlayerModifiedDamage -= handler; }
        }
        else
        {
            if (listen) { onEnemyModifiedDamage += handler; }
            else { onEnemyModifiedDamage -= handler; }
        }
    }
    public void ModifiedDamage(Actor actor, DamageData data)
    {
        if (actor.isPlayer) { onPlayerModifiedDamage?.Invoke(data); }
        else { onEnemyModifiedDamage?.Invoke(data); }
    }

    private event Action<DamageData> onPlayerReceiveDamage;
    private event Action<DamageData> onEnemyReceiveDamage;
    public void ListenReceiveDamage(Actor actor, Action<DamageData> handler, bool listen = true)
    {
        if (actor.isPlayer)
        {
            if (listen) { onPlayerReceiveDamage += handler; }
            else { onPlayerReceiveDamage -= handler; }
        }
        else
        {
            if (listen) { onEnemyReceiveDamage += handler; }
            else { onEnemyReceiveDamage -= handler; }
        }
    }
    public void ReceiveDamage(Actor actor, DamageData data)
    {
        if (actor.isPlayer) { onPlayerReceiveDamage?.Invoke(data); }
        else { onEnemyReceiveDamage?.Invoke(data); }
    }
    */
}

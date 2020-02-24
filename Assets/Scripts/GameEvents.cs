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
    public event Action<Actor> onEndTurn;
    public event Action<TargetTemplate, ITargetable, bool> onQueryTarget;
    public event Action<TemplateModifier> onAddGlobalModifier;
    public event Action<TemplateModifier> onRemoveGlobalModifier;
    public void Refresh() { onRefresh?.Invoke(); }
    public void StartTurn(Actor actor) { onStartTurn?.Invoke(actor); }
    public void EndTurn(Actor actor) { onEndTurn?.Invoke(actor); }
    public void QueryTarget(TargetTemplate query, ITargetable source, bool show)
    {
        onQueryTarget?.Invoke(query, source, show);
    }
    public void AddGlobalModifier(TemplateModifier mod) { onAddGlobalModifier?.Invoke(mod); }
    public void RemoveGlobalModifier(TemplateModifier mod) { onRemoveGlobalModifier?.Invoke(mod); }
}

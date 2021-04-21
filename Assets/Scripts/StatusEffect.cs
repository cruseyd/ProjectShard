using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;


public class StatusEffectEvents
{
    private StatusEffect _source;
    public StatusEffectEvents(StatusEffect source)
    {
        _source = source;
    }

    public event Action<StatusEffect> onRemove;
    public event Action<StatusEffect, int> onGainStacks;
    public event Action<StatusEffect, int> onLoseStacks;

    public void Remove() { onRemove?.Invoke(_source); }
    public void GainStacks(int stacks) { onGainStacks?.Invoke(_source, stacks); }
    public void LoseStacks(int stacks) { onLoseStacks?.Invoke(_source, stacks); }

}
public class StatusEffect : MonoBehaviour
{

    public enum ID
    {
        DEFAULT,
        DAZE,
        STUN,
        CHILL,
        FROZEN,
        IMPALE,
        BURN,
        MEMORIZED,
        INSIGHT,
        ARMOR,
        FRENZY,
        FIREBRAND,
        ALACRITY,
        MIGHT,
        ATROPHY,
        SHARPEN = 1000,
        LEGERDEMAIN
    }

    private static GameObject _prefab;

    private StatusData _data;
    private ITargetable _target;
    private StatusAbility _ability;
    private int _stacks;

    [SerializeField] private ValueDisplay _display;
    [SerializeField] private Image _border;
    [SerializeField] private Image _icon;
    [SerializeField] private Tooltip _tooltip;

    public StatusEffectEvents events;

    public static string Parse(string input)
    {
        string output = input;
        string[] StatusNames = System.Enum.GetNames(typeof(StatusEffect.ID));
        foreach (string name in StatusNames)
        {
            output = output.Replace(name, "<color=#00FFFF><b>" + name.ToLower() + "</b></color>");
        }
        return output;
    }

    public bool stackable { get { return _data.stackable; } }
    public bool idDebuff { get { return _data.isDebuff; } }
    public ID id { get { return _data.id; } }
    public ITargetable target { get { return _target; } }
    public int stacks
    {
        get
        {
            return _stacks;
        }
        set
        {
            int delta = value - _stacks;
            _stacks = value;
            if (delta > 0) { events.GainStacks(delta); }
            if (delta < 0) { events.LoseStacks(-delta); }
            if (_stacks <= 0)
            {
                target.RemoveStatus(id, 9999);
            }
        }
    }

    public static float Threat(ID a_id)
    {
        switch (a_id)
        {
            case ID.DAZE:
            case ID.CHILL:
                return -0.1f;
            case ID.FROZEN:
            case ID.STUN:
            case ID.IMPALE:
                return -0.5f;
            case ID.BURN:
                return -0.2f;
            case ID.INSIGHT:
            case ID.FRENZY:
                return +0.2f;
            case ID.FIREBRAND:
                return +0.1f;
            case ID.MIGHT:
                return +1.0f;
            default: return 0f;
        }
    }
    public static StatusEffect Spawn(ID a_id, ITargetable a_target, int a_stacks = 1)
    {
        if (_prefab == null)
        {
            _prefab = Resources.Load<GameObject>("Prefabs/StatusEffect") as GameObject;
        }

        GameObject effectGO = Instantiate(_prefab) as GameObject;
        StatusEffect effect = effectGO.GetComponent<StatusEffect>();
        effect._data = StatusData.Get(a_id);
        Debug.Assert(effect._data != null);
        effect._target = a_target;
        effect._stacks = a_stacks;

        effect._border.color = effect._data.backgroundColor;
        effect._icon.color = effect._data.iconColor;
        effect._tooltip.header = effect._data.tooltipHeader;
        effect._tooltip.content = effect._data.tooltipContent;

        effect.events = new StatusEffectEvents(effect);
        effect._ability = StatusAbility.Get(effect, a_target, a_stacks);
        a_target.targetEvents.onRefresh += effect.Refresh;
        return effect;
    }
    public void Refresh()
    {
        if (stackable)
        {
            _display.value = stacks;
            _display.Refresh();
        } else
        {
            _display.SetActive(false);
        }
    }
    public void Remove()
    {
        _ability?.Remove();
        _target.targetEvents.onRefresh -= Refresh;
        events.Remove();
        Destroy(this.gameObject);
    }
}

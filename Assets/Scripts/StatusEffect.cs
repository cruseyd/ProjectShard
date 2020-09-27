using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        ATROPHY
    }

    private static GameObject _prefab;

    private StatusData _data;
    private ITargetable _target;
    private StatusAbility _ability;
    [SerializeField] private ValueDisplay _display;
    [SerializeField] private Image _border;
    [SerializeField] private Image _icon;
    [SerializeField] private Tooltip _tooltip;

    
    public bool stackable { get { return _data.stackable; } }
    public ID id { get { return _data.id; } }
    public ITargetable target { get { return _target; } }
    public int stacks
    {
        get
        {
            return _ability.stacks;
        }
        set
        {
            _ability.stacks = value;
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
        
        effect._border.color = effect._data.backgroundColor;
        effect._icon.color = effect._data.iconColor;
        effect._tooltip.header = effect._data.tooltipHeader;
        effect._tooltip.content = effect._data.tooltipContent;

        effect._ability = StatusAbility.Get(a_id, a_target, a_stacks);
        Debug.Assert(effect._ability != null);
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
        _ability.Remove();
        _target.targetEvents.onRefresh -= Refresh;
        Destroy(this.gameObject);
    }
}

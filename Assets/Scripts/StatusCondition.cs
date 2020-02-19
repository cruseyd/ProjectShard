using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


[RequireComponent(typeof(Tooltip))]
public class StatusCondition : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Image _background;
    [SerializeField] private Image _border;
    [SerializeField] private int _stacks;

    public ITargetable target;
    public int stacks
    {
        get { return _stacks; }
        set {
            _stacks = Mathf.Max(value, 0);
            if (!_data.stackable)
            {
                _stacks = Mathf.Min(_stacks, 1);
            } else
            {
                _text.text = _stacks.ToString();
            }
            if (_stacks <= 0)
            {
                Remove();
            }
        }
    }
    [SerializeField] private StatusData _data;
    
    public static string Icon(StatusName id)
    {
        switch (id)
        {
            default: return "";
        }
    }

    public void SetStatus(StatusData data, int numStacks = 1)
    {
        gameObject.SetActive(true);
        _data = data;
        _background.color = _data.backgroundColor;
        _text.color = _data.iconColor;
        stacks = numStacks;
        if (!_data.stackable)
        {
            _text.text = Icon(_data.id);
        }
        GetComponent<Tooltip>().header = _data.tooltipHeader;
        GetComponent<Tooltip>().content = _data.tooltipContent;
        Debug.Log("Setting status of " + target.name + " to " + _data.id);
        
        switch (_data.id)
        {
            case StatusName.POISON:
                target.Controller().events.onStartTurn += Poison; break;
            case StatusName.BURN:
                target.Controller().events.onStartTurn += Burn; break;
            case StatusName.STUN:
                target.Controller().events.onStartTurn += Stun; break;
            default: break;
        }
    }
    public void Remove()
    {
        switch (_data.id)
        {
            case StatusName.POISON:
                target.Controller().events.onStartTurn -= Poison; break;
            case StatusName.BURN:
                target.Controller().events.onStartTurn -= Burn; break;
            case StatusName.STUN:
                target.Controller().events.onStartTurn -= Stun; break;
            default: break;
        }
        gameObject.SetActive(false);
        _data = null;
        _stacks = 0;
        _text.text = "";
    }

    private void Poison()
    {
        Debug.Assert(target is IDamageable);
        ((IDamageable)target).Damage(new DamageData(1, Keyword.POISON, null, (IDamageable)target));
        stacks -= 1;
    }
    private void Burn()
    {
        Debug.Assert(target is IDamageable);
        ((IDamageable)target).Damage(new DamageData(stacks, Keyword.FIRE, null, (IDamageable)target));
        stacks -= 1;
    }

    private void Stun()
    {
        Debug.Assert(target is Card);
        Debug.Assert(((Card)target).type == Card.Type.THRALL);
        ((Card)target).attackAvailable = false;
        Remove();
    }
}

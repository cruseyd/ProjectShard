using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ValueDisplay : Display
{
    [SerializeField] public string _valueName;
    [SerializeField] private bool checkBaseValue;
    [SerializeField] private bool suppressHighlight;
    [SerializeField] private bool reverseHighlight;
    [SerializeField] private bool useFraction;

    public int _value;
    private int _baseValue;

    public string valueName { get { return _valueName; } set { _valueName = value; Refresh(); } }
    public int value { get { return _value;} set { _value = value; Refresh(); } }
    public int baseValue { get { return _baseValue; } set { _baseValue = value; Refresh(); } }

    [SerializeField] private TextMeshProUGUI text;

    public override void Refresh()
    {
        if (_valueName != "")
        {
            text.text = _valueName + " ";
        } else
        {
            text.text = "";
        }
        if (checkBaseValue)
        {
            if ((_value > _baseValue && !reverseHighlight)
            || (_value < _baseValue && reverseHighlight))
            {
                if (!suppressHighlight){ text.text += "<color=#00ffff>"; }
                text.text += _value.ToString();
                if (!suppressHighlight) { text.text += "</color>"; }
                if (useFraction)
                {
                    text.text += "<color=white>/" + _baseValue.ToString() + "</color>";
                }
                return;
            }
            else if ((_value < _baseValue && !reverseHighlight)
                  || (_value > _baseValue && reverseHighlight))
            {
                if (!suppressHighlight) { text.text += "<color=red>"; }
                text.text += _value.ToString();
                if (!suppressHighlight) { text.text += "</color>"; }
                if (useFraction)
                {
                    text.text += "<color=white>/" + _baseValue.ToString() + "</color>";
                }
                return;
            }
        }
        text.text += "<color=white>" + _value.ToString() + "</color>";
        if (useFraction)
        {
            text.text += "<color=white>/" + _baseValue.ToString() + "</color>";
        }

    }
}

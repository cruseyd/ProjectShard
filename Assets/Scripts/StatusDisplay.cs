using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Tooltip))]
public class StatusDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _content;
    [SerializeField] private Image _background;
    [SerializeField] private Image _border;

    private StatusEffect _status;
    public StatusEffect status { get { return _status; } }

    public void SetStatus(StatusEffect a_status)
    {
        if (a_status == null)
        {
            _status = null;
            gameObject.SetActive(false);
        } else
        {
            _status = a_status;
            gameObject.SetActive(true);
            _background.color = status.data.backgroundColor;
            _content.color = status.data.iconColor;

            Tooltip tooltip = GetComponent<Tooltip>();
            tooltip.content = status.data.tooltipContent;
            tooltip.header = status.data.tooltipHeader;
            // set tooltip info
            Refresh();
        }
    }

    public void Refresh()
    {
        if (_status == null) { return; }
        if (status.data.stackable)
        {
            _content.text = status.stacks.ToString();
        }
    }
}

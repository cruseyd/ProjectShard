using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class DoubleClickEvent : UnityEvent<Transform> { }

public class DoubleClickable : MonoBehaviour, IPointerClickHandler
{
    public DoubleClickEvent doubleClickEvent;
    public float clickDelay = 0.5f;
    private float prevClickTime = 0.0f;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if ((Time.time - prevClickTime) < clickDelay)
            {
                doubleClickEvent.Invoke(this.transform);
            }
            prevClickTime = Time.time;
        }
    }
}

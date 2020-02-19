using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string header;
    [TextArea(minLines: 5, maxLines: 10)]
    public string content;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector2 pos = eventData.position;
        GameObject window = Dungeon.tooltipWindow;
        window.transform.Find("header").GetComponent<TextMeshProUGUI>().text = header;
        window.transform.Find("content").GetComponent<TextMeshProUGUI>().text = content;
        window.SetActive(true);
        float dx = window.GetComponent<RectTransform>().rect.width * 0.6f;
        float dy = window.GetComponent<RectTransform>().rect.height * 0.6f;
        if ((pos.x - Screen.width/2.0f) > 0) { dx *= -1; }
        if ((pos.y - Screen.height/2.0f) > 0) { dy *= -1; }
        window.transform.position = new Vector2( pos.x + dx, pos.y + dy);
        window.transform.SetAsLastSibling();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Dungeon.tooltipWindow.SetActive(false);
    }
}

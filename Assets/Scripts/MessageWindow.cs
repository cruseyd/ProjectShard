using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class MessageWindow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int maxMessages;
    private Vector2 _baseRect;

    [SerializeField] private Transform _content;
    [SerializeField] private GameObject _contentPrefab;
    [SerializeField] private ScrollRect scrollRect;

    private Queue<GameObject> _messages;
    // Start is called before the first frame update
    void Awake()
    {
        _messages = new Queue<GameObject>();
        _baseRect = GetComponent<RectTransform>().sizeDelta;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Add("You pressed 'space' " + _messages.Count);
        }
    }

    public void Add(string message)
    {
        GameObject msgGO = Instantiate(_contentPrefab) as GameObject;
        msgGO.transform.SetParent(_content);
        //msgGO.transform.SetSiblingIndex(0);
        msgGO.GetComponent<TextMeshProUGUI>().text = message;
        _messages.Enqueue(msgGO);
        if (_messages.Count > maxMessages)
        {
            Destroy(_messages.Dequeue());
        }
        scrollRect.verticalNormalizedPosition = 0;
    }
    public IEnumerator Expand(bool flag, float heightScale = 1, float duration = 0.25f)
    {
        if (flag)
        {
            yield return new WaitForSeconds(1);
        }
        float t = 0;
        Vector2 start = GetComponent<RectTransform>().sizeDelta;
        Vector2 end = _baseRect;
        if (flag)
        {
            end = new Vector2(_baseRect.x, _baseRect.y * heightScale);
        }
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(start, end, t);
            yield return null;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
        StopAllCoroutines();
        StartCoroutine(Expand(true, 3));
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(Expand(false));
    }
}

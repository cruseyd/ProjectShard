using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


public class CardHeader : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{

    private static GameObject _prefab;

    [SerializeField] private Image _border;
    [SerializeField] private ValueDisplay _cost;
    [SerializeField] private GameObject _affinity;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _stats;

    private CardData _data;
    private Transform _parent;
    public CardData data { get { return _data; } }

    public static CardHeader Spawn(CardData a_data)
    {
        if (CardHeader._prefab == null)
        {
            _prefab = Resources.Load("Prefabs/CardHeader") as GameObject;
        }
        GameObject go = Instantiate(_prefab, Vector3.zero, Quaternion.identity);
        CardHeader header = go.GetComponent<CardHeader>();

        header._data = a_data;
        header._border.color = GameData.GetColor(a_data.color);

        header._title.text = a_data.name;
        if (a_data.type == Card.Type.THRALL)
        {
            header._stats.text = a_data.power + " / " + a_data.endurance + " / " + a_data.upkeep;
        } else if (a_data.type == Card.Type.CONSTANT)
        {
            header._stats.text = a_data.upkeep.ToString();
        } else
        {
            header._stats.text = a_data.strength + " / " + a_data.finesse + " / " + a_data.perception;
        }
        header._cost.value = a_data.level;
        header._cost.Refresh();

        List<Image> pips = new List<Image>();
        foreach (Transform pip in header._affinity.transform)
        {
            pips.Add(pip.GetComponent<Image>());
        }

        int pipNum = 0;
        for (int ii = 0; ii < a_data.violetAffinity; ii++)
        {
            pips[pipNum].enabled = true;
            pips[pipNum].color = GameData.GetColor(Card.Color.LIS);
            pipNum++;
        }
        for (int ii = 0; ii < a_data.redAffinity; ii++)
        {
            pips[pipNum].enabled = true;
            pips[pipNum].color = GameData.GetColor(Card.Color.RAIZ);
            pipNum++;
        }
        for (int ii = 0; ii < a_data.goldAffinity; ii++)
        {
            pips[pipNum].enabled = true;
            pips[pipNum].color = GameData.GetColor(Card.Color.ORA);
            pipNum++;
        }
        for (int ii = 0; ii < a_data.greenAffinity; ii++)
        {
            pips[pipNum].enabled = true;
            pips[pipNum].color = GameData.GetColor(Card.Color.FEN);
            pipNum++;
        }
        for (int ii = 0; ii < a_data.blueAffinity; ii++)
        {
            pips[pipNum].enabled = true;
            pips[pipNum].color = GameData.GetColor(Card.Color.IRI);
            pipNum++;
        }
        for (int ii = 0; ii < a_data.indigoAffinity; ii++)
        {
            pips[pipNum].enabled = true;
            pips[pipNum].color = GameData.GetColor(Card.Color.VAEL);
            pipNum++;
        }
        while (pipNum < pips.Count)
        {
            pips[pipNum].gameObject.SetActive(false);
            pipNum++;
        }

        return header;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Dungeon.EnableDropZones(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        List<RaycastResult> hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, hits);
        foreach (RaycastResult hit in hits)
        {
            if (hit.gameObject == DecklistDisplay.transferZone)
            {
                DecklistDisplay.instance.Transfer(this);
                Dungeon.EnableDropZones(false);
                return;
            }
        }
        _parent = transform.parent;
        transform.parent = null;
        transform.parent = _parent;
        Dungeon.EnableDropZones(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CardZone zone = Dungeon.GetZone(CardZone.Type.PREVIEW);
        CardGraphic card = CardGraphic.Spawn(_data, zone.transform.position);
        card.FaceUp(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        List<Card> cards = Dungeon.GetCards(CardZone.Type.PREVIEW);
        for (int ii = 0; ii < cards.Count; ii++)
        {
            cards[ii].Delete();
        }
    }
}

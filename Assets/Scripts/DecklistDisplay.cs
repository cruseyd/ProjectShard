using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DecklistDisplay : MonoBehaviour
{
    public static DecklistDisplay instance;
    public enum LabelIndex
    {
        COST,
        AFFINITY,
        TYPE
    }

    public enum ColumnIndex_Type
    {
        SPELL,
        TECHNIQUE,
        THRALL,
        INCANTATION
    }

    [SerializeField] private List<ColumnDisplay> _columns;
    [SerializeField] private GameObject _toggleButton;
    [SerializeField] private GameObject _transferDropZone;

    private LabelIndex _sortedBy;
    private bool _showingDeck;
    private List<CardHeader> _cards;
    private List<CardData> _deckData;
    private List<CardData> _binData;

    public static GameObject transferZone
    {
        get
        {
            return instance?._transferDropZone;
        }
    }
    public List<CardData> deck
    {
        get
        {
            CardData filler = Resources.Load<CardData>("Cards/Neutral/Fatigue");
            while (_deckData.Count < GameData.instance.minDeckSize)
            {
                _deckData.Add(filler);
            }
            return _deckData;
        }
    }
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            _cards = new List<CardHeader>();
            _deckData = new List<CardData>();
            _binData = new List<CardData>();
        } else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        _showingDeck = true;
        _transferDropZone.SetActive(false);
        _toggleButton.GetComponentInChildren<TextMeshProUGUI>().text = "Show Bin";
        SortBy(LabelIndex.COST);
    }

    public void SortBy(LabelIndex label)
    {
        if (_sortedBy == label) { return; }
        _sortedBy = label;
        foreach (ColumnDisplay display in _columns)
        {
            display.ShowLabel((int)label);
        }
        foreach (CardHeader item in _cards)
        {
            Sort(item);
        }
    }
    public void AddCard(CardData data)
    {
        CardHeader item = CardHeader.Spawn(data);
        Sort(item);
        _cards.Add(item);
        if (_showingDeck)
        {
            _deckData.Add(item.data);
        } else
        {
            _binData.Add(item.data);
        }
    }
    public void Sort(CardHeader item)
    {
        switch (_sortedBy)
        {
            case LabelIndex.COST:
                int cost = Mathf.Clamp(item.data.level, 0, 6);
                item.transform.parent = _columns[cost].transform;
                break;
            case LabelIndex.AFFINITY:
                item.transform.parent = _columns[(int)item.data.color - 1].transform;
                break;
            case LabelIndex.TYPE:
                item.transform.parent = _columns[(int)item.data.type - 1].transform;
                break;
            default:
                Debug.Log("Couldn't determine how the DecklistDisplay is being sorted");
                return;
        }
    }
    public void SortByCost() { SortBy(LabelIndex.COST); }
    public void SortByAffinity() { SortBy(LabelIndex.AFFINITY); }
    public void SortByType() { SortBy(LabelIndex.TYPE); }
    public void ToggleBin()
    {
        for (int ii = _cards.Count - 1; ii >= 0; ii--)
        {
            Destroy(_cards[ii].gameObject);
        }
        _cards.Clear();
        _showingDeck = !_showingDeck;
        if (_showingDeck)
        {
            _toggleButton.GetComponentInChildren<TextMeshProUGUI>().text = "Show Bin";
            foreach (CardData data in _deckData)
            {
                CardHeader item = CardHeader.Spawn(data);
                Sort(item);
                _cards.Add(item);
            }
        } else
        {
            _toggleButton.GetComponentInChildren<TextMeshProUGUI>().text = "Show Deck";
            foreach (CardData data in _binData)
            {
                CardHeader item = CardHeader.Spawn(data);
                Sort(item);
                _cards.Add(item);
            }
        }
    }
    public void Transfer(CardHeader card)
    {
        if (_showingDeck)
        {
            _deckData.Remove(card.data);
            _binData.Add(card.data);
        } else
        {
            _binData.Remove(card.data);
            _deckData.Add(card.data);
        }
        _cards.Remove(card);
        Destroy(card.gameObject);
    }

    public void Clear()
    {
        for (int ii = _cards.Count - 1; ii >= 0; ii--)
        {
            Destroy(_cards[ii].gameObject);
        }
        _cards.Clear();
        _showingDeck = true;
        _deckData.Clear();
        _binData.Clear();
    }
}

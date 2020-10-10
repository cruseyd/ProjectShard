using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CardZone : MonoBehaviour
{
    public enum Type
    {
        DEFAULT,
        ACTIVE,
        HAND,
        DISCARD,
        MAGNIFY,
        TRIBUTE,
        PLAY,
        DECKBUILDER,
        DRAFT,
        PREVIEW
    }

    public Type type;
    public bool playerOwned;

    private RectTransform _zone;

    public void Awake()
    {
        _zone = GetComponent<RectTransform>();
    }

    public List<Card> Cards(Card.Type type = Card.Type.DEFAULT)
    {
        List<Card> cards = new List<Card>();
        Card[] allCards = transform.GetComponentsInChildren<Card>();
        foreach (Card card in allCards)
        {
            if (type == Card.Type.DEFAULT || type == card.type)
            {
                cards.Add(card);
            }
        }
        return cards;
    }

    public List<CardGraphic> CardGraphics(Card.Type type = Card.Type.DEFAULT)
    {
        List<CardGraphic> cards = new List<CardGraphic>();
        CardGraphic[] allCards = transform.GetComponentsInChildren<CardGraphic>();
        foreach (CardGraphic card in allCards)
        {
            if (type == Card.Type.DEFAULT || type == card.data.type)
            {
                cards.Add(card);
            }
        }
        return cards;
    }

    public float Position(int zoneIndex)
    {
        int numCards = transform.childCount;
        float width = _zone.rect.width;
        float spacing = width / (1.0f * numCards);
        float xpos = -width / 2.0f + spacing / 2.0f;
        return xpos + zoneIndex * spacing;
    }

    public void Organize()
    {
        List<CardGraphic> graphics = CardGraphics();
        for (int ii = 0; ii < graphics.Count; ii++)
        {
            graphics[ii].zoneIndex = ii;
        }
        if (_zone.rect.width < 200)
        {
            foreach (CardGraphic card in graphics)
            {
                Vector2 dest = _zone.TransformPoint(0, 0, 0);
                card.StartCoroutine(card.DoTranslate(dest));
            }
        }
        else
        {
            foreach (CardGraphic card in graphics)
            {
                card.StartCoroutine(card.DoZoom(false));
                Vector2 dest = _zone.TransformPoint(Position(card.zoneIndex), 0, 0);
                card.StartCoroutine(card.DoTranslate(dest));
                card.transform.SetSiblingIndex(card.zoneIndex);
            }
        }
    }
}

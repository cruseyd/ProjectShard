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

    public void Organize()
    {
        List < Card > cards = Cards();
        for (int ii = 0; ii < cards.Count; ii++)
        {
            cards[ii].zoneIndex = ii;
        }
        if (_zone.rect.width < 200)
        {
            foreach (Card card in cards)
            {
                Vector2 dest = _zone.TransformPoint(0, 0, 0);
                card.StartCoroutine(card.Translate(dest));
            }
        }
        else
        {
            float width = _zone.rect.width;
            float spacing = width / (1.0f * cards.Count);
            float xpos = -width / 2.0f + spacing / 2.0f;
            foreach (Card card in cards)
            {
                card.StartCoroutine(card.Zoom(false));
                Vector2 dest = _zone.TransformPoint(xpos + card.zoneIndex * spacing, 0, 0);
                card.StartCoroutine(card.Translate(dest));
                card.transform.SetSiblingIndex(card.zoneIndex);
            }
        }
    }
}

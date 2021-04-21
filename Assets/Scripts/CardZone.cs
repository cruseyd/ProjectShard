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
    public float scale = 1.0f;
    private RectTransform _rect;
    public void Awake()
    {
        _rect = GetComponent<RectTransform>();
        scale = _rect.rect.height / 200.0f;
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
        float width = _rect.rect.width;
        float spacing = width / (1.0f * numCards);
        float xpos = -width / 2.0f + spacing / 2.0f;
        return xpos + zoneIndex * spacing;
    }

    public void Organize()
    {
        //Debug.Log("Organize: " + type);
        List<CardGraphic> graphics = CardGraphics();
        for (int ii = 0; ii < graphics.Count; ii++)
        {
            graphics[ii].zoneIndex = ii;
        }
        foreach (CardGraphic card in graphics)
        {
            
            Vector2 dest = _rect.TransformPoint(0, 0, 0);
            if (type == Type.HAND || type == Type.ACTIVE || type == Type.DRAFT)
            {
                dest = _rect.TransformPoint(Position(card.zoneIndex), 0, 0);
            }

            card.Translate(dest);
            card.transform.SetSiblingIndex(card.zoneIndex);
        }
    }
}

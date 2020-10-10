using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCardGraphic : MonoBehaviour
{
    [SerializeField] private CardZone _zoneA;
    [SerializeField] private CardZone _zoneB;

    public void AddCard()
    {
        CardGraphic card = CardGraphic.Spawn(CardIndex.Rand(), transform.position);
        card.FaceUp(true);
        card.Move(_zoneA);
    }
    public void MoveCard()
    {
        List<CardGraphic> cards = _zoneA.CardGraphics();
        if (cards.Count > 0)
        {
            cards[Random.Range(0, cards.Count)].Move(_zoneB);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Deck : MonoBehaviour
{
    [SerializeField] RectTransform _discard;
    [SerializeField] TextMeshProUGUI _counter;

    private List<CardData> cards;
    public bool playerDeck;

    private void UpdateCounter()
    {
        _counter.text = cards.Count.ToString();
    }
    public void Init(Decklist list)
    {
        if (cards == null)
        {
            cards = new List<CardData>();
        }
        cards.Clear();
        foreach (DecklistItem item in list.list)
        {
            for (int ii = 0; ii < item.quantity; ii++)
            {
                cards.Add(item.card);
            }
        }
        Shuffle();
        UpdateCounter();
    }
    public void Init(CardPool pool, float bias = 0)
    {
        Debug.Assert(bias >= 0 && bias <= 1);
        if (cards == null)
        {
            cards = new List<CardData>();
        }
        cards.Clear();
        foreach (CardPoolEntry entry in pool.pool)
        {
            float roll = Random.Range(bias, 1);
            bool done = false;
            int index = 0;
            while (!done)
            {
                CardRate item = entry.rates[index];
                if (item.rate >= roll || index == (entry.rates.Count - 1))
                {
                    for (int ii = 0; ii < entry.quantity; ii++)
                    {
                        cards.Add(item.card);
                    }
                    done = true;
                }
                index++;
            }
        }
        Shuffle();
        UpdateCounter();
    }
    public void Shuffle()
    {
        int n = cards.Count;
        for (int ii = 0; ii < cards.Count; ii++)
        {
            int a = Random.Range(0, n - 1);
            int b = Random.Range(0, n - 1);
            if (a != b)
            {
                CardData tmp = cards[a];
                cards[a] = cards[b];
                cards[b] = tmp;
            }
        }
    }
    public void Shuffle(Card card)
    {
        CardData data = card.data;
        Destroy(card.gameObject);
        Shuffle(data);
    }
    public void Shuffle(CardData data)
    {
        cards.Insert(Random.Range(0, cards.Count - 1), data);
        UpdateCounter();
    }
    public void Shuffle(Card[] cards)
    {
        foreach (Card card in cards)
        {
            Shuffle(card);
        }
        UpdateCounter();
    }
    public Card Draw()
    {
        if (cards.Count == 0)
        {
            Shuffle(_discard.GetComponentsInChildren<Card>());
        }
        CardData data = cards[0];
        cards.RemoveAt(0);
        Card card = Card.Spawn(data, playerDeck, transform.position);
        card.FaceUp(false);
        UpdateCounter();
        return card;

    }
}

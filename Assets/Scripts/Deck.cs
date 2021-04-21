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
    public int count { get { return cards.Count; } }
    private void UpdateCounter()
    {
        _counter.text = cards.Count.ToString();
    }

    public void Init(List<CardData> list)
    {
        if (cards == null)
        {
            cards = new List<CardData>();
        }
        cards.Clear();
        foreach (CardData data in list)
        {
            cards.Add(data);
        }
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
            CardData data = CardIndex.Get(item.id);
            for (int ii = 0; ii < item.qty; ii++)
            {
                cards.Add(data);
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
            for (int ii = 0; ii < entry.quantity; ii++)
            {
                float roll = Random.Range(bias, 1);
                int index = entry.rates.Count-1;
                while (index >= 0)
                {
                    if (entry.rates[index].rate <= roll)
                    {
                        cards.Add(entry.rates[index].card);
                    }
                    index--;
                }
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
        card.Delete();
        Shuffle(data);
    }
    public void Insert(Card card, int index)
    {
        cards.Insert(index, card.data);
        Destroy(card.gameObject);
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
        return Remove(0);
    }
    public Card Remove(int index)
    {
        Cycle();
        if (cards.Count < index) { return null; }
        CardData data = cards[index];
        cards.RemoveAt(index);
        Card card = Card.Spawn(data, playerDeck, transform.position);
        card.FaceUp(false);
        UpdateCounter();
        return card;

    }
    public CardData Reveal(int index = 0)
    {
        Cycle();
        if (cards.Count < index) { return null; }
        CardData data = cards[index];
        return data;
    }

    public List<CardData> RevealTop(int n)
    {
        List<CardData> revealed = new List<CardData>();
        for (int ii = 0; ii < n; ii++)
        {
            revealed.Add(Reveal(ii));
        }
        return revealed;
    }

    private void Cycle()
    {
        if (cards.Count == 0)
        {
            if (playerDeck)
            {
                Player.instance.CycleDeck();
            }
            else
            {
                Enemy.instance.CycleDeck();
            }
            Shuffle(_discard.GetComponentsInChildren<Card>());
        }
    }
}

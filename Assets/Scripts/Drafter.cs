using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drafter : MonoBehaviour
{
    public static Drafter instance;

    [SerializeField] private GameObject _finishedButton;

    private List<List<CardData>> _undraftedCards;
    private List<List<CardData>> _draftedCards;

    private int _round;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            _finishedButton.SetActive(false);
            _undraftedCards = new List<List<CardData>>();
            _draftedCards = new List<List<CardData>>();
            for (int ii = 0; ii < GameData.instance.numDrafters; ii++)
            {
                _undraftedCards.Add(new List<CardData>());
                _draftedCards.Add(new List<CardData>());
            }

        } else
        {
            Destroy(this.gameObject);
        }
    }

    public void StartDraft()
    {
        GeneratePacks();
        _round = 0;
        ShowPack();
    }

    public void GeneratePacks()
    {
        foreach (List<CardData> pack in _undraftedCards)
        {
            pack.Clear();
            for (int ii = 0; ii < GameData.instance.draftPackSize; ii++)
            {
                if (ii == 0)
                {
                    double roll = Random.Range(0, 1);
                    if (roll > 0.75) { pack.Add(CardIndex.Rand(Card.Rarity.MYTHIC)); }
                    else { pack.Add(CardIndex.Rand(Card.Rarity.RARE)); }

                } else if (ii < 3)
                {
                    pack.Add(CardIndex.Rand(Card.Rarity.SCARCE));
                } else
                {
                    pack.Add(CardIndex.Rand(Card.Rarity.COMMON));
                }
                
            }
        }
    }

    public List<CardData> CurrentPack(int player)
    {
        Debug.Assert(player < GameData.instance.numDrafters);
        return _undraftedCards[(_round + player) % GameData.instance.numDrafters];
    }
    public List<CardData> DraftedCards(int player)
    {
        Debug.Assert(player < GameData.instance.numDrafters);
        return _draftedCards[(_round + player) % GameData.instance.numDrafters];
    }
    public void Rotate()
    {
        for (int ii = 1; ii < GameData.instance.numDrafters; ii++)
        {
            List<CardData> pack = CurrentPack(ii);
            Draft(pack[Random.Range(0, pack.Count)], ii);
            // compute probability of choosing each card based on cards drafted
            // sample probability and choose
        }
        _round++;
        if (_round == GameData.instance.numDraftPacks * GameData.instance.draftPackSize)
        {
            _finishedButton.SetActive(true);
        } else
        {
            ShowPack();
        }
        
    }

    public void ShowPack(int player = 0)
    {
        List<CardGraphic> oldCards = Dungeon.GetCardGraphics(CardZone.Type.DRAFT);
        for (int ii = 0; ii < oldCards.Count; ii++)
        {
            Destroy(oldCards[ii].gameObject);
        }

        if (_round % GameData.instance.draftPackSize == 0 && _round < GameData.instance.numDraftPacks*GameData.instance.draftPackSize)
        {
            GeneratePacks();
        }

        foreach (CardData data in CurrentPack(0))
        {
            CardGraphic card = CardGraphic.Spawn(data, Vector3.zero);
            card.Move(Dungeon.GetZone(CardZone.Type.DRAFT));
        }
    }
    public void Draft(CardData data, int player)
    {
        CurrentPack(player).Remove(data);
        DraftedCards(player).Add(data);
        if (player == 0)
        {
            DecklistDisplay.instance.AddCard(data);
            Rotate();
        }
    }
    
    public void Finish()
    {
        GameData.instance.playerDeck = DecklistDisplay.instance.deck;
        //_round = 0;
        //_draftedCards.Clear();
        //_undraftedCards.Clear();
        _finishedButton.SetActive(false);
        Dungeon.instance.StartEncounter();
        
    }
}

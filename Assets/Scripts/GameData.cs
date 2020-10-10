using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData instance;

    public Color VIOLET;
    public Color RED;
    public Color GOLD;
    public Color GREEN;
    public Color BLUE;
    public Color INDIGO;
    public Color TAN;

    public Color COMMON;
    public Color SCARCE;
    public Color RARE;
    public Color LEGENDARY;
    public Color TOKEN;

    // rules parameters
    public int playerHandSize = 3;
    public int enemyDrawMin = 1;
    public int enemyDrawMax = 3;
    public int draftPackSize = 7;
    public int numDrafters = 3;
    public int minDeckSize = 15;
    public int numDraftPacks = 2;

    // animation parameters
    public float hoverDelay = 0.5f;
    public float cardAnimationRate = 0.25f;

    // configuration parameters
    public bool startEncounter = false;
    public bool startDraft = false;
    public bool invincible = false;
    public bool ignoreResources = false;

    private EnemyData _enemy;
    private Decklist _playerDecklist;
    private List<CardData> _playerDeck;

    public List<CardData> playerDeck
    {
        get
        {
            return _playerDeck;
        }
        set
        {
            if (_playerDeck == null)
            {
                _playerDeck = new List<CardData>();
            }
            _playerDeck.Clear();
            foreach (CardData data in value)
            {
                _playerDeck.Add(data);
            }
        }
    }

    public static EnemyData enemy
    {
        get
        {
            return instance._enemy;
        } set
        {
            instance._enemy = value;
        }
    }
    public static Decklist playerDecklist
    {
        get
        {
            return instance._playerDecklist;
        } set
        {
            instance._playerDecklist = value;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        } else
        {
            Destroy(this.gameObject);
        }
    }

    public static Color GetColor(Card.Color color)
    {
        switch (color)
        {
            case Card.Color.LIS: return instance.VIOLET;
            case Card.Color.RAIZ: return instance.RED;
            case Card.Color.ORA: return instance.GOLD;
            case Card.Color.FEN: return instance.GREEN;
            case Card.Color.IRI: return instance.BLUE;
            case Card.Color.VAEL: return instance.INDIGO;
            case Card.Color.NEUTRAL: return instance.TAN;
            default: return Color.white;
        }
    }
    public static Color GetColor(Card.Rarity rarity)
    {
        switch (rarity)
        {
            case Card.Rarity.COMMON: return instance.COMMON;
            case Card.Rarity.SCARCE: return instance.SCARCE;
            case Card.Rarity.RARE: return instance.RARE;
            case Card.Rarity.MYTHIC: return instance.LEGENDARY;
            case Card.Rarity.TOKEN: return instance.TOKEN;
            default: return Color.white;
        }
    }

}

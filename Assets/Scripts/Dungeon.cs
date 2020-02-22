using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum CardZone
{
    DEFAULT,
    PLAYER_HAND,
    PLAYER_ACTIVE,
    PLAYER_DISCARD,
    DUNGEON_HAND,
    DUNGEON_ACTIVE,
    DUNGEON_DISCARD,
    MAGNIFY,
    DROP,
    BURN,
}

public class Dungeon : MonoBehaviour
{
    public static Dungeon instance;

    private GameParams _params;
    //[SerializeField] private Enemy _enemy;
    [SerializeField] private GamePhase _phase;

    [SerializeField] private RectTransform _dropZone;
    [SerializeField] private RectTransform _burnZone;
    [SerializeField] private GameObject _confirmButton;
    [SerializeField] private TextMeshProUGUI _confirmButtonText;
    [SerializeField] private RectTransform _playerHandZone;
    [SerializeField] private RectTransform _dungeonHandZone;
    [SerializeField] private RectTransform _playerActiveZone;
    [SerializeField] private RectTransform _dungeonActiveZone;
    [SerializeField] private RectTransform _playerDiscardZone;
    [SerializeField] private RectTransform _dungeonDiscardZone;
    [SerializeField] private RectTransform _playerDeckZone;
    [SerializeField] private RectTransform _dungeonDeckZone;
    [SerializeField] private RectTransform _magnifyWindow;
    [SerializeField] private GameObject _tooltipWindow;

    [SerializeField] private ParticleSystem _playerParticleUnderlay;
    [SerializeField] private ParticleSystem _enemyParticleUnderlay;

    public static List<TemplateModifier> modifiers;

    public static int combatTurn;
    public static GameParams gameParams
    {
        get
        {
            if (instance._params == null)
            {
                instance._params = Resources.Load("GameParams") as GameParams;
            }
            return instance._params;
        }
    }
    public static GamePhase phase
    {
        get
        {
            return instance._phase;
        }
        set
        {
            instance._phase?.Exit();
            instance._phase = value;
            instance._phase.Enter();
            GameEvents.current.Refresh();
        }
    }
    public static GameObject tooltipWindow
    {
        get
        {
            return instance._tooltipWindow;
        }
    }
    public static Card magnifiedCard
    {
        get
        {
            Card[] cards = GetCards(CardZone.MAGNIFY);
            if (cards.Length == 1) { return cards[0]; }
            else if (cards.Length == 0) { return null; }
            else { Debug.Log("There should not be more than 1 card magnified"); }
            return null;
        }
    }

    public void Awake()
    {
        if (instance == null) { instance = this; }
        else { Destroy(this); }
        modifiers = new List<TemplateModifier>();
    }
    public void Start()
    {
        StartEncounter();
    }
    public void Update()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            Targeter.Clear();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Confirm();
        }
    }
    public static Card[] GetCards(CardZone zone)
    {
        return GetZone(zone).GetComponentsInChildren<Card>();
    }
    public static RectTransform GetZone(CardZone zone)
    {
        switch (zone)
        {
            case CardZone.PLAYER_HAND: return instance._playerHandZone;
            case CardZone.PLAYER_ACTIVE: return instance._playerActiveZone;
            case CardZone.PLAYER_DISCARD: return instance._playerDiscardZone;
            case CardZone.DUNGEON_ACTIVE: return instance._dungeonActiveZone;
            case CardZone.DUNGEON_DISCARD: return instance._dungeonDiscardZone;
            case CardZone.DUNGEON_HAND: return instance._dungeonHandZone;
            case CardZone.MAGNIFY: return instance._magnifyWindow;
            case CardZone.DROP: return instance._dropZone;
            case CardZone.BURN: return instance._burnZone;
            default: return null;
        }
    }

    public static bool AddModifier(TemplateModifier mod)
    {
        if (modifiers.Contains(mod)) { return false; }
        else
        {
            modifiers.Add(mod);
            GameEvents.current.AddGlobalModifier(mod);
            return true;
        }
    }
    public static bool RemoveModifier(TemplateModifier mod)
    {
        if (modifiers.Contains(mod))
        {
            modifiers.Remove(mod);
            GameEvents.current.RemoveGlobalModifier(mod);
            return true;
        } else
        {
            return false;
        }
    }
    public static void Organize(CardZone zone)
    {
        Card[] cards = GetCards(zone);
        RectTransform tf = GetZone(zone);
        for (int ii = 0; ii < cards.Length; ii++)
        {
            cards[ii].zoneIndex = ii;
        }
        if (tf.rect.width < 200)
        {
            foreach (Card card in cards)
            {
                Vector2 dest = tf.TransformPoint(0, 0, 0);
                card.StartCoroutine(card.Translate(dest));
            }
        } else
        {
            float width = tf.rect.width;
            float spacing = width / (1.0f * cards.Length);
            float xpos = -width / 2.0f + spacing / 2.0f;
            foreach (Card card in cards)
            {
                Vector2 dest = tf.TransformPoint(xpos + card.zoneIndex * spacing, 0, 0);
                card.StartCoroutine(card.Translate(dest));
                card.transform.SetSiblingIndex(card.zoneIndex);
            }
        }
    }
    public static void MoveCard(Card card, CardZone zone)
    {
        if (card == null) { return; }
        CardZone prevZone = card.zone;
        card.zone = zone;
        card.transform.SetParent(GetZone(zone));
        RectTransform cardTF = card.GetComponent<RectTransform>();
        card.transform.localScale = Vector3.one * GetZone(zone).rect.height / card.GetComponent<RectTransform>().rect.height;
        Organize(zone);
        if (prevZone != CardZone.DEFAULT)
        {
            Organize(prevZone);
        }
    }
    public static void SetParticleUnderlay(bool playerSide)
    {
        if (playerSide)
        {
            instance._playerParticleUnderlay.Play();
            instance._enemyParticleUnderlay.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        } else
        {
            instance._enemyParticleUnderlay.Play();
            instance._playerParticleUnderlay.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
    public static void FixLayering()
    {
        instance._magnifyWindow.transform.SetAsLastSibling();
        instance._tooltipWindow.transform.SetAsLastSibling();
    }

    public void Confirm() { _phase.Confirm(); }
    public static void SetConfirmButtonText(string text)
    {
        instance._confirmButtonText.text = text;
    }
    public static void EnableConfirmButton(bool flag)
    {
        instance._confirmButton.GetComponent<Button>().enabled = flag;
    }

    public void StartEncounter() { StartCoroutine(DoStartEncounter()); }
    private IEnumerator DoStartEncounter()
    {
        Player.instance.StartEncounter();
        Enemy.instance.StartEncounter();
        yield return new WaitForSeconds(1.0f);
        yield return Enemy.instance.DoDrawRandom();
        Dungeon.phase = GamePhase.player;
    }
}
